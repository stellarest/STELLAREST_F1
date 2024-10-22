using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Linq;
using UnityEngine;
using STELLAREST_F1.Data;
using static STELLAREST_F1.Define;
using System.Buffers;

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
            HeroData = CreatureData as HeroData;
            gameObject.name += $"_{HeroData.Dev_NameTextID}";
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            HeroBody.HeroEmoji = EHeroEmoji.Idle;
            // --- Default Heroes Dir: Right
            LookAtDir = ELookAtDirection.Right;
            CreatureAIState = ECreatureAIState.Move;

            base.EnterInGame(spawnPos);
            HeroBody.StartCoFadeInEffect(startCallback: () =>
            {
                GenerateGlobalEffect(
                        globalEffectID: EGlobalEffectID.TeleportBlue,
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
            /*
                Passive: 101000(Lv.01), 101002(Lv.03), 101004(Lv.05), 101007(Lv.08)
                Skill_A: 101100(Lv.01), 101002(Lv.03), 101004(Lv.05), 101007(Lv.08)
                Skill_B: 101201(Lv.02), 101203(Lv.04), 101205(Lv.06), 102007(Lv.08)
                Skill_C: 101302(Lv.03), 101304(Lv.05), 101306(Lv.07), 103007(Lv.08)
            */

            // --- SKILL_A
            if (CreatureSkill.SkillArray[(int)ESkillType.Skill_A] == null)
                CreatureSkill.TryUnlockSkill(ESkillType.Skill_A);
            else if (CreatureSkill.SkillArray[(int)ESkillType.Skill_A] != null)
                CreatureSkill.TryLevelUpSkill(CreatureSkill.SkillArray[(int)ESkillType.Skill_A]);

            // --- SKILL_B
            if (CreatureSkill.SkillArray[(int)ESkillType.Skill_B] == null)
                CreatureSkill.TryUnlockSkill(ESkillType.Skill_B);
            else if (CreatureSkill.SkillArray[(int)ESkillType.Skill_B] != null)
                CreatureSkill.TryLevelUpSkill(CreatureSkill.SkillArray[(int)ESkillType.Skill_B]);

            // --- SKILL_C
            if (CreatureSkill.SkillArray[(int)ESkillType.Skill_C] == null)
                CreatureSkill.TryUnlockSkill(ESkillType.Skill_C);
            else if (CreatureSkill.SkillArray[(int)ESkillType.Skill_C] != null)
                CreatureSkill.TryLevelUpSkill(CreatureSkill.SkillArray[(int)ESkillType.Skill_C]);

            // --- Leader Skill (TODO, TEMP)
            // ApplyNewPassive();
            CreatureAnim.RefreshAnimEventHandlers();
            if (IsMaxLevel && Managers.Game.HasElitePackage)
            {
                Debug.Log($"<color=yellow>MaxUp(Elite) Hero</color>");
                HeroBody.ChangeSpriteSet(Managers.Data.HeroSpriteDataDict[BaseStat.LevelID]);
            }

            return true;
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
