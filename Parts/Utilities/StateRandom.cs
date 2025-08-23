using System;

namespace PartsKit
{
    public interface IRandom
    {
        int Next();
        int Next(int maxValue);
        int Next(int minValue, int maxValue);
        double NextDouble();
        void NextBytes(byte[] buffer);
    }

    public class StateRandom : IRandom
    {
        public struct State
        {
            public int Inext { get; set; }
            public int Inextp { get; set; }
            public int[] SeedArray { get; set; }
        }

        private const int MBIG = Int32.MaxValue;
        private const int MSEED = 161803398;

        private int inext;
        private int inextp;
        private int[] seedArray = new int[56];

        public int Seed { get; private set; }

        public StateRandom() : this(Environment.TickCount)
        {
        }

        public StateRandom(int seed)
        {
            Seed = seed;

            int ii;
            int mj, mk;

            //Initialize our Seed array.
            //This algorithm comes from Numerical Recipes in C (2nd Ed.)
            int subtraction = (seed == Int32.MinValue) ? Int32.MaxValue : Math.Abs(seed);
            mj = MSEED - subtraction;
            seedArray[55] = mj;
            mk = 1;
            for (int i = 1; i < 55; i++)
            {
                //Apparently the range [1..55] is special (Knuth) and so we're wasting the 0'th position.
                ii = (21 * i) % 55;
                seedArray[ii] = mk;
                mk = mj - mk;
                if (mk < 0) mk += MBIG;
                mj = seedArray[ii];
            }

            for (int k = 1; k < 5; k++)
            {
                for (int i = 1; i < 56; i++)
                {
                    seedArray[i] -= seedArray[1 + (i + 30) % 55];
                    if (seedArray[i] < 0) seedArray[i] += MBIG;
                }
            }

            inext = 0;
            inextp = 21;
            seed = 1;
        }

        public StateRandom(int seed, State state)
        {
            SetData(seed, state);
        }

        public void SetData(int seed, State state)
        {
            Seed = seed;
            SetState(state);
        }

        protected virtual double Sample()
        {
            //Including this division at the end gives us significantly improved
            //random number distribution.
            return (InternalSample() * (1.0 / MBIG));
        }

        private int InternalSample()
        {
            int retVal;
            int locINext = inext;
            int locINextp = inextp;

            if (++locINext >= 56) locINext = 1;
            if (++locINextp >= 56) locINextp = 1;

            retVal = seedArray[locINext] - seedArray[locINextp];

            if (retVal == MBIG) retVal--;
            if (retVal < 0) retVal += MBIG;

            seedArray[locINext] = retVal;

            inext = locINext;
            inextp = locINextp;

            return retVal;
        }

        public virtual int Next()
        {
            return InternalSample();
        }

        private double GetSampleForLargeRange()
        {
            // The distribution of double value returned by Sample 
            // is not distributed well enough for a large range.
            // If we use Sample for a range [Int32.MinValue..Int32.MaxValue)
            // We will end up getting even numbers only.

            int result = InternalSample();
            // Note we can't use addition here. The distribution will be bad if we do that.
            bool negative = (InternalSample() % 2 == 0) ? true : false; // decide the sign based on second sample
            if (negative)
            {
                result = -result;
            }

            double d = result;
            d += (Int32.MaxValue - 1); // get a number in range [0 .. 2 * Int32MaxValue - 1)
            d /= 2 * (uint)Int32.MaxValue - 1;
            return d;
        }

        public virtual int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue),
                    $"{nameof(minValue)} must be less than or equal to {nameof(maxValue)}.");
            }

            long range = (long)maxValue - minValue;
            if (range <= (long)Int32.MaxValue)
            {
                return ((int)(Sample() * range) + minValue);
            }
            else
            {
                return (int)((long)(GetSampleForLargeRange() * range) + minValue);
            }
        }

        public virtual int Next(int maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxValue),
                    $"ArgumentOutOfRange_MustBePositive {nameof(maxValue)}.");
            }

            return (int)(Sample() * maxValue);
        }

        public virtual double NextDouble()
        {
            return Sample();
        }

        public virtual void NextBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(InternalSample() % (Byte.MaxValue + 1));
            }
        }

        public State GetState()
        {
            var state = new State()
            {
                Inext = inext,
                Inextp = inextp,
                SeedArray = new int[seedArray.Length]
            };
            for (var i = 0; i < seedArray.Length; i++)
            {
                var seed = seedArray[i];
                state.SeedArray[i] = seed;
            }

            return state;
        }

        public void SetState(State state)
        {
            inext = state.Inext;
            inextp = state.Inextp;
            if (state.SeedArray.Length != seedArray.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(state.SeedArray),
                    $"Invalid seed array length: expected {seedArray.Length}, but got {state.SeedArray.Length}.");
            }

            for (var i = 0; i < seedArray.Length; i++)
            {
                seedArray[i] = state.SeedArray[i];
            }
        }
    }
}