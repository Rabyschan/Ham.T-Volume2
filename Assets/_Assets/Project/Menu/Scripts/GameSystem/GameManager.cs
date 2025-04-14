using UnityEngine;
using static AudioManager;

public class GameManager : MonoBehaviour
{
    // 싱글톤 접근용 프로퍼티
    public static GameManager Instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static GameManager m_instance; //싱글톤이 할당될 static 변수

    public float score = 0;
    public bool isEating;
    public float totalScore;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        //UIManager.Instance.ScoreUI(0);
    }
    //점수를 증가시킴

    public void SetScore(float newScore)
    {
        score += newScore;
        UIManager.Instance.ScoreUI(newScore);
        AudioManager.Instance.PlaySound(SoundType.Eat);
    }

    //현재 점수를 반환함
    public float GetScore()
    {
        return score; // 점수를 반환
    }

}
