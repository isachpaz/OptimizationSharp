namespace OptimizationPSO.RandomEngines
{
    public class MersenneTwisterEngine : IRandomEngine
    {
        private MathNet.Numerics.Random.MersenneTwister _mt64;

        public MersenneTwisterEngine(int seed)
        {
            _mt64 = new MathNet.Numerics.Random.MersenneTwister(seed);
        }

        public double NextDouble()
        {
            return _mt64.NextDouble();
        }
    }
}