using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Linq;
using STELLAREST_F1.Data;
using UnityEngine;
using UnityEngine.UIElements;
using static STELLAREST_F1.Define;
using Unity.VisualScripting;

namespace STELLAREST_F1
{
    public class Hero : Creature
    {
        // private void Update()
        // {
        //    // Debug.Log($"LookAtDir: {(int)LookAtDir}");
        //     if (Input.GetKeyDown("5"))
        //     {
        //         //LevelUp();
        //         // BaseEffect.GenerateEffect(ReadOnly.DataAndPoolingID.DNPID_Effect_TeleportRed, 
        //         //     Managers.Map.GetCenterWorld(Vector3Int.up + CellPos));
        //     }
        // }

        private void Update()
        {
            if (_coUpdateAI != null)
                Debug.Log("Valid AI TICK.");
        }

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

        protected override void OnDeadFadeOutCompleted()
        {
            if (IsLeader)
                Managers.Game.ChangeHeroLeader(autoChangeFromDead: true);

            base.OnDeadFadeOutCompleted();
        }

        public override bool CanCollectEnv
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

        public void LevelUp()
        {
            if (this.IsValid() == false)
                return;

            if (_level < _maxLevel)
            {
                _level = Mathf.Clamp(_level + 1, DataTemplateID, _maxLevel);
                Debug.Log($"<color=white>++LV: {Level}</color>");

                if (Managers.Data.StatDataDict.TryGetValue(_level, out Data.StatData statData))
                    SetStat(statData);
            }

            if (IsMaxLevel)
            {
                Debug.Log("<color=yellow>Try ChangeSpriteSet</color>");
                CreatureRarity = ECreatureRarity.Elite;
                HeroBody.ChangeSpriteSet(Managers.Data.HeroSpriteDataDict[_level]);
            }
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

        #region Init Core
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
            for (int i = DataTemplateID; i < DataTemplateID + ReadOnly.Util.HeroMaxLevel;)
            {
                if (Managers.Data.StatDataDict.ContainsKey(i) == false)
                    break;

                _maxLevel = i++;
            }

            gameObject.name += $"_{HeroData.DevTextID.Replace(" ", "")}";
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            HeroBody.HeroEmoji = EHeroEmoji.Idle;
            LookAtDir = ELookAtDirection.Right; // --- Default Heroes Dir: Right
            CreatureAIState = ECreatureAIState.Move;

            // Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
            // Managers.Game.OnJoystickStateChangedHandler += OnJoystickStateChanged;
            
            base.EnterInGame(spawnPos);
            StartCoroutine(CoInitialReleaseLeaderHeroAI());
        }
        #endregion Init Core

        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
            => base.OnDamaged(attacker, skillFromAttacker);
        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            if (IsLeader)
            {
                Managers.Object.HeroLeaderController.EnableLeaderMark(false);
                Managers.Object.HeroLeaderController.EnablePointer(false);
            }

            base.OnDead(attacker, skillFromAttacker);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            if (Managers.Game == null)
                return;
        }
    }
}

/*
        // private void OnJoystickStateChanged(EJoystickState joystickState)
        // {   
        //     if (this.IsValid() == false)
        //         return;

        //     switch (joystickState)
        //     {
        //         case EJoystickState.Drag:
        //             ForceMove = true;
        //             Managers.Object.HeroLeaderController.EnablePointer(true);
        //             break;

        //         case EJoystickState.PointerUp:
        //             ForceMove = false;
        //             Managers.Object.HeroLeaderController.EnablePointer(false);
        //             break;
        //     }
        // }
*/