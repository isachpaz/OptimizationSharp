namespace OptimizationPSO.RandomEngines
{
    public class RandomEngineFactory
    {
        public static IRandomEngine Create(RandomEngine randomEngine)
        {
            switch (randomEngine)
            {
                case RandomEngine.Default:
                    return new DefaultWindowsRandomEngine();
                case RandomEngine.MersenneTwister:
                    return new MersenneTwisterEngine();
            }

            return null;
        }
    }
}