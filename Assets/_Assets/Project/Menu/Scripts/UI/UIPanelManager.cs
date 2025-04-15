using System.Collections.Generic;
using UnityEngine;

public class UIPanelManager : MonoBehaviour
{
    public static UIPanelManager Instance { get; private set; }

    public List<GameObject> uiPanels = new List<GameObject>();  // 모든 UI 패널 리스트
    private List<GameObject> activePanels = new List<GameObject>(); // 현재 활성화된 패널들
    private List<GameObject> excludePanels = new List<GameObject>(); // 제외할 패널 리스트

    int totalSlotCount = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    // 씬이 변경될 때 새 UI 요소 자동으로 찾기
    public void FindUIElements()
    {
        GameObject[] uiObjs = GameObjectUtils.FindInActiveObjectByTag("UIPanel");

        uiPanels.Clear();
        uiPanels.AddRange(uiObjs);

        HideAllPanels();
        UpdateActivePanels();
        SlotPanelState();

        Debug.Log("🔄 UI 요소 및 오디오 설정 자동 연결 완료!");
    }

    public void UpdateActivePanels()
    {
        activePanels.Clear();
        foreach (GameObject panel in uiPanels)
        {
            if (panel.activeSelf)
            {
                activePanels.Add(panel);
            }
        }
    }

    GameObject targetPanel = null;

    public void ShowPanel(string panelName, bool allowMultiple = false)
    {
        targetPanel = GetPanelByName(panelName);

        if (!allowMultiple)
        {
            CloseAllPanels(); // 기존 패널을 모두 닫음
        }

        targetPanel.SetActive(true);
        if (!activePanels.Contains(targetPanel))
        {
            activePanels.Add(targetPanel);
        }
        UpdateActivePanels();
    }

    public void ClosePanel(string panelName)
    {
        GameObject panelToClose = GetPanelByName(panelName);

        if (panelToClose != null && panelToClose.activeSelf)
        {
            panelToClose.SetActive(false);
            activePanels.Remove(panelToClose);
        }

        UpdateActivePanels();
    }

    public void CloseAllPanels()
    {
        foreach (GameObject panel in activePanels)
        {
            panel.SetActive(false);
        }
        activePanels.Clear();
        UpdateActivePanels();
    }

    // 모든 패널을 비활성화하는 함수 (활성화/비활성화 모두 포함)
    public void HideAllPanels()
    {
        foreach (var panel in uiPanels)
        {
            panel.SetActive(false);
        }
        UpdateActivePanels();
    }

    private GameObject GetPanelByName(string panelName)
    {
        foreach (var obj in uiPanels)
        {
            if (obj.name == panelName)
            {
                return obj;
            }
        }
        return null;
    }

    public void SlotPanelState()
    {
        for (int i = 1; i <= totalSlotCount; i++)
        {
            bool hasSave = GameDataManager.Instance != null && GameDataManager.Instance.TryGetSlotData(i, out _);

            string savePanelName = $"Slot{i}Save";
            string emptyPanelName = $"Slot{i}Empty";

            GameObject savePanel = GetPanelByName(savePanelName);
            GameObject emptyPanel = GetPanelByName(emptyPanelName);

            if (savePanel != null) savePanel.SetActive(hasSave);
            if (emptyPanel != null) emptyPanel.SetActive(!hasSave);
        }
    }
}
