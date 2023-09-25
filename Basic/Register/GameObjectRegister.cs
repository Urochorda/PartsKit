using System;
using UnityEngine;

namespace PartsKit
{
    public class GameObjectRegister : MonoBehaviour
    {
        public event Action onDestory;

        private void OnDestroy()
        {
            onDestory?.Invoke();
        }
    }
}