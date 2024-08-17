using System.Collections;
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
        public ProjectileMotionBase ProjectileMotion { get; private set; } = null;
        public EProjectileMotionType ProjectileMotionType { get; private set; } = EProjectileMotionType.None;

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
                Debug.LogError($"{nameof(Projectile)}, {nameof(InitialSetInfo)}");
                return;
            }

            DataTemplateID = dataID;
            ProjectileData = projectileData;
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            base.EnterInGame(spawnPos);
            
            transform.position = spawnPos;
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
