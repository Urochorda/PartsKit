using System;
using UnityEngine;

namespace PartsKit
{
    public static class RegisterExtension
    {
        public static IRegister UnRegisterWhenGameObjectDestroyed(this IRegister register, GameObject gameObject)
        {
            gameObject.AddDestroyListener(register.UnRegister);
            return register;
        }

        public static void AddDestroyListener(this GameObject gameObject, Action register)
        {
            GameObjectRegister trigger = gameObject.GetComponent<GameObjectRegister>();

            if (!trigger)
            {
                trigger = gameObject.AddComponent<GameObjectRegister>();
            }

            trigger.onDestory += register;
        }

        public static void RemoveDestroyListener(this GameObject gameObject, Action register)
        {
            GameObjectRegister trigger = gameObject.GetComponent<GameObjectRegister>();

            if (!trigger)
            {
                return;
            }

            trigger.onDestory -= register;
        }
    }
}