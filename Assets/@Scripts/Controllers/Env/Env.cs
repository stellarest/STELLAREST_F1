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
                Refresh();
                return false;
            }

            base.SetInfo(dataID);
            EnvAnim = BaseAnim as EnvAnimation;
            EnvAnim.SetInfo(dataID, this);
            Managers.Sprite.SetInfo(dataID, this);

            EnvData = Managers.Data.EnvDataDict[dataID];
            gameObject.name += $"_{EnvData.DescriptionTextID}";

            Refresh();
            return true;
        }

        protected override void Refresh()
        {
            base.Refresh();
            EnvState = EEnvState.Idle;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
                EnvState = EEnvState.Idle;

            if (Input.GetKeyDown(KeyCode.W))
                EnvState = EEnvState.Dead;
        }
    }
}
