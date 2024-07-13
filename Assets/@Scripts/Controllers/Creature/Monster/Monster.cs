using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Monster : Creature
    {
        #region Background
        public Data.MonsterData MonsterData { get; private set; } = null;
        [SerializeField] private MonsterBody _monsterBody = null;
        public MonsterBody MonsterBody
        {
            get => _monsterBody;
            private set
            {
                _monsterBody = value;
                if (CreatureBody == null)
                    CreatureBody = _monsterBody;
            }
        }
        public MonsterAnimation MonsterAnim { get; private set; } = null;
        public EMonsterType MonsterType { get; private set; } = EMonsterType.None;

        public override ECreatureAIState CreatureAIState
        {
            get => base.CreatureAIState;
            set
            {
                base.CreatureAIState = value;
                switch (value)
                {

                    case ECreatureAIState.Idle:
                        {
                            Moving = false;
                            return;
                        }

                    case ECreatureAIState.Move:
                        {
                            Moving = true;
                            return;
                        }

                    case ECreatureAIState.Dead:
                        {
                            Dead();
                            return;
                        }
                }
            }
        }
        public MonsterAI MonsterAI { get; private set; } = null;
        #endregion

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Monster;
            MonsterAnim = CreatureAnim as MonsterAnimation;
            MonsterBody = GetComponent<MonsterBody>();
            return true;
        }

        public override bool SetInfo(int dataID)
        {
            // --- EnterInGame from BaseObject
            if (base.SetInfo(dataID) == false)
                return false;

            InitialSetInfo(dataID);
            EnterInGame();
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            _maxLevel = dataID;

            MonsterData = Managers.Data.MonsterDataDict[dataID];
            MonsterAnim.SetInfo(dataID, this);
            MonsterBody.SetInfo(dataID, this);
            //MonsterAnim = CreatureAnim as MonsterAnimation;
            //Managers.Sprite.SetInfo(dataID, target: this);

            Type aiClassType = Util.GetTypeFromName(MonsterData.AIClassName);
            CreatureAI = gameObject.AddComponent(aiClassType) as CreatureAI;
            CreatureAI.SetInfo(this);
            MonsterAI = CreatureAI as MonsterAI;
            MonsterType = MonsterData.MonsterType;

            CreatureRarity = MonsterData.CreatureRarity;
            gameObject.name += $"_{MonsterData.NameTextID.Replace(" ", "")}";
            Collider.radius = MonsterData.ColliderRadius;

            CreatureSkill = gameObject.GetOrAddComponent<SkillComponent>();
            CreatureSkill.SetInfo(owner: this, MonsterData);
        }

        protected override void EnterInGame()
        {
            MonsterBody.MonsterEmoji = EMonsterEmoji.Normal;
            // --- Default Monsters Dir: Left
            LookAtDir = ELookAtDirection.Left;
            base.EnterInGame();
            CreatureAIState = ECreatureAIState.Idle;
        }

        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            base.OnDamaged(attacker, skillFromAttacker);
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            base.OnDead(attacker, skillFromAttacker);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
        }
        #endregion
    }
}
