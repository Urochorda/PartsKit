using UnityEngine;

namespace PartsKit
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class ButtonMenuAttribute : PropertyAttribute
    {
        public string ButtonText { get; }

        public ButtonMenuAttribute(string buttonText)
        {
            ButtonText = buttonText;
        }
    }
}