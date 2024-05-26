using UnityEngine;
using UnityEngine.UI;

namespace PartsKit
{
    public class RedDotUI : MonoBehaviour
    {
        [field: SerializeField] public string Key { get; private set; }
        [SerializeField] private CheckNullProperty<Text> countText;
        [SerializeField] private CheckNullProperty<Animator> stateAnim;
        [SerializeField] [DisplayOnly] private string hasRedDotBoolAnimKey = "hasRedDot";

        private void Awake()
        {
            TypeEventSystem.Global.Register<RedDotUpdateEvent>(UpdateDot).UnRegisterWhenGameObjectDestroyed(gameObject);
            Refresh();
        }

        public void SetData(string key)
        {
            Key = key;
            Refresh();
        }

        private void Refresh()
        {
            if (RedDotController.GetIns != null)
            {
                int count = RedDotController.GetIns.Invoke().GetRedDotCount(Key);
                UpdateCount(count);
            }
        }

        private void UpdateDot(RedDotUpdateEvent eventData)
        {
            int targetIndex = eventData.RedDot.FindIndex(item => item.Key == Key);
            if (targetIndex < 0)
            {
                return;
            }

            int count = eventData.RedDot[targetIndex].SumCount;
            UpdateCount(count);
        }

        private void UpdateCount(int count)
        {
            if (countText.GetValue(out Text countValue))
            {
                countValue.text = count.ToString();
            }

            if (stateAnim.GetValue(out Animator animValue))
            {
                animValue.SetBool(hasRedDotBoolAnimKey, count > 0);
            }

            gameObject.SetActive(count > 0);
        }
    }
}