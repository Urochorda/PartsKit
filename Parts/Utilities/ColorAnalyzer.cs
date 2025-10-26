using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace PartsKit
{
    /// <summary>
    /// 颜色模式（必须连续）
    /// </summary>
    public enum ColorType
    {
        Unknown = 0,
        AlphaLess = 1,
        White = 2,
        Gray = 3,
        Black = 4,
        Red = 5,
        Orange = 6,
        Yellow = 7,
        Green = 8,
        Cyan = 9,
        Blue = 10,
        Purple = 11,
    }

    /// <summary>
    /// 分析颜色数量比例
    /// </summary>
    public static class ColorAnalyzer
    {
        private const float ColorVGray = 0.15f;
        private const float ColorHBlack = 0.15f;
        private const float ColorHWhite = 0.9f;

        private const float RedMin = 315f;
        private const float RedMax = 15f;
        private const float OrangeMax = 45f;
        private const float YellowMax = 75f;
        private const float GreenMax = 165f;
        private const float CyanMax = 195f;
        private const float BlueMax = 255f;
        private const float PurpleMax = 315f;

        [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast)]
        private struct ColorAnalysisJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Color32> Pixels;

            [NativeDisableContainerSafetyRestriction]
            public NativeArray<NativeArray<int>> CounterMaps;

            [ReadOnly] public int ChunkSize;

            public void Execute(int index)
            {
                int start = index * ChunkSize;
                int end = Mathf.Min((index + 1) * ChunkSize, Pixels.Length);

                NativeArray<int> counter = CounterMaps[index];

                for (int i = start; i < end; i++)
                {
                    Color32 color = Pixels[i];
                    ColorType colorType = CheckColor(color);
                    int key = (int)colorType;
                    counter[key] += 1;
                }
            }
        }

        public static int[] NewColorModeArrayInt()
        {
            return new int[Enum.GetValues(typeof(ColorType)).Length];
        }

        public static float[] NewColorModeArrayFloat()
        {
            return new float[Enum.GetValues(typeof(ColorType)).Length];
        }

        public static bool[] NewColorModeArrayBool()
        {
            return new bool[Enum.GetValues(typeof(ColorType)).Length];
        }

        public static void ColorRateExclude(float[] colorRate, bool[] colorExclude)
        {
            float totalCount = 0;
            for (var i = 0; i < colorRate.Length; i++)
            {
                bool isExclude = colorExclude[i];
                if (isExclude)
                {
                    colorRate[i] = 0;
                }
                else
                {
                    totalCount += colorRate[i];
                }
            }

            for (var i = 0; i < colorRate.Length; i++)
            {
                colorRate[i] /= totalCount;
            }
        }

        [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast)]
        public static ColorType CheckColor(Color32 rgbColor)
        {
            if (rgbColor.a <= 128)
            {
                return ColorType.AlphaLess;
            }

            // //转换到HSV空间
            Color color = new Color(rgbColor.r / 255f, rgbColor.g / 255f, rgbColor.b / 255f, rgbColor.a / 255f);
            Color.RGBToHSV(color, out float h, out float s, out float v);

            //黑色明度
            if (v <= ColorHBlack)
            {
                return ColorType.Black;
            }

            //灰色色相
            if (s <= ColorVGray)
            {
                if (v >= ColorHWhite)
                {
                    return ColorType.White;
                }

                return ColorType.Gray;
            }

            //将色相转换为角度 (0-360°)
            float hueAngle = h * 360f;

            if (hueAngle >= RedMin || hueAngle < RedMax) return ColorType.Red;
            if (hueAngle < OrangeMax) return ColorType.Orange;
            if (hueAngle < YellowMax) return ColorType.Yellow;
            if (hueAngle < GreenMax) return ColorType.Green;
            if (hueAngle < CyanMax) return ColorType.Cyan;
            if (hueAngle < BlueMax) return ColorType.Blue;
            if (hueAngle < PurpleMax) return ColorType.Purple;
            return ColorType.Unknown;
        }

        public static void GetColorCount(Texture2D texture, int[] colorCountOut)
        {
            //获取原始像素数据
            var pixels = texture.GetRawTextureData<Color32>();
            GetColorCount(pixels, colorCountOut);
        }

        public static void GetColorCount(NativeArray<Color32> pixels, int[] colorCountOut)
        {
            //构造基础数据
            for (var i = 0; i < colorCountOut.Length; i++)
            {
                colorCountOut[i] = 0;
            }

            int colorTypeLenght = colorCountOut.Length;
            int pixelsLenght = pixels.Length;
            int localCounterLenght = Mathf.Max(64, SystemInfo.processorCount);
            int localChunkSize = (pixelsLenght + localCounterLenght - 1) / localCounterLenght;
            NativeArray<NativeArray<int>> localCounterMapArray =
                new NativeArray<NativeArray<int>>(localCounterLenght, Allocator.TempJob);
            for (int i = 0; i < localCounterLenght; i++)
            {
                localCounterMapArray[i] = new NativeArray<int>(colorTypeLenght, Allocator.TempJob);
            }

            //创建并调度Job
            var job = new ColorAnalysisJob
            {
                Pixels = pixels,
                CounterMaps = localCounterMapArray,
                ChunkSize = localChunkSize
            };
            var handle = job.Schedule(localCounterLenght, 1);
            handle.Complete();

            //合并分块处理的数据
            foreach (var counter in localCounterMapArray)
            {
                for (int i = 0; i < counter.Length; i++)
                {
                    var colorType = i;
                    var colorCount = counter[i];
                    colorCountOut[colorType] += colorCount;
                }
            }

            //清理本地计数器
            for (int i = 0; i < localCounterLenght; i++)
            {
                localCounterMapArray[i].Dispose();
            }

            localCounterMapArray.Dispose();
        }

        public static void GetColorRate(Texture2D texture, int[] colorCountOut, float[] colorRateOut)
        {
            //获取原始像素数据
            var pixels = texture.GetRawTextureData<Color32>();
            GetColorRate(pixels, colorCountOut, colorRateOut);
        }

        public static void GetColorRate(NativeArray<Color32> pixels, int[] colorCountOut, float[] colorRateOut)
        {
            GetColorCount(pixels, colorCountOut);
            int totalPixels = pixels.Length;
            //设置输出的比例
            for (var i = 0; i < colorCountOut.Length; i++)
            {
                var colorType = i;
                var colorCount = colorCountOut[i];
                var percentage = (float)colorCount / totalPixels;
                colorRateOut[colorType] = percentage;
            }
        }
    }
}