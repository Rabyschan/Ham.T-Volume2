using ChangeScene;
using UnityEngine.UI;
using UnityEngine;
using GameSave;

public class GameDataLoadButton : MonoBehaviour
{
    [Header("슬롯 고유 ID")]
    [SerializeField] private int slotId;

    [Header("로드 관련 UI")]
    [SerializeField] private GameObject savePopupUI;  // 저장 확인 팝업
    [SerializeField] private LoadScene loadScene;      // 로딩 처리 스크립트
    [SerializeField] private Button slotSelectButton;  // 슬롯 선택 버튼

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

        // 로딩 UI 보여주기 (또는 저장된 데이터 팝업 등)
        savePopupUI.SetActive(true);

        Debug.Log($"슬롯 {slotId} 선택됨 → 로딩 준비 완료");
    }
}
