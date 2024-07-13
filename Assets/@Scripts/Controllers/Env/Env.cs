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
                    if (_envState == EEnvState.Dead)
                        EnvAnim.Dead();
                }
            }
        }

        public EnvAnimation EnvAnim { get; private set; } = null;
        public EEnvType EnvType { get; private set; } = EEnvType.None;
        public EnvBody EnvBody { get; private set; } = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Env;
            RigidBody.bodyType = RigidbodyType2D.Static;
            Collider.isTrigger = true;
            EnvBody = GetComponent<EnvBody>();

            return true;
        }

        public override bool SetInfo(int dataID)
        {
            // --- EnterInGame from BaseObject
            if (base.SetInfo(dataID) == false)
                return false;
        
            InitialSetInfo(dataID);
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            _maxLevel = dataID;

            EnvBody.SetInfo(dataID, this);
            EnvAnim = BaseAnim as EnvAnimation;
            EnvAnim.SetInfo(dataID, this);
            //Managers.Sprite.SetInfo(dataID, this);

            EnvData = Managers.Data.EnvDataDict[dataID];
            EnvType = EnvData.EnvType;

            gameObject.name += $"_{EnvData.NameTextID.Replace(" ", "")}";
            EnterInGame();
        }

        protected override void EnterInGame()
        {
            base.EnterInGame();
            //ShowBody(true);            
            EnvState = EEnvState.Idle;
            Debug.Log($"<color=white>EnterInGame, {gameObject.name}, {EnvState}</color>");
        }

        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            if (EnvState == EEnvState.Dead)
                return;

            float finalDamage = 1f;
            if ((attacker as Hero).CreatureRarity == ECreatureRarity.Elite) // TEMP
                finalDamage++;

            Hp = Mathf.Clamp(Hp - finalDamage, 0f, MaxHp);
            Managers.Object.ShowDamageFont(position: this.CenterPosition, damage: finalDamage, isCritical: false);
            if (Hp <= 0f)
            {
                Hp = 0f;
                OnDead(attacker, skillFromAttacker);
                return;
            }
            else
            {
                HitShakeMovement(duration: 0.1f, power: 0.5f, vibrato: 20);
                EnvBody.StartCoHurtFlashEffect(isCritical: false);
            }
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            EnvBody.ResetMaterialsAndColors();
            EnvState = EEnvState.Dead;
            base.OnDead(attacker, skillFromAttacker);
            // --- TODO : Drop Item
        }
    }
}
