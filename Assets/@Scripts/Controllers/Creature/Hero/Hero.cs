using System.Collections;
using System.Collections.Generic;
using STELLAREST_F1.Data;
using UnityEngine;
using UnityEngine.Diagnostics;
using static STELLAREST_F1.Define;

namespace STELLAREST_F1
{
    public class Hero : Creature
    {
        public Data.HeroData HeroData { get; private set; } = null;
        public Data.HeroSpriteData HeroSpriteData { get; private set; } = null;
        public Data.HeroAnimationData HeroAnimationData { get; private set; } = null;   

        private EHeroEmoji _heroEmoji = EHeroEmoji.Default;     
        public EHeroEmoji HeroEmoji
        {
            get => _heroEmoji;
            set
            {
                if (_heroEmoji != value)
                {
                    _heroEmoji = value;
                    //SetEmoji(value);
                }
            }
        }

        public HeroBody HeroBody { get; private set; } = null;

        private HeroAnimation _heroAnim = null;
        public override BaseAnimation BaseAnim 
        { 
            get => _heroAnim;
            protected set
            {
                if (_heroAnim == null)
                    _heroAnim = value as HeroAnimation;
            } 
        }

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

            HeroBody = new HeroBody(this);
            return true;
        }

        public override void SetInfo(int dataID)
        {
            base.SetInfo(dataID);
            Managers.Sprite.SetInfo(dataID, target: this);
            BaseAnim.SetInfoFromOwner(dataID, this);
            
            HeroData = Managers.Data.HeroDataDict[dataID];
            HeroSpriteData = Managers.Data.HeroSpriteDataDict[dataID];
            HeroAnimationData = Managers.Data.HeroAnimationDataDict[dataID];
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                HeroBody.SetEmoji(EHeroEmoji.Default);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                HeroBody.SetEmoji(EHeroEmoji.Sick);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                HeroBody.SetEmoji(EHeroEmoji.Dead);
            }

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
                    break;

                case EJoystickState.PointerUp:
                    CreatureState = ECreatureState.Idle;
                    break;

                case EJoystickState.Drag:
                    CreatureState = ECreatureState.Move;
                    break;

                default:
                    break;
            }
        }
    }
}
