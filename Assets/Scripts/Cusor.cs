using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cusor : MonoBehaviour
{
    // Start is called before the first frame update

    public void cursor()
    {
        // ���콺�� ȭ�� �߾ӿ� �����ϰ� ������ �ʰ� ����
        Cursor.lockState = CursorLockMode.Locked;  // ���콺�� ȭ�� �߾ӿ� ����
        Cursor.visible = false;                    // ���콺�� ȭ�鿡�� ������ �ʰ� ����
    }

}
