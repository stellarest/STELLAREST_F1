using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using STELLAREST_F1.Data;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Projectile : BaseObject
    {
        public Creature Owner { get; set; } = null;
        public SkillBase Skill { get; private set; } = null;
        public ProjectileData ProjectileData { get; private set; } = null;
        public EAnimationCurveType ProjectileCurveType { get; private set; } = EAnimationCurveType.None;
        public EProjectileMotionType ProjectileMotionType { get; private set; } = EProjectileMotionType.None;
        public ProjectileMotionBase ProjectileMotion { get; private set; } = null;

        public bool RotateToTarget { get; private set; } = false;
        public int CanPenetrateCount { get; private set; } = 0;
        private int _currentPenetrationCount = 0;

        public float ProjectileSpeed { get; private set; } = 0f;
        public Vector3 nStartShootDir { get; private set; } = Vector3.zero;
        private Vector3Int _lastCellPos = Vector3Int.zero;
       

        protected float _projectileLifeTime = 0f;
        protected ESkillTargetRange _targetRange = ESkillTargetRange.None;
        protected int _targetDistance = 0;
        protected List<BaseObject> _projectileSkillTargets = new List<BaseObject>();

        private Coroutine _coProjectileLifeTime = null;
        private Coroutine _coCollisionDelay = null;
        private HashSet<Collider2D> _hitColliders = new HashSet<Collider2D>();
        #region Init Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Projectile;
            Collider.isTrigger = true;
            SortingGroup.sortingOrder = ReadOnly.SortingLayers.SLOrder_Projectile;
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            if (Managers.Data.ProjectileDataDict.TryGetValue(dataID, out ProjectileData projectileData) == false)
            {
                Debug.LogError($"{nameof(Projectile)}, {nameof(InitialSetInfo)}, Failed to load ProjectileData");
                return;
            }

            DataTemplateID = dataID;
            ProjectileData = projectileData;
            RotateToTarget = projectileData.RotateToTarget;
            CanPenetrateCount = projectileData.PenetrationCount;
            ProjectileSpeed = projectileData.ProjectileSpeed;
            _projectileLifeTime = projectileData.ProjectileLifeTime;
            InitialSetProjectileSize(projectileData.ProjectileSize);
            ProjectileCurveType = projectileData.ProjectileCurveType;
            InitialSetProjectileMotion(projectileData.ProjectileMotionType);
            ProjectileMotionType = projectileData.ProjectileMotionType;

            Skill = Owner.CreatureSkill.FindSkill(DataTemplateID);
            _targetRange = Skill.SkillData.TargetRange;
            _targetDistance = Skill.SkillData.TargetDistance;
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            if (Owner.IsValid() == false)
                return;

            if (Owner.Target.IsValid() == false)
                return;

            _projectileSkillTargets.Clear();
            transform.position = spawnPos;

            LayerMask excludeLayerMask = 0;
            excludeLayerMask.AddLayer(ELayer.Default);
            excludeLayerMask.AddLayer(ELayer.Projectile);
            excludeLayerMask.AddLayer(ELayer.Env);
            excludeLayerMask.AddLayer(ELayer.Obstacle);
            excludeLayerMask.AddLayer(ELayer.Hero);
            excludeLayerMask.AddLayer(ELayer.Monster);

            if (Owner.ObjectType == EObjectType.Hero)
                excludeLayerMask.RemoveLayer(ELayer.Monster);
            else if (Owner.ObjectType == EObjectType.Monster)
                excludeLayerMask.RemoveLayer(ELayer.Hero);
            Collider.excludeLayers = excludeLayerMask;

            Vector3 startPos = Owner.GetFirePosition();
            Vector3 targetPos = Owner.Target.CenterPosition;
            nStartShootDir = (targetPos - startPos).normalized;

            _currentPenetrationCount = 0;

            RigidBody.simulated = true;
            ProjectileMotion.ReadyToLaunch(startPos, targetPos, this);
            StartCoProjectileLifeTime();
            _hitColliders.Clear();
            StopCoDelayCollision();
        }
        #endregion

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_hitColliders.Contains(other))
                return;

            if (_targetDistance < 1)
                return;

            StartCoDelayCollision(other);
            // _targetRange, _targetDistance
            BaseObject target = other.GetComponent<BaseObject>();
            if (target.IsValid() == false)
                return;

            _projectileSkillTargets.Clear();
            switch (_targetRange)
            {
                case ESkillTargetRange.Single:
                    ReserveSingleTargets(target);
                    break;

                case ESkillTargetRange.Half:
                    break;

                case ESkillTargetRange.Around:
                    break;
            }

            // Damage To Targets
            for (int i = 0; i < _projectileSkillTargets.Count; ++i)
            {
                if (_projectileSkillTargets[i].IsValid() == false)
                    continue;

                DamageToTarget(_projectileSkillTargets[i]);
            }

            if (_currentPenetrationCount++ >= CanPenetrateCount)
            {
                RigidBody.simulated = false;
                Managers.Object.Despawn(this, DataTemplateID);
            }
        }

        private void ReserveSingleTargets(BaseObject target)
        {
            if (_targetDistance == 1)
            {
                _projectileSkillTargets.Add(target);
                return;
            }

            Vector3Int targetCellPos = target.CellPos;
            BaseObject nextTarget = null;
            float threshold = 0.1f;
            Vector3 nHitDir = ProjectileMotion.LaunchingDir.normalized;
            if (Vector3.Dot(nHitDir, Vector3.up) > 1 - threshold)
            {
                for (int y = 0; y < _targetDistance; ++y)
                {
                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(0, y, 0));
                    if (nextTarget != null)
                        _projectileSkillTargets.Add(nextTarget);
                }
            }
            else if (Vector3.Dot(nHitDir, Vector3.down) > 1 - threshold)
            {
                for (int y = 0; y < _targetDistance; ++y)
                {
                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(0, -y, 0));
                    if (nextTarget != null)
                        _projectileSkillTargets.Add(nextTarget);
                }
            }
            else if (Vector3.Dot(nHitDir, Vector3.left) > 1 - threshold)
            {
                for (int x = 0; x < _targetDistance; ++x)
                {
                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(-x, 0, 0));
                    if (nextTarget != null)
                        _projectileSkillTargets.Add(nextTarget);
                }
            }
            else if (Vector3.Dot(nHitDir, Vector3.right) > 1 - threshold)
            {
                for (int x = 0; x < _targetDistance; ++x)
                {
                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(x, 0, 0));
                    if (nextTarget != null)
                        _projectileSkillTargets.Add(nextTarget);
                }
            }
            else if (Vector3.Dot(nHitDir, new Vector3(-1, 1, 0).normalized) > 1 - threshold)
            {
                for (int dxy = 0; dxy < _targetDistance; ++dxy)
                {
                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(-dxy, dxy, 0));
                    if (nextTarget != null)
                        _projectileSkillTargets.Add(nextTarget);
                }
            }
            else if (Vector3.Dot(nHitDir, new Vector3(1, 1, 0).normalized) > 1 - threshold)
            {
                for (int dxy = 0; dxy < _targetDistance; ++dxy)
                {
                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(dxy, dxy, 0));
                    if (nextTarget != null)
                        _projectileSkillTargets.Add(nextTarget);
                }
            }
            else if (Vector3.Dot(nHitDir, new Vector3(1, -1, 0).normalized) > 1 - threshold)
            {
                for (int dxy = 0; dxy < _targetDistance; ++dxy)
                {
                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(dxy, -dxy, 0));
                    if (nextTarget != null)
                        _projectileSkillTargets.Add(nextTarget);
                }
            }
            else if (Vector3.Dot(nHitDir, new Vector3(-1, -1, 0).normalized) > 1 - threshold)
            {
                for (int dxy = 0; dxy < _targetDistance; ++dxy)
                {
                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(-dxy, -dxy, 0));
                    if (nextTarget != null)
                        _projectileSkillTargets.Add(nextTarget);
                }
            }
        }

        private void ReserveHalfTargets(BaseObject target)
        {
            Vector3Int targetCellPos = target.CellPos;
            // BaseObject nextTarget = null;
        }

        // _targetDistance
        private void ReserveAroundTargets(BaseObject target)
        {
            Vector3Int targetCellPos = target.CellPos;
            for (int x = _targetDistance * -1; x <= _targetDistance; ++x)
            {
                for (int y = _targetDistance * -1; y <= _targetDistance; ++y)
                {
                    Vector3Int cellPos = new Vector3Int(targetCellPos.x + x, targetCellPos.y + y, 0);
                    BaseObject obj = Managers.Map.GetObject(cellPos);
                    if (obj.IsValid() && obj != null)
                        _projectileSkillTargets.Add(obj);
                }
            }
        }

        private void DamageToTarget(BaseObject target)
        {
            if (target.IsValid() == false)
                return;

            target.OnDamaged(attacker: Owner, skillByAttacker: Skill);
            // --- Effect
            if (Skill.SkillData.EffectIDs.Length != 0)
            {
                List<EffectBase> effects = Owner.BaseEffect.GenerateEffects(
                effectIDs: Skill.SkillData.EffectIDs,
                spawnPos: Util.GetRandomQuadPosition(target.CenterPosition),
                startCallback: null
                );
            }
        }

        private void InitialSetProjectileSize(EObjectSize size)
        {
            switch (size)
            {
                case EObjectSize.VerySmall:
                    break;

                case EObjectSize.Small:
                    transform.localScale *= 0.5f;
                    break;

                case EObjectSize.Medium:
                    break;

                case EObjectSize.Large:
                    break;

                case EObjectSize.VeryLarge:
                    break;

                case EObjectSize.PresetSize:
                    return;
            }
        }

        private void InitialSetProjectileMotion(EProjectileMotionType projectileMotionType)
        {
            switch (projectileMotionType)
            {
                case EProjectileMotionType.Straight:
                    ProjectileMotion = gameObject.AddComponent<StraightMotion>();
                    break;

                case EProjectileMotionType.Parabola:
                    ProjectileMotion = gameObject.AddComponent<ParabolaMotion>();
                    break;
            }
        }

        private void StartCoProjectileLifeTime()
        {
            StopCoProjectileLifeTime();
            _coProjectileLifeTime = StartCoroutine(CoProjectileLifeTime());
        }

        private void StopCoProjectileLifeTime()
        {
            if (_coProjectileLifeTime != null)
                StopCoroutine(_coProjectileLifeTime);

            _coProjectileLifeTime = null;
        }

        private IEnumerator CoProjectileLifeTime()
        {
            yield return new WaitForSeconds(_projectileLifeTime);
            if (this.IsValid())
            {
                RigidBody.simulated = false;
                Managers.Object.Despawn(this, DataTemplateID);
            }
        }

        private void StartCoDelayCollision(Collider2D other, float delayTime = 0.5f)
        {
            _coCollisionDelay = StartCoroutine(CoDelayCollision(other, delayTime));
        }

        private void StopCoDelayCollision()
        {
            if (_coCollisionDelay != null)
                StopCoroutine(_coCollisionDelay);

            _hitColliders.Clear();
            _coCollisionDelay = null;
        }

        private IEnumerator CoDelayCollision(Collider2D other, float delayTime = 0.1f)
        {
            _hitColliders.Add(other);
            yield return new WaitForSeconds(delayTime);
            _hitColliders.Remove(other);
        }

        /*
            private void ReserveSingleTargets_PREV(BaseObject target)
        {
            if (_targetDistance == 1)
            {
                _projectileSkillTargets.Add(target);
                return;
            }

            Vector3Int targetCellPos = target.CellPos;
            BaseObject nextTarget = null;
            Vector3 hitDir = (transform.position - target.CenterPosition).normalized;
            switch (ProjectileMotionType)
            {
                // --- Single Straight
                case EProjectileMotionType.Straight:
                    {
                        int roundX = Mathf.RoundToInt(Mathf.Abs(nStartShootDir.normalized.x));
                        int roundY = Mathf.RoundToInt(Mathf.Abs(nStartShootDir.normalized.y));

                        bool isHorizontal = roundX == 1 && roundY == 0;
                        bool isVertical = roundX == 0 && roundY == 1;
                        bool isDiagonal = roundX == 1 && roundY == 1;
                        if (isHorizontal)
                        {
                            // --- Hit by Right
                            if (hitDir.x > 0)
                            {
                                Debug.Log($"Hit by Right: {_targetDistance}");
                                for (int x = 0; x < _targetDistance; ++x)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(x * -1, 0, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                            // --- Hit by Left
                            else
                            {
                                Debug.Log($"Hit by Left: {_targetDistance}");
                                for (int x = 0; x < _targetDistance; ++x)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(x, 0, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                        }
                        else if (isVertical)
                        {
                            // --- Hit by Up
                            if (hitDir.y > 0)
                            {
                                Debug.Log($"Hit by Up: {_targetDistance}");
                                for (int y = 0; y < _targetDistance; ++y)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(0, y * -1, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                            // --- Hit by Down
                            else
                            {
                                Debug.Log($"Hit by Down: {_targetDistance}");
                                for (int y = 0; y < _targetDistance; ++y)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(0, y, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                        }
                        else if (isDiagonal)
                        {
                            // --- Hit by LeftUp
                            if (hitDir.x < 0 && hitDir.y > 0)
                            {
                                Debug.Log($"Hit by LeftUp: {_targetDistance}");
                                // --- Go RightDown
                                for (int dxy = 0; dxy < _targetDistance; ++dxy)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(dxy, dxy * -1, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                            // --- Hit by RightUp
                            else if (hitDir.x > 0 && hitDir.y > 0)
                            {
                                Debug.Log($"Hit by RightUp: {_targetDistance}");
                                // --- Go LeftDown
                                for (int dxy = 0; dxy < _targetDistance; ++dxy)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(dxy * -1, dxy * -1, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                            // --- Hit by LeftDown
                            else if (hitDir.x < 0 && hitDir.y < 0)
                            {
                                Debug.Log($"Hit by LeftDown: {_targetDistance}");
                                // --- Go RightUp
                                for (int dxy = 0; dxy < _targetDistance; ++dxy)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(dxy, dxy, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                            // --- Hit by RightDown
                            else if (hitDir.x > 0 && hitDir.y < 0)
                            {
                                Debug.Log($"Hit by RightDown: {_targetDistance}");
                                // --- Go LeftUp
                                for (int dxy = 0; dxy < _targetDistance; ++dxy)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(dxy * -1, dxy, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                        }
                    }
                    break;

                // --- Single Parabola
                case EProjectileMotionType.Parabola:
                    {
                        int roundX = Mathf.RoundToInt(Mathf.Abs(nStartShootDir.normalized.x));
                        int roundY = Mathf.RoundToInt(Mathf.Abs(nStartShootDir.normalized.y));

                        bool isHorizontal = roundX == 1 && roundY == 0;
                        bool isVertical = roundX == 0 && roundY == 1;
                        bool isDiagonal = roundX == 1 && roundY == 1;
                        if (isHorizontal)
                        {
                            // --- Hit by Right
                            if (hitDir.x > 0)
                            {
                                Debug.Log($"Hit by Right: {_targetDistance}");
                                for (int x = 0; x < _targetDistance; ++x)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(x * -1, 0, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                            // --- Hit by Left
                            else
                            {
                                Debug.Log($"Hit by Left: {_targetDistance}");
                                for (int x = 0; x < _targetDistance; ++x)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(x, 0, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                        }
                        else if (isVertical)
                        {
                            // --- Hit by Up
                            if (hitDir.y > 0)
                            {
                                Debug.Log($"Hit by Up: {_targetDistance}");
                                for (int y = 0; y < _targetDistance; ++y)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(0, y * -1, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                            // --- Hit by Down
                            else
                            {
                                Debug.Log($"Hit by Down: {_targetDistance}");
                                for (int y = 0; y < _targetDistance; ++y)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(0, y, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                        }
                        else if (isDiagonal)
                        {
                            // --- Hit by LeftUp
                            if (hitDir.x < 0 && hitDir.y > 0)
                            {
                                Debug.Log($"Hit by LeftUp: {_targetDistance}");
                                // --- Go RightDown
                                for (int dxy = 0; dxy < _targetDistance; ++dxy)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(dxy, dxy * -1, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                            // --- Hit by RightUp
                            else if (hitDir.x > 0 && hitDir.y > 0)
                            {
                                Debug.Log($"Hit by RightUp: {_targetDistance}");
                                // --- Go LeftDown
                                for (int dxy = 0; dxy < _targetDistance; ++dxy)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(dxy * -1, dxy * -1, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                            // --- Hit by LeftDown
                            else if (hitDir.x < 0 && hitDir.y < 0)
                            {
                                Debug.Log($"Hit by LeftDown: {_targetDistance}");
                                // --- Go RightUp
                                for (int dxy = 0; dxy < _targetDistance; ++dxy)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(dxy, dxy, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                            // --- Hit by RightDown
                            else if (hitDir.x > 0 && hitDir.y < 0)
                            {
                                Debug.Log($"Hit by RightDown: {_targetDistance}");
                                // --- Go LeftUp
                                for (int dxy = 0; dxy < _targetDistance; ++dxy)
                                {
                                    nextTarget = Managers.Map.GetObject(targetCellPos + new Vector3Int(dxy * -1, dxy, 0));
                                    if (nextTarget != null)
                                        _projectileSkillTargets.Add(nextTarget);
                                }
                            }
                        }
                    }
                    break;
            }
        }
        */

        // public override bool Init()
        // {
        //     if (base.Init() == false)
        //         return false;

        //     ObjectType = EObjectType.Projectile;
        //     Collider.isTrigger = true;
        //     SortingGroup.sortingOrder = ReadOnly.SortingLayers.SLOrder_Projectile;
        //     return true;
        // }

        // protected override void InitialSetInfo(int dataID)
        // {
        //     if (Managers.Data.ProjectileDataDict.TryGetValue(dataID, out ProjectileData projectileData) == false)
        //     {
        //         Debug.LogError($"{nameof(Projectile)}, Input : \"{dataID}\"");
        //         Debug.Break();
        //         return;
        //     }

        //     DataTemplateID = dataID;
        //     ProjectileData = projectileData;

        //     // --- Make ProjectileBody And Set,, Maybe. or Prefab Load like effect
        //     SpriteRenderer spr = GetComponent<SpriteRenderer>();
        //     Sprite sprite = Managers.Resource.Load<Sprite>(projectileData.Body);
        //     if (sprite != null)
        //     {
        //         spr.sprite = sprite;
        //         if (ColorUtility.TryParseHtmlString(projectileData.BodyColor, out Color bodyColor))
        //             spr.color = bodyColor;
        //     }

        //     switch (projectileData.ProjectileSize)
        //     {
        //         case EObjectSize.None:
        //             break;

        //         case EObjectSize.VerySmall:
        //             break;

        //         case EObjectSize.Small:
        //             transform.localScale *= 0.5f;
        //             break;

        //         case EObjectSize.Medium:
        //             break;

        //         case EObjectSize.Large:
        //             break;

        //         case EObjectSize.VeryLarge:
        //             break;
        //     }

        //     Collider.radius = projectileData.ColliderRadius;
        //     switch (projectileData.MotionType)
        //     {
        //         case EProjectileMotionType.Parabola:
        //             ProjectileMotion = gameObject.AddComponent<ParabolaMotion>();
        //             break;

        //         case EProjectileMotionType.Straight:
        //             ProjectileMotion = gameObject.AddComponent<StraightMotion>();
        //             break;
        //     }

        //     ProjectileMotion.InitialSetInfo(projectileData);
        // }

        // protected override void EnterInGame(Vector3 spawnPos)
        // {
        //     SpawnedPos = spawnPos;
        //     Skill = Owner.CreatureSkill.FindSkill(DataTemplateID);

        //     LayerMask excludeLayerMask = 0;
        //     excludeLayerMask.AddLayer(ELayer.Default);
        //     excludeLayerMask.AddLayer(ELayer.Projectile);
        //     excludeLayerMask.AddLayer(ELayer.Env);
        //     excludeLayerMask.AddLayer(ELayer.Obstacle);
        //     excludeLayerMask.AddLayer(ELayer.Hero);
        //     excludeLayerMask.AddLayer(ELayer.Monster);

        //     if (Owner.ObjectType == EObjectType.Hero)
        //         excludeLayerMask.RemoveLayer(ELayer.Monster);
        //     else if (Owner.ObjectType == EObjectType.Monster)
        //         excludeLayerMask.RemoveLayer(ELayer.Hero);

        //     Collider.excludeLayers = excludeLayerMask;

        //     ProjectileMotion.SetEndCallback(() => Managers.Object.Despawn(this, DataTemplateID));

        //     Vector3 startPos = Owner.GetFirePosition();
        //     Vector3 targetPos = Owner.Target.IsValid() ? Owner.Target.transform.position : startPos;

        //     ProjectileMotion.EnterInGame(startPos, targetPos);
        //     StartCoroutine(CoLifeTime(ReadOnly.Util.ProjectileLifeTime));
        // }

        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     BaseObject target = other.GetComponent<BaseObject>();
        //     if (target.IsValid() == false)
        //         return;

        //     target.OnDamaged(Owner, Skill);
        //     Managers.Object.Despawn(this, DataTemplateID);
        // }

        // private IEnumerator CoLifeTime(float duration)
        // {
        //     yield return new WaitForSeconds(duration);
        //     if (this.IsValid())
        //         Managers.Object.Despawn(this, DataTemplateID);
        // }
    }
}
