using System;

namespace PartsKit
{
    public interface IReadOnlyBindableProperty<T>
    {
        public T GetValue();
        public IRegister Register(Action<T, T> onValueChanged);
        public IRegister RegisterWithInitValue(Action<T, T> onValueChanged);
        public void UnRegister(Action<T, T> onValueChanged);
        public string ToString();
    }

    public class BindableProperty<T> : IReadOnlyBindableProperty<T>
    {
        private T mValue;
        private Action<T, T> mOnValueChanged;

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

            var oldValue = mValue;
            mValue = newValue;
            mOnValueChanged?.Invoke(oldValue, newValue);
        }

        public void SetValueWithoutEvent(T newValue)
        {
            mValue = newValue;
        }

        public IRegister Register(Action<T, T> onValueChanged)
        {
            mOnValueChanged += onValueChanged;
            return new ActionRegister(() => UnRegister(onValueChanged));
        }

        public IRegister RegisterWithInitValue(Action<T, T> onValueChanged)
        {
            onValueChanged(mValue, mValue);
            return Register(onValueChanged);
        }

        public void UnRegister(Action<T, T> onValueChanged)
        {
            mOnValueChanged -= onValueChanged;
        }

        public override string ToString()
        {
            return GetValue().ToString();
        }
    }
}