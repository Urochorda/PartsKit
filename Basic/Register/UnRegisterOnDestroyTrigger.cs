using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    public class UnRegisterOnDestroyTrigger : MonoBehaviour
    {
        private readonly HashSet<IRegister> mUnRegisters = new HashSet<IRegister>();

        public void AddUnRegister(IRegister register)
        {
            mUnRegisters.Add(register);
        }

        public void RemoveUnRegister(IRegister register)
        {
            mUnRegisters.Remove(register);
        }

        private void OnDestroy()
        {
            foreach (IRegister unRegister in mUnRegisters)
            {
                unRegister.UnRegister();
            }

            mUnRegisters.Clear();
        }
    }
}
