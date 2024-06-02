using System;
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
        public Creature Owner { get; private set; } = null;
        public SkillBase Skill { get; private set; } = null;
        public Data.ProjectileData ProjectileData { get; private set; } = null;
        public ProjectileMotionBase ProjectileMotion { get; private set; } = null;
        public EProjectileMotionType ProjectileMotionType { get; private set; } = EProjectileMotionType.None;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Projectile;
            Collider.isTrigger = true;
            SortingGroup.sortingOrder = ReadOnly.Numeric.SortingLayer_Projectile;
            return true;
        }

        public override bool SetInfo(BaseObject owner, int dataID)
        {
            if (base.SetInfo(owner, dataID) == false)
            {
                EnterInGame(owner, dataID);
                return false;
            }

            if (Managers.Data.ProjectileDataDict.TryGetValue(dataID, out Data.ProjectileData projectileData) == false)
            {
                Util.LogError($"{nameof(Projectile)}, {nameof(SetInfo)}, Input : \"{dataID}\"");
                return false;
            }

            DataTemplateID = dataID;
            ProjectileData = projectileData;
            Owner = owner as Creature;
            Skill = Owner.CreatureSkill.FindSkill(dataID);
            Managers.Sprite.SetInfo(dataID, target: this);

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

            Type classType = Util.GetTypeFromName(projectileData.Type);
            ProjectileMotion = gameObject.AddComponent(classType) as ProjectileMotionBase;
            if (ProjectileMotion == null)
            {
                Util.LogError($"{nameof(Projectile)}, {nameof(SetInfo)}, Input : \"{projectileData.Type}\"");
                return false;
            }

            SetMotionInfo(owner, dataID);
            return true;
        }

        protected override void EnterInGame(BaseObject owner, int dataID)
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
            //ProjectileMotion.SetInfo(owner, dataID);
            //ProjectileMotion.StartMotion();
        }

        // public void SetSpawnInfo(Creature owner, Vector3 spawnPosition, SkillBase skill, LayerMask excludeLayerMask)
        // {
        //     Owner = owner;
        //     Skill = skill;
        //     switch(owner.ObjectType)
        //     {
        //         case EObjectType.Hero:
        //             excludeLayerMask.AddLayer(ELayer.Hero);
        //             break;

        //         case EObjectType.Monster:
        //             excludeLayerMask.AddLayer(ELayer.Monster);
        //             break;
        //     }
        //     Collider.excludeLayers = excludeLayerMask;

        //     // ### DO PROJECTILE MOTION HERE ###
        //     switch (ProjectileMotion.MotionType)
        //     {
        //         case EProjectileMotionType.StraightMotion:
        //             (ProjectileMotion as ParabolaMotion).SetMotion(startPos: spawnPosition, targetPos: owner.Target.CenterPosition,
        //                                                             motionType: EProjectileMotionType.None, animCurveType: EAnimationCurveType.None,
        //                                                             hasTargetToRotate: true, movementSpeed: 0f, atkRange: 0f, endCallback: () => {
        //                                                                 Managers.Object.Despawn(this, DataTemplateID);
        //                                                             });
        //             break;

        //         case EProjectileMotionType.ParabolaMotion:
        //             (ProjectileMotion as ParabolaMotion).SetMotion(startPos: spawnPosition, targetPos: owner.Target.CenterPosition,
        //                                                             motionType: EProjectileMotionType.None, animCurveType: EAnimationCurveType.None,
        //                                                             hasTargetToRotate: true, movementSpeed: 0f, atkRange: 0f, endCallback: () => {
        //                                                                 Managers.Object.Despawn(this, DataTemplateID);
        //                                                             });
        //             break;
        //     }

        //     //StartCoroutine(CoLifeTime(ReadOnly.Numeric.ProjectileLifeTime)); // FIXED
        // }

        private void SetMotionInfo(BaseObject owner, int dataID)
        {
            ProjectileData = Managers.Data.ProjectileDataDict[dataID];
            ProjectileMotionType = Util.GetEnumFromString<EProjectileMotionType>(ProjectileData.Type);
            switch (ProjectileMotionType)
            {
                case EProjectileMotionType.StraightMotion:
                    ProjectileMotion.SetEndCallback(() => 
                    {
                        Managers.Object.Despawn(this, dataID);
                    });
                    (ProjectileMotion as StraightMotion).SetInfo(owner, dataID);
                    break;

                case EProjectileMotionType.ParabolaMotion:
                    ProjectileMotion.SetEndCallback(() => 
                    {
                        Managers.Object.Despawn(this, dataID);
                    });
                    (ProjectileMotion as ParabolaMotion).SetInfo(owner, dataID);
                    break;
            }

            StartCoroutine(CoLifeTime(ReadOnly.Numeric.ProjectileLifeTime));
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

/*

            // // ******************************************************************************************
            // EProjectileMotionType motionType = Util.GetEnumFromString<EProjectileMotionType>(projectileData.Type);
            // if (motionType <= EProjectileMotionType.None || ProjectileMotion.MotionType >= EProjectileMotionType.Max)
            // {
            //     Debug.LogError($"{nameof(Projectile)}, {nameof(SetInfo)}, Input : \"{ProjectileMotion.MotionType}\"(Invalid motion type)");
            //     return false;
            // }
            // ProjectileMotion.MotionType = motionType;

            // EAnimationCurveType curveType = Util.GetEnumFromString<EAnimationCurveType>(projectileData.MotionCurveType);
            // if (curveType <= EAnimationCurveType.None || curveType >= EAnimationCurveType.Max)
            // {
            //     Debug.LogError($"{nameof(Projectile)}, {nameof(SetInfo)}, Input : \"{curveType}\"(Invalid curve type)");
            //     return false;
            // }
            // ProjectileMotion.MotionCurveType = curveType;

*/