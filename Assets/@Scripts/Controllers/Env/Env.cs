using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Env : BaseObject
    {
        public Data.EnvData EnvData { get; private set; } = null;
        [SerializeField] private EEnvState _envState = EEnvState.None;
        public EEnvState EnvState
        {
            get => _envState;
            set
            {
                if (_envState != value)
                {
                    _envState = value;
                    EnvAnim.UpdateAnimation();
                }
            }
        }

        public EnvAnimation EnvAnim { get; private set; } = null;
        public EEnvType EnvType { get; private set; } = EEnvType.None;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Env;
            RigidBody.bodyType = RigidbodyType2D.Static;
            Collider.isTrigger = true;

            return true;
        }

        public override bool SetInfo(int dataID)
        {
            if (base.SetInfo(dataID) == false)
            {
                EnterInGame();
                return false;
            }
        
            InitialSetInfo(dataID);
            EnterInGame();
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            EnvAnim = BaseAnim as EnvAnimation;
            EnvAnim.SetInfo(dataID, this);
            Managers.Sprite.SetInfo(dataID, this);

            EnvData = Managers.Data.EnvDataDict[dataID];
            //EnvType = Util.GetEnumFromString<EEnvType>(EnvData.Type);
            EnvType = EnvData.EnvType;

            gameObject.name += $"_{EnvData.DescriptionTextID.Replace(" ", "")}";
        }

        protected override void InitStat(int dataID)
        {
            base.InitStat(dataID);
            _maxLevel = dataID;
        }

        protected override void EnterInGame()
        {
            base.EnterInGame();
            ShowBody(true);            
            EnvState = EEnvState.Idle;
        }

        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            if (EnvState == EEnvState.Dead)
                return;

            float finalDamage = 1f;
            //if (attacker.ObjectRarity == EObjectRarity.Elite)
            if ((attacker as Hero).CreatureRarity == ECreatureRarity.Elite) // TEMP
                finalDamage++;

            EnvState = EEnvState.OnDamaged;

            // TODO : Show UI
            Hp = UnityEngine.Mathf.Clamp(Hp - finalDamage, 0f, MaxHp);
            // Debug.Log($"{gameObject.name}: {Hp}/{MaxHp}");

            Managers.Object.ShowDamageFont(position: this.CenterPosition, damage: finalDamage, isCritical: false);
            //Debug.Log($"{gameObject.name} is damaged. ({Hp} / {MaxHp})");

            if (Hp <= 0f)
            {
                Hp = 0f;
                OnDead(attacker, skillFromAttacker);
            }
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            // (attacker as Creature).StartCoPauseSearchTarget(0.5f); // TEST
            EnvState = EEnvState.Dead;
            base.OnDead(attacker, skillFromAttacker);

            // --- TODO : Drop Item
            // Managers.Object.Despawn(this);
        }
    }
}
