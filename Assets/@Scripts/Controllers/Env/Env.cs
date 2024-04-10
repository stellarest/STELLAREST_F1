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

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Env;
            RigidBody.bodyType = RigidbodyType2D.Static;

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
            _maxHp = new Stat(EnvData.MaxHp);
            gameObject.name += $"_{EnvData.DescriptionTextID.Replace(" ", "")}";

            EnterInGame();
            return true;
        }

        protected override void EnterInGame()
        {
            base.EnterInGame();
            Hp = MaxHp;
            EnvState = EEnvState.Idle;
        }

        public override void OnDamaged(BaseObject attacker)
        {
            if (EnvState == EEnvState.Dead)
                return;

            float finalDamage = 1f;
            EnvState = EEnvState.OnDamaged;

            // TODO : Show UI
            Hp = UnityEngine.Mathf.Clamp(Hp - finalDamage, 0f, MaxHp);
            Debug.Log($"{gameObject.name} Hp : {Hp} / {MaxHp}");
            if (Hp <= 0f)
            {
                Hp = 0f;
                OnDead(attacker);
            }
        }

        public override void OnDead(BaseObject attacker)
        {
            Managers.Object.Despawn(this); // TEMP
            //EnvState = EEnvState.Dead;
            // TODO : Drop Item
            // Managers.Object.Despawn(this);
        }
    }
}
