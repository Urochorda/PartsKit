using UnityEngine;
using UnityEngine.UI;

namespace PartsKit
{
    public enum DialogueCharacterStyle
    {
        Normal = 0, //普通
        Gray = 1, //置灰
    }

    public class DefaultDialogueShowCharacter : MonoBehaviour
    {
        [field: SerializeField] public int Seat { get; set; }
        [field: SerializeField] public Image HeadImage { get; set; }
        [field: SerializeField] public Text NameText { get; set; }

        public void SetShow(DialogueCharacter data)
        {
            if (data == null || data.Config == null)
            {
                return;
            }

            gameObject.SetActive(true);
            HeadImage.sprite = data.Config.HeadSprite;
            NameText.text = data.Config.NameKey.GetLocalizedString();

            HeadImage.color = Color.white;
            switch ((DialogueCharacterStyle)data.StyleType)
            {
                default:
                case DialogueCharacterStyle.Normal:
                    break;
                case DialogueCharacterStyle.Gray:
                    HeadImage.color = new Color(0.3f, 0.3f, 0.3f, 1);
                    break;
            }
        }

        public void SetHide()
        {
            gameObject.SetActive(false);
        }
    }
}