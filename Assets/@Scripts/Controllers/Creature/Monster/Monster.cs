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
                BaseEffect.GenerateEffect(
                            effectID: ReadOnly.DataAndPoolingID.DNPID_Effect_TeleportPurple,
                            spawnPos: Managers.Map.CellToCenteredWorld(Vector3Int.up + SpawnedCellPos)
                            );
            });
        }

        public override bool OnDamaged(BaseCellObject attacker, SkillBase skillByAttacker)
        {
            if (base.OnDamaged(attacker, skillByAttacker) == false)
                return false;

            float damage = UnityEngine.Random.Range(attacker.MinAtk, attacker.MaxAtk);
            float finalDamage = Mathf.FloorToInt(damage);
            // --- 몬스터도 ShieldHp Buff 가능
            if (ShieldHp > 0.0f)
            {
                OnDamagedShieldHp(finalDamage);
                return true;
            }

            // --- TEMP CRITICAL ---
            bool isCritical = UnityEngine.Random.Range(0, 2) == 0 ? true : false;
            if (isCritical)
                finalDamage *= 1 + 0.5f;

            Hp = Mathf.Clamp(Hp - finalDamage, 0f, MaxHp);
            List<EffectBase> hitEffects = skillByAttacker.GenerateSkillEffects(
                                                    effectIDs: skillByAttacker.SkillData.HitEffectIDs,
                                                    spawnPos: Util.GetRandomQuadPosition(this.CenterPosition)
                                                    );

            Managers.Object.ShowDamageFont(
                                                position: CenterPosition,
                                                damage: finalDamage,
                                                textColor: Color.white,
                                                isCritical: isCritical,
                                                fontSignType: EFontSignType.None,
                                                EFontAnimationType.EndFalling
                                          );

            // --- 몬스터에게도 Critical Rate를 줘야할지?
            if (isCritical)
            {
                // Managers.Object.ShowImpactCriticalHit(CenterPosition, this);
                BaseEffect.GenerateEffect(effectID: ReadOnly.DataAndPoolingID.DNPID_Effect_ImpactCriticalHit,
                                          skill: null);
                Managers.Object.ShowTextFont(
                               position: CenterPosition + Vector3.up * 0.65f,
                               text: "CRITICAL",
                               textSize: 5.0f,
                               textColor: Color.red,
                               fontAssetType: EFontAssetType.Comic,
                               fontAnimType: EFontAnimationType.EndFallingShake
                           );
            }

            if (Hp <= 0f)
            {
                Hp = 0f;
                OnDead(attacker, skillByAttacker);
            }
            else
                BaseBody.StartCoHurtFlashEffect(isCritical: isCritical);

            HitShakeMovement(duration: 0.05f, power: 0.5f, vibrato: 10); // --- TEMP
            return true;
        }

        public override void OnDamagedShieldHp(float finalDamage)
        {
            ShieldHp = Mathf.Clamp(ShieldHp - finalDamage, 0.0f, ShieldHp);
            if (ShieldHp == 0.0f)
                BaseEffect.ExitShowBuffEffects(EEffectBuffType.ShieldHp);
            else
            {
                BaseEffect.OnShowBuffEffects(EEffectBuffType.ShieldHp);
                Managers.Object.ShowDamageFont(
                                position: CenterPosition,
                                damage: finalDamage,
                                textColor: Color.blue,
                                isCritical: false, // --- Shield는 치명타 면역
                                fontSignType: EFontSignType.Minus,
                                fontAnimFunc: () =>
                                {
                                    return UnityEngine.Random.Range(0, 2) == 0 ?
                                                EFontAnimationType.EndBouncingLeftUp :
                                                EFontAnimationType.EndBouncingRightUp;
                                });
            }
        }

        // public override void OnDamaged(BaseCellObject attacker, SkillBase skillFromAttacker)
        // {
        //     base.OnDamaged(attacker, skillFromAttacker);
        //     HitShakeMovement(duration: 0.05f, power: 0.5f, vibrato: 10); // --- TEMP
        // }

        public override void OnDead(BaseCellObject attacker, SkillBase skillFromAttacker)
            => base.OnDead(attacker, skillFromAttacker);

        public override void LevelUp()
        {
            if (this.IsValid() == false)
                return;

            if (IsMaxLevel)
            {
                Debug.Log($"<color=magenta>MAX LEVEL !!{gameObject.name}</color>");
                return;
            }

            _levelID = Mathf.Clamp(_levelID + 1, DataTemplateID, _maxLevelID);
            Debug.Log($"<color=white>Lv: {Level} / MaxLv: {MaxLevel}</color>");
            if (Managers.Data.MonsterStatDataDict.TryGetValue(key: _levelID, value: out MonsterStatData statData))
            {
                SetStat(_levelID);
                CreatureSkill.LevelUpSkill(ownerLevelID: _levelID);
            }
        }

        protected override void OnDisable()
            => base.OnDisable();
        #endregion Core

        #region Background
        #endregion Background
    }
}
