using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_F1.Define;
using STELLAREST_F1.Data;

namespace STELLAREST_F1
{
    public class Monster : Creature
    {
        #if UNITY_EDITOR
        private static int SpawnNumber = 0;
        #endif

        public MonsterData MonsterData { get; private set; } = null;
        public MonsterStatData MonsterStatData { get; private set; } = null;
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

        public override BaseObject Target
        {
            get
            {
                if (this.IsValid() == false)
                    return null;

                BaseObject target = base.Target;
                // --- base에서 null을 리턴하므로 null 체크를 먼저 해야함
                if (target != null && target.IsValid())
                    MonsterBody.MonsterEmoji = EMonsterEmoji.Angry;
                else
                    MonsterBody.MonsterEmoji = EMonsterEmoji.Normal;

                return target;
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
            MonsterStatData = StatData as MonsterStatData;
            _maxLevelID = dataID;

            MonsterType = MonsterData.MonsterType;
            gameObject.name += $"_{MonsterData.DevTextID.Replace(" ", "")}_{SpawnNumber++}";
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
                    spawnPos: Managers.Map.CellToCenteredWorld(Vector3Int.up + SpawnedCellPos),
                    dataID: ReadOnly.DataAndPoolingID.DNPID_Effect_TeleportRed,
                    owner: this);
            });
        }
        #endregion

        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            base.OnDamaged(attacker, skillFromAttacker);
            HitShakeMovement(duration: 0.05f, power: 0.5f, vibrato: 10); // --- TEMP
        }
        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
            => base.OnDead(attacker, skillFromAttacker);
        protected override void OnDisable()
            => base.OnDisable();
    }
}
