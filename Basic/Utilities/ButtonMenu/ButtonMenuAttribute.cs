using System;

namespace PartsKit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonMenuAttribute : Attribute
    {
        public string ButtonText { get; }

        public ButtonMenuAttribute(string buttonText)
        {
            ButtonText = buttonText;
        }
    }
}