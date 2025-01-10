using Spine;
using Spine.Unity;

namespace _Test
{
    //todo 整理代码
    public static class Ex
    {
        public static bool IsPlaying(this AnimationState state)
        {
            if (state.Tracks.Count <= 0)
            {
                return false;
            }

            foreach (var stateTrack in state.Tracks)
            {
                if (stateTrack.Loop)
                {
                    return true;
                }

                return !stateTrack.IsComplete;
            }

            return false;
        }
    }

    public class SpineAnimPlayer
    {
        private SkeletonAnimation mSkeletonAnimation;
        private SkeletonGraphic mSkeletonGraphic;
        private Skeleton mSkeleton;
        private AnimationState mAnimationState;
        public bool IsUI { get; }

        public SpineAnimPlayer(SkeletonAnimation skeletonAnimation)
        {
            IsUI = false;
            mSkeletonAnimation = skeletonAnimation;
            mSkeleton = skeletonAnimation.Skeleton;
            mAnimationState = skeletonAnimation.AnimationState;
        }

        public SpineAnimPlayer(SkeletonGraphic skeletonGraphic)
        {
            IsUI = true;
            mSkeletonGraphic = skeletonGraphic;
            mSkeleton = skeletonGraphic.Skeleton;
            mAnimationState = skeletonGraphic.AnimationState;
        }

        public bool TryGetAnimation(string animName, out Animation animation)
        {
            animation = mSkeleton.Data.FindAnimation(animName);
            return animation != null;
        }

        public void ResetEvent(int trackIndex)
        {
            var current = mAnimationState.GetCurrent(trackIndex);
            if (current == null)
            {
                return;
            }

            current.ResetEvent();
        }

        public TrackEntry PlayAnimation(int trackIndex, string animName, bool isLoop)
        {
            if (mAnimationState == null)
            {
                return null;
            }

            TrackEntry trackEntry = mAnimationState.SetAnimation(trackIndex, animName, isLoop);
            return trackEntry;
        }

        public TrackEntry AddAnimation(int trackIndex, string animName, bool isLoop, float delay = 0)
        {
            if (mAnimationState == null)
            {
                return null;
            }

            TrackEntry trackEntry = mAnimationState.AddAnimation(trackIndex, animName, isLoop, delay);
            return trackEntry;
        }

        public void StopAnimation(int trackIndex)
        {
            if (mAnimationState == null)
            {
                return;
            }

            mAnimationState.ClearTrack(trackIndex);
        }

        public void StopAnimationAll()
        {
            if (mAnimationState == null)
            {
                return;
            }

            mAnimationState.ClearTracks();
        }

        public void SetTimeScale(float speed)
        {
            if (mAnimationState == null)
            {
                return;
            }

            mAnimationState.TimeScale = speed;
        }

        public bool IsPlaying()
        {
            return mAnimationState.IsPlaying();
        }
    }
}