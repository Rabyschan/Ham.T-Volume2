using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public event Action<bool> PauseEvent;

    public static UIManager Instance { get; private set; }

    public bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    private void Start()
    {
        SceneSetup();
    }

    private void Update()
    {
        // ESC 입력 감지는 게임 씬에서만 실행
        if (SceneManager.GetActiveScene().name != "MainMenu" && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    // 씬이 변경될 때마다 실행되는 함수
    private void SceneSetup()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // 씬이 바뀌었으므로, UI 요소를 다시 찾음
        UIPanelManager.Instance.FindUIElements();

        if (currentScene == "MainMenu")
        {
            SetupMainMenu();
        }
        else if (currentScene == "PlayScenev2")
        {
            SetGameScene();
        }
    }

    #region Scene UI

    // 메인 메뉴 전용 설정
    private void SetupMainMenu()
    {
        foreach (var obj in UIPanelManager.Instance.uiPanels)
        {
            if (obj.name == "Main")
            {
                // 이름이 일치하면 true로 설정 (여기서는 gameObject의 활성화를 예시로 듬)
                obj.SetActive(true);
                break; // 찾았으므로 더 이상 탐색하지 않음
            }
        }
    }

    private void SetGameScene()
    {
        foreach (var obj in UIPanelManager.Instance.uiPanels)
        {
            if (obj.name == "PlayUICanvas")
            {
                // 이름이 일치하면 true로 설정 (여기서는 gameObject의 활성화를 예시로 듬)
                obj.SetActive(true);
                break; // 찾았으므로 더 이상 탐색하지 않음
            }
        }
    }

    // 퍼즈창 설정
    private void PausePanel(bool isPaused)
    {
        foreach (var obj in UIPanelManager.Instance.uiPanels)
        {
            if (obj.name == "Pause")
            {
                obj.SetActive(isPaused);
                break; // 찾았으므로 더 이상 탐색하지 않음
            }
        }
    }

    // ESC 입력 시 Pause 창 열기/닫기
    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        PausePanel(isPaused);
        Time.timeScale = isPaused ? 0 : 1; // 게임 멈춤/재개
        var hamsterController = FindObjectOfType<HamsterController2>();
        hamsterController.enabled = !isPaused;
        PauseEvent?.Invoke(isPaused);
    }

    public void InteractUI(bool isInteracted)
    {
        foreach (var obj in UIPanelManager.Instance.uiPanels)
        {
            if (obj.name == "Interact")
            {
                obj.SetActive(isInteracted);
                break; // 찾았으므로 더 이상 탐색하지 않음
            }
        }
    }

    public void ScoreUI(float nowScore)
    {
        GameObject _scoreText = GameObject.Find("ScoreText"); // TMP_Text 컴포넌트 가져오기
        TextMeshProUGUI scoreText = _scoreText.GetComponent<TextMeshProUGUI>();
        if (scoreText != null)
        {
            scoreText.text = nowScore.ToString();  // 점수를 UI에 업데이트
        }
    }

    public void GetCostumeUI(int costumeId)
    {
        foreach (var obj in UIPanelManager.Instance.uiPanels)
        {
            if (obj.name == "GetCostume")
            {
                obj.SetActive(true);

                // 모든 자식 코스튬 패널을 먼저 꺼줌
                for (int i = 1; i <= 3; i++)
                {
                    Transform childPanel = obj.transform.Find($"CostumePanel_{i}");
                    if (childPanel != null)
                    {
                        childPanel.gameObject.SetActive(false);
                    }
                }

                // 선택된 코스튬 패널만 활성화
                Transform targetPanel = obj.transform.Find($"CostumePanel_{costumeId}");
                if (targetPanel != null)
                    targetPanel.gameObject.SetActive(true);

                StartCoroutine(FadeInOutPanel(obj));
                break;
            }
        }
    }

    #endregion

    #region FadeInOutPanel
    private IEnumerator FadeInOutPanel(GameObject panel)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }

        panel.SetActive(true);
        canvasGroup.alpha = 0f;

        // Fade In
        float fadeDuration = 0.5f;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Fade Out
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - (elapsed / fadeDuration));
            yield return null;
        }

        panel.SetActive(false);
    }
    #endregion

    #region NocticeAnimation

    public void ShowNoticeByAnimator(string message, float duration)
    {
        foreach (var obj in UIPanelManager.Instance.uiPanels)
        {
            if (obj.name == "Notice")
            {
                var text = obj.GetComponentInChildren<TextMeshProUGUI>();
                var anim = obj.GetComponent<Animator>();

                if (text != null && anim != null)
                {
                    obj.SetActive(true); // 꺼져 있으면 켜줌
                    text.text = message;
                    anim.SetTrigger("NoticeShow");
                }
                break;
            }
        }
    }
    private IEnumerator HideNoticeAfterDelay(GameObject noticeobj, float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // 타임 스케일 멈춰도 보임
    }

    #endregion

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneSetup();
    }
}
