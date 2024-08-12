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
        private MonsterAI _monsterAI = null;
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

        #region Init Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Monster;
            MonsterBody = CreatureBody as MonsterBody;
            MonsterAnim = CreatureAnim as MonsterAnimation;
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            _monsterAI = CreatureAI as MonsterAI;
            MonsterData = Managers.Data.MonsterDataDict[dataID];
            _maxLevel = dataID;

            MonsterType = MonsterData.MonsterType;
            gameObject.name += $"_{MonsterData.NameTextID.Replace(" ", "")}";
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            MonsterBody.MonsterEmoji = EMonsterEmoji.Normal;
            LookAtDir = ELookAtDirection.Left; // --- Default Monsters Dir: Left
            CreatureAIState = ECreatureAIState.Idle;
            
            base.EnterInGame(spawnPos);
            MonsterBody.StartCoFadeInEffect(startCallback: () => 
            {
                Managers.Object.SpawnBaseObject<EffectBase>(
                    objectType: EObjectType.Effect,
                    spawnPos: Managers.Map.GetCenterWorld(Vector3Int.up + SpawnedCellPos),
                    dataID: ReadOnly.DataAndPoolingID.DNPID_Effect_TeleportRed,
                    owner: this);
            });
        }
        #endregion

        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
            => base.OnDamaged(attacker, skillFromAttacker);
        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
            => base.OnDead(attacker, skillFromAttacker);
        protected override void OnDisable()
            => base.OnDisable();
    }
}
