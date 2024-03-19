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
            public string Tag { get; set; } = null;
            public Transform TR { get; set; } = null;
            public SpriteRenderer SPR { get; set; } = null;
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
                        for (int i = 0; i < length; ++i)
                            containers[i] = new HeroBodyContainer();
                        _bodyDict.Add(EHeroBodyParts.Head, containers);

                        containers[(int)EHeroHead.Hair].Tag = ReadOnly.String.HBody_Hair;
                        containers[(int)EHeroHead.Hair].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Hair, true, true);
                        containers[(int)EHeroHead.Hair].SPR = containers[(int)EHeroHead.Hair].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroHead.Eyes].Tag = ReadOnly.String.HBody_Eyes;
                        containers[(int)EHeroHead.Eyes].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Eyes, true, true);
                        containers[(int)EHeroHead.Eyes].SPR = containers[(int)EHeroHead.Eyes].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroHead.Eyebrows].Tag = ReadOnly.String.HBody_Eyebrows;
                        containers[(int)EHeroHead.Eyebrows].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Eyebrows, true, true);
                        containers[(int)EHeroHead.Eyebrows].SPR = containers[(int)EHeroHead.Eyebrows].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroHead.Mouth].Tag = ReadOnly.String.HBody_Mouth;
                        containers[(int)EHeroHead.Mouth].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Mouth, true, true);
                        containers[(int)EHeroHead.Mouth].SPR = containers[(int)EHeroHead.Mouth].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroHead.Beard].Tag = ReadOnly.String.HBody_Beard;
                        containers[(int)EHeroHead.Beard].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Beard, true, true);
                        containers[(int)EHeroHead.Beard].SPR = containers[(int)EHeroHead.Beard].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroHead.Earrings].Tag = ReadOnly.String.HBody_Earrings;
                        containers[(int)EHeroHead.Earrings].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Earrings, true, true);
                        containers[(int)EHeroHead.Earrings].SPR = containers[(int)EHeroHead.Earrings].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroHead.Mask].Tag = ReadOnly.String.HBody_Mask;
                        containers[(int)EHeroHead.Mask].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Mask, true, true);
                        containers[(int)EHeroHead.Mask].SPR = containers[(int)EHeroHead.Mask].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroHead.Glasses].Tag = ReadOnly.String.HBody_Glasses;
                        containers[(int)EHeroHead.Glasses].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Glasses, true, true);
                        containers[(int)EHeroHead.Glasses].SPR = containers[(int)EHeroHead.Glasses].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroHead.Helmet].Tag = ReadOnly.String.HBody_Helmet;
                        containers[(int)EHeroHead.Helmet].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Helmet, true, true);
                        containers[(int)EHeroHead.Helmet].SPR = containers[(int)EHeroHead.Helmet].TR.GetComponent<SpriteRenderer>();
                    }
                    break;

                case EHeroBodyParts.UpperBody:
                    {
                        HeroBodyContainer[] containers = new HeroBodyContainer[length];
                        for (int i = 0; i < length; ++i)
                            containers[i] = new HeroBodyContainer();
                        _bodyDict.Add(EHeroBodyParts.UpperBody, containers);

                        containers[(int)EHeroUpperBody.Torso].Tag = ReadOnly.String.HBody_Torso;
                        containers[(int)EHeroUpperBody.Torso].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Torso, true, true);
                        containers[(int)EHeroUpperBody.Torso].SPR = containers[(int)EHeroUpperBody.Torso].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroUpperBody.Cape].Tag = ReadOnly.String.HBody_Cape;
                        containers[(int)EHeroUpperBody.Cape].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Cape, true, true);
                        containers[(int)EHeroUpperBody.Cape].SPR = containers[(int)EHeroUpperBody.Cape].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroUpperBody.ArmL].Tag = ReadOnly.String.HBody_ArmL;
                        containers[(int)EHeroUpperBody.ArmL].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ArmL, true, true);
                        containers[(int)EHeroUpperBody.ArmL].SPR = containers[(int)EHeroUpperBody.ArmL].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroUpperBody.ForearmL].Tag = ReadOnly.String.HBody_ForearmL;
                        containers[(int)EHeroUpperBody.ForearmL].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ForearmL, true, true);
                        containers[(int)EHeroUpperBody.ForearmL].SPR = containers[(int)EHeroUpperBody.ForearmL].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroUpperBody.HandL].Tag = ReadOnly.String.HBody_HandL;
                        containers[(int)EHeroUpperBody.HandL].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_HandL, true, true);
                        containers[(int)EHeroUpperBody.HandL].SPR = containers[(int)EHeroUpperBody.HandL].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroUpperBody.Finger].Tag = ReadOnly.String.HBody_Finger;
                        containers[(int)EHeroUpperBody.Finger].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Finger, true, true);
                        containers[(int)EHeroUpperBody.Finger].SPR = containers[(int)EHeroUpperBody.Finger].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroUpperBody.ArmR].Tag = ReadOnly.String.HBody_ArmR;
                        containers[(int)EHeroUpperBody.ArmR].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ArmR, true, true);
                        containers[(int)EHeroUpperBody.ArmR].SPR = containers[(int)EHeroUpperBody.ArmR].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroUpperBody.ForearmR].Tag = ReadOnly.String.HBody_ForearmR;
                        containers[(int)EHeroUpperBody.ForearmR].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ForearmR, true, true);
                        containers[(int)EHeroUpperBody.ForearmR].SPR = containers[(int)EHeroUpperBody.ForearmR].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroUpperBody.SleeveR].Tag = ReadOnly.String.HBody_SleeveR;
                        containers[(int)EHeroUpperBody.SleeveR].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_SleeveR, true, true);
                        containers[(int)EHeroUpperBody.SleeveR].SPR = containers[(int)EHeroUpperBody.SleeveR].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroUpperBody.HandR].Tag = ReadOnly.String.HBody_HandR;
                        containers[(int)EHeroUpperBody.HandR].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_HandR, true, true);
                        containers[(int)EHeroUpperBody.HandR].SPR = containers[(int)EHeroUpperBody.HandR].TR.GetComponent<SpriteRenderer>();
                    }
                    break;

                case EHeroBodyParts.LowerBody:
                    {
                        HeroBodyContainer[] containers = new HeroBodyContainer[length];
                        for (int i = 0; i < length; ++i)
                            containers[i] = new HeroBodyContainer();
                        _bodyDict.Add(EHeroBodyParts.LowerBody, containers);

                        containers[(int)EHeroLowerBody.Pelvis].Tag = ReadOnly.String.HBody_Pelvis;
                        containers[(int)EHeroLowerBody.Pelvis].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_Pelvis, true, true);
                        containers[(int)EHeroLowerBody.Pelvis].SPR = containers[(int)EHeroLowerBody.Pelvis].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroLowerBody.LegL].Tag = ReadOnly.String.HBody_LegL;
                        containers[(int)EHeroLowerBody.LegL].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_LegL, true, true);
                        containers[(int)EHeroLowerBody.LegL].SPR = containers[(int)EHeroLowerBody.LegL].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroLowerBody.ShinL].Tag = ReadOnly.String.HBody_ShinL;
                        containers[(int)EHeroLowerBody.ShinL].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ShinL, true, true);
                        containers[(int)EHeroLowerBody.ShinL].SPR = containers[(int)EHeroLowerBody.ShinL].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroLowerBody.LegR].Tag = ReadOnly.String.HBody_LegR;
                        containers[(int)EHeroLowerBody.LegR].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_LegR, true, true);
                        containers[(int)EHeroLowerBody.LegR].SPR = containers[(int)EHeroLowerBody.LegR].TR.GetComponent<SpriteRenderer>();

                        containers[(int)EHeroLowerBody.ShinR].Tag = ReadOnly.String.HBody_ShinR;
                        containers[(int)EHeroLowerBody.ShinR].TR = Util.FindChild<Transform>(Owner.gameObject, ReadOnly.String.HBody_ShinR, true, true);
                        containers[(int)EHeroLowerBody.ShinR].SPR = containers[(int)EHeroLowerBody.ShinR].TR.GetComponent<SpriteRenderer>();
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
