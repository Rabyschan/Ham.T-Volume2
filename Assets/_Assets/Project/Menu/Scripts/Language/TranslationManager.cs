using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace Language
{
    public class TranslationManager : MonoBehaviour
    {
        private void OnDestroy()
        {
            LanguageManager.instance.OnLanguageChanged -= UpdateAllText;
        }

        private void Start()
        {
            LanguageManager.instance.OnLanguageChanged += UpdateAllText;
            UpdateAllText(); // 씬 시작할 때 기존 언어 적용
        }

        private void UpdateTextInternal(GameObject targetObject)
        {
            if (LanguageManager.instance == null) return;

            LanguageAsset languageAsset = LanguageManager.instance.CurrentLanguageAsset;
            TMP_FontAsset primaryFont = LanguageManager.instance.CurrentFontPrimary;
            TMP_FontAsset secondaryFont = LanguageManager.instance.CurrentFontSecondary;

            TextMeshProUGUI[] texts;

            if (targetObject == null)
            {
                // 전체 씬에서 찾음
                texts = FindObjectsOfType<TextMeshProUGUI>(true);
            }
            else
            {
                // 특정 오브젝트 하위에서만 찾음
                texts = targetObject.GetComponentsInChildren<TextMeshProUGUI>(true);
            }

            foreach (var textObj in texts)
            {
                TranslationKey keyComponent = textObj.GetComponent<TranslationKey>();
                if (keyComponent != null && !string.IsNullOrEmpty(keyComponent.translationKey))
                {
                    string key = keyComponent.translationKey;
                    if (languageAsset.translationDict.ContainsKey(key))
                    {
                        textObj.text = languageAsset.GetTranslation(key);
                        textObj.font = languageAsset.UseSecondaryFont(key) ? secondaryFont : primaryFont;
                        float? fontSize = languageAsset.GetFontSize(key);
                        if (fontSize.HasValue) textObj.fontSize = fontSize.Value;
                        textObj.fontStyle = languageAsset.IsBold(key) ? FontStyles.Bold : FontStyles.Normal;
                    }
                    else
                    {
                        Debug.LogWarning($" 번역 Key 없음: {key}");
                    }
                }
            }
        }

        private void UpdateAllText()
        {
            UpdateTextInternal(null);
        }

        public void UpdateText(GameObject targetObject)
        {
            // 특정 게임 오브젝트 내의 텍스트만 갱신
            UpdateTextInternal(targetObject);
        }
    }
}
