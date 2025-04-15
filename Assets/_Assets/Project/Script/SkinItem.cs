using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinItem : MonoBehaviour
{
    public int skinId;

    [SerializeField] private float rotationSpeed = 450f;
    [SerializeField] GameObject skinItem;

    Vector3 _origin;

    private void Awake()
    {
        _origin = transform.eulerAngles;
    }

    private void Update()
    {
        Vector3 rotation = transform.eulerAngles;
        rotation.y += rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, _origin.z);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 특정 태그의 아이템과 충돌 감지
        {
            PlayerPrefs.SetInt($"HasSkin_{skinId}", 1);
            PlayerPrefs.Save();

            GameManager.Instance.isEating = true;

            CostumeUI.Instance.SkinButton(skinId);
            Debug.Log("UI갱신");

            Destroy(this.gameObject); // 아이템 제거
        }
    }
}
