using System.Collections;
using System.Collections.Generic;
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
        protected List<BaseObject> _projectileSkillTargets = new List<BaseObject>();

        private Coroutine _coProjectileLifeTime = null;

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

            if (Managers.Data.SkillDataDict.TryGetValue(dataID, out SkillData skillData) == false)
            {
                Debug.LogError($"{nameof(Projectile)}, {nameof(InitialSetInfo)}, Failed to load SkillData");
                return;
            }
            _targetRange = skillData.TargetRange;
            _targetDistance = skillData.TargetDistance;
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
            
            RigidBody.simulated = true;
            ProjectileMotion.ReadyToLaunch(startPos, targetPos, this);
            StartCoProjectileLifeTime();
        }
        #endregion

        private void OnTriggerEnter2D(Collider2D other)
        {
            // _projectileSkillTargets.Add
            Debug.Log("NO DMG NOW !!");
            RigidBody.simulated = false;
            Managers.Object.Despawn(this, DataTemplateID);
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
