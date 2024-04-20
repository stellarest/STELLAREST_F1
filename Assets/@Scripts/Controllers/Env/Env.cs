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
            Collider.isTrigger = true; // TEMP

            return true;
        }

        public override bool SetInfo(int dataID)
        {
            if (base.SetInfo(dataID) == false)
            {
                EnterInGame();
                return false;
            }

            base.SetInfo(dataID);
            EnvAnim = BaseAnim as EnvAnimation;
            EnvAnim.SetInfo(dataID, this);
            Managers.Sprite.SetInfo(dataID, this);

            EnvData = Managers.Data.EnvDataDict[dataID];
            EnvType = Util.GetEnumFromString<EEnvType>(EnvData.Type);
            _maxHp = new Stat(EnvData.MaxHp);
            
            gameObject.name += $"_{EnvData.DescriptionTextID.Replace(" ", "")}";
            EnterInGame();
            return true;
        }

        protected override void EnterInGame()
        {
            ShowBody(true);            
            Hp = MaxHp;
            EnvState = EEnvState.Idle;
        }

        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            if (EnvState == EEnvState.Dead)
                return;

            float finalDamage = 1f;
            if (attacker.ObjectRarity == EObjectRarity.Elite)
                finalDamage++;

            EnvState = EEnvState.OnDamaged;

            // TODO : Show UI
            Hp = UnityEngine.Mathf.Clamp(Hp - finalDamage, 0f, MaxHp);
            Debug.Log($"{gameObject.name} Hp : {Hp} / {MaxHp}");
            if (Hp <= 0f)
            {
                Hp = 0f;
                OnDead(attacker, skillFromAttacker);
            }
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            //Managers.Object.Despawn(this); // TEMP
            EnvState = EEnvState.Dead;
            base.OnDead(attacker, skillFromAttacker);
            // TODO : Drop Item
            // Managers.Object.Despawn(this);
        }
    }
}
