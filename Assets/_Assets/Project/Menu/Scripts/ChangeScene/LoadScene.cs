using UnityEngine;
using UnityEngine.UI;

namespace ChangeScene
{
    // <YSA> chatGPT
    public class LoadScene : MonoBehaviour
    {
        public Button startButton; //  버튼 오브젝트
        public GameObject save_Pop;
        public SceneLoader sceneLoader; // SceneLoader 스크립트 참조

        private SlotData? selectedSlot; // 슬롯 전체 정보 저장

        private void OnEnable()
        {
            SaveSlotInfo.OnSlotSelected += HandleSlotSelected;
        }

        private void OnDisable()
        {
            SaveSlotInfo.OnSlotSelected -= HandleSlotSelected;
        }

        private void Start()
        {
            startButton.onClick.AddListener(OnClickStart);
        }

        private void HandleSlotSelected(SlotData data)
        {
            selectedSlot = data;
        }


        // 로드 버튼 클릭 시
        public void OnClickStart()
        {
            if (selectedSlot.HasValue)
            {
                var data = selectedSlot.Value;

                PlayerPrefs.SetInt("SelectedSlot", data.slotId);
                PlayerPrefs.Save();

                GameDataManager.Instance.LoadSlot(data.slotId);

                sceneLoader.LoadGameScene();
            }
            else
            {
                Debug.LogWarning("선택된 슬롯이 없습니다.");
            }
        }

        private void OnDestroy()
        {
            //씬이 변경될 때 메모리 누수를 방지하기 위해 이벤트 제거
            startButton.onClick.RemoveListener(OnClickStart);
        }
    }
}
