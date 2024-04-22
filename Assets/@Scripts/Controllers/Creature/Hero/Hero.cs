using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using STELLAREST_F1.Data;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Diagnostics;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Hero : Creature
    {
        public Data.HeroData HeroData { get; private set; } = null;

        private HeroBody _heroBody = null;
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

        public override ECreatureState CreatureState
        {
            get => base.CreatureState;
            set
            {
                base.CreatureState = value;
                switch (value)
                {
                    case ECreatureState.Move:
                        RigidBody.mass = 3.0f * 5.0f;
                        break;

                    case ECreatureState.Skill_Attack:
                        RigidBody.mass = 3.0f * 500.0f;
                        break;

                    default:
                        RigidBody.mass = 3.0f;
                        break;
                }
            }
        }

        public override ECreatureMoveState CreatureMoveState
        {
            get => base.CreatureMoveState;
            protected set
            {
                base.CreatureMoveState = value;
                switch (value)
                {
                    case ECreatureMoveState.TargetToEnemy:
                        NeedArrange = true; // -> ChangeColliderSize(EColliderSize.Large);
                        break;

                    case ECreatureMoveState.CollectEnv:
                        NeedArrange = true; // -> ChangeColliderSize(EColliderSize.Large);
                        break;

                    case ECreatureMoveState.ForceMove:
                        NeedArrange = true; // -> ChangeColliderSize(EColliderSize.Large);
                        CancelWait();
                        Target = null;
                        break;
                }
            }
        }

        private Transform Destination
        {
            get
            {
                HeroCamp camp = Managers.Object.Camp;
                if (CreatureMoveState == ECreatureMoveState.ReturnToBase)
                    return camp.Pivot;

                return camp.Destination;
            }
        }

        public override bool CollectEnv
        {
            get => base.CollectEnv;
            protected set
            {
                base.CollectEnv = value;
                if (value && Target.IsValid())
                {
                    Env envTarget = Target as Env;
                    HeroBody.EnvWeapon(envTarget.EnvType);
                }
                else
                    HeroBody.DefaultWeapon();
            }
        }

        public HeroAnimation HeroAnim { get; private set; } = null;

        private bool _needArrange = true;
        public bool NeedArrange
        {
            get => _needArrange;
            private set
            {
                _needArrange = value;
                if (value)
                    ChangeColliderSize(EColliderSize.Large);
                else
                    TryResizeCollider();
            }
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Hero;

            return true;
        }
        
        public override bool SetInfo(int dataID)
        {
            if (base.SetInfo(dataID) == false)
            {
                EnterInGame();
                return false;
            }

            HeroBody = new HeroBody(this, dataID);
            this.GetComponent<Creature>().HeroBody = HeroBody; // TEMP

            HeroAnim = CreatureAnim as HeroAnimation;
            HeroAnim.SetInfo(dataID, this);
            Managers.Sprite.SetInfo(dataID, target: this);

            HeroData = Managers.Data.HeroDataDict[dataID];
            gameObject.name += $"_{HeroData.DescriptionTextID.Replace(" ", "")}";
            Collider.radius = HeroData.ColliderRadius;

            CreatureSkillComponent = gameObject.GetOrAddComponent<SkillComponent>();
            CreatureSkillComponent.SetInfo(this, Managers.Data.HeroDataDict[dataID].SkillIDs);

            EnterInGame();
            return true;
        }

        protected override void EnterInGame()
        {
            LookAtDir = ELookAtDirection.Right;
            base.EnterInGame();
            
            // 나오고 나서 조이스틱 등록
            Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
            Managers.Game.OnJoystickStateChangedHandler += OnJoystickStateChanged;
        }

        #region AI
        protected override void UpdateIdle()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Transform tr = HeroBody.GetComponent<Transform>(EHeroWeapon.WeaponL);
                Debug.Log($"L : {tr.GetComponent<SpriteRenderer>().enabled}");

                tr = HeroBody.GetComponent<Transform>(EHeroWeapon.WeaponR);
                Debug.Log($"R : {tr.GetComponent<SpriteRenderer>().enabled}");
            }

            SetRigidBodyVelocity(Vector2.zero);
            if (CreatureMoveState == ECreatureMoveState.ForceMove)
            {
                CreatureState = ECreatureState.Move;
                return;
            }

            if (Target.IsValid())
                LookAtTarget(Target);

            // 조금 극단적인 방법. 쳐다보면서 가만히 짱박혀있어라.
            if (CreatureSkillComponent.IsRemainingCoolTime((int)ESkillType.Skill_Attack))
                return;

            Creature creature = FindClosestInRange(ReadOnly.Numeric.Temp_SearchDistance, Managers.Object.Monsters, func: IsValid) as Creature;
            if (creature.IsValid())
            {
                Target = creature;
                CreatureState = ECreatureState.Move;
                CreatureMoveState = ECreatureMoveState.TargetToEnemy;
                return;
            }

            Env env = FindClosestInRange(ReadOnly.Numeric.Temp_SearchDistance, Managers.Object.Envs, func: IsValid) as Env;
            if (env.IsValid())
            {
                Target = env;
                CreatureState = ECreatureState.Move;
                CreatureMoveState = ECreatureMoveState.CollectEnv;
                return;
            }

            if (NeedArrange)
            {
                CreatureState = ECreatureState.Move;
                CreatureMoveState = ECreatureMoveState.ReturnToBase;
                return;
            }
        }

        protected override void UpdateMove()
        {
            if (CreatureMoveState == ECreatureMoveState.ForceMove)
            {
                Vector3 dir = Destination.transform.position - transform.position;
                SetRigidBodyVelocity(dir.normalized * MovementSpeed);
                return;
            }

            if (CreatureMoveState == ECreatureMoveState.TargetToEnemy)
            {
                if (Target.IsValid() == false)
                {
                    CreatureMoveState = ECreatureMoveState.None;
                    CreatureState = ECreatureState.Move;
                    return;
                }

                ChaseOrAttackTarget(ReadOnly.Numeric.Temp_SearchDistance, AttackDistance);
                return;
            }

            if (CreatureMoveState == ECreatureMoveState.CollectEnv)
            {
                // Research Enemies
                Creature creature = FindClosestInRange(ReadOnly.Numeric.Temp_SearchDistance, Managers.Object.Monsters, func: IsValid) as Creature;
                if (creature != null)
                {
                    Target = creature;
                    CreatureMoveState = ECreatureMoveState.TargetToEnemy;
                    CreatureState = ECreatureState.Move;
                    return;
                }

                // 이미 채집했으면 포기
                if (Target.IsValid() == false)
                {
                    CreatureMoveState = ECreatureMoveState.None;
                    CreatureState = ECreatureState.Move;
                    CollectEnv = false;
                    return;
                }

                ChaseOrAttackTarget(ReadOnly.Numeric.Temp_SearchDistance, AttackDistance);
                return;
            }

            if (CreatureMoveState == ECreatureMoveState.ReturnToBase)
            {
                // Research Enemies
                Creature creature = FindClosestInRange(ReadOnly.Numeric.Temp_SearchDistance, Managers.Object.Monsters, func: IsValid) as Creature;
                if (creature != null)
                {
                    CollectEnv = false;
                    Target = creature;
                    CreatureMoveState = ECreatureMoveState.TargetToEnemy;
                    CreatureState = ECreatureState.Move;
                    return;
                }

                Vector3 toDir = Destination.position - transform.position;
                float stopDistSQR = ReadOnly.Numeric.Temp_StopDistance * ReadOnly.Numeric.Temp_StopDistance;
                if (toDir.sqrMagnitude < stopDistSQR)
                {
                    CreatureMoveState = ECreatureMoveState.None;
                    CreatureState = ECreatureState.Idle;
                    NeedArrange = false; // -> TryResizeCollider()
                }
                else
                {
                    // *** 로그함수로 바꿔보기 ***
                    // float ratio = Mathf.Min(1, toDir.magnitude);
                    // float moveSpeed = MovementSpeed * (float)Math.Pow(ratio, 3);
                    // SetRigidBodyVelocity(toDir.normalized * moveSpeed);

                    // 캠프로 돌아올 때만 거리에 따라 스피드 조정
                    float movementSpeed = CalculateMovementSpeed(toDir.sqrMagnitude);
                    SetRigidBodyVelocity(toDir.normalized * movementSpeed);

                }
                return;
            }

            // 눌렀다가 땟을때
            CreatureState = ECreatureState.Idle;
        }

        protected override void UpdateSkill()
        {
            base.UpdateSkill();
            SetRigidBodyVelocity(Vector2.zero);
            if (CreatureMoveState == ECreatureMoveState.ForceMove)
            {
                CreatureState = ECreatureState.Move;
                return;
            }
            else if (Target.IsValid())
                LookAtTarget(Target);
            else if (Target.IsValid() == false)
            {
                CreatureState = ECreatureState.Move;
                return;
            }

            ChangeColliderSize(EColliderSize.Default);
        }

        protected override void UpdateCollectEnv()
        {
            SetRigidBodyVelocity(Vector2.zero);
            if (CreatureMoveState == ECreatureMoveState.ForceMove || Target.IsValid() == false)
            {
                CollectEnv = false;
                CreatureState = ECreatureState.Move;
            }
            else if (Target.IsValid())
            {
                // Research Enemies
                Creature creature = FindClosestInRange(ReadOnly.Numeric.Temp_SearchDistance, Managers.Object.Monsters, func: IsValid) as Creature;
                if (creature != null)
                {
                    CollectEnv = false;
                    Target = creature;
                    CreatureMoveState = ECreatureMoveState.TargetToEnemy;
                    CreatureState = ECreatureState.Move;
                    return;
                }

                LookAtTarget(Target);
            }
        }

        protected override void UpdateDead()
        {
            base.UpdateDead();
        }
        #endregion

        protected override void OnCollectEnvStateUpdate()
        {
            if (Target.IsValid() == false)
                return;

            Target.OnDamaged(this, null);
        }

        protected override IEnumerator CoDeadFadeOut(Action callback = null)
        {
            if (this.isActiveAndEnabled == false)
                yield break;

            yield return new WaitForSeconds(ReadOnly.Numeric.StartDeadFadeOutTime);

            float delta = 0f;
            float percent = 1f;
            AnimationCurve curve = Managers.Animation.Curve(EAnimationCurveType.Ease_In);

            // 1. Fade Out - Skin
            while (percent > 0f)
            {
                delta += Time.deltaTime;
                percent = 1f - (delta / ReadOnly.Numeric.DesiredDeadFadeOutEndTime);
                for (int i = 0; i < HeroBody.Skin.Count; ++i)
                {
                    float current = Mathf.Lerp(0f, 1f, curve.Evaluate(percent));
                    HeroBody.Skin[i].color = new Color(HeroBody.Skin[i].color.r,
                                                       HeroBody.Skin[i].color.g, 
                                                       HeroBody.Skin[i].color.b, current);
                }

                yield return null;
            }

            // 2. Fade Out - Appearance
            delta = 0f;
            percent = 1f;
            while (percent > 0f)
            {
                delta += Time.deltaTime;
                percent = 1f - (delta / ReadOnly.Numeric.DesiredDeadFadeOutEndTime);
                for (int i = 0; i < HeroBody.Appearance.Count; ++i)
                {
                    float current = Mathf.Lerp(0f, 1f, curve.Evaluate(percent));
                    HeroBody.Appearance[i].color = new Color(HeroBody.Appearance[i].color.r,
                                                             HeroBody.Appearance[i].color.g,
                                                             HeroBody.Appearance[i].color.b, current);
                }

                yield return null;
            }

            callback?.Invoke();
        }

        private void OnJoystickStateChanged(EJoystickState joystickState)
        {
            switch (joystickState)
            {
                case EJoystickState.PointerUp:
                    CreatureMoveState = ECreatureMoveState.None;
                    break;

                case EJoystickState.Drag:
                    CreatureMoveState = ECreatureMoveState.ForceMove;
                    break;

                default:
                    break;
            }
        }

        // ColliderRadius 이런건 CreatureData로 빼서 상위로 올려도 될듯
        // 근데 어차피 길찾기 적용시킬것이라..
        protected override void ChangeColliderSize(EColliderSize colliderSize = EColliderSize.Default)
        {
            switch (colliderSize)
            {
                case EColliderSize.Small:
                    Collider.radius = HeroData.ColliderRadius * 0.8f;
                    break;

                case EColliderSize.Default:
                    Collider.radius = HeroData.ColliderRadius;
                    break;

                case EColliderSize.Large:
                    Collider.radius = HeroData.ColliderRadius * 1.2f;
                    break;
            }
        }

        protected override void TryResizeCollider()
        {
            ChangeColliderSize(EColliderSize.Small);

            foreach (Hero hero in Managers.Object.Heroes)
            {
                if (hero.CreatureMoveState == ECreatureMoveState.ReturnToBase)
                    return;
            }

            foreach (Hero hero in Managers.Object.Heroes)
            {
                if (hero.CreatureState == ECreatureState.Idle)
                    hero.ChangeColliderSize(EColliderSize.Large);
            }
        }

        #region Battle
        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            base.OnDamaged(attacker, skillFromAttacker);
            Debug.Log($"{gameObject.name} is damaged. ({Hp} / {MaxHp})");
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            base.OnDead(attacker, skillFromAttacker);
        }
        protected override void OnDisable()
        {
            Debug.Log("Hero::OnDisable");
            base.OnDisable();

            if (Managers.Game == null)
                return;

            Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
        }
        #endregion
    }
}
