using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    public int totalCostumes = 3;
    public int totalSkins = 1;

    [SerializeField] private Transform player;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool TryGetSlotData(int slotId, out SlotData data)
    {
        data = new SlotData { slotId = slotId };

        if (!IsSlotSaved(slotId))
            return false;

        data.position = LoadVector3($"PlayerPos", slotId);

        data.saveTime = PlayerPrefs.GetString($"SaveTime_{slotId}", "No Save");

        for (int i = 1; i <= totalCostumes; i++)
        {
            if (PlayerPrefs.GetInt($"Slot{slotId}_HasCostume_{i}", 0) == 1)
                data.costumeCount++;
        }

        for (int i = 1; i <= totalSkins; i++)
        {
            if (PlayerPrefs.GetInt($"Slot{slotId}_HasSkin_{i}", 0) == 1)
                data.skinCount++;
        }

        data.totalCostumes = totalCostumes;
        data.totalSkins = totalSkins;

        return true;
    }

    public void SaveSlot(int slotId)
    {
        if (player != null)
        {
            SaveVector3("PlayerPos", slotId, player.position);
        }
        else
        {
            // 플레이어 객체가 없을 경우 기본 위치 저장 (선택사항)
            SaveVector3("PlayerPos", slotId, Vector3.zero);
        }

        // 선택된 코스튬/스킨 저장
        int selectedCostume = PlayerPrefs.GetInt("SelectedCostume", 0);
        int selectedSkin = PlayerPrefs.GetInt("SelectedSkined", 0);
        PlayerPrefs.SetInt($"SelectedCostume_{slotId}", selectedCostume);
        PlayerPrefs.SetInt($"SelectedSkined_{slotId}", selectedSkin);

        // 현재 시간 저장
        string formattedTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm", CultureInfo.CurrentCulture);
        PlayerPrefs.SetString($"SaveTime_{slotId}", formattedTime);

        // 보유 코스튬 저장
        for (int i = 1; i <= totalCostumes; i++)
        {
            int hasCostume = PlayerPrefs.GetInt($"HasCostume_{i}", 0);
            PlayerPrefs.SetInt($"Slot{slotId}_HasCostume_{i}", hasCostume);
        }

        // 보유 스킨 저장
        for (int i = 1; i <= totalSkins; i++)
        {
            int hasSkin = PlayerPrefs.GetInt($"HasSkin_{i}", 0);
            PlayerPrefs.SetInt($"Slot{slotId}_HasSkin_{i}", hasSkin);
        }

        PlayerPrefs.Save();
        Debug.Log($"슬롯 {slotId} 저장 완료");
    }

    public void LoadSlot(int slotId)
    {
        if (!PlayerPrefs.HasKey($"PlayerPosX_{slotId}")) return;

        HamsterController2.NeedLoadPosition = true;
        HamsterController2.PositionOnLoad = LoadVector3($"PlayerPos", slotId);

        //player.position = LoadVector3($"PlayerPos", slotId);

        int selectedCostume = PlayerPrefs.GetInt($"SelectedCostume_{slotId}", 0);
        int selectedSkin = PlayerPrefs.GetInt($"SelectedSkined_{slotId}", 0);

        PlayerPrefs.SetInt("SelectedCostume", selectedCostume);
        PlayerPrefs.SetInt("SelectedSkined", selectedSkin);

        // 복원: 코스튬/스킨 보유 정보
        for (int i = 1; i <= totalCostumes; i++)
        {
            int hasCostume = PlayerPrefs.GetInt($"Slot{slotId}_HasCostume_{i}", 0);
            PlayerPrefs.SetInt($"HasCostume_{i}", hasCostume);
        }

        for (int i = 1; i <= totalSkins; i++)
        {
            int hasSkin = PlayerPrefs.GetInt($"Slot{slotId}_HasSkin_{i}", 0);
            PlayerPrefs.SetInt($"HasSkin_{i}", hasSkin);
        }

        Debug.Log($"슬롯 {slotId} 불러오기 완료");
    }

    public void RemoveSlot(int slotId)
    {
        PlayerPrefs.DeleteKey($"PlayerPosX_{slotId}");
        PlayerPrefs.DeleteKey($"PlayerPosY_{slotId}");
        PlayerPrefs.DeleteKey($"PlayerPosZ_{slotId}");

        // 스튬 & 스킨 정보 삭제
        PlayerPrefs.DeleteKey($"SelectedCostume_{slotId}");
        PlayerPrefs.DeleteKey($"SelectedSkined_{slotId}");

        PlayerPrefs.DeleteKey($"IsSaved_{slotId}");

        PlayerPrefs.DeleteKey($"HasCostume_{slotId}");
        PlayerPrefs.DeleteKey($"SaveTime_{slotId}");

        Debug.Log($"슬롯 {slotId}: 저장된 위치 데이터 삭제 완료");
    }

    private void RemoveCollectedCostumeItems()
    {
        for (int i = 1; i <= 3; i++)
        {
            if (PlayerPrefs.GetInt($"HasCostume_{i}", 0) == 1)
            {
                // 씬에 있는 모든 CostumeItem을 검색
                var items = FindObjectsOfType<CostumeItem>();
                foreach (var item in items)
                {
                    if (item.costumeId == i)
                    {
                        Destroy(item.gameObject);
                        Debug.Log($"CostumeItem {i} 이미 보유 중 → 제거");
                    }
                }
            }
        }
    }


private bool IsSlotSaved(int slotId)
    {
        return PlayerPrefs.HasKey($"SaveTime_{slotId}") &&
               PlayerPrefs.HasKey($"PlayerPosX_{slotId}") &&
               PlayerPrefs.HasKey($"PlayerPosY_{slotId}") &&
               PlayerPrefs.HasKey($"PlayerPosZ_{slotId}");
    }

    private void SaveVector3(string prefix, int slotId, Vector3 position)
    {
        PlayerPrefs.SetFloat($"{prefix}X_{slotId}", position.x);
        PlayerPrefs.SetFloat($"{prefix}Y_{slotId}", position.y);
        PlayerPrefs.SetFloat($"{prefix}Z_{slotId}", position.z);
    }

    private Vector3 LoadVector3(string prefix, int slotId)
    {
        float x = PlayerPrefs.GetFloat($"{prefix}X_{slotId}");
        float y = PlayerPrefs.GetFloat($"{prefix}Y_{slotId}");
        float z = PlayerPrefs.GetFloat($"{prefix}Z_{slotId}");
        return new Vector3(x, y, z);
    }

}
