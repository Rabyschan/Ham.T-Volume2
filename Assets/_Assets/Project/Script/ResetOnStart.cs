using System.Collections;
using UnityEngine;

public class ResetOnStart : MonoBehaviour
{
    public int defaultSlotId = 0;

    void Awake()
    {
        int selectedSlot = PlayerPrefs.GetInt("SelectedSlot", -1);
        bool needsInit = PlayerPrefs.GetInt("NeedsInit", 0) == 1;

        if (selectedSlot == -1 || !PlayerPrefs.HasKey($"PlayerPosX_{selectedSlot}"))
        {
            ResetAllCostumeDataOnStart();
        }
        else
        {
            Debug.Log("저장된 데이터가 있으므로 초기화하지 않음.");
        }
        PlayerPrefs.DeleteKey("NeedsInit"); // 한 번만 초기화하게
    }

    void Start()
    {
        // 씬이 전환된 후 점수 UI 갱신
        int selectedSlot = PlayerPrefs.GetInt("SelectedSlot", -1);
        StartCoroutine(DelayedScoreLoad(selectedSlot));
        GameDataManager.Instance.LoadCheckpoint(selectedSlot);
    }

    IEnumerator DelayedScoreLoad(int selectedSlot)
    {
        // UIManager 인스턴스가 완성될 때까지 기다림
        yield return new WaitUntil(() => UIManager.Instance != null);

        // ScoreText가 씬에 실제로 생성된 다음 한 프레임 대기
        yield return null;

        //int selectedSlot = PlayerPrefs.GetInt("SelectedSlot", -1);
        int loadedScore = PlayerPrefs.GetInt($"Score_{selectedSlot}", 0);

        Debug.Log($"로드된 점수: {loadedScore}");
        GameManager.Instance.SetScoreDirect(loadedScore); // 이 시점이면 안전
    }

    private void ResetAllCostumeDataOnStart()
    {
        PlayerPrefs.DeleteKey("SelectedCostume");
        PlayerPrefs.DeleteKey("SelectedSkined");

        for (int i = 1; i <= 3; i++)
        {
            PlayerPrefs.SetInt($"HasCostume_{i}", 0);
        }

        PlayerPrefs.Save();
        Debug.Log("저장된 데이터가 없어서 초기화 완료!");
    }
}