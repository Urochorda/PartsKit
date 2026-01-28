using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    public static class SpineAnimUtils
    {
        /// <summary>
        /// 计算SpineClip的长度保证非Loop，只考虑轨道覆盖长度
        /// </summary>
        public static float CalculateClipLength(SpineAnimRecordData.AnimClipData animClipData,
            Spine.SkeletonData skeletonData, out int maxLengthTrack)
        {
            var trackTimes = new Dictionary<int, float>();

            //1.SetAnimation（重置Track）
            foreach (var set in animClipData.SetAnimPool)
            {
                float dur = skeletonData.FindAnimation(set.AnimName).Duration;
                trackTimes[set.TrackIndex] = dur;
            }

            //2.AddAnimation（排队）
            foreach (var add in animClipData.AddAnimPool)
            {
                var time = trackTimes.GetValueOrDefault(add.TrackIndex, 0f);
                float dur = skeletonData.FindAnimation(add.AnimName).Duration;
                time = time + Mathf.Max(add.Delay, 0f) + dur;
                trackTimes[add.TrackIndex] = time;
            }

            // 3.总长度=所有Track的最大结束时间
            float max = 0f;
            maxLengthTrack = 0;
            foreach (var pair in trackTimes)
            {
                if (pair.Value >= max)
                {
                    max = pair.Value;
                    maxLengthTrack = pair.Key;
                }
            }

            return max;
        }
    }
}