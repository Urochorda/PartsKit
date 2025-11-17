using System;

namespace PartsKit
{
    public interface IReadOnlyBindableProperty<T>
    {
        public T GetValue();
        public IRegister Register(Action<T, T> onValueChanged);
        public IRegister RegisterWithInitValue(Action<T, T> onValueChanged);
        public void UnRegister(Action<T, T> onValueChanged);
        public IRegister RegisterLate(Action<T, T> onValueChanged);
        public IRegister RegisterLateWithInitValue(Action<T, T> onValueChanged);
        public void UnRegisterLate(Action<T, T> onValueChanged);
        public string ToString();
    }

    public class BindableProperty<T> : IReadOnlyBindableProperty<T>
    {
        private T mValue;
        private Action<T, T> mOnValueChanged;
        private Action<T, T> mOnValueChangedLate;

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
            mOnValueChanged?.Invoke(oldValue, mValue);
            mOnValueChangedLate?.Invoke(oldValue, mValue);
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

        public IRegister RegisterLate(Action<T, T> onValueChanged)
        {
            mOnValueChangedLate += onValueChanged;
            return new ActionRegister(() => UnRegisterLate(onValueChanged));
        }

        public IRegister RegisterLateWithInitValue(Action<T, T> onValueChanged)
        {
            onValueChanged(mValue, mValue);
            return RegisterLate(onValueChanged);
        }

        public void UnRegisterLate(Action<T, T> onValueChanged)
        {
            mOnValueChangedLate -= onValueChanged;
        }

        public override string ToString()
        {
            return GetValue().ToString();
        }
    }

    public interface IReadOnlyBindableConvertProperty<TIn, TOut> : IReadOnlyBindableProperty<TOut>
    {
        public TIn GetValueIn();
    }

    public class BindableConvertProperty<TIn, TOut> : IReadOnlyBindableConvertProperty<TIn, TOut>
    {
        private readonly Func<TIn, TOut> convert;
        private TIn mInValue;
        private TOut mOutValue;
        private Action<TOut, TOut> mOnValueChanged;
        private Action<TOut, TOut> mOnValueChangedLate;

        public BindableConvertProperty(Func<TIn, TOut> convertVar, TIn defaultValue = default)
        {
            mInValue = defaultValue;
            mOutValue = convertVar(mInValue);
            convert = convertVar;
        }

        public TOut GetValue()
        {
            return mOutValue;
        }

        public TIn GetValueIn()
        {
            return mInValue;
        }

        public void SetValue(TIn newValue)
        {
            if (newValue == null && mInValue == null)
            {
                return;
            }

            if (newValue != null && newValue.Equals(mInValue))
            {
                return;
            }

            mInValue = newValue;
            var oldOutValue = mOutValue;
            mOutValue = convert(mInValue);
            mOnValueChanged?.Invoke(oldOutValue, mOutValue);
            mOnValueChangedLate?.Invoke(oldOutValue, mOutValue);
        }

        public void SetValueWithoutEvent(TIn newValue)
        {
            mInValue = newValue;
            mOutValue = convert(mInValue);
        }

        public IRegister Register(Action<TOut, TOut> onValueChanged)
        {
            mOnValueChanged += onValueChanged;
            return new ActionRegister(() => UnRegister(onValueChanged));
        }

        public IRegister RegisterWithInitValue(Action<TOut, TOut> onValueChanged)
        {
            onValueChanged(mOutValue, mOutValue);
            return Register(onValueChanged);
        }

        public void UnRegister(Action<TOut, TOut> onValueChanged)
        {
            mOnValueChanged -= onValueChanged;
        }

        public IRegister RegisterLate(Action<TOut, TOut> onValueChanged)
        {
            mOnValueChangedLate += onValueChanged;
            return new ActionRegister(() => UnRegisterLate(onValueChanged));
        }

        public IRegister RegisterLateWithInitValue(Action<TOut, TOut> onValueChanged)
        {
            onValueChanged(mOutValue, mOutValue);
            return RegisterLate(onValueChanged);
        }

        public void UnRegisterLate(Action<TOut, TOut> onValueChanged)
        {
            mOnValueChangedLate -= onValueChanged;
        }

        public override string ToString()
        {
            return GetValue().ToString();
        }
    }
}