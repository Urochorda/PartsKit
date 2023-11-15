using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace PartsKit
{
    public static class GizmosUtilities
    {
        /// <summary>
        /// 绘制半圆锥题线框
        /// </summary>
        public static void DrawWireHemisphericalCone(Vector3 center, float height, float radius, Quaternion rotation,
            int divide = 25)
        {
            float coneHeight = height - radius;
            Vector3 sphericalPoint = new Vector3(0, coneHeight, 0);
            DrawWireRootCircle(center, sphericalPoint, Vector3.forward, Vector3.up, radius, 180, rotation, divide);
            DrawWireRootCircle(center, sphericalPoint, Vector3.right, Vector3.up, radius, 180, rotation, divide);
            DrawWireRootCircle(center, sphericalPoint, Vector3.forward, Vector3.right, radius, 360, rotation, divide);

            for (var i = 0.0f; i < 360.0f; i += 90)
            {
                var rad = Mathf.Deg2Rad * i;
                var x = radius * Mathf.Cos(rad);
                var z = radius * Mathf.Sin(rad);
                Gizmos.DrawLine(center, center + rotation * new Vector3(x, coneHeight, z));
            }
        }

        /// <summary>
        /// 绘制半圆锥题线框（根据角度）
        /// </summary>
        public static void DrawWireHemisphericalConeByAngle(Vector3 center, float height, float angle,
            Quaternion rotation,
            int divide = 25)
        {
            float tanValue = Mathf.Tan(angle / 2 * Mathf.Deg2Rad);
            float radius = height * tanValue / (1 + tanValue);
            DrawWireHemisphericalCone(center, height, radius, rotation, divide);
        }

        /// <summary>
        /// 绘制视锥线框
        /// </summary>
        public static void DrawWireVisionCone(Vector3 center, float radius, float angle, Quaternion rotation,
            int divide = 25)
        {
            DrawWireCircle(center, Vector3.forward, Vector3.up, radius, angle,
                rotation * Quaternion.Euler(-Vector3.right * (90 - angle / 2)), divide);
            DrawWireCircle(center, Vector3.right, Vector3.up, radius, angle,
                rotation * Quaternion.Euler(Vector3.forward * (90 - angle / 2)), divide);

            Vector3 rootCircleCenter = Vector3.zero;
            float rootCircleAngle = Mathf.Deg2Rad * (90 - angle / 2);
            rootCircleCenter.y = radius * Mathf.Sin(rootCircleAngle);
            float rootCircleRadius = radius * Mathf.Cos(rootCircleAngle);
            DrawWireRootCircle(center, rootCircleCenter, Vector3.forward, Vector3.right, rootCircleRadius, 360,
                rotation, divide);
            for (var i = 0.0f; i < 360.0f; i += 90)
            {
                var rad = Mathf.Deg2Rad * i;
                var x = rootCircleRadius * Mathf.Cos(rad);
                var z = rootCircleRadius * Mathf.Sin(rad);
                Gizmos.DrawLine(center, center + rotation * new Vector3(x, rootCircleCenter.y, z));
            }
        }

        /// <summary>
        /// 绘制偏心圆线框
        /// </summary>
        public static void DrawWireRootCircle(Vector3 root, Vector3 center, Vector3 forward, Vector3 right,
            float radius,
            float maxAngle,
            Quaternion rotation, int divide = 25)
        {
            if (radius <= 0 || divide < 3)
            {
                return;
            }

            forward = forward.normalized;
            right = right.normalized;
            if (Vector3.Dot(forward, right) > 0.0001f)
            {
                return;
            }

            List<Vector3> vertices = new List<Vector3>();
            Vector3 startPos = center + forward * radius;
            vertices.Add(startPos);
            float stepAngle = maxAngle / divide;
            float curAngle = 0f;
            int curDivideIndex = 0;
            while (curDivideIndex < divide)
            {
                curAngle += stepAngle;
                curDivideIndex++;
                if (curDivideIndex <= divide)
                {
                    float x = radius * Mathf.Cos(Mathf.Deg2Rad * curAngle);
                    float y = radius * Mathf.Sin(Mathf.Deg2Rad * curAngle);
                    Vector3 vertex = center + right * y + forward * x;
                    vertices.Add(vertex);
                }
                else
                {
                    vertices.Add(startPos);
                }
            }

            for (int i = 1; i < vertices.Count; i++)
            {
                Gizmos.DrawLine(root + rotation * vertices[i - 1], root + rotation * vertices[i]);
            }
        }

        /// <summary>
        /// 绘制圆线框
        /// </summary>
        public static void DrawWireCircle(Vector3 center, Vector3 forward, Vector3 right, float radius, float maxAngle,
            Quaternion rotation, int divide = 25)
        {
            DrawWireRootCircle(center, Vector3.zero, forward, right, radius, maxAngle, rotation, divide);
        }

        // /// <summary>
        // /// 创建圆锥Mesh
        // /// </summary>
        // /// <param name="radius">圆锥底面半径</param>
        // /// <param name="height">圆锥高度</param>
        // /// <returns>Mesh对象</returns>
        // private Mesh CreateConeMesh(float radius, float height)
        // {
        //     var vertices = new List<Vector3>();
        //     var indices = new List<int>();
        //
        //     vertices.Add(Vector3.zero);
        //     //底圆面
        //     for (var i = 0.0f; i < 360.0f; i += 90)
        //     {
        //         var rad = Mathf.Deg2Rad * i;
        //         var x = radius * Mathf.Cos(rad);
        //         var z = radius * Mathf.Sin(rad);
        //         vertices.Add(new Vector3(x, height - radius, z));
        //     }
        //
        //     //按三角形设置线
        //     for (var i = 1; i <= 4; i++)
        //     {
        //         indices.Add(i);
        //         if (i < 4)
        //         {
        //             indices.Add(i + 1);
        //         }
        //         else
        //         {
        //             indices.Add(1);
        //         }
        //
        //         indices.Add(0);
        //     }
        //
        //     Mesh mesh = new Mesh();
        //     mesh.SetVertices(vertices);
        //     mesh.SetTriangles(indices, 0);
        //     mesh.RecalculateNormals();
        //
        //     return mesh;
        // }

        public static Color GenerateColorByType(Type type)
        {
            string typeName = type.FullName;
            byte[] hashBytes = ComputeSHA256Hash(typeName);
            
            float hue = Mathf.Repeat((hashBytes[0] ^ hashBytes[1]) / 255.0f, 1.0f);
            
            float minS = 0.5f;
            float saturation = Mathf.Repeat((hashBytes[2] ^ hashBytes[3]) / 255.0f, 1.0f);
            saturation = minS + saturation * (1 - minS);
            
            float minV = 0.9f;
            float value = Mathf.Repeat((hashBytes[4] ^ hashBytes[5]) / 255.0f, 1.0f);
            value = minV + value * (1 - minV);
            
            return Color.HSVToRGB(hue, saturation, value);

            byte[] ComputeSHA256Hash(string input)
            {
                using SHA256 sha256 = SHA256.Create();
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                return sha256.ComputeHash(inputBytes);
            }
        }
    }
}