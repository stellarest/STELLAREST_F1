using System.Collections;
using System.Collections.Generic;
using System.Data;
using STELLAREST_F1.Data;
using Unity.VisualScripting;
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
        public ProjectileMotionBase ProjectileMotion { get; private set; } = null;
        public EProjectileMotionType ProjectileMotionType { get; private set; } = EProjectileMotionType.None;

        public bool RotateToTarget { get; private set; } = false;
        public float ProjectileSpeed { get; private set; } = 0f;

        protected float _projectileLifeTime = 0f;
        protected ESkillTargetRange _targetRange = ESkillTargetRange.None;
        protected int _targetDistance = -1;
        private int _penetrateCount = 0;
        public int CanPenetrateCount { get; private set; } = 0;
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
            ProjectileSpeed = projectileData.ProjectileSpeed;
            _projectileLifeTime = projectileData.ProjectileLifeTime;
            InitialSetProjectileSize(projectileData.ProjectileSize);
            ProjectileCurveType = projectileData.ProjectileCurveType;
            InitialSetProjectileMotion(projectileData.ProjectileMotionType);

            Skill = Owner.CreatureSkill.FindSkill(DataTemplateID);
            _targetRange = Skill.SkillData.TargetRange;
            _targetDistance = Skill.SkillData.TargetDistance;
            CanPenetrateCount = Skill.SkillData.TargetDistance;
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
            Vector3 targetPos = Owner.Target.transform.position;
            _penetrateCount = 0;
            
            RigidBody.simulated = true;
            ProjectileMotion.ReadyToLaunch(startPos, targetPos, this);
            StartCoProjectileLifeTime();
            _hitColliders.Clear();
            StopCoDelayCollision();
            //Debug.Log("ENTER IN GAME PROJECTILE");
        }
        #endregion

        // private Coroutine _coCollisionDelay = null;


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_hitColliders.Contains(other))
            {
                Debug.Log("<color=cyan>DelayCollider</color>");
                return;
            }

            StartCoDelayCollision(other);
            
            // _targetRange, _targetDistance
            BaseObject target = other.GetComponent<BaseObject>();
            if (target.IsValid() == false)
                return;

            switch (_targetRange)
            {
                case ESkillTargetRange.Single:
                    {
                        DamageToTarget(target);
                        if (++_penetrateCount >= CanPenetrateCount)
                        {
                            RigidBody.simulated = false;
                            Managers.Object.Despawn(this, DataTemplateID);
                        }
                    }
                    break;

                case ESkillTargetRange.Half:
                    _projectileSkillTargets.Clear();
                    // ReserveHalfTargets(target);
                    break;

                case ESkillTargetRange.Around:
                    _projectileSkillTargets.Clear();
                    // ReserveAroundTargets(target);
                    break;
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

        private void DamageToTargets()
        {
            for (int i = 0; i < _projectileSkillTargets.Count; ++i)
            {
                if (_projectileSkillTargets[i].IsValid() == false)
                    continue;

                BaseObject target = _projectileSkillTargets[i];
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
        }

        // _targetDistance
        private void ReserveHalfTargets(BaseObject target)
        {
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

        private void StartCoDelayCollision(Collider2D other, float delayTime = 0.1f)
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
