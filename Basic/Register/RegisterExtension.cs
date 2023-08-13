using UnityEngine;

namespace PartsKit
{
    public static class RegisterExtension
    {
        public static IRegister UnRegisterWhenGameObjectDestroyed(this IRegister register, GameObject gameObject)
        {
            UnRegisterOnDestroyTrigger trigger = gameObject.GetComponent<UnRegisterOnDestroyTrigger>();

            if (!trigger)
            {
                trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();
            }

            trigger.AddUnRegister(register);

            return register;
        }
    }
}
