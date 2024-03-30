using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Monster : Creature
    {
        public Data.MonsterData MonsterData { get; private set;} = null;
        public MonsterBody MonsterBody { get; private set; } = null;
        public MonsterAnimation MonsterAnim { get; private set; } = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Monster;
            return true;
        }

        public override void SetInfo(int dataID)
        {
            base.SetInfo(dataID);
            if (MonsterAnim == null && MonsterBody == null)
            {
                MonsterBody = new MonsterBody(this, dataID);
                MonsterAnim = CreatureAnim as MonsterAnimation;
                MonsterAnim.SetInfo(dataID, this);
                Managers.Sprite.SetInfo(dataID, target: this);
                SetCreatureFromData(dataID);
            }

            RefreshCreature();
        }

        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.E))
        //     {
        //         MonsterBody.SetEmoji(EMonsterEmoji.Default);
        //     }

        //     if (Input.GetKeyDown(KeyCode.R))
        //     {
        //         MonsterBody.SetEmoji(EMonsterEmoji.Angry);
        //     }

        //     if (Input.GetKeyDown(KeyCode.T))
        //     {
        //         MonsterBody.SetEmoji(EMonsterEmoji.Dead);
        //     }
        // }

        protected override void RefreshCreature()
        {
            base.RefreshCreature();
            Speed = MonsterData.MovementSpeed;
        }

        protected override void SetCreatureFromData(int dataID)
        {
            MonsterData = Managers.Data.MonsterDataDict[dataID];
            gameObject.name += $"_{MonsterData.DescriptionTextID}";
            Collider.radius = MonsterData.ColliderRadius;
            Speed = MonsterData.MovementSpeed;
            
            /*
                TODO : Set Monster Stat..
            */
        }
    }
}
