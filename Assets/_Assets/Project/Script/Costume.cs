using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Costume : MonoBehaviour
{
    private static event Action<int> _costumeChangedEvent;

    [SerializeField] GameObject sunflowerHat;
    [SerializeField] GameObject bearHat;
    [SerializeField] GameObject chefHat;

    private static Costume _instance;

    public static Costume Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에 추가된 코스튬 매니저를 찾는다.
                _instance = FindObjectOfType<Costume>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _costumeChangedEvent += OnCostumeChanged;
    }

    private void OnDestroy()
    {
        _costumeChangedEvent -= OnCostumeChanged;
    }

    public void Start()
    {
        // 항상 처음엔 기본 코스튬으로 초기화
        ResetCostume();

        if (PlayerPrefs.HasKey("SelectedCostume"))
        {
            int savedCostumeId = PlayerPrefs.GetInt("SelectedCostume");
            Debug.Log($"저장된 코스튬 ID: {savedCostumeId}");
            ActiveCostume(savedCostumeId);
        }
        else
        {
            PlayerPrefs.SetInt("SelectedCostume", 0); // 기본값 저장
            PlayerPrefs.Save();
        }
    }

    private void ResetCostume()
    {
        OnCostumeChanged(0);
    }

    public void ActiveCostume(int costumeId)
    {
        _costumeChangedEvent?.Invoke(costumeId);
    }

    private void OnCostumeChanged(int costumeId)
    {
        switch (costumeId)
        {
            case 0:
                sunflowerHat.SetActive(false);
                bearHat.SetActive(false);
                chefHat.SetActive(false);
                break;
            case 1:
                sunflowerHat.SetActive(true);
                bearHat.SetActive(false);
                chefHat.SetActive(false);
                break;
            case 2:
                bearHat.SetActive(true);
                sunflowerHat.SetActive(false);
                chefHat.SetActive(false);
                break;
            case 3:
                chefHat.SetActive(true);
                sunflowerHat.SetActive(false);
                bearHat.SetActive(false);
                break;
        }
    }
}
