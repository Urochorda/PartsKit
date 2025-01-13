using Spine;

namespace PartsKit
{
    public static class SpineExtensions
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
}