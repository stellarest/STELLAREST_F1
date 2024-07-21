using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Monster : Creature
    {
        private void Update()
        {
            if (Input.GetKeyDown("5"))
            {
                Debug.Log(ColliderRadius);
            }
        }

        #region Background
        public Data.MonsterData MonsterData { get; private set; } = null;
        [SerializeField] private MonsterBody _monsterBody = null;
        public MonsterBody MonsterBody
        {
            get => _monsterBody;
            private set
            {
                _monsterBody = value;
                if (CreatureBody == null)
                    CreatureBody = _monsterBody;
            }
        }
        public MonsterAnimation MonsterAnim { get; private set; } = null;
        public EMonsterType MonsterType { get; private set; } = EMonsterType.None;
        public override ECreatureAIState CreatureAIState
        {
            get => base.CreatureAIState;
            set
            {
                base.CreatureAIState = value;
                switch (value)
                {

                    case ECreatureAIState.Idle:
                        {
                            Moving = false;
                            return;
                        }

                    case ECreatureAIState.Move:
                        {
                            Moving = true;
                            return;
                        }

                    case ECreatureAIState.Dead:
                        {
                            Dead();
                            return;
                        }
                }
            }
        }
        //public MonsterAI MonsterAI { get; private set; } = null;
        private MonsterAI _monsterAI = null;
        #endregion

        #region Core
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Monster;
            MonsterAnim = CreatureAnim as MonsterAnimation;
            MonsterBody = GetComponent<MonsterBody>();
            return true;
        }

        protected override void InitialSetInfo(int dataID)
        {
            base.InitialSetInfo(dataID);
            _maxLevel = dataID;

            MonsterData = Managers.Data.MonsterDataDict[dataID];
            MonsterAnim.InitialSetInfo(dataID, this);
            MonsterBody.InitialSetInfo(dataID, this);

            Type aiClassType = Util.GetTypeFromName(MonsterData.AIClassName);
            CreatureAI = gameObject.AddComponent(aiClassType) as CreatureAI;
            CreatureAI.InitialSetInfo(this);
            _monsterAI = CreatureAI as MonsterAI;
            MonsterType = MonsterData.MonsterType;

            CreatureRarity = MonsterData.CreatureRarity;
            gameObject.name += $"_{MonsterData.NameTextID.Replace(" ", "")}";
            Collider.radius = MonsterData.ColliderRadius;

            CreatureSkill = gameObject.GetOrAddComponent<SkillComponent>();
            CreatureSkill.SetInfo(owner: this, MonsterData);
        }

        protected override void EnterInGame(Vector3 spawnPos)
        {
            MonsterBody.MonsterEmoji = EMonsterEmoji.Normal;
            LookAtDir = ELookAtDirection.Left; // --- Default Monsters Dir: Left
            base.EnterInGame(spawnPos);
            CreatureAIState = ECreatureAIState.Idle;
            MonsterBody.StartCoFadeInEffect(startCallback: () => 
            {
                Managers.Object.SpawnBaseObject<EffectBase>(
                    objectType: EObjectType.Effect,
                    spawnPos: Managers.Map.GetCenterWorld(Vector3Int.up + SpawnedCellPos),
                    dataID: ReadOnly.DataAndPoolingID.DNPID_Effect_TeleportRed,
                    owner: this);
            });
        }

        public override void OnDamaged(BaseObject attacker, SkillBase skillFromAttacker)
            => base.OnDamaged(attacker, skillFromAttacker);
        public override void OnDead(BaseObject attacker, SkillBase skillFromAttacker)
            => base.OnDead(attacker, skillFromAttacker);
        protected override void OnDisable()
            => base.OnDisable();
        #endregion
    }
}
