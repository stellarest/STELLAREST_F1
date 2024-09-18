using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Linq;
using UnityEngine;
using STELLAREST_F1.Data;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Hero : Creature
    {
        #if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (LevelUp() == false)
                    Debug.LogWarning($"Faield to LvUp, IsMaxLv: {IsMaxLevel}");
                // BaseStat.LevelUp();
            }
        }
        #endif

        public HeroData HeroData { get; private set; } = null;
        public HeroAnimation HeroAnim { get; private set; } = null;
        public HeroAI HeroAI { get; private set; } = null;
        [SerializeField] private HeroBody _heroBody = null;
        public HeroBody HeroBody
        {
            get => _heroBody;
            private set
            {
                _heroBody = value;
                if (CreatureBody == null)
                    CreatureBody = value;
            }
        }

        [SerializeField] private bool _isLeader = false;
        public bool IsLeader
        {
            get => _isLeader;
            set
            {
                if (_isLeader == value)
                    return;

                _isLeader = value;
                if (value)
                    StopCoLerpToCellPos();
                else
                    StartCoLerpToCellPos();
            }
        }

        [SerializeField] private EHeroWeaponType _heroWeaponType = EHeroWeaponType.Default;
        public EHeroWeaponType HeroWeaponType
        {
            get => _heroWeaponType;
            set
            {
                if (_heroWeaponType != value)
                {
                    _heroWeaponType = value;
                    HeroBody.ChangeWeaponType(value);
                }
            }
        }

        public override bool Moving
        {
            get => base.Moving;
            set
            {
                base.Moving = value;
                if (value && HeroWeaponType != EHeroWeaponType.Default)
                    HeroWeaponType = EHeroWeaponType.Default;
            }
        }

        public bool CanCollectEnv
        {
            get
            {
                // if (CreatureAnim.CanEnterAnimState(ECreatureAnimState.Upper_Idle_To_CollectEnv) == false)
                //     return false;

                if (Moving)
                {
                    if (HeroWeaponType != EHeroWeaponType.Default)
                        HeroWeaponType = EHeroWeaponType.Default;
                    return false;
                }

                if (this.IsValid() == false)
                {
                    if (HeroWeaponType != EHeroWeaponType.Default)
                        HeroWeaponType = EHeroWeaponType.Default;

                    return false;
                }

                if (Target.IsValid() == false)
                {
                    if (HeroWeaponType != EHeroWeaponType.Default)
                        HeroWeaponType = EHeroWeaponType.Default;

                    return false;
                }

                if (Target.IsValid() && Target.ObjectType != EObjectType.Env)
                {
                    if (HeroWeaponType != EHeroWeaponType.Default)
                        HeroWeaponType = EHeroWeaponType.Default;

                    return false;
                }

                if (HeroAnim.CanEnterAnimState(ECreatureAnimState.Upper_CollectEnv) == false)
                    return false;

                // if (ForceMove)
                // {
                //     if (HeroWeaponType != EHeroWeaponType.Default)
                //         HeroWeaponType = EHeroWeaponType.Default;

                //     return false;
                // }

                int dx = Mathf.Abs(Target.CellPos.x - CellPos.x);
                int dy = Mathf.Abs(Target.CellPos.y - CellPos.y);
                if (dx <= 1 && dy <= 1)
                    return true;

                return false;
            }
        }

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Hero;
            HeroBody = CreatureBody as HeroBody;
            HeroAnim = CreatureAnim as HeroAnimation;
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            HeroAI = CreatureAI as HeroAI;
            HeroData = Managers.Data.HeroDataDict[dataID];
#if UNITY_EDITOR
            Dev_TextID = HeroData.DevTextID;
#endif
            // --- BaseStat
            // for (int i = DataTemplateID; i < DataTemplateID + ReadOnly.Util.HeroMaxLevel;)
            //     _maxLevelID = i++;

            gameObject.name += $"_{HeroData.DevTextID.Replace(" ", "")}";
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            HeroBody.HeroEmoji = EHeroEmoji.Idle;
            LookAtDir = ELookAtDirection.Right; // --- Default Heroes Dir: Right
            CreatureAIState = ECreatureAIState.Move;

            base.EnterInGame(spawnPos);
            HeroBody.StartCoFadeInEffect(startCallback: () =>
            {
                BaseEffect.GenerateEffect(
                            effectID: ReadOnly.DataAndPoolingID.DNPID_Effect_TeleportBlue,
                            spawnPos: Managers.Map.CellToCenteredWorld(Vector3Int.up + SpawnedCellPos)
                            );
            });
            StartCoroutine(CoInitialReleaseLeaderHeroAI());
        }

        public override void OnDead(BaseCellObject attacker, SkillBase skillFromAttacker)
        {
            if (IsLeader)
            {
                Managers.Object.HeroLeaderController.EnableLeaderMark(false);
                Managers.Object.HeroLeaderController.EnablePointer(false);
            }

            base.OnDead(attacker, skillFromAttacker);
        }
        #endregion Core

        #region Background
        // --- Comment
        protected override float ApplyFinalStat(float baseValue, EApplyStatType applyStatType)
        {
            float value = baseValue;

            // --- Shield는 약간 예외다.
            if (applyStatType == EApplyStatType.BonusHealth)
            {
                // --- 예를 들어, 현재 최대 체력이 1000이고, "최대 체력의 10% 만큼에 해당하는 쉴드 에너지를 얻는다"라고 했을 때
                value *= 1 + BaseEffect.ApplyStatModifier(applyStatType, EStatModType.AddPercent);
                // --- 100의 쉴드 에너지를 얻게 된다.
                value = Mathf.Clamp(value - baseValue, 0.0f, baseValue);
                // --- 이후 AddAmount가 되는 양이 있다면, 그것을 고정적으로 더해준다.
                value += BaseEffect.ApplyStatModifier(applyStatType, EStatModType.AddAmount);

                // --- 최종 설명 예시
                // 스킬 설명: 최대 체력의 10%만큼 해당하는 쉴드 에너지를 얻는다.
                // 아이템 설명: 275의 쉴드 에너지를 추가로 얻는다.
                // ---> 1000(MaxHpBase), Shield Percent(0.1), Result: 100 + AddAmount(250) = 375
                
                // 아니면 애초에 AddPercent가 아닌 그냥 Percent 옵션도 추가해서 플래그를 통해
                // Percent의 경우에는 Percent 또는 AddPercent 둘 중에 하나만 적용이 가능하도록 해도 될듯.
            }
            else
            {
                // --- 기본적인 순서는 +부터 하는 것이 맞다고 봄
                value += BaseEffect.ApplyStatModifier(applyStatType, EStatModType.AddAmount);
                value *= 1 + BaseEffect.ApplyStatModifier(applyStatType, EStatModType.AddPercent);
            }

            return value;
        }

        public override void LerpToCellPos(float movementSpeed)
        {
            if (IsLeader)
                return;

            Hero leader = Managers.Object.HeroLeaderController.Leader;
            if (LerpToCellPosCompleted && leader.Moving == false)
            {
                Moving = false;
                return;
            }

            Vector3 destPos = Managers.Map.CellToCenteredWorld(CellPos); // 이동은 가운데로.
            Vector3 dir = destPos - transform.position;

            if (dir.x < 0f)
                LookAtDir = ELookAtDirection.Left;
            else if (dir.x > 0f)
                LookAtDir = ELookAtDirection.Right;

            if (dir.sqrMagnitude < 0.001f)
            {
                transform.position = destPos;
                LerpToCellPosCompleted = true;
                return;
            }

            Moving = true;
            float moveDist = Mathf.Min(dir.magnitude, movementSpeed * Time.deltaTime);
            transform.position += dir.normalized * moveDist;
        }

        protected override void OnDeadFadeOutCompleted()
        {
            if (IsLeader)
                Managers.Game.ChangeHeroLeader(autoChangeFromDead: true);

            base.OnDeadFadeOutCompleted();
        }

        public override bool LevelUp()
        {
            if (BaseStat.LevelUp() == false)
                return false;

            CreatureSkill.LevelUpSkill(ownerLevelID: BaseStat.LevelID);
            if (IsMaxLevel)
            {
                Debug.Log($"<color=yellow>MaxUp Hero</color>");
                CreatureRarity = ECreatureRarity.Elite; // --- 이거 바꿔야할듯.
                HeroBody.ChangeSpriteSet(Managers.Data.HeroSpriteDataDict[BaseStat.LevelID]);
            }

            return true;

            // ---------- PREV ----------
            // if (this.IsValid() == false)
            //     return;

            // if (IsMaxLevel)
            // {
            //     Debug.Log($"<color=magenta>MAX LEVEL !!{gameObject.name}</color>");
            //     return;
            // }

            // _levelID = Mathf.Clamp(_levelID + 1, DataTemplateID, _maxLevelID);
            // Debug.Log($"<color=white>Lv: {Level} / MaxLv: {MaxLevel}</color>");
            // if (Managers.Data.HeroStatDataDict.TryGetValue(key: _levelID, value: out HeroStatData statData))
            // {
            //     SetStat(_levelID);
            //     CreatureSkill.LevelUpSkill(ownerLevelID: _levelID);
            // }

            // if (IsMaxLevel)
            // {
            //     Debug.Log("<color=yellow>CHANGE: LEGENDARY HERO</color>");
            //     CreatureRarity = ECreatureRarity.Elite;
            //     HeroBody.ChangeSpriteSet(Managers.Data.HeroSpriteDataDict[_levelID]);
            // }
        }
        
        private IEnumerator CoInitialReleaseLeaderHeroAI()
        {
            // 여기서 하면 안됨... Leader랑 관련 있는듯.
            // Debug.Log($"1 - Inactive: {HeroBody.GetContainer(EHeroWeapon.WeaponL_Armor).TR.gameObject.name}");
            // HeroBody.GetContainer(EHeroWeapon.WeaponL_Armor).TR.gameObject.SetActive(false);
            yield return new WaitUntil(() =>
            {
                bool allStartedHeroAI = true;
                for (int i = 0; i < Managers.Object.Heroes.Count; ++i)
                {
                    Hero hero = Managers.Object.Heroes[i];
                    if (hero._coUpdateAI == null)
                        allStartedHeroAI = false;
                }

                if (allStartedHeroAI)
                    return true;

                return false;
            });

            if (IsLeader)
            {
                StopCoUpdateAI();
                Debug.Log("<color=white>Initial Release Leader Hero's AI</color>");
                // 여기서 하니까 됨.. 이거 때문인가??
                // Debug.Log($"1 - Inactive: {HeroBody.GetContainer(EHeroWeapon.WeaponL_Armor).TR.gameObject.name}");
                // HeroBody.GetContainer(EHeroWeapon.WeaponL_Armor).TR.gameObject.SetActive(false);
            }
        }
        #endregion Background
    }
}
