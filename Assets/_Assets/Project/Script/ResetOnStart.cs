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