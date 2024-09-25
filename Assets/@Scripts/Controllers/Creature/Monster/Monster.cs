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
        // public override ECreatureAIState CreatureAIState
        // {
        //     get => base.CreatureAIState;
        //     set
        //     {
        //         base.CreatureAIState = value;
        //         switch (value)
        //         {
        //             case ECreatureAIState.Idle:
        //                 {
        //                     Moving = false;
        //                     return;
        //                 }

        //             case ECreatureAIState.Move:
        //                 {
        //                     Moving = true;
        //                     return;
        //                 }

        //             case ECreatureAIState.Dead:
        //                 {
        //                     Dead();
        //                     return;
        //                 }
        //         }
        //     }
        // }

        public override BaseCellObject Target
        {
            get
            {
                if (this.IsValid() == false)
                    return null;

                BaseCellObject target = base.Target;
                // --- base에서 null을 리턴하므로 null 체크를 먼저 해야함
                if (target != null && target.IsValid())
                    MonsterBody.MonsterEmoji = EMonsterEmoji.Angry;
                else
                    MonsterBody.MonsterEmoji = EMonsterEmoji.Normal;

                return target;
            }
        }

        #region Core
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
#if UNITY_EDITOR
            Dev_NameTextID = MonsterData.Dev_NameTextID;
#endif

            // --- BaseStat
            // _maxLevelID = dataID;

            MonsterType = MonsterData.MonsterType;
            gameObject.name += $"_{MonsterData.Dev_NameTextID.Replace(" ", "")}_{SpawnNumber++}";
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            MonsterBody.MonsterEmoji = EMonsterEmoji.Normal;
            LookAtDir = ELookAtDirection.Left; // --- Default Monsters Dir: Left
            CreatureAIState = ECreatureAIState.Idle;

            base.EnterInGame(spawnPos);
            MonsterBody.StartCoFadeInEffect(startCallback: () =>
            {
                BaseEffect.GenerateEffect(
                            effectID: ReadOnly.DataAndPoolingID.DNPID_Effect_Global_TeleportPurple,
                            spawnPos: Managers.Map.CellToCenteredWorld(Vector3Int.up + SpawnedCellPos)
                            );
            });
        }

        // public override void OnDamaged(BaseCellObject attacker, SkillBase skillFromAttacker)
        // {
        //     base.OnDamaged(attacker, skillFromAttacker);
        //     HitShakeMovement(duration: 0.05f, power: 0.5f, vibrato: 10); // --- TEMP
        // }

        public override void OnDead(BaseCellObject attacker, SkillBase skillFromAttacker)
            => base.OnDead(attacker, skillFromAttacker);

        // public override void LevelUp()
        // {
        //     if (this.IsValid() == false)
        //         return;

        //     if (IsMaxLevel)
        //     {
        //         Debug.Log($"<color=magenta>MAX LEVEL !!{gameObject.name}</color>");
        //         return;
        //     }

        //     _levelID = Mathf.Clamp(_levelID + 1, DataTemplateID, _maxLevelID);
        //     Debug.Log($"<color=white>Lv: {Level} / MaxLv: {MaxLevel}</color>");
        //     if (Managers.Data.MonsterStatDataDict.TryGetValue(key: _levelID, value: out MonsterStatData statData))
        //     {
        //         SetStat(_levelID);
        //         CreatureSkill.LevelUpSkill(ownerLevelID: _levelID);
        //     }
        // }

        protected override void OnDisable()
            => base.OnDisable();
        #endregion Core

        #region Background
        #endregion Background
    }
}
