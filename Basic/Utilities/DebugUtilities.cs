using UnityEngine;

namespace PartsKit
{
    public static class DebugUtilities
    {
        public static Color CastColor { get; set; } = Color.red;
        public static float CastTime { get; set; } = 1;
        public static Color PointColor { get; set; } = Color.green;
        public static float PointTime { get; set; } = 1;

        public static void DrawPoint(Vector2 point)
        {
#if UNITY_EDITOR
            int segments = 36;
            float radius = 20;
            DrawCircle(point, radius, segments, PointColor, PointTime);
#endif
        }

        public static void DrawRayCast(Vector2 start, Vector2 direction, float distance)
        {
#if UNITY_EDITOR
            Vector2 end = start + direction.normalized * distance;
            Debug.DrawLine(start, end, CastColor, CastTime);
#endif
        }

        public static void DrawBoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
        {
#if UNITY_EDITOR
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector2[] corners = new Vector2[4];

            // 计算盒子四个角的位置
            corners[0] = origin + (Vector2)(rotation * new Vector3(-size.x / 2, -size.y / 2));
            corners[1] = origin + (Vector2)(rotation * new Vector3(size.x / 2, -size.y / 2));
            corners[2] = origin + (Vector2)(rotation * new Vector3(size.x / 2, size.y / 2));
            corners[3] = origin + (Vector2)(rotation * new Vector3(-size.x / 2, size.y / 2));

            Vector2 offset = direction.normalized * distance;

            // 绘制原始盒子
            for (int i = 0; i < 4; i++)
            {
                Vector2 start = corners[i];
                Vector2 end = corners[(i + 1) % 4];
                Debug.DrawLine(start, end, CastColor, CastTime);
            }

            // 绘制移动后的盒子
            for (int i = 0; i < 4; i++)
            {
                Vector2 start = corners[i] + offset;
                Vector2 end = corners[(i + 1) % 4] + offset;
                Debug.DrawLine(start, end, CastColor, CastTime);
            }

            // 绘制连接线
            for (int i = 0; i < 4; i++)
            {
                Debug.DrawLine(corners[i], corners[i] + offset, CastColor, CastTime);
            }
#endif
        }

        public static void DrawCircleCast(Vector2 origin, float radius, Vector2 direction,
            float distance)
        {
#if UNITY_EDITOR
            int segments = 36;
            Vector2 offset = direction.normalized * distance;
            DrawCircle(origin, radius, segments, CastColor, CastTime);
            DrawCircle(origin + offset, radius, segments, CastColor, CastTime);

            for (var i = 0.0f; i < 360.0f; i += 90)
            {
                float angle = Vector2.Angle(Vector2.right, direction) + i;
                var rad = Mathf.Deg2Rad * angle;
                var x = radius * Mathf.Cos(rad);
                var z = radius * Mathf.Sin(rad);
                Vector2 pos = new Vector2(x, z) + origin;
                Debug.DrawLine(pos, pos + offset, CastColor, CastTime);
            }
#endif
        }

        public static void DrawCapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection,
            float angle, Vector2 direction, float distance)
        {
#if UNITY_EDITOR
            int segments = 18; // 半圆分段数
            float angleStep = 180f / segments;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector2 offset = direction.normalized * distance;

            // 计算胶囊的半径和长度
            float radius = capsuleDirection == CapsuleDirection2D.Vertical ? size.x / 2 : size.y / 2;
            float length = capsuleDirection == CapsuleDirection2D.Vertical ? size.y - size.x : size.x - size.y;
            Vector2 lengthOffset = (capsuleDirection == CapsuleDirection2D.Vertical ? Vector2.up : Vector2.right) *
                length / 2;

            // 计算起始胶囊的各个点
            Vector2[] startPointsTop = new Vector2[segments + 1];
            Vector2[] startPointsBottom = new Vector2[segments + 1];
            for (int i = 0; i <= segments; i++)
            {
                float a = Mathf.Deg2Rad * (i * angleStep);
                startPointsTop[i] = origin +
                                    (Vector2)(rotation * (new Vector2(Mathf.Cos(a) * radius, Mathf.Sin(a) * radius) +
                                                          lengthOffset));
                startPointsBottom[i] = origin +
                                       (Vector2)(rotation *
                                                 (new Vector2(Mathf.Cos(a) * radius, -Mathf.Sin(a) * radius) -
                                                  lengthOffset));
            }

            // 计算结束胶囊的各个点
            Vector2[] endPointsTop = new Vector2[segments + 1];
            Vector2[] endPointsBottom = new Vector2[segments + 1];
            for (int i = 0; i <= segments; i++)
            {
                endPointsTop[i] = startPointsTop[i] + offset;
                endPointsBottom[i] = startPointsBottom[i] + offset;
            }

            // 绘制起始胶囊的线框
            for (int i = 0; i < segments; i++)
            {
                Debug.DrawLine(startPointsTop[i], startPointsTop[i + 1], CastColor, CastTime);
                Debug.DrawLine(startPointsBottom[i], startPointsBottom[i + 1], CastColor, CastTime);
            }

            // 连接半圆两端
            Debug.DrawLine(startPointsTop[0], startPointsBottom[0], CastColor, CastTime);
            Debug.DrawLine(startPointsTop[segments], startPointsBottom[segments], CastColor, CastTime);

            // 绘制结束胶囊的线框
            for (int i = 0; i < segments; i++)
            {
                Debug.DrawLine(endPointsTop[i], endPointsTop[i + 1], CastColor, CastTime);
                Debug.DrawLine(endPointsBottom[i], endPointsBottom[i + 1], CastColor, CastTime);
            }

            // 连接半圆两端
            Debug.DrawLine(endPointsTop[0], endPointsBottom[0], CastColor, CastTime);
            Debug.DrawLine(endPointsTop[segments], endPointsBottom[segments], CastColor, CastTime);

            // 绘制连接

            Debug.DrawLine(startPointsTop[0], endPointsTop[0], CastColor, CastTime);
            Debug.DrawLine(startPointsBottom[segments], endPointsBottom[segments], CastColor, CastTime);

            Debug.DrawLine(startPointsTop[segments], endPointsTop[segments], CastColor, CastTime);
            Debug.DrawLine(startPointsBottom[0], endPointsBottom[0], CastColor, CastTime);

            Debug.DrawLine(startPointsTop[segments / 2], endPointsTop[segments / 2], CastColor, CastTime);
            Debug.DrawLine(startPointsBottom[segments / 2], endPointsBottom[segments / 2], CastColor, CastTime);

#endif
        }

        public static void DrawCircle(Vector2 origin, float radius, int segments, Color color, float time)
        {
#if UNITY_EDITOR
            float angleStep = 2f * Mathf.PI / segments;
            Vector2 previousPoint = origin + new Vector2(radius, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep;
                Vector2 currentPoint = origin + new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
                Debug.DrawLine(previousPoint, currentPoint, color, time);
                previousPoint = currentPoint;
            }
#endif
        }
    }
}