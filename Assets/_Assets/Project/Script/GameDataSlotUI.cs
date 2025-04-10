using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChangeScene;

public class GameDataSlotUI : MonoBehaviour
{
    [Header("연결된 슬롯 ID (데이터 확인용)")]
    [SerializeField] public int slotId;
    [SerializeField] private Button slotSelectButton;  // 슬롯 선택 버튼

    [Header("UI 텍스트")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI costumeText;
    [SerializeField] private TextMeshProUGUI skinText;

    [Header("UI 오브젝트")]
    [SerializeField] private GameObject saveUI; //저장된 경우 보임
    [SerializeField] private GameObject emptyUI;    //저장 안된 경우 보임

    [Header("로드 관련 UI")]
    [SerializeField] private LoadScene loadScene;      // 로딩 처리 스크립트

    private void OnEnable()
    {
        UpdateSlotUI();    
    }

    private void Start()
    {
        // 슬롯 버튼 눌렀을 때 실행될 이벤트 등록
        slotSelectButton.onClick.AddListener(OnSlotSelected);
    }

    private void OnSlotSelected()
    {
        // 슬롯 ID를 PlayerPrefs에 저장
        PlayerPrefs.SetInt("SelectedSlot", slotId);

        // 로딩 준비
        loadScene.SetSlotId(slotId);

        Debug.Log($"슬롯 {slotId} 선택됨 → 로딩 준비 완료");
    }

    public void UpdateSlotUI()
    {
        bool hasSave = PlayerPrefs.HasKey($"PlayerPosX_{slotId}");

        if (hasSave)
        {
            saveUI.SetActive(true);
            emptyUI.SetActive(false);

            // 저장된 시간 불러오기
            string saveTime = PlayerPrefs.GetString($"SaveTime_{slotId}", "No Save");
            timeText.text = saveTime;

            // 코스튬 개수 계산
            int costumeCount = 0;
            int totalCostumes = 3; // 총 코스튬 수
            for (int i = 1; i <= totalCostumes; i++)
            {
                if (PlayerPrefs.GetInt($"Slot{slotId}_HasCostume_{i}", 0) == 1)
                    costumeCount++;
            }
            costumeText.text = $"Costume : {costumeCount}/{totalCostumes}";

            // 스킨 개수 계산
            int skinCount = 0;
            int totalSkins = 1; // 총 스킨 수 (예시)
            for (int i = 1; i <= totalSkins; i++)
            {
                if (PlayerPrefs.GetInt($"Slot{slotId}_HasSkin_{i}", 0) == 1)
                    skinCount++;
            }
            skinText.text = $"Skin : {skinCount}/{totalSkins}";
        }
        else
        {
            saveUI.SetActive(false);
            emptyUI.SetActive(true);
        }   
    }
}

