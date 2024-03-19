using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1.Data;
using UnityEngine;
using UnityEngine.Diagnostics;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class HeroBody
    {
        public class HeroBodyContainer
        {
            public HeroBodyContainer(string tag, Transform tr, SpriteRenderer spr)
            {
                Tag = tag;
                TR = tr;
                SPR = spr;
            }

            public string Tag { get; private set; } = null;
            public Transform TR { get; private set; } = null;
            public SpriteRenderer SPR { get; private set; } = null;
        }

        public HeroBody(Hero owner)
        {
            Owner = owner;
            InitBody(EHeroBodyParts.Head, (int)EHeroHead.Max);
            InitBody(EHeroBodyParts.UpperBody, (int)EHeroUpperBody.Max);
            InitBody(EHeroBodyParts.LowerBody, (int)EHeroLowerBody.Max);
        }

        private void InitBody(EHeroBodyParts bodyParts, int length)
        {
            switch (bodyParts)
            {
                case EHeroBodyParts.Head:
                    {
                        HeroBodyContainer[] containers = new HeroBodyContainer[length];
                        _bodyDict.Add(EHeroBodyParts.Head, containers);

                        string tag = ReadOnly.String.HBody_Hair;
                        Transform tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Hair, true, true);
                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Hair] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Eyes;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Eyes, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Eyes] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Eyebrows;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Eyebrows, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Eyebrows] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Mouth;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Mouth, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Mouth] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Beard;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Beard, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Beard] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Earrings;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Earrings, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Earrings] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Mask;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Mask, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Mask] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Glasses;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Glasses, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Glasses] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Helmet;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Helmet, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroHead.Helmet] = new HeroBodyContainer(tag, tr, spr);
                    }
                    break;

                case EHeroBodyParts.UpperBody:
                    {
                        HeroBodyContainer[] containers = new HeroBodyContainer[length];
                        _bodyDict.Add(EHeroBodyParts.UpperBody, containers);

                        string tag = ReadOnly.String.HBody_Torso;
                        Transform tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Torso, true, true);
                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.Torso] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Cape;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Cape, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.Cape] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ArmL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ArmL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.ArmL] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ForearmL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ForearmL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.ForearmL] = new HeroBodyContainer(tag, tr, spr);
                     
                        tag = ReadOnly.String.HBody_HandL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_HandL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.HandL] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_Finger;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Finger, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.Finger] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ArmR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ArmR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.ArmR] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ForearmR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ForearmR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.ForearmR] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_SleeveR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_SleeveR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.SleeveR] = new HeroBodyContainer(tag, tr, spr);
                      
                        tag = ReadOnly.String.HBody_HandR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_HandR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroUpperBody.HandR] = new HeroBodyContainer(tag, tr, spr);
                    }
                    break;

                case EHeroBodyParts.LowerBody:
                    {
                        HeroBodyContainer[] containers = new HeroBodyContainer[length];
                        _bodyDict.Add(EHeroBodyParts.LowerBody, containers);

                        string tag = ReadOnly.String.HBody_Pelvis;
                        Transform tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Pelvis, true, true);
                        SpriteRenderer spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroLowerBody.Pelvis] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_LegL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_LegL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroLowerBody.LegL] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_ShinL;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ShinL, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroLowerBody.ShinL] = new HeroBodyContainer(tag, tr, spr);

                        tag = ReadOnly.String.HBody_LegR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_LegR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroLowerBody.LegR] = new HeroBodyContainer(tag, tr, spr);
                   
                        tag = ReadOnly.String.HBody_ShinR;
                        tr = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ShinR, true, true);
                        spr = tr.GetComponent<SpriteRenderer>();
                        containers[(int)EHeroLowerBody.ShinR] = new HeroBodyContainer(tag, tr, spr);
                    }
                    break;
            }
        }

        public void SetInfo(int dataID)
        {

        }

        public Hero Owner { get; private set; } = null;
        private Dictionary<EHeroBodyParts, HeroBodyContainer[]> _bodyDict = new Dictionary<EHeroBodyParts, HeroBodyContainer[]>();


        public void Test(EHeroBodyParts key)
        {
            HeroBodyContainer[] cons = _bodyDict[key];
            for (int i = 0; i < cons.Length; ++i)
            {
                Debug.Log($"TAG[{i}] : {cons[i].TR.name}");
            }
        }

        public HeroBodyContainer[] GetBody(EHeroBodyParts bodyPart)
        {
            return _bodyDict[bodyPart];
        }


        // public Transform GetHead(EHeroHead headType)
        // {
        //     if (headType == EHeroHead.None || headType == EHeroHead.Max)
        //     {
        //         Debug.LogError($"{nameof(HeroBody)}, {nameof(GetHead)}, Input : \"{headType}\"");
        //         return null;
        //     }

        //     return _head[(int)headType];
        // }

        // private Transform[] _bodyParts = new Transform[(int)EHeroBodyParts.Max];
        // public void SetInfo(Hero target, int dataID)
        // {
        //     _target = target;
        //     for (int i = 0; i < _bodyParts.Length; ++i)
        //     {
        //         // Head
        //         _bodyParts[(int)EHeroBodyParts.Hair] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_Hair, true, true);
        //         _bodyParts[(int)EHeroBodyParts.Eyes] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_Eyes, true, true);
        //         _bodyParts[(int)EHeroBodyParts.Eyebrows] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_Eyebrows, true, true);
        //         _bodyParts[(int)EHeroBodyParts.Mouth] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_Mouth, true, true);
        //         _bodyParts[(int)EHeroBodyParts.Beard] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_Beard, true, true);
        //         _bodyParts[(int)EHeroBodyParts.Earrings] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_Earrings, true, true);
        //         _bodyParts[(int)EHeroBodyParts.Mask] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_Mask, true, true);
        //         _bodyParts[(int)EHeroBodyParts.Glasses] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_Glasses, true, true);
        //         _bodyParts[(int)EHeroBodyParts.Helmet] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_Helmet, true, true);

        //         // UPPER
        //         _bodyParts[(int)EHeroBodyParts.Torso] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_Torso, true, true);
        //         _bodyParts[(int)EHeroBodyParts.Cape] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_Cape, true, true);
        //         _bodyParts[(int)EHeroBodyParts.ArmL] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_ArmL, true, true);
        //         _bodyParts[(int)EHeroBodyParts.ForearmL] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_ForearmL, true, true);
        //         _bodyParts[(int)EHeroBodyParts.HandL] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_HandL, true, true);
        //         _bodyParts[(int)EHeroBodyParts.Finger] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_Finger, true, true);
        //         _bodyParts[(int)EHeroBodyParts.ArmR] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_ArmR, true, true);
        //         _bodyParts[(int)EHeroBodyParts.ForearmR] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_ForearmR, true, true);
        //         _bodyParts[(int)EHeroBodyParts.SleeveR] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_SleeveR, true, true);
        //         _bodyParts[(int)EHeroBodyParts.HandR] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_HandR, true, true);

        //         // LOWER
        //         _bodyParts[(int)EHeroBodyParts.Pelvis] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_Pelvis, true, true);
        //         _bodyParts[(int)EHeroBodyParts.LegL] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_LegL, true, true);
        //         _bodyParts[(int)EHeroBodyParts.ShinL] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_ShinL, true, true);
        //         _bodyParts[(int)EHeroBodyParts.LegR] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_LegL, true, true);
        //         _bodyParts[(int)EHeroBodyParts.ShinR] = Util.FindChild<Transform>(target.gameObject, ReadOnly.String.HBody_ShinR, true, true);
        //     }

        //     Managers.Sprite.InitSprites(dataID, _bodyParts, EObjectType.Hero);
        // }

        // public Transform GetBodyPart(EHeroBodyParts bodyPart)
        // {
        //     if (bodyPart == EHeroBodyParts.None || bodyPart == EHeroBodyParts.Max)
        //     {
        //         Debug.LogError($"{nameof(HeroBody)}, {nameof(GetBodyPart)}, Input : \"{bodyPart}\"");
        //         return null;
        //     }

        //     return _bodyParts[(int)bodyPart];
        // }
    }

    public class Hero : Creature
    {
        public Data.HeroData HeroData { get; private set; } = null;
        public HeroBody HeroBody { get; private set; } = null;


        private HeroAnimation _heroAnim = null;


        private Vector2 _moveDir = Vector2.zero;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Hero;
            CreatureState = ECreatureState.Idle;
            Speed = 5.0f;

            Managers.Game.OnMoveDirChangedHandler -= OnMoveDirChanged;
            Managers.Game.OnMoveDirChangedHandler += OnMoveDirChanged;

            Managers.Game.OnJoystickStateChangedHandler -= OnJoystickStateChanged;
            Managers.Game.OnJoystickStateChangedHandler += OnJoystickStateChanged;

            if (BaseAnim != null)
                _heroAnim = BaseAnim as HeroAnimation;


            HeroBody = new HeroBody(this);

            // TEMP
            //Torso_Armor_Temp = Util.FindChild<Transform>(gameObject, "Torso_Armor", true, true);
            return true;
        }

        /*
            foreach (var pair in Managers.Data.HeroesSpritesDict)
            {
                Debug.Log($"Key : {pair.Key}");
                Debug.Log($"Value -  DataID : {pair.Value.DataID}");
                Debug.Log($"Value -  Tag : {pair.Value.Tag}");
                Debug.Log($"Value -  Torso : {pair.Value.Torso}");
                Debug.Log($"Value -  Cape : {pair.Value.Cape}");
                Debug.Log($"Value -  ArmL : {pair.Value.ArmL}");
                Debug.Log($"Value -  ForearmL : {pair.Value.ForearmL}");
                Debug.Log($"Value -  HandL : {pair.Value.HandL}");
                Debug.Log($"Value -  Finger : {pair.Value.Finger}");
                Debug.Log($"Value -  ArmR : {pair.Value.ArmR}");
                Debug.Log($"Value -  ForearmR : {pair.Value.ForearmR}");
                Debug.Log($"Value -  SleeveR : {pair.Value.SleeveR}");
                Debug.Log($"Value -  HandR : {pair.Value.HandR}");
                Debug.Log($"Value -  WeaponType : {pair.Value.WeaponType}");
                Debug.Log($"Value -  WeaponName : {pair.Value.WeaponName}");
                Debug.Log($"Value -  Leg : {pair.Value.Leg}");
                Debug.Log($"Value -  Shin : {pair.Value.Shin}");
            }
        */
        public override void SetInfo(int dataID)
        {
            base.SetInfo(dataID);
            //HeroBody.SetInfo(this, dataID);

            Data.HeroSpriteData data = Managers.Data.HeroesSpritesDict[ReadOnly.Numeric.DataID_Lancer];
            HeroData = Managers.Data.HeroesDict[dataID];
        }

        public EHeroBodyParts PartsTest = EHeroBodyParts.Head;

        private void Test()
        {
            HeroBody.Test(PartsTest);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                Test();

            float distancePerFrame = Speed * Time.deltaTime;
            transform.TranslateEx(_moveDir * distancePerFrame);
        }

        private void OnMoveDirChanged(Vector2 dir)
        {
            _moveDir = dir;
        }

        private void OnJoystickStateChanged(EJoystickState joystickState)
        {
            switch (joystickState)
            {
                case EJoystickState.PointerDown:
                    CreatureState = ECreatureState.Move;
                    break;

                case EJoystickState.PointerUp:
                    CreatureState = ECreatureState.Idle;
                    break;

                case EJoystickState.Drag:
                    break;

                default:
                    break;
            }
        }
    }
}
