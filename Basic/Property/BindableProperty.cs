using System;

namespace PartsKit
{
    public class BindableProperty<T>
    {
        private T mValue;
        private Action<T> mOnValueChanged;

        public BindableProperty(T defaultValue = default)
        {
            mValue = defaultValue;
        }

        public T GetValue()
        {
            return mValue;
        }

        public void SetValue(T newValue)
        {
            if (newValue == null && mValue == null)
            {
                return;
            }

            if (newValue != null && newValue.Equals(mValue))
            {
                return;
            }

            mValue = newValue;
            mOnValueChanged?.Invoke(newValue);
        }

        public void SetValueWithoutEvent(T newValue)
        {
            mValue = newValue;
        }

        public IRegister Register(Action<T> onValueChanged)
        {
            mOnValueChanged += onValueChanged;
            return new ActionRegister(() => UnRegister(onValueChanged));
        }

        public IRegister RegisterWithInitValue(Action<T> onValueChanged)
        {
            onValueChanged(mValue);
            return Register(onValueChanged);
        }

        public void UnRegister(Action<T> onValueChanged)
        {
            mOnValueChanged -= onValueChanged;
        }

        public override string ToString()
        {
            return GetValue().ToString();
        }
    }
}