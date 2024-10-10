using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STELLAREST_F1.Data;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Creature : BaseCellObject
    {
        public CreatureData CreatureData { get; protected set; } = null;
        public CreatureAI CreatureAI { get; protected set; } = null;
        public SkillComponent CreatureSkill { get; protected set; } = null; // Skills
        public CreatureBody CreatureBody { get; protected set; } = null;
        public CreatureAnimation CreatureAnim { get; private set; } = null;
        public CreatureAnimationCallback CreatureAnimCallback { get; private set; } = null;

        public bool CanSkill
        {
            get
            {
                if (this.IsValid() == false)
                {
                    CreatureAnim.ReadySkill = false;
                    return false;
                }

                if (Target.IsValid() == false)
                {
                    CreatureAnim.ReadySkill = false;
                    return false;
                }

                // --- 스킬이 진행중일 수도 있음
                if (CreatureAnim.CanSkillTrigger == false)
                    return false;

                SkillBase currentSkill = CreatureSkill.ReadyToActivate;
                if (currentSkill == null || currentSkill.RemainCoolTime > 0f)
                {
                    CreatureAnim.ReadySkill = false;
                    return false;
                }

                Vector3Int worldToCell = Managers.Map.WorldToCell(transform.position);
                Vector3Int targetWorldToCell = Managers.Map.WorldToCell(Target.transform.position);
                int dx = Mathf.Abs(worldToCell.x - targetWorldToCell.x);
                int dy = Mathf.Abs(worldToCell.y - targetWorldToCell.y);
                int invokeRange = currentSkill.InvokeRange;
                if (Target.ObjectType != EObjectType.Env)
                {
                    if (dx <= invokeRange && dy <= invokeRange)
                    {
                        CreatureSkill.CurrentSkillType = currentSkill.SkillType;
                        CreatureAnim.ReadySkill = true;
                        return true;
                    }
                }

                CreatureAnim.ReadySkill = false;
                return false;
            }
        }

        [SerializeField] private ECreatureAIState _creatureAIState = ECreatureAIState.Idle;
        public virtual ECreatureAIState CreatureAIState
        {
            get => _creatureAIState;
            set
            {
                if (_creatureAIState != value)
                {
                    _creatureAIState = value;
                    switch (value)
                    {
                        case ECreatureAIState.Idle:
                            // Debug.Log("111");
                            Moving = false;
                            break;

                        case ECreatureAIState.Move:
                            // Debug.Log("222");
                            Moving = true;
                            break;

                        case ECreatureAIState.Dead:
                            {
                                Dead();
                                CreatureAI.OnDead();
                            }
                            break;
                    }
                }
            }
        }

        public virtual bool Moving
        {
            get => CreatureAnim.Moving;
            set => CreatureAnim.Moving = value;
        }

        public bool ReadySkill
        {
            get => CreatureAnim.ReadySkill;
            set => CreatureAnim.ReadySkill = value;
        }

        public bool IsInTheNearestTarget
        {
            get
            {
                if (Target.IsValid() == false)
                    return false;

                int dx = Mathf.Abs(Target.CellPos.x - CellPos.x);
                int dy = Mathf.Abs(Target.CellPos.y - CellPos.y);
                if (Target.ObjectType == EObjectType.Monster || Target.ObjectType == EObjectType.Hero)
                {
                    float bestShortRange = float.MaxValue;
                    for (int i = 0; i < (int)ESkillType.Max; ++i)
                    {
                        SkillBase skill = CreatureSkill.SkillArray[i];
                        if (skill == null)
                            continue;

                        if (skill.InvokeRange < bestShortRange)
                            bestShortRange = skill.InvokeRange;
                    }

                    if (dx <= bestShortRange && dy <= bestShortRange)
                        return true;
                }
                else if (Target.ObjectType == EObjectType.Env)
                {
                    if (dx <= 1 && dy <= 1)
                        return true;
                }

                return false;
            }
        }

        public float TheShortestSkillInvokeRange
        {
            get
            {
                float bestShortRange = float.MaxValue;
                for (int i = 0; i < (int)ESkillType.Max; ++i)
                {
                    SkillBase skill = CreatureSkill.SkillArray[i];
                    if (skill == null)
                        continue;

                    if (skill.InvokeRange < bestShortRange)
                        bestShortRange = skill.InvokeRange;
                }

                return bestShortRange;
            }
        }

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            CreatureBody = BaseBody as CreatureBody;
            CreatureAnim = BaseAnim as CreatureAnimation;
            CreatureAnimCallback = CreatureAnim.GetComponent<CreatureAnimationCallback>();
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            // if (ObjectType == EObjectType.Hero)
            //     CreatureData = Managers.Data.HeroDataDict[dataID];
            // else if (ObjectType == EObjectType.Monster)
            //     CreatureData = Managers.Data.MonsterDataDict[dataID];
            CreatureData = Util.GetCreatureData(dataID, this);

#if UNITY_EDITOR
            Dev_NameTextID = CreatureData.Dev_NameTextID;
#endif

            Type aiClassType = Util.GetTypeFromName(CreatureData.AIClassName);
            CreatureAI = gameObject.AddComponent(aiClassType) as CreatureAI;
            CreatureAI.InitialSetInfo(this);

            Collider.radius = CreatureData.ColliderRadius;
            Collider.offset = CreatureData.ColliderOffset;

            CreatureSkill = gameObject.GetOrAddComponent<SkillComponent>();
            CreatureSkill.InitialSetInfo(owner: this, creatureData: CreatureData);
            CreatureAnimCallback.InitialSetInfo(this); // --- ??? 이게 있어야 하나??? Anim에 있으면 안되나.
            // --- CreatureAnim에서 InitialSetInfo하고 CreatureAnimCallback을 들고있는게 더 좋을 것 같은데.

            // --- Init Creature Passive
            if (Util.GetEffectData(dataID: LevelID, owner: this) != null)
                BaseEffect.GenerateEffect(effectID: LevelID);
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {            
            StartCoWait(waitCondition: () => BaseAnim.IsPlay() == false,
                      callbackWaitCompleted: () =>
                      {
                          base.EnterInGame(spawnPos);
                          CreatureAI.EnterInGame();
                          CreatureAnim.EnterInGame();
                          StartCoUpdateAI();
                          StartCoLerpToCellPos();
                          RigidBody.simulated = true;
                      });
        }

        private void LateUpdate()
            => UpdateCellPos();

        public override void OnDamaged(BaseCellObject attacker, SkillBase skillByAttacker)
        {
            if (CreatureAIState == ECreatureAIState.Dead)
                return;

            if (attacker.IsValid() == false)
                return;

            float damage = UnityEngine.Random.Range(attacker.MinDamage, attacker.MaxDamage);
            bool isCritical = UnityEngine.Random.Range(0.0f, 1.0f) <= attacker.CriticalRate;
            if (isCritical)
                damage *= 1 + 0.5f;

            float remainedDamage = 0.0f;
            float finalDamage = 0.0f;
            if (Armor > 0.0f)
            {
                finalDamage = Mathf.Max(damage * (1 - Armor), 1.0f);
                finalDamage = Mathf.Floor(finalDamage);
            }
            else
                finalDamage = Mathf.Round(damage);
            
            // DO SOMETHING: + Check Dodge, etc...
            // if (InvincibleCountPerWave > 0) { ... }
            // --- Bonus Health
            if (BonusHealth > 0.0f)
            {
                // --- 남아있있던 쉴드에 적용된 데미지 폰트를 표시하기 위해 이전의 마지막 보너스 데미지를 받아놓음.(EEffectBuffType.BonusHealth)
                float prevBonusHealth = finalDamage > BonusHealth ? BonusHealth : 0.0f;
                remainedDamage = OnDamagedBonusHealth(finalDamage);
                // --- Bonus Health: Shield
                if (BaseEffect.IsOnEffectBuff(EEffectBuffType.BonusHealthShield))
                {
                    BaseEffect.DoBuffEffects(EEffectBuffType.BonusHealthShield);
                    if (BonusHealth == 0.0f || remainedDamage > 0.0f) // Bonus Helath가 0.0이라는 의미다.
                    {
                        // --- Shield의 경우에는 보호막이 깨지면, 나머지 잔여 데미지량은 무시(무효)한다.
                        // --- FontAnim에 Random Height도 있으면 좋을 것 같은데.
                        // --- Height + 좌측을 바라볼 때 BouncingRight, 우측을 바라볼 때 BouncingLeft.
                        EFontAnimationType shieldBreakAnimType =  LookAtDir == ELookAtDirection.Left ? 
                                                               EFontAnimationType.EndBouncingRightUp :
                                                               EFontAnimationType.EndBouncingLeftUp;

                        // --- Duration 추가해야 할 것 같은데.. BREAK !! 부분은 yellow로 하고 싶기도 하고
                        ShowTextFont(text: "SHIELD\n  BREAK !!", fontSize: 5.5f, textColor: Managers.MonoContents.BrightBlue, fontAnimType: shieldBreakAnimType);
                        //BaseEffect.ExitShowBuffEffects(EEffectBuffType.BonusHealthShield);
                        BaseEffect.RemoveBuffEffects(EEffectBuffType.BonusHealthShield);
                        return;
                    }
                    else
                    {
                        // --- 좌측을 바라볼 땐, EndBouncingRightUp, 우측을 바라볼 땐 EndBouncingLeftUp으로 설정하고
                        // --- 데미지 폰트가 출력되는 높이를 랜덤으로 하는게 좋을듯.
                        EFontAnimationType fontAnimType = UnityEngine.Random.Range(0, 2) == 0 ?
                                                            EFontAnimationType.EndBouncingLeftUp :
                                                            EFontAnimationType.EndBouncingRightUp;

                        ShowDamageFont(damage: finalDamage, fontColor: ObjectType == EObjectType.Hero ? Color.cyan : Color.blue,
                            fontSignType: EFontSignType.Minus, isCritical: isCritical, fontAnimType: fontAnimType);
                    }
                }
                // --- Bonus Health: Other (잔여 데미지 처리)
                // --- 나중에 쉴드 버프 종류가 추가되면 분기처리 로직으로 변경(지금은 일반 보너스 체력으로 가정)
                else if (BaseEffect.IsOnEffectBuff(EEffectBuffType.BonusHealth))
                {
                    // DO SOMETHING...
                    BaseEffect.DoBuffEffects(EEffectBuffType.BonusHealth);
                    if (BonusHealth == 0.0f || remainedDamage > 0.0f) // Bonus Helath가 0.0이라는 의미다.
                    {
                        // --- 잔여 데미지 처리
                        // BaseEffect.ExitShowBuffEffects(EEffectBuffType.BonusHealth);
                        // BaseEffect.RemoveBuffEffects(EEffectBuffType.BonusHealth);

                        // --- 남아있있던 쉴드에 적용된 데미지 폰트 표시
                        if (prevBonusHealth != 0.0f)
                        {
                            // --- 이전 컬러는 BrightBlue였음. (기본 쉴드 데미지 폰터 컬러로 다시 변경함)
                            ShowDamageFont(damage: prevBonusHealth, fontColor: ObjectType == EObjectType.Hero ? Color.cyan : Color.blue,
                                fontSignType: EFontSignType.Minus, isCritical: isCritical, fontAnimType: EFontAnimationType.EndBouncingLeftUp);
                        }

                        // --- 실제로체력에 적용된 데미지 포트 표시
                        if (remainedDamage != 0.0f)
                        {
                            // --- Damage FontColor: BrightRed
                            ShowDamageFont(damage: remainedDamage, fontColor: Managers.MonoContents.BrightRed,
                                fontSignType: EFontSignType.None, isCritical: isCritical, fontAnimType: EFontAnimationType.EndBouncingRightUp);
                        }
                        else
                            ShowTextFont(text: "ZERO DAMAGE", fontSize: 4.0f, textColor: Managers.MonoContents.BrightRed, fontAnimType: EFontAnimationType.EndBouncingRightUp);

                        // BaseEffect.ExitShowBuffEffects(EEffectBuffType.BonusHealth); -- ???
                        BaseEffect.RemoveBuffEffects(EEffectBuffType.BonusHealth);
                        Health = Mathf.Clamp(Health - remainedDamage, 0.0f, MaxHealth);
                        if (Health <= 0.0f)
                        {
                            Health = 0.0f;
                            OnDead(attacker, skillByAttacker);
                        }
                        else
                        {
                            BaseBody.StartCoHurtFlashEffect(isCritical: isCritical);
                        }
                    }
                    else
                    {
                        EFontAnimationType fontAnimType = UnityEngine.Random.Range(0, 2) == 0 ?
                                                            EFontAnimationType.EndBouncingLeftUp :
                                                            EFontAnimationType.EndBouncingRightUp;

                        ShowDamageFont(damage: finalDamage, fontColor: ObjectType == EObjectType.Hero ? Color.cyan : Color.blue,
                            fontSignType: EFontSignType.Minus, isCritical: isCritical, fontAnimType: fontAnimType);
                    }
                }
            }
            // --- Damage to Default Health (기본 데미지 처리)
            else
            {
                Health = Mathf.Clamp(Health - finalDamage, 0.0f, MaxHealth);

                ShowDamageFont(damage: finalDamage, fontColor: ObjectType == EObjectType.Monster ? Color.white : Color.red,
                    isCritical: isCritical, fontSignType: EFontSignType.None, EFontAnimationType.EndFalling);

                ShowImpactHit(skillByAttacker, isCritical);
                if (isCritical)
                    ShowTextFont(text: "CRITICAL", fontSize: 5.0f, textColor: Color.red, fontAnimType: EFontAnimationType.EndFallingShake);

                if (Health <= 0.0f)
                {
                    Health = 0.0f;
                    OnDead(attacker, skillByAttacker);
                }
                else
                {
                    BaseBody.StartCoHurtFlashEffect(isCritical: isCritical);
                }
            }
        }

        private float OnDamagedBonusHealth(float finalDamage)
        {
            float remainedDamage = 0.0f;
            if (finalDamage > BonusHealth)
            {
                remainedDamage = finalDamage - BonusHealth;
                BonusHealth = 0.0f;
            }
            else
                BonusHealth = Mathf.Clamp(BonusHealth - finalDamage, 0.0f, BonusHealth);

            return remainedDamage;
        }

        public override void OnDead(BaseCellObject attacker, SkillBase skillFromAttacker)
        {
            Moving = false;
            StopCoLerpToCellPos(); // --- Stop Path Finding
            StopCoUpdateAI(); // --- Stop AI Tick
            CreatureAIState = ECreatureAIState.Dead;
            attacker.Targets.Remove(this);
            base.OnDead(attacker, skillFromAttacker);
            CreatureBody.StartCoFadeOutEffect(
                startCallback: () =>
                {
                    BaseEffect.GenerateEffect(
                            effectID: ReadOnly.DataAndPoolingID.DNPID_Effect_Global_OnDeadSkull,
                            spawnPos: CenterPosition
                            );
                },
                endCallback: () => OnDeadFadeOutCompleted()
            );
        }
        #endregion Core

        #region Background
        public virtual Vector3 GetFirePosition() // --- TEMP
            => CenterPosition;
        
        public void CollectEnv()
            => CreatureAnim.CollectEnv();
        public void Dead()
            => CreatureAnim.Dead();

        public virtual bool LevelUp() 
            => false;

        // --Ready to Move "BaseCellObject.cs"
        // public EFindPathResult FindPathAndMoveToCellPos(Vector3 destPos, int maxDepth, EObjectType ignoreCellObjType = EObjectType.None)
        //     => FindPathAndMoveToCellPos(Managers.Map.WorldToCell(destPos), maxDepth, ignoreCellObjType);

        // public EFindPathResult FindPathAndMoveToCellPos(Vector3Int destPos, int maxDepth, EObjectType ignoreCellObjType = EObjectType.None)
        // {
        //     if (IsForceMovingPingPongObject)
        //         return EFindPathResult.Fail_ForceMove;

        //     // ---A*
        //     List<Vector3Int> path = Managers.Map.FindPath(startCellPos: CellPos, destPos, maxDepth, ignoreCellObjType);
        //     if (path.Count == 1)
        //     {
        //         if (IsOnTheCellCenter == false)
        //         {
        //             NextCellPos = path[0];
        //             LerpToCellPosCompleted = false;
        //             return EFindPathResult.Success;
        //         }
        //         else if (LerpToCellPosCompleted) // --- 무조건 가운데까지 간다.
        //         {
        //             return EFindPathResult.Fail_LerpCell;
        //         }
        //     }

        //     else if (path.Count > 1)
        //     {
        //         Vector3Int dirCellPos = path[1] - CellPos;
        //         Vector3Int nextCellPos = CellPos + dirCellPos;

        //         if (Managers.Map.TryMove(moveToCellPos: nextCellPos, ignoreCellObjType: ignoreCellObjType) == false)
        //             return EFindPathResult.Fail_MoveTo;

        //         NextCellPos = nextCellPos;
        //         LerpToCellPosCompleted = false;
        //     }

        //     return EFindPathResult.Success;
        // }
        #endregion Background

        public bool IsRunningAITick => _coUpdateAI != null;
        protected Coroutine _coUpdateAI = null;
        protected IEnumerator CoUpdateAI()
        {
            if (ObjectType == EObjectType.Monster)
                yield break;

            while (true)
            {
                if (CreatureAI.ForceWaitCompleted == false)
                {
                    yield return null;
                    continue;
                }

                switch (CreatureAIState)
                {
                    case ECreatureAIState.Idle:
                        CreatureAI.UpdateIdle();
                        break;

                    case ECreatureAIState.Move:
                        CreatureAI.UpdateMove();
                        break;
                }

                yield return null;
            }
        }

        public void StartCoUpdateAI()
        {
            StopCoUpdateAI();
            _coUpdateAI = StartCoroutine(CoUpdateAI());
        }

        public void StopCoUpdateAI()
        {
            if (_coUpdateAI != null)
                StopCoroutine(_coUpdateAI);

            _coUpdateAI = null;
        }

        protected Coroutine _coWait = null;
        private IEnumerator CoWait(Func<bool> waitCondition, Action waitCompleted = null)
        {
            yield return new WaitUntil(waitCondition);
            _coWait = null;
            waitCompleted?.Invoke();
        }
        private IEnumerator CoWait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _coWait = null;
        }

        protected void StartCoWait(Func<bool> waitCondition, Action callbackWaitCompleted = null)
        {
            StopCoWait();
            _coWait = StartCoroutine(CoWait(waitCondition, callbackWaitCompleted));
        }
        protected void StartCoWait(float seconds)
        {
            StopCoWait();
            _coWait = StartCoroutine(CoWait(seconds));
        }

        protected void StopCoWait()
        {
            if (_coWait != null)
                StopCoroutine(_coWait);
            _coWait = null;
        }

        #region Util - OnDamaged
        protected void ShowDamageFont(float damage, Color fontColor, bool isCritical, EFontSignType fontSignType, EFontAnimationType fontAnimType)
            => Managers.Object.ShowDamageFont(position: CenterPosition, damage: damage, textColor: fontColor, isCritical: isCritical,
                                                fontSignType: fontSignType, fontAnimType: fontAnimType);

        protected void ShowImpactHit(SkillBase skillByAttacker, bool isCritical)
        {
            if (isCritical)
                BaseEffect.GenerateEffect(effectID: ReadOnly.DataAndPoolingID.DNPID_Effect_Global_ImpactCriticalHit, skill: null);

            List<EffectBase> hitEffects = skillByAttacker.GenerateSkillEffects
                                    (
                                        effectIDs: skillByAttacker.SkillData.HitEffectIDs,
                                        spawnPos: Util.GetRandomQuadPosition(this.CenterPosition)
                                    );
        }

        // TextFont의 FontAssetType은 일단 Comic으로 고정
        protected void ShowTextFont(string text, float fontSize, Color textColor, EFontAnimationType fontAnimType)
            => Managers.Object.ShowTextFont(position: CenterPosition + Vector3.up * 0.65f, text: text, textSize: fontSize, textColor: textColor,
                                                fontAssetType: EFontAssetType.Comic, fontAnimType: fontAnimType);
        #endregion

        #region Passive
        protected void ApplyNewPassive()
        {
            if (Util.GetEffectData(dataID: LevelID, owner: this) == null)
            {
                Debug.LogWarning($"Invalid Effect Data - {ObjectType}: {LevelID}");
                return;
            }

            EffectBase prevPassive = BaseEffect.FindPrevEffect(dataID: LevelID);
            if (prevPassive != null)
            {
                Debug.Log($"Remove Passive: {prevPassive.Dev_NameTextID}");
                BaseEffect.RemoveEffect(prevPassive, destroyPooling: true);
                BaseEffect.GenerateEffect(effectID: LevelID); // 알아서 ApplyStat이 될 테므로,,,
            }
        }
        #endregion

        // protected Coroutine _coLerpToCellPos = null;
        // protected IEnumerator CoLerpToCellPos()
        // {
        //     while (true)
        //     {
        //         if (IsForceMovingPingPongObject || CreatureAIState == ECreatureAIState.Idle)
        //         {
        //             //LerpToCellPosCompleted = true;
        //             yield return null;
        //             continue;
        //         }

        //         Hero hero = this as Hero;
        //         if (hero.IsValid())
        //         {
        //             // 1. 리더의 MovementSpeed가 느리면 멤버들의 Movement Speed도 Leader에게 맞춰진다.
        //             // --> 리더 주변으로 Chase해야하기 때문
        //             // 2. 1번이 어색하게 느껴지면 MovementSpeed는 통합 Stat으로 관리
        //             float movementSpeed = Util.CalculateValueFromDistance(
        //                 value: Managers.Object.HeroLeaderController.Leader.MovementSpeed,
        //                 maxValue: Managers.Object.HeroLeaderController.Leader.MovementSpeed * 2f,
        //                 distanceToTargetSQR: (CellPos - Managers.Object.HeroLeaderController.Leader.CellPos).sqrMagnitude,
        //                 maxDistanceSQR: ReadOnly.Util.HeroDefaultScanRange * ReadOnly.Util.HeroDefaultScanRange
        //             );

        //             LerpToCellPos(movementSpeed);
        //         }
        //         else //--- Monster
        //         {
        //             LerpToCellPos(MovementSpeed);
        //         }

        //         yield return null;
        //     }
        // }

        // public void StartCoLerpToCellPos()
        // {
        //     StopCoLerpToCellPos();
        //     _coLerpToCellPos = StartCoroutine(CoLerpToCellPos());
        // }

        // public void StopCoLerpToCellPos()
        // {
        //     if (_coLerpToCellPos != null)
        //         StopCoroutine(_coLerpToCellPos);
        //     _coLerpToCellPos = null;
        // }

        // /*
        //     1 - 큐에 A 추가: [A]
        //     2 - 큐에 B 추가: [A, B]
        //     3 - 큐에 A 추가: [A, B, A]
        //     4 - 큐에 B 추가: [A, B, A, B]
        //     5 - 1 (검사)
        //     5 - 2 Dequeue: [B, A, B]
        //     5 - 3 큐에 A 추가: [B, A, B, A]
        //     6 - 1 (검사)
        //     6 - 2 Dequeue: [A, B, A]
        //     6 - 3 큐에 B 추가: [A, B, A, B]
        //     7 - 1 (검사)
        //     ...

        //     이게 정상이긴한데 위치가 정확하게 입력되지 않음.
        //     그래서 큐의 4개의 요소 안에 A가 2개, B가 2개가 있는지만 확인.
        //     나중에 몬스터에서도 필요하면 Creature로 옮겨주면 됨
        // */
        // private Queue<Vector3Int> _cantMoveCheckQueue = new Queue<Vector3Int>();
        // [SerializeField] protected int _currentPingPongCantMoveCount = 0;
        // private int maxCantMoveCheckCount = 4; // 2칸에 대해 왔다 갔다만 조사하는 것이라 4로 설정
        // protected bool IsPingPongAndCantMoveToDest(Vector3Int cellPos)
        // {
        //     if (_cantMoveCheckQueue.Count >= maxCantMoveCheckCount)
        //         _cantMoveCheckQueue.Dequeue();

        //     _cantMoveCheckQueue.Enqueue(cellPos);
        //     if (_cantMoveCheckQueue.Count == maxCantMoveCheckCount)
        //     {
        //         Vector3Int[] cellArr = _cantMoveCheckQueue.ToArray();
        //         HashSet<Vector3Int> uniqueCellPos = new HashSet<Vector3Int>(cellArr);
        //         if (uniqueCellPos.Count == 2)
        //         {
        //             Dictionary<Vector3Int, int> checkCellPosCountDict = new Dictionary<Vector3Int, int>();
        //             foreach (var pos in _cantMoveCheckQueue)
        //             {
        //                 if (checkCellPosCountDict.ContainsKey(pos))
        //                     checkCellPosCountDict[pos]++;
        //                 else
        //                     checkCellPosCountDict[pos] = 1;
        //             }

        //             foreach (var count in checkCellPosCountDict.Values)
        //             {
        //                 if (count != 2)
        //                     return false;
        //             }

        //             return true;
        //         }
        //     }

        //     return false;
        // }

        // // ***** Force Move Ping Pong Object Coroutine *****
        // private Coroutine _coForceMovePingPongObject = null;
        // protected bool IsForceMovingPingPongObject => _coForceMovePingPongObject != null;
        // private IEnumerator CoForceMovePingPongObject(Vector3Int currentCellPos, Vector3Int destCellPos, System.Action endCallback = null)
        // {
        //     List<Vector3Int> path = Managers.Map.FindPath(currentCellPos, destCellPos);

        //     Queue<Vector3Int> pathQueue = new Queue<Vector3Int>();
        //     for (int i = 0; i < path.Count; ++i)
        //         pathQueue.Enqueue(path[i]);
        //     pathQueue.Dequeue();

        //     Vector3Int nextPos = pathQueue.Dequeue();
        //     Vector3 currentWorldPos = Managers.Map.CellToWorld(currentCellPos);
        //     while (pathQueue.Count != 0)
        //     {
        //         Vector3 destPos = Managers.Map.CellToCenteredWorld(nextPos);
        //         Vector3 dir = destPos - transform.position; // 왜 이걸로하면 안되지
        //         if (dir.x < 0f)
        //             LookAtDir = ELookAtDirection.Left;
        //         else if (dir.x > 0f)
        //             LookAtDir = ELookAtDirection.Right;

        //         if (dir.sqrMagnitude < 0.01f)
        //         {
        //             transform.position = destPos;
        //             currentWorldPos = transform.position;
        //             nextPos = pathQueue.Dequeue();
        //         }
        //         else
        //         {
        //             float moveDist = Mathf.Min(dir.magnitude, MovementSpeed * Time.deltaTime);
        //             transform.position += dir.normalized * moveDist; // Movement per frame.
        //         }

        //         yield return null;
        //     }

        //     endCallback?.Invoke();
        //     // UpdateCellPos();
        // }

        // protected void CoStartForceMovePingPongObject(Vector3Int currentCellPos, Vector3Int destCellPos, System.Action endCallback = null)
        // {
        //     if (_coForceMovePingPongObject != null)
        //         return;

        //     _coForceMovePingPongObject = StartCoroutine(CoForceMovePingPongObject(currentCellPos, destCellPos, endCallback));
        // }

        // protected void CoStopForceMovePingPongObject()
        // {
        //     if (_coForceMovePingPongObject != null)
        //     {
        //         StopCoroutine(_coForceMovePingPongObject);
        //         _coForceMovePingPongObject = null;
        //     }
        // }
    }
}

/*

        // public bool IsAtCenter(Vector3Int destPos)
        // {
        //     Vector3 center = Managers.Map.GetCenterWorld(destPos);
        //     Vector3 dir = center - transform.position;

        //     float threshold = 0.1f;
        //     if (dir.sqrMagnitude < threshold * threshold)
        //     {
        //         transform.position = center;
        //         return true;
        //     }

        //     return false;
        // }
*/