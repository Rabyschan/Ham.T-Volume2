using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance
    {
        get
        {
            //���� �ν��Ͻ��� ã�� Ÿ�ֿ̹� �̱����� ���ٸ�
            if (m_instance == null)
            {
                //�� ������ UI Manager Ŭ������ ã�Ƽ� �̱������� ����Ѵ�.
                m_instance = FindObjectOfType<UIManager>();
            }

            return m_instance;
        }
    }

    private static UIManager m_instance;

    public TMP_Text scoreText;
    public TMP_Text achievementText;

    public void UpdateScoreText(int newScore)
    {
        if(scoreText != null) scoreText.text = " X " + newScore;
    }

    public void UpdateAchieveText(string newText)
    {
        if (achievementText != null) achievementText.text = " Achievement : " + newText;
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
