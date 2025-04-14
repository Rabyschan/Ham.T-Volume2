using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputManagerEntry;

public class CostumeItem : MonoBehaviour
{
    public int costumeId;
    public string itemName;

    [SerializeField] private float rotationSpeed = 450f;
    [SerializeField] GameObject costumitem;

    Vector3 _origin;

    public bool _isEating = false;

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
            _isEating = true;

            PlayerPrefs.SetInt($"HasCostume_{costumeId}", 1);
            PlayerPrefs.Save();

            CostumeUI.Instance.UpdateBtn(costumeId);
            Debug.Log("UI갱신");
            //UIManager.Instance.GetCostumeUI(costumeId);

            challangeCondition.Instance.GetItem(costumeId);

            Destroy(this.gameObject); // 아이템 제거
        }
    }
}
