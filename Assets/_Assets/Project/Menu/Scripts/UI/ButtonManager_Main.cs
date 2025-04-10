using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ButtonManager_Main : MonoBehaviour
{
    [Header("MainMenu")]
    [SerializeField] Button bTN_Main_Start;
    [SerializeField] Button bTN_Main_Option;
    [SerializeField] Button bTN_Main_Quit;

    [Header("MainMenu_Save")]
    [SerializeField] Button bTN_M_Slot_Quit;

    [Header("MainMenu_Option")]
    [SerializeField] Button bTN_M_Option_Control;
    [SerializeField] Button bTN_M_Option_Game;
    [SerializeField] Button bTN_M_Option_Quit;

    [Header("MainMenu_Pop-up")]
    [SerializeField] Button bTN_M_Save_No;
    [SerializeField] Button bTN_M_Quit_No;

    public Dictionary<ButtonType, Button> buttonDictionary = new Dictionary<ButtonType, Button>();

    private void Awake()
    {
        // 기존 Dictionary 초기화 방지
        if (buttonDictionary == null)
        {
            buttonDictionary = new Dictionary<ButtonType, Button>();
        }
        else
        {
            buttonDictionary.Clear();
        }

        buttonDictionary = new Dictionary<ButtonType, Button>
        {
            {ButtonType.Main_Start,bTN_Main_Start},
            {ButtonType.Main_Option,bTN_Main_Option},
            {ButtonType.Main_Quit,bTN_Main_Quit},

            {ButtonType.M_Slot_Quit,bTN_M_Slot_Quit},

            {ButtonType.M_Option_Control,bTN_M_Option_Control},
            {ButtonType.M_Option_Game,bTN_M_Option_Game},
            {ButtonType.M_Option_Quit,bTN_M_Option_Quit},

            {ButtonType.M_Save_No,bTN_M_Save_No},
            {ButtonType.M_Quit_No,bTN_M_Quit_No}
        };
    }

    void Start()
    {
        foreach (var kvp in buttonDictionary)
        {
            ButtonType type = kvp.Key;
            Button button = kvp.Value;

            if (button != null)
            {
                // 기존 이벤트 제거 후 새 이벤트 추가
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => Main_OnButtonClicked(type));
            }
        }
    }

    void Main_OnButtonClicked(ButtonType type)
    {
        switch (type)
        {
            case ButtonType.Main_Start:
                UIPanelManager.Instance.ShowPanel("Save", false);
                UIPanelManager.Instance.SlotPanelState();
                break;
            case ButtonType.Main_Option:
                UIPanelManager.Instance.ShowPanel("Option", false);
                UIPanelManager.Instance.ShowPanel("Option_Game", true);
                break;
            case ButtonType.Main_Quit:
                UIPanelManager.Instance.ShowPanel("Quit_P", true);
                break;
            case ButtonType.M_Slot_Quit:
            case ButtonType.M_Option_Quit:
                UIPanelManager.Instance.ShowPanel("Main", false);
                break;
            case ButtonType.M_Option_Control:
                UIPanelManager.Instance.ShowPanel("Option_Control", false);
                UIPanelManager.Instance.ShowPanel("Option", true);
                break;
            case ButtonType.M_Option_Game:
                UIPanelManager.Instance.ShowPanel("Option_Game", false);
                UIPanelManager.Instance.ShowPanel("Option", true);
                break;
            case ButtonType.M_Save_No:
                UIPanelManager.Instance.ClosePanel("Save_P");
                break;
            case ButtonType.M_Quit_No:
                UIPanelManager.Instance.ClosePanel("Quit_P");
                break;
            default:
                Debug.LogWarning($"[ButtonManager] 처리되지 않은 버튼 타입: {type}");
                break;
        }
    }
}
