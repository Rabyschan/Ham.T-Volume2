using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallangeUI : MonoBehaviour
{
    private static ChallangeUI _instance;

    public static ChallangeUI Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에 추가된 코스튬 매니저를 찾는다.
                _instance = FindObjectOfType<ChallangeUI>();
            }

            //// 찾아봐도 없으면 새로 생성
            //if (_instance == null)
            //{
            //    var prefab = Resources.Load<CostumeUI>("CostumeUInew");
            //    _instance = Instantiate(prefab);
            //    DontDestroyOnLoad(_instance.gameObject);
            //    Close();
            //}

            return _instance;
        }
    }

    public GameObject challange1;
    public GameObject challange2;
    public GameObject challange3;

    public GameObject challange_Text;
    public Sprite challangeImage;

    void Start()
    {

    }
    public void Challange1(string Challange1_Text)
    {
        Transform child = transform.Find(Challange1_Text);

        if (child != null)
        {
            // 자식 오브젝트를 활성화
            child.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("자식 오브젝트를 찾을 수 없습니다: " + Challange1_Text);
        }
    }


    public void ChangeSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = challangeImage;
    }

    public GameObject[] parentObjects;  // 부모 오브젝트 배열
    public Sprite ChanllangeSprite;         // 하나의 스프라이트
    private int currentCondition = 0;   // 현재 조건을 나타내는 변수 (1, 2, 3)

    void Update()
    {
        // 조건 1 달성 시
        if (currentCondition == 1)
        {
            ActivateParentObject(0);  // 부모 1을 활성화
        }
        // 조건 2 달성 시
        else if (currentCondition == 2)
        {
            ActivateParentObject(1);  // 부모 2를 활성화
        }
        // 조건 3 달성 시
        else if (currentCondition == 3)
        {
            ActivateParentObject(2);  // 부모 3을 활성화
        }
    }

    // 조건에 맞는 부모 오브젝트의 자식과 스프라이트를 변경하는 함수
    void ActivateParentObject(int index)
    {
        // 부모 오브젝트가 있는지 확인
        if (parentObjects[index] != null)
        {
            // 자식 오브젝트 활성화 (여기서 자식 오브젝트는 첫 번째 자식으로 가정)
            Transform childTransform = parentObjects[index].transform.GetChild(0);
            if (childTransform != null)
            {
                childTransform.gameObject.SetActive(true);  // 자식 오브젝트 활성화
            }

            // 부모 오브젝트의 스프라이트 변경
            SpriteRenderer spriteRenderer = parentObjects[index].GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = ChanllangeSprite;  // 스프라이트 변경
            }
        }
    }

    // 조건을 변경하는 메서드 (예: 조건을 1, 2, 3으로 변경)
    public void SetCondition(int condition)
    {
        if (condition >= 1 && condition <= 3)
        {
            currentCondition = condition;
        }
    }
}
