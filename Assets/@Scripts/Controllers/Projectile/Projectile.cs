using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Projectile : BaseObject
    {
        public Creature Owner { get; private set; } = null;
        public SkillBase Skill { get; private set; } = null;
        public Data.ProjectileData ProjectileData { get; private set; } = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Projectile;
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
            return true;
        }

        protected override void EnterInGame()
        {
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

            // Set Projectile Movement Motion
        }
    }
}

