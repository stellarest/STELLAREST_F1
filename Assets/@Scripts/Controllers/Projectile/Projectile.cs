using System.Collections;
using STELLAREST_F1.Data;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Projectile : BaseObject
    {
        public Creature Owner { get; private set; } = null;
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

        public override bool SetInfo(int dataID, BaseObject owner)
        {
            if (base.SetInfo(dataID, owner) == false)
            {
                EnterInGame(owner, dataID);
                return false;
            }

            if (Managers.Data.ProjectileDataDict.TryGetValue(dataID, out Data.ProjectileData projectileData) == false)
            {
                Debug.LogError($"{nameof(Projectile)}, {nameof(SetInfo)}, Input : \"{dataID}\"");
                Debug.Break();
                return false;
            }

            DataTemplateID = dataID;
            ProjectileData = projectileData;
            Owner = owner as Creature;
            Skill = Owner.CreatureSkill.FindSkill(dataID);

            SpriteRenderer spr = GetComponent<SpriteRenderer>();
            Sprite sprite = Managers.Resource.Load<Sprite>(projectileData.Body);
            if (sprite != null)
            {
                spr.sprite = sprite;
                if (ColorUtility.TryParseHtmlString(projectileData.BodyColor, out Color bodyColor))
                    spr.color = bodyColor;
                // 이미 BaseObject의 SortingGroup으로 layer 정렬 중임
                //spr.sortingOrder = ReadOnly.Numeric.SortingLayer_Projectile;
            }

            switch (projectileData.ProjectileSize)
            {
                case EObjectSize.None:
                    break;

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
            }

            Collider.radius = projectileData.ColliderRadius;
            
            LayerMask excludeLayerMask = 0;
            excludeLayerMask.AddLayer(ELayer.Default);
            excludeLayerMask.AddLayer(ELayer.Projectile);
            excludeLayerMask.AddLayer(ELayer.Env);
            excludeLayerMask.AddLayer(ELayer.Obstacle);

            if (Owner.ObjectType == EObjectType.Hero)
                excludeLayerMask.AddLayer(ELayer.Hero);
            else if (Owner.ObjectType == EObjectType.Monster)
                excludeLayerMask.AddLayer(ELayer.Monster);

            Collider.excludeLayers = excludeLayerMask;

            switch (projectileData.MotionType)
            {
                case EProjectileMotionType.Parabola:
                    ProjectileMotion = gameObject.AddComponent<ParabolaMotion>();
                    break;

                case EProjectileMotionType.Straight:
                    ProjectileMotion = gameObject.AddComponent<StraightMotion>();
                    break;
            }

            SetMotionInfo(owner, dataID);
            return true;
        }

        protected virtual void EnterInGame(BaseObject owner, int dataID) // virtual -- TEMP
        {
            LayerMask excludeLayerMask = Collider.excludeLayers;
            if (Owner.ObjectType != owner.ObjectType)
            {
                Owner = owner as Creature;
                Skill = Owner.CreatureSkill.FindSkill(dataID);

                // Player to Monster
                if (owner.ObjectType == EObjectType.Monster)
                {
                    excludeLayerMask.RemoveLayer(ELayer.Hero);
                    excludeLayerMask.AddLayer(ELayer.Monster);
                }
                // Monster to Player
                else if (owner.ObjectType == EObjectType.Hero)
                {
                    excludeLayerMask.RemoveLayer(ELayer.Monster);
                    excludeLayerMask.AddLayer(ELayer.Hero);
                }

                Collider.excludeLayers = excludeLayerMask;
            }

            SetMotionInfo(owner, dataID);
        }

        private void SetMotionInfo(BaseObject owner, int dataID)
        {
            //ProjectileData = Managers.Data.ProjectileDataDict[dataID];
            //ProjectileMotionType = Util.GetEnumFromString<EProjectileMotionType>(ProjectileData.MotionType);
            switch (ProjectileData.MotionType)
            {
                case EProjectileMotionType.Straight:
                    ProjectileMotion.SetEndCallback(() => 
                    {
                        Managers.Object.Despawn(this, dataID);
                    });
                    (ProjectileMotion as StraightMotion).SetInfo(dataID, owner);
                    break;

                case EProjectileMotionType.Parabola:
                    ProjectileMotion.SetEndCallback(() => 
                    {
                        Managers.Object.Despawn(this, dataID);
                    });
                    (ProjectileMotion as ParabolaMotion).SetInfo(dataID, owner);
                    break;
            }

            StartCoroutine(CoLifeTime(ReadOnly.Util.ProjectileLifeTime));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            BaseObject target = other.GetComponent<BaseObject>();
            if (target.IsValid() == false)
                return;

            target.OnDamaged(Owner, Skill);
            Managers.Object.Despawn(this, DataTemplateID);
        }

        private IEnumerator CoLifeTime(float duration)
        {
            yield return new WaitForSeconds(duration);
            if (this.IsValid())
            {
                Managers.Object.Despawn(this, DataTemplateID);
            }
        }
    }
}
