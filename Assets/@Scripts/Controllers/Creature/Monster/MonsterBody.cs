using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    [System.Serializable]
    public class MonsterBody : CreatureBody
    {
        public MonsterBody(Monster owner, int dataID) : base(owner, dataID)
        {
            Data.MonsterData monsterData = Managers.Data.MonsterDataDict[dataID];
            this.Tag = monsterData.DescriptionTextID;
            this.Type = monsterData.MonsterType;
            this.Size = Util.GetEnumFromString<EMonsterSize>(monsterData.MonsterSize);
            _heads = new Sprite[(int)EMonsterEmoji.Max];
            InitBody(Type);
        }

        public string Tag { get; private set; } = string.Empty;
        public EMonsterType Type { get; private set; } = EMonsterType.None;
        public EMonsterSize Size { get; private set; } = EMonsterSize.None;

        private Sprite[] _heads = null;
        private Dictionary<EBirdBodyParts, Container> _birdBodyDict = null;
        private Dictionary<EQuadrupedsParts, Container> _quadrupedsBodyDict = null; // TODO : 추가해볼것

        private void InitBody(EMonsterType monsterType)
        {
            switch (monsterType)
            {
                case EMonsterType.Bird:
                    {
                        _birdBodyDict = new Dictionary<EBirdBodyParts, Container>();

                        string tag = ReadOnly.Util.AnimationBody;
                        Transform tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        _birdBodyDict.Add(EBirdBodyParts.Body, new Container(tag, tr, spr));
                        
                        tag = ReadOnly.Util.MBody_Head;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.MBody_Head, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        _birdBodyDict.Add(EBirdBodyParts.Head, new Container(tag, tr, spr));

                        tag = ReadOnly.Util.MBody_Wing;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.MBody_Wing, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        _birdBodyDict.Add(EBirdBodyParts.Wing, new Container(tag, tr, spr));

                        tag = ReadOnly.Util.MBody_LegL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.MBody_LegL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        _birdBodyDict.Add(EBirdBodyParts.LegL, new Container(tag, tr, spr));

                        tag = ReadOnly.Util.MBody_LegR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.MBody_LegR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        _birdBodyDict.Add(EBirdBodyParts.LegR, new Container(tag, tr, spr));

                        tag = ReadOnly.Util.MBody_Tail;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.MBody_Tail, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        _birdBodyDict.Add(EBirdBodyParts.Tail, new Container(tag, tr, spr));
                    }
                    break;

                case EMonsterType.Quadrupeds:
                    {
                        _quadrupedsBodyDict = new Dictionary<EQuadrupedsParts, Container>();

                        string tag = ReadOnly.Util.AnimationBody;
                        Transform tr = Util.FindChild<Transform>(Owner.gameObject, tag, true, true);
                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        _quadrupedsBodyDict.Add(EQuadrupedsParts.Body, new Container(tag, tr, spr));

                        tag = ReadOnly.Util.MBody_Head;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.MBody_Head, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        _quadrupedsBodyDict.Add(EQuadrupedsParts.Head, new Container(tag, tr, spr));

                        tag = ReadOnly.Util.MBody_LegFrontL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.MBody_LegFrontL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        _quadrupedsBodyDict.Add(EQuadrupedsParts.LegFrontL, new Container(tag, tr, spr));

                        tag = ReadOnly.Util.MBody_LegFrontR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.MBody_LegFrontR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        _quadrupedsBodyDict.Add(EQuadrupedsParts.LegFrontR, new Container(tag, tr, spr));

                        tag = ReadOnly.Util.MBody_LegBackL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.MBody_LegBackL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        _quadrupedsBodyDict.Add(EQuadrupedsParts.LegBackL, new Container(tag, tr, spr));

                        tag = ReadOnly.Util.MBody_LegBackR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.MBody_LegBackR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        _quadrupedsBodyDict.Add(EQuadrupedsParts.LegBackR, new Container(tag, tr, spr));

                        tag = ReadOnly.Util.MBody_Tail;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.Util.MBody_Tail, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        _quadrupedsBodyDict.Add(EQuadrupedsParts.Tail, new Container(tag, tr, spr));
                    }
                    break;
            }
        }

        [SerializeField] private EMonsterEmoji _monsterEmoji = EMonsterEmoji.None;
        public EMonsterEmoji MonsterEmoji
        {
            get => _monsterEmoji;
            set
            {
                if (_monsterEmoji != value)
                {
                    _monsterEmoji = value;
                    switch (value)
                    {
                        case EMonsterEmoji.Default:
                            {
                                if (Type == EMonsterType.Bird)
                                    _birdBodyDict[EBirdBodyParts.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Default];
                                else if (Type == EMonsterType.Quadrupeds)
                                    _quadrupedsBodyDict[EQuadrupedsParts.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Default];
                            }
                            break;

                        case EMonsterEmoji.Angry:
                            {
                                if (Type == EMonsterType.Bird)
                                    _birdBodyDict[EBirdBodyParts.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Angry];
                                else if (Type == EMonsterType.Quadrupeds)
                                    _quadrupedsBodyDict[EQuadrupedsParts.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Angry];
                            }
                            break;

                        case EMonsterEmoji.Dead:
                            {
                                if (Type == EMonsterType.Bird)
                                    _birdBodyDict[EBirdBodyParts.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Dead];
                                else if (Type == EMonsterType.Quadrupeds)
                                    _quadrupedsBodyDict[EQuadrupedsParts.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Dead];
                            }
                            break;
                    }
                }
            }
        }

        // TODO : 개선 필요, 타입에 따라 분기하도록
        public void SetEmoji(EMonsterEmoji emoji)
        {
            switch (emoji)
            {
                case EMonsterEmoji.Default:
                    {
                        if (Type == EMonsterType.Bird)
                            _birdBodyDict[EBirdBodyParts.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Default];
                        else if (Type == EMonsterType.Quadrupeds)
                            _quadrupedsBodyDict[EQuadrupedsParts.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Default];
                    }
                    break;

                case EMonsterEmoji.Angry:
                    {
                        if (Type == EMonsterType.Bird)
                            _birdBodyDict[EBirdBodyParts.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Angry];
                        else if (Type == EMonsterType.Quadrupeds)
                            _quadrupedsBodyDict[EQuadrupedsParts.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Angry];
                    }
                    break;

                case EMonsterEmoji.Dead:
                    {
                        if (Type == EMonsterType.Bird)
                            _birdBodyDict[EBirdBodyParts.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Dead];
                        else if (Type == EMonsterType.Quadrupeds)
                            _quadrupedsBodyDict[EQuadrupedsParts.Head].SPR.sprite = _heads[(int)EMonsterEmoji.Dead];
                    }
                    break;
            }
        }

        public void SetFace(EMonsterEmoji emoji, string spriteLabel)
            => _heads[(int)emoji] = Managers.Resource.Load<Sprite>(spriteLabel);
            
        public T GetComponent<T>(EBirdBodyParts findTarget) where T : UnityEngine.Component
        {
            if (findTarget == EBirdBodyParts.None || findTarget == EBirdBodyParts.Max)
                return null;

            if (_birdBodyDict.TryGetValue(findTarget, out Container container) == false)
                return null;

            Type type = typeof(T);
            if (type == typeof(Transform))
                return container.TR as T;
            else if (type == typeof(SpriteRenderer))
                return container.SPR as T;

            return null;
        }

        public T GetComponent<T>(EQuadrupedsParts findTarget) where T : UnityEngine.Component
        {
            if (findTarget == EQuadrupedsParts.None || findTarget == EQuadrupedsParts.Max)
                return null;

            if (_quadrupedsBodyDict.TryGetValue(findTarget, out Container container) == false)
                return null;

            Type type = typeof(T);
            if (type == typeof(Transform))
                return container.TR as T;
            else if (type == typeof(SpriteRenderer))
                return container.SPR as T;

            return null;
        }

        public override Vector3 GetFirePosition()
        {
            // 개선 필요
            return GetOwner<Monster>().MonsterBody.GetFirePosition();
        }
    }
}
