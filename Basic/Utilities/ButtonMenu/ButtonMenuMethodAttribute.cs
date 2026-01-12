using System;

namespace PartsKit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonMenuMethodAttribute : Attribute
    {
        public string ButtonText { get; }

        public ButtonMenuMethodAttribute(string buttonText)
        {
            ButtonText = buttonText;
        }
    }
}