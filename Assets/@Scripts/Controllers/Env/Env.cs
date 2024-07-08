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
            if (base.SetInfo(dataID) == false)
            {
                // --- BaseObject에서 호출하는중!
                //EnterInGame();
                return false;
            }
        
            InitialSetInfo(dataID);
            EnterInGame();
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            EnvBody.SetInfo(this, dataID);
            EnvAnim = BaseAnim as EnvAnimation;
            EnvAnim.SetInfo(dataID, this);
            //Managers.Sprite.SetInfo(dataID, this);

            EnvData = Managers.Data.EnvDataDict[dataID];
            EnvType = EnvData.EnvType;

            gameObject.name += $"_{EnvData.NameTextID.Replace(" ", "")}";
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
            Debug.Log($"<color=white>EnterInGame, {gameObject.name}, {EnvState}</color>");
        }

        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
        {
            if (EnvState == EEnvState.Dead)
                return;

            float finalDamage = 1f;
            if ((attacker as Hero).CreatureRarity == ECreatureRarity.Elite) // TEMP
                finalDamage++;

            // TODO : Show UI
            Hp = UnityEngine.Mathf.Clamp(Hp - finalDamage, 0f, MaxHp);
            Managers.Object.ShowDamageFont(position: this.CenterPosition, damage: finalDamage, isCritical: false);
            if (Hp <= 0f)
            {
                Hp = 0f;
                OnDead(attacker, skillFromAttacker);
                return;
            }
            EnvBody.StartCoHurtFlashEffect();
        }

        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
        {
            EnvBody.ResetEnvMaterialsAndColors(EnvType);
            EnvState = EEnvState.Dead;
            base.OnDead(attacker, skillFromAttacker);
            // --- TODO : Drop Item
        }
    }
}
