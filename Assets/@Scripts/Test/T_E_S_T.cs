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
