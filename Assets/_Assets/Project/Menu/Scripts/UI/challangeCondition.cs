using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class challangeCondition : MonoBehaviour
{
    private static challangeCondition _instance;

    public static challangeCondition Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<challangeCondition>(); // æ¿ø° ¿÷¥¬ ChallengeUI √£±‚
            return _instance;
        }
    }

    public void GetItem(int costumeId)
    {
        switch (costumeId)
        {
            case 1:
                ChallengeUI.Instance.CompleteCondition(1);
                Debug.Log("∏‘¿Ω1");
                break;
            case 2:
                ChallengeUI.Instance.CompleteCondition(2);
                Debug.Log("∏‘¿Ω2");
                break;
            case 3:
                ChallengeUI.Instance.CompleteCondition(3);
                Debug.Log("∏‘¿Ω3");
                break;
        }
    }

    public void SeedScore()
    {
        float seedScore = GameManager.Instance.score;
        float totalSeedScore = GameManager.Instance.totalScore;

        if (seedScore == 9)
        {
            ChallengeUI.Instance.CompleteCondition(5);
        }
        if (seedScore == totalSeedScore)
        {
            ChallengeUI.Instance.CompleteCondition(4);
        }

        ChallengeUI.Instance.scoreText.text = $"{seedScore} / {totalSeedScore}";
    }

}
