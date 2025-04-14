using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool hasSaved = false; // 중복 방지

    private void OnTriggerEnter(Collider other)
    {
        int slotId = PlayerPrefs.GetInt("SelectedSlot", -1); // 선택된 슬롯 확인

        if (slotId != -1)
        {
            GameDataManager.Instance.SaveSlot(slotId); // 자동 저장
            Debug.Log($"CheckPoint 도달 → 슬롯 {slotId} 자동 저장됨!");
            UIManager.Instance.ShowNoticeByAnimator(2f, true); // 저장 알림

            hasSaved = true;

            // 콜라이더 비활성화 (또는 오브젝트 제거)
            // 1) 콜라이더만 비활성화:
            GetComponent<Collider>().enabled = false;

            // 2) 혹시 시각적 표시도 없애고 싶다면 전체 제거:
            // Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("저장 슬롯이 선택되지 않음 → 자동 저장 건너뜀");
        }
    }
}
