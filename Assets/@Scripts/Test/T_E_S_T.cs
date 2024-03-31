using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using STELLAREST_F1;
using static STELLAREST_F1.Define;
using UnityEngine;

public class T_E_S_T : MonoBehaviour
{
    private float _movementSpeed = 3.0f;
    private Vector2 _moveDir = Vector3.zero;
    private int TestProperty { get; set; }

    private IEnumerator Start()
    {
        Managers.Game.OnMoveDirChangedHandler -= OnMoveDirChanged;
        Managers.Game.OnMoveDirChangedHandler += OnMoveDirChanged;
        yield return null;
        // yield return new WaitForSeconds(3f);
        // Debug.Log("MOVE START !!");
        // while (true)
        // {
        //     // 거 = 속 * 시
        //     float distance = _movementSpeed * Time.deltaTime;
        //     while (true)
        //     {
        //         transform.position += Vector3.up * distance;
        //         //transform.position = transform.position + (Vector3.up * _movementSpeed * Time.deltaTime);
        //         yield return null;
        //     }
        // }
    }

    private void Update()
    {
        if (Managers.Game.JoystickState == EJoystickState.Drag)
        {
            float distancePerFrame = _movementSpeed * Time.deltaTime;
            transform.Translate(_moveDir * distancePerFrame);
        }
    }

    private void OnMoveDirChanged(Vector2 dir)
    {
        _moveDir = dir;
    }
}

/* Memo

// 아이들
// 이동
// 공격
// 죽음

// 히어로 애니메이션 Upper, Lower 나눌 필요가 없을 것 같긴함.
// 나누면, 애니메이션 제작하기 너무 빡세짐
// 나눴을 때 이점은 세부적으로, 다양하게 조합이 가능하겠지만, 그럴 이유는 아직까진 없음.
// 그리고 이점이 하나 더 있긴한데, Movement Speed가 빨라지면 하반신만 빨라지도록. 이런식의 조정이 가능함.

    public SpriteRenderer[] GetSkin()
    {
        string[] names = System.Enum.GetNames(typeof(EHeroHead));
        for (int i = 0; i < names.Length; ++i)
        {
            if (names[i].Contains("Skin"))
            {
                SpriteRenderer spr = GetSpriteRenderer(Util.GetEnumFromString<EHeroHead>(names[i]));
                if (spr != null)
                    Skin.Add(spr);
                else
                    Debug.LogWarning($"{nameof(HeroBody)}, {nameof(GetSkin)}, Input : \"{names[i]}\"");
            }
        }

        names = System.Enum.GetNames(typeof(EHeroUpperBody));
        for (int i = 0; i < names.Length; ++i)
        {
            if (names[i].Contains("Skin"))
            {
                SpriteRenderer spr = GetSpriteRenderer(Util.GetEnumFromString<EHeroUpperBody>(names[i]));
                if (spr != null)
                    Skin.Add(spr);
                else
                    Debug.LogWarning($"{nameof(HeroBody)}, {nameof(GetSkin)}, Input : \"{names[i]}\"");
            }
        }

        names = System.Enum.GetNames(typeof(EHeroLowerBody));
        for (int i = 0; i < names.Length; ++i)
        {
            if (names[i].Contains("Skin"))
            {
                SpriteRenderer spr = GetSpriteRenderer(Util.GetEnumFromString<EHeroLowerBody>(names[i]));
                if (spr != null)
                    Skin.Add(spr);
                else
                    Debug.LogWarning($"{nameof(HeroBody)}, {nameof(GetSkin)}, Input : \"{names[i]}\"");
            }
        }

        return Skin.ToArray();
    }

    // Reflection Memo
    private void InitHeroSprites(int dataID, Transform[] targets)
    {
        Data.HeroSpriteData sprites = Managers.Data.HeroesSpritesDict[dataID];
        Type type = typeof(Data.HeroSpriteData);
        FieldInfo[] fields = type.GetFields();
    }

    private void InitHeroSprites(int dataID, Transform[] targets)
    {
        Data.HeroSpriteData sprites = Managers.Data.HeroesSpritesDict[dataID];

        Type type = typeof(Data.HeroSpriteData);
        FieldInfo[] fields = type.GetFields();
        for (int i = 0; i < fields.Length; ++i)
        {
            if (fields[i].FieldType != typeof(string) || fields[i].Name.Contains("Tag"))
                continue;

            string fieldValue = (string)fields[i].GetValue(sprites);
            if (string.IsNullOrEmpty(fieldValue))
                continue;

            string fieldName = fields[i].Name;
            EHeroBodyParts bodyPart = Util.GetEnumFromString<EHeroBodyParts>(fieldName);
            Sprite sprite = Managers.Resource.Load<Sprite>(fieldValue);
            targets[(int)bodyPart].GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
*/
