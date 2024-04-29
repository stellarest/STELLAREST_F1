using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1.Data;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Projectile : BaseObject
    {
        public Creature Owner { get; private set; } = null;
        public SkillBase Skill { get; private set; } = null;
        public Data.ProjectileData ProjectileData { get; private set; } = null;
        public ProjectileMotionBase ProjectileMotion { get; private set; } = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Projectile;
            Collider.isTrigger = true;
            SortingGroup.sortingOrder = ReadOnly.Numeric.SortingLayer_Projectile;
            return true;
        }

        public override bool SetInfo(int dataID)
        {
            if (base.SetInfo(dataID) == false)
            {
                EnterInGame();
                return false;
            }

            if (Managers.Data.ProjectileDataDict.TryGetValue(dataID, out Data.ProjectileData projectileData) == false)
            {
                Debug.LogError($"{nameof(Projectile)}, {nameof(SetInfo)}, Input : \"{dataID}\"");
                return false;
            }

            ProjectileData = projectileData;
            Managers.Sprite.SetInfo(dataID, target: this);
            Type classType = Util.GetTypeFromClassName(projectileData.Type);
            // ******************************************************************************************
            ProjectileMotion = gameObject.AddComponent(classType) as ProjectileMotionBase;
            if (ProjectileMotion == null)
            {
                Debug.LogError($"{nameof(Projectile)}, {nameof(SetInfo)}, Input : \"{projectileData.Type}\"");
                return false;
            }
            // ******************************************************************************************
            EProjectileMotionType motionType = Util.GetEnumFromString<EProjectileMotionType>(projectileData.Type);
            if (motionType == EProjectileMotionType.None || ProjectileMotion.MotionType == EProjectileMotionType.Max)
            {
                Debug.LogError($"{nameof(Projectile)}, {nameof(SetInfo)}, Input : \"{ProjectileMotion.MotionType}\"(Invalid motion type)");
                return false;
            }
            ProjectileMotion.MotionType = motionType;

            EnterInGame();
            return true;
        }

        protected override void EnterInGame()
        {
            // 뇌피셜 : 프로젝타일을 발사하는 주체마다 Owner가 달라질 수 있다. 오브젝트가 풀링되어있기 때문.
        }

        public void SetSpawnInfo(Creature owner, SkillBase skill, LayerMask excludeLayerMask)
        {
            Owner = owner;
            Skill = skill;
            switch(owner.ObjectType)
            {
                case EObjectType.Hero:
                    excludeLayerMask.AddLayer(ELayer.Hero);
                    break;

                case EObjectType.Monster:
                    excludeLayerMask.AddLayer(ELayer.Monster);
                    break;
            }
            Collider.excludeLayers = excludeLayerMask;

            // ## DO PROJECTILE MOTION HERE ###
            switch (ProjectileMotion.MotionType)
            {
                case EProjectileMotionType.StraightMotion:
                    (ProjectileMotion as StraightMotion).
                                    SetMotion(DataTemplateID,
                                            owner.CenterPosition,
                                            owner.Target.CenterPosition,
                                            () => {
//                                                Managers.Object.Despawn(this, DataTemplateID);
                                            });
                    break;

                case EProjectileMotionType.ParabolaMotion:
                    (ProjectileMotion as ParabolaMotion).SetMotion(DataTemplateID,
                                            owner.CenterPosition,
                                            owner.Target.CenterPosition,
                                            () => { 
                                                //Managers.Object.Despawn(this, DataTemplateID);
                                            });
                    break;
            }

            // 꼬불꼬불 미사일 만들어보기
            StartCoroutine(CoLifeTime(ReadOnly.Numeric.ProjectileLifeTime)); // FIXED
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
                Managers.Object.Despawn(this, DataTemplateID);
        }
    }
}

