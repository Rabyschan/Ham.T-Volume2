using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class ChallengeUI : MonoBehaviour
{
    private static ChallengeUI _instance;
    public static ChallengeUI Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ChallengeUI>();
            return _instance;
        }
    }

    public TextMeshProUGUI scoreText;          // UI에 표시할 점수 텍스트
    public Transform canvasParent;             // 프리팹이 생성될 부모(Canvas 또는 Panel)

    [System.Serializable]
    public class ChallengeData
    {
        public int conditionID;                // 조건 ID
        public GameObject parentObject;        // 조건에 해당하는 부모 오브젝트
        public GameObject rewardPrefab;        // 조건 달성 시 생성할 프리팹
        [HideInInspector] public Sprite originalSprite; // 원래 스프라이트 저장용
    }

    public ChallengeData[] challengeList;      // 조건 목록
    public Sprite challengeSprite;             // 조건 달성 시 바꿀 스프라이트

    private Dictionary<int, GameObject> challengeMap = new Dictionary<int, GameObject>();
    private HashSet<int> activatedConditions = new HashSet<int>();

    void Awake()
    {
        foreach (var data in challengeList)
        {
            // ID - 오브젝트 매핑
            if (!challengeMap.ContainsKey(data.conditionID))
                challengeMap.Add(data.conditionID, data.parentObject);

            // 원래 스프라이트 저장
            if (data.parentObject != null)
            {
                Image img = data.parentObject.GetComponent<Image>();
                if (img != null)
                    data.originalSprite = img.sprite;
                else
                {
                    SpriteRenderer sr = data.parentObject.GetComponent<SpriteRenderer>();
                    if (sr != null)
                        data.originalSprite = sr.sprite;
                }
            }
        }
    }

    public void CompleteCondition(int conditionID)
    {
        if (activatedConditions.Contains(conditionID)) return;

        if (challengeMap.TryGetValue(conditionID, out GameObject parentObj) && parentObj != null)
        {
            // 자식 오브젝트 활성화
            Transform child = parentObj.transform.GetChild(0);
            if (child != null)
                child.gameObject.SetActive(true);

            // 스프라이트 변경
            Image image = parentObj.GetComponent<Image>();
            if (image != null)
                image.sprite = challengeSprite;
            else
            {
                SpriteRenderer sr = parentObj.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = challengeSprite;
            }

            // 조건 달성 프리팹 생성
            ChallengeData data = GetChallengeData(conditionID);
            if (data != null && data.rewardPrefab != null && canvasParent != null)
            {
                GameObject popup = Instantiate(data.rewardPrefab, canvasParent);

                Animator animator = popup.GetComponent<Animator>();
                if (animator != null)
                {
                    StartCoroutine(WaitAndDestroy(popup, 4f));
                }
                else
                {
                    // 애니메이터 없으면 그냥 자동 제거
                    Destroy(popup, 2f);
                }
            }

            // 중복 처리 방지
            activatedConditions.Add(conditionID);
        }
    }

    private IEnumerator WaitAndDestroy(GameObject obj, float wait)
    {
        yield return new WaitForSeconds(wait);
        Destroy(obj);
    }



    // 조건 원복 및 초기화
    public void ResetAllConditions()
    {
        foreach (var data in challengeList)
        {
            if (data.parentObject == null) continue;

            // 자식 비활성화
            Transform child = data.parentObject.transform.GetChild(0);
            if (child != null)
                child.gameObject.SetActive(false);

            // 스프라이트 복원
            Image image = data.parentObject.GetComponent<Image>();
            if (image != null)
                image.sprite = data.originalSprite;
            else
            {
                SpriteRenderer sr = data.parentObject.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = data.originalSprite;
            }
        }

        activatedConditions.Clear();
    }

    // 특정 ID의 ChallengeData 가져오기
    private ChallengeData GetChallengeData(int conditionID)
    {
        foreach (var data in challengeList)
        {
            if (data.conditionID == conditionID)
                return data;
        }
        return null;
    }

    // 예시: ChallengeUI.Instance.CompleteCondition(1);
    //       ChallengeUI.Instance.ResetAllConditions();
}


