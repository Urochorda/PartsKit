using UnityEngine;
using UnityEngine.UI;

namespace PartsKit
{
    public class DefaultDialogShowCharacter : MonoBehaviour
    {
        [field: SerializeField] public int Seat { get; set; }
        [field: SerializeField] public Image HeadImage { get; set; }
        [field: SerializeField] public Text NameText { get; set; }

        public void SetShow(DialogueCharacterConfig configData)
        {
            if (configData == null)
            {
                return;
            }

            gameObject.SetActive(true);
            HeadImage.sprite = configData.HeadSprite;
            NameText.text = configData.NameKey.GetLocalizedString();
        }

        public void SetHide()
        {
            gameObject.SetActive(false);
        }
    }
}