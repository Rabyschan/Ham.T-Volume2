using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostumeScene : MonoBehaviour
{
    private HamsterController2 _player;
    void Start()
    {
        CostumeUI.Open();
        _player = FindObjectOfType<HamsterController2>();
        if (_player != null)
        {
            _player.allowInput = false; // 입력 정지
        }
        CursorManager.Instance.SetCursorVisable(true);
    }

    private void OnDestroy()
    {
        CostumeUI.Close();
    }
}
