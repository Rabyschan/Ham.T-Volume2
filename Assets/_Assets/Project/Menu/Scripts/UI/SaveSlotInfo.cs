using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotInfo : MonoBehaviour
{
    public static event Action<SlotData> OnSlotSelected;

    public int slotId;

    [SerializeField] private Button bTN_Slot;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI skinText;
    [SerializeField] private TextMeshProUGUI costumeText;
    
    private void Awake()
    {
        bTN_Slot.onClick.AddListener(OnSlotClicked);
    }

    private void Start()
    {
        // UI 초기화
        if (GameDataManager.Instance.TryGetSlotData(slotId, out var data))
        {
            UpdateUI(data);
        }
        else
        {
            timeText.text = "No Save";
            costumeText.text = $"Costume : 0/{GameDataManager.Instance.totalCostumes}";
            skinText.text = $"Skin : 0/{GameDataManager.Instance.totalSkins}";
        }
    }

    private void OnSlotClicked()
    {
        PlayerPrefs.SetInt("SelectedSlot", slotId);
        Debug.Log($"슬롯 {slotId} 클릭됨");

        SlotData data;

        if (GameDataManager.Instance.TryGetSlotData(slotId, out data))
        {
            UpdateUI(data);
            Debug.Log($"슬롯 {slotId} 선택 완료: 저장된 데이터 있음");
        }
        else
        {
            Debug.Log("빈 슬롯 클릭됨: 저장된 데이터 없음");
            data = new SlotData
            {
                slotId = slotId,
                saveTime = "No Save",
                costumeCount = 0,
                totalCostumes = GameDataManager.Instance.totalCostumes,
                skinCount = 0,
                totalSkins = GameDataManager.Instance.totalSkins,
                position = Vector3.zero
            };
            UpdateUI(data); // 빈 슬롯도 UI 업데이트 필요함
        }

        // 무조건 호출되어야 함 (빈 슬롯이든 아니든)
        OnSlotSelected?.Invoke(data);
    }


    private void UpdateUI(SlotData data)
    {
        timeText.text = data.saveTime;
        costumeText.text = $"Costume : {data.costumeCount}/{data.totalCostumes}";
        skinText.text = $"Skin : {data.skinCount}/{data.totalSkins}";
    }
}
