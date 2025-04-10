using GameSave;
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

        private int selectedSlotId = -1;

        //슬롯 선택 시 호출 (외부에서 슬롯ID 세팅)
        public void SetSlotId(int Id)
        {
            selectedSlotId = Id;
        }

        private void Start()
        {
            // 버튼 클릭 이벤트를 코드에서 직접 등록 (OnClick 사용 X)
            startButton.onClick.AddListener(OnClickStart);
        }

        // 로드 버튼 클릭 시
        public void OnClickStart()
        {
            if (selectedSlotId >= 0)
            {
                // 선택된 슬롯 저장 (다른 스크립트용)
                PlayerPrefs.SetInt("SelectedSlot", selectedSlotId);
                PlayerPrefs.Save();

                // 저장 데이터 로드
                GameDataSaveLoadSlot.LoadSlotById(selectedSlotId);

                // 로딩 UI 닫기
                save_Pop.SetActive(false);

                // 씬 전환
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
