using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace GameSave
{
    public class GameDataSaveLoadSlot : MonoBehaviour
    {
        [field: Header("이 슬롯의 고유한 번호")]
        [field: SerializeField] public int slotId { private set; get; } = -1;

        private static event Action<int> _saveLoadEvent;
        public static GameDataSaveLoadSlot Instance { get; private set; }

        [Space(3)]
        [Header("슬롯의 버튼 (저장, 불러오기, 삭제)")]
        [SerializeField] private Button save_btn;
        [SerializeField] private Button load_btn;
        [SerializeField] private Button remove_btn;

        [SerializeField] private Transform playerPosition;
        //[SerializeField] private TMP_Text saveTimeText;

        private void Awake()
        {
            UpdateSlot();
        }

        private void Start()
        {
            // 게임 씬 진입 시 자동 로드 처리
            int selectedSlot = PlayerPrefs.GetInt("SelectedSlot", -1);
            if (selectedSlot == slotId && PlayerPrefs.HasKey($"PlayerPosX_{slotId}"))
            {
                CheckPointLoad();
            }
        }

        public void OnClickSlotButton()
        {
            PlayerPrefs.SetInt("SelectedSlot", slotId); // 슬롯 선택 기억

            bool hasSaveData = PlayerPrefs.HasKey($"PlayerPosX_{slotId}");

            if (hasSaveData)
            {
                // 저장된 데이터가 있을 경우 → 로드 & 씬 전환
                SceneManager.LoadScene("GameScenev2");
            }
            else
            {
                // 저장된 데이터가 없을 경우 → 초기화 & 씬 전환
                PlayerPrefs.SetInt("NeedsInit", 1);
                SceneManager.LoadScene("GameScenev2");
            }
        }

        public void UpdateSlot()
        {
            bool hasSave = PlayerPrefs.HasKey($"PlayerPosX_{slotId}");
            load_btn.interactable = hasSave;
            remove_btn.interactable = hasSave;

            // UI 쪽에서 PlayerPrefs를 직접 확인하여 표시하도록 유도
        }

        public void CheckPointSave()
        {
            // 현재 날짜와 시간 저장
            string saveTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm");
            PlayerPrefs.SetString($"SaveTime_{slotId}", saveTime);

            Vector3 pos = playerPosition.position;

            PlayerPrefs.SetFloat($"PlayerPosX_{slotId}", pos.x);
            PlayerPrefs.SetFloat($"PlayerPosY_{slotId}", pos.y);
            PlayerPrefs.SetFloat($"PlayerPosZ_{slotId}", pos.z);

            // 현재 선택된 코스튬 & 스킨 ID 저장
            PlayerPrefs.SetInt($"SelectedCostume_{slotId}", PlayerPrefs.GetInt("SelectedCostume", 0));
            PlayerPrefs.SetInt($"SelectedSkined_{slotId}", PlayerPrefs.GetInt("SelectedSkined", 0));

            PlayerPrefs.SetInt($"IsSaved_{slotId}", 1); // 저장 여부 표시

            // 코스튬/스킨 UI 저장
            for (int i = 1; i <= 3; i++)
            {
                int hasCostume = PlayerPrefs.GetInt($"HasCostume_{i}", 0);
                PlayerPrefs.SetInt($"Slot{slotId}_HasCostume_{i}", hasCostume);

                int hasSkin = PlayerPrefs.GetInt($"HasSkin_{i}", 0);
                PlayerPrefs.SetInt($"Slot{slotId}_HasSkin_{i}", hasSkin);
            }

            Debug.Log($"슬롯 {slotId}: 현재 위치 저장 완료 - {pos}");
            PlayerPrefs.Save(); //디스크에 강제 저장
            UpdateSlot(); // 버튼 활성화 갱신
        }

        public void CheckPointLoad()
        {
            // 저장된 위치 불러오기
            if (PlayerPrefs.HasKey($"PlayerPosX_{slotId}"))
            {
                float x = PlayerPrefs.GetFloat($"PlayerPosX_{slotId}");
                float y = PlayerPrefs.GetFloat($"PlayerPosY_{slotId}");
                float z = PlayerPrefs.GetFloat($"PlayerPosZ_{slotId}");

                Debug.Log(x);

                playerPosition.position = new Vector3(x, y, z);
                Debug.Log($"슬롯 {slotId}: 위치 불러오기 완료 - {playerPosition.position}");

                // 저장된 코스튬 불러와 적용
                int savedCostumeId = PlayerPrefs.GetInt($"SelectedCostume_{slotId}", 0);
                int savedSkinId = PlayerPrefs.GetInt($"SelectedSkined_{slotId}", 0);

                // 저장된 시간 표시
                string savedTime = PlayerPrefs.GetString($"SaveTime_{slotId}");
                Debug.Log($"슬롯 {slotId}의 저장 시간: {savedTime}");

                PlayerPrefs.SetInt("IsLoadedFromSlot", 1);  // 로드 플래그 설정

                // 코스튬 UI 갱신
                for (int i = 1; i <= 3; i++)
                {
                    int hasCostume = PlayerPrefs.GetInt($"Slot{slotId}_HasCostume_{i}", 0);
                    PlayerPrefs.SetInt($"HasCostume_{i}", hasCostume);

                    if (hasCostume == 1)
                    {
                        CostumeUI.Instance.UpdateBtn(i);
                    }
                }

                CostumeUI.Instance.SelectCostume(savedCostumeId);
                CostumeUI.Instance.SelectSkin(savedSkinId);

                // CostumeItem 제거 로직 추가
                RemoveCollectedCostumeItems();

                Debug.Log($"슬롯 {slotId}: 코스튬 {savedCostumeId} / 스킨 {savedSkinId} 적용 완료!");
            }
            else
            {
                Debug.Log($"슬롯 {slotId}: 저장된 위치 없음. 기본 위치 사용");
            }
        }

        public static void LoadSlotById(int slotId)
        {
            var slots = FindObjectsOfType<GameDataSaveLoadSlot>();
            foreach (var slot in slots)
            {
                if (slot.slotId == slotId)
                {
                    slot.CheckPointLoad();
                    break;
                }
            }
        }

        public void CheckPointRemove()
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
            UpdateSlot();
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
    }
}

