using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace PartsKit
{
    /// <summary>
    /// 对应UnityAnimation（暂未实现Event）
    /// </summary>
    public class SpineAnimation : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation skeletonAnimation;
        [SerializeField] private SpineAnimRecordData animRecordData;
        [SerializeField] private string defaultAnimName;
        [SerializeField] private float defaultAnimSpeed = 1;

        public SkeletonAnimation SkeletonAnimation => skeletonAnimation;
        public IReadOnlyList<SpineAnimRecordData.AnimClipData> Clips => animRecordData.ClipData;
        private bool isInit;
        private int curPlayClipIndex;
        private SpineAnimRecordData.AnimClipData curPlayClipData;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            if (isInit)
            {
                return;
            }

            isInit = true;
            curPlayClipIndex = -1;
            curPlayClipData = null;
            PlayAnim(defaultAnimName, defaultAnimSpeed);
        }

        public bool IsPlaying()
        {
            return skeletonAnimation.AnimationState.IsPlaying();
        }

        public void PlayAnim(string animName, float speed)
        {
            var animId = Animator.StringToHash(animName);
            PlayAnim(animId, speed);
        }

        public void PlayAnim(int animId, float speed)
        {
            skeletonAnimation.AnimationState.ClearTracks();
            int clipIndex = Array.FindIndex(animRecordData.ClipData, item => item.AnimHash == animId);
            if (clipIndex < 0)
            {
                return;
            }

            var clipData = animRecordData.ClipData[clipIndex];
            curPlayClipData = clipData;
            curPlayClipIndex = clipIndex;
            skeletonAnimation.timeScale = speed;
            SpineAnimUtils.CalculateClipLength(clipData, skeletonAnimation.skeleton.Data, out int maxLengthTrack);
            TrackEntry lastTrack = null;
            foreach (var data in clipData.SetAnimPool)
            {
                var track = skeletonAnimation.state.SetAnimation(data.TrackIndex, data.AnimName, false);
                if (track.TrackIndex == maxLengthTrack)
                {
                    lastTrack = track;
                }
            }

            foreach (var data in clipData.AddAnimPool)
            {
                var track = skeletonAnimation.state.AddAnimation(data.TrackIndex, data.AnimName, false, data.Delay);
                if (track.TrackIndex == maxLengthTrack)
                {
                    lastTrack = track;
                }
            }

            if (clipData.IsLoop && lastTrack != null)
            {
                lastTrack.Complete += _ => { PlayAnim(animId, speed); };
            }
        }

        public void StopAnim()
        {
            skeletonAnimation.AnimationState.ClearTracks();
        }

        public void UpdateSpeed(float speed)
        {
            skeletonAnimation.timeScale = speed;
        }
    }
}