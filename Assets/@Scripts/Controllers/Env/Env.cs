using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;
using STELLAREST_F1.Data;

namespace STELLAREST_F1
{
    // --- How to Collider Remove From BaseObject ???
    public class Env : BaseCellObject
    {
        public EnvData EnvData { get; private set; } = null;
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

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Env;
            RigidBody.bodyType = RigidbodyType2D.Static;

            EnvBody = BaseBody as EnvBody;
            EnvAnim = BaseAnim as EnvAnimation;
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            EnvData = Managers.Data.EnvDataDict[dataID];
            EnvType = EnvData.EnvType;
            _maxLevelID = dataID;
            gameObject.name += $"_{EnvData.DevTextID.Replace(" ", "")}";
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            base.EnterInGame(spawnPos);
            EnvBody.StartCoFadeInEffect(startCallback: () =>
                        {
                            BaseEffect.GenerateEffect(
                                    effectID: EnvType == EEnvType.Tree ? 
                                            ReadOnly.DataAndPoolingID.DNPID_Effect_TeleportGreen :
                                            ReadOnly.DataAndPoolingID.DNPID_Effect_TeleportRed,
                                    spawnPos: Managers.Map.CellToCenteredWorld(Vector3Int.up + SpawnedCellPos)
                                    );
                        });
                        
            EnvState = EEnvState.Idle;

            // switch (EnvType)
            // {
            //     case EEnvType.Tree:
            //         {
            //             EnvBody.StartCoFadeInEffect(startCallback: () => 
            //             {
            //                 BaseEffect.GenerateEffect(
            //                             effectID: ReadOnly.DataAndPoolingID.DNPID_Effect_TeleportGreen,
            //                             spawnPos: Managers.Map.CellToCenteredWorld(Vector3Int.up + SpawnedCellPos)
            //                             );
            //             });
            //         }
            //         break;
            //     case EEnvType.Rock:
            //         {
            //             BaseEffect.GenerateEffect(
            //                         effectID: ReadOnly.DataAndPoolingID.DNPID_Effect_TeleportPurple,
            //                         spawnPos: Managers.Map.CellToCenteredWorld(Vector3Int.up + SpawnedCellPos)
            //                         );
            //         }
            //         break;
            // }

        }

        public override bool OnDamaged(BaseCellObject attacker, SkillBase skillByAttacker)
        {
            if (this.IsValid() == false)
                return false;

            float finalDamage = 1f;
            // --- TEMP
            if ((attacker as Hero).CreatureRarity == ECreatureRarity.Elite)
                finalDamage++;

            Hp = Mathf.Clamp(Hp - finalDamage, 0f, MaxHp);
            Managers.Object.ShowDamageFont(
                                            position: CenterPosition,
                                            damage: finalDamage,
                                            Color.white,
                                            isCritical: false,
                                            fontSignType: EFontSignType.None,
                                            EFontAnimationType.EndSmaller
                                        );
            if (Hp <= 0f)
            {
                Hp = 0f;
                OnDead(attacker, skillByAttacker);
            }
            else
            {
                HitShakeMovement(duration: 0.1f, power: 0.5f, vibrato: 20);
                EnvBody.StartCoHurtFlashEffect(isCritical: false);
            }

            return true;
        }

        // public override void OnDamaged(BaseCellObject attacker, SkillBase skillFromAttacker)
        // {
        //     if (this.IsValid() == false)
        //         return;

        //     float finalDamage = 1f;

        //     // --- TEMP
        //     if ((attacker as Hero).CreatureRarity == ECreatureRarity.Elite)
        //         finalDamage++;

        //     Hp = Mathf.Clamp(Hp - finalDamage, 0f, MaxHp);
        //     Managers.Object.ShowDamageFont(
        //                                     position: CenterPosition,
        //                                     damage: finalDamage,
        //                                     Color.white,
        //                                     isCritical: false,
        //                                     EFontOutAnimationType.OutSmaller
        //                                 );
        //     if (Hp <= 0f)
        //     {
        //         Hp = 0f;
        //         OnDead(attacker, skillFromAttacker);
        //         return;
        //     }
        //     else
        //     {
        //         HitShakeMovement(duration: 0.1f, power: 0.5f, vibrato: 20);
        //         EnvBody.StartCoHurtFlashEffect(isCritical: false);
        //     }
        // }

        public override void OnDead(BaseCellObject attacker, SkillBase skillFromAttacker)
        {
            //EnvBody.ResetMaterialsAndColors(); ---> BaseObject::EnterInGame
            EnvState = EEnvState.Dead;
            base.OnDead(attacker, skillFromAttacker);
            EnvBody.StartCoFadeOutEffect(
                startCallback: null,
                endCallback: () => OnDeadFadeOutCompleted()
            );
            // --- TODO : Drop Item
        }
        #endregion
    }
}
