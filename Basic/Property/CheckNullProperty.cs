using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PartsKit
{
    [Serializable]
    public class CheckNullProperty<T>
    {
        [SerializeField] private T property;
        [SerializeField] private bool timelyCheck; //是否每次获取value都去检测
        private bool mIsNull; //上次检测后是否为空
        private bool mIsStartCheck; //记录是否第一次检测过为空

        /// <summary>
        /// 初始化构造
        /// </summary>
        /// <param name="value">要检测的值</param>
        /// <param name="isTimelyCheck">是否每次获取都检测</param>
        public CheckNullProperty(T value, bool isTimelyCheck = true)
        {
            SetTimelyCheck(isTimelyCheck);
            SetData(value);
        }

        public void SetData(T value)
        {
            property = value;
            UpdateIsNull();
        }

        public void SetTimelyCheck(bool isIsTimelyCheck)
        {
            timelyCheck = isIsTimelyCheck;
        }

        public bool GetValue(out T value)
        {
            if (!mIsStartCheck || timelyCheck) //第一次没有检测||需要每次获取都检测
            {
                UpdateIsNull();
            }

            value = mIsNull ? default : property;

            return !mIsNull;
        }

        private void UpdateIsNull()
        {
            mIsStartCheck = true;
            if (property is Object behaviour)
            {
                mIsNull = behaviour == null;
                return;
            }

            mIsNull = property == null;
        }
    }
}