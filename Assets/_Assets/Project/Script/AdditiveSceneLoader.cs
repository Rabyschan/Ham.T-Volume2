using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class AdditiveSceneLoader : MonoBehaviour
{
    private static AdditiveSceneLoader _instance;
    private HamsterController2 _player;
    //private HamsterController2 _costumescenePlayer;
    [SerializeField] GameObject _playUICanvas;

    public static AdditiveSceneLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AdditiveSceneLoader>();


                if (_instance == null)
                {
                    GameObject obj = new GameObject("AdditiveSceneLoader");
                    _instance = obj.AddComponent<AdditiveSceneLoader>();
                    DontDestroyOnLoad(obj);
                }
            }

            return _instance;
        }
    }

    private void Start()
    {
        _player = FindObjectOfType<HamsterController2>();
    }

    //원하는 타이밍에 씬 업로드도 가능
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenCostumeScene();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCostumeScene();
            CloseMGScene();
        }
    }

    public void OpenCostumeScene()
    {
        //UIScene이 이미 로드되어 있지 않으면 Additive로 로드
        if (!SceneManager.GetSceneByName("CostumeScene").isLoaded)
        {
            SceneManager.LoadScene("CostumeScene", LoadSceneMode.Additive);
            _playUICanvas.gameObject.SetActive(false);
            PlayerInputControll(true);
            Debug.Log("CostumeScene을 Additive 모드로 로드함 + player 입력 차단");
        }
    }

    public void CloseCostumeScene()
    {
        if (SceneManager.GetSceneByName("CostumeScene").isLoaded)
        {
            SceneManager.UnloadSceneAsync("CostumeScene");
            _playUICanvas.gameObject.SetActive(true);
            PlayerInputControll(false);
            Debug.Log("CostumeScene 언로드 요청 + player 입력 허용");
        }
    }

    public void OpenMGScene()
    {
        //UIScene이 이미 로드되어 있지 않으면 Additive로 로드
        if (!SceneManager.GetSceneByName("MiniGameScene").isLoaded)
        {
            SceneManager.LoadScene("MiniGameScene", LoadSceneMode.Additive);
            _playUICanvas.gameObject.SetActive(false);
            PlayerInputControll(true);
            Debug.Log("CostumeScene을 Additive 모드로 로드함 + player 입력 차단");
        }
    }

    public void CloseMGScene()
    {
        if (SceneManager.GetSceneByName("MiniGameScene").isLoaded)
        {
            SceneManager.UnloadSceneAsync("MiniGameScene");
            _playUICanvas.gameObject.SetActive(true);
            PlayerInputControll(false);
            Debug.Log("CostumeScene 언로드 요청 + player 입력 허용");
        }
    }

    public void PlayerInputControll(bool state)
    {
        if(state == true)
        {
            if (_player != null)
            {
                _player.canMove = false; // 입력 정지
            }
        }
        else
        {
            if (_player != null)
            {
                _player.canMove = true; // 다시 입력 허용
            }
        }
    }
}
