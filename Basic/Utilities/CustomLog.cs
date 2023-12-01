using System;
using UnityEngine;

namespace PartsKit
{
    public class CustomLog
    {
        public static bool EnableLog { get; set; } = true;

        public static void DrawLine(Vector3 start, Vector3 end)
        {
            if (!EnableLog)
                return;
            Debug.DrawLine(start, end);
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            if (!EnableLog)
                return;
            Debug.DrawLine(start, end, color);
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
        {
            if (!EnableLog)
                return;
            Debug.DrawLine(start, end, color, duration);
        }

        public static void Log(object message)
        {
            if (!EnableLog)
                return;
            Debug.Log(message);
        }

        public static void Log(object message, UnityEngine.Object context)
        {
            if (!EnableLog)
                return;
            Debug.Log(message, context);
        }

        public static void LogAssertion(object message)
        {
            int num = EnableLog ? 1 : 0;
        }

        public static void LogAssertionFormat(string format, params object[] args)
        {
            int num = EnableLog ? 1 : 0;
        }

        public static void LogError(object message) => Debug.LogError(message);

        public static void LogError(object message, UnityEngine.Object context) => Debug.LogError(message, context);

        public static void LogErrorFormat(string format, params object[] args) => Debug.LogErrorFormat(format, args);

        public static void LogException(Exception exception)
        {
            if (!EnableLog)
                return;
            Debug.LogError((object)exception);
        }

        public static void LogFormat(string format, params object[] args)
        {
            if (!EnableLog)
                return;
            Debug.LogFormat(format, args);
        }

        public static void LogWarning(object message)
        {
            if (!EnableLog)
                return;
            Debug.LogWarning(message);
        }

        public static void LogWarning(object message, UnityEngine.Object context)
        {
            if (!EnableLog)
                return;
            Debug.LogWarning(message, context);
        }

        public static void LogWarningFormat(string format, params object[] args)
        {
            if (!EnableLog)
                return;
            Debug.LogWarningFormat(format, args);
        }
    }
}