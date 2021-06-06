namespace OptimizationPSO.RandomEngines
{
    public class RandomEngineFactory
    {
        public static IRandomEngine Create(RandomEngine randomEngine, int seed)
        {
            switch (randomEngine)
            {
                case RandomEngine.Default:
                    return new DefaultWindowsRandomEngine(seed);
                case RandomEngine.MersenneTwister:
                    return new MersenneTwisterEngine(seed);
            }

            return null;
        }
    }
}