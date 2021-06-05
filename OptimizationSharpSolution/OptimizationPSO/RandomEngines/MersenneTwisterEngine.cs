namespace OptimizationPSO.RandomEngines
{
    public class MersenneTwisterEngine : IRandomEngine
    {
        MersenneTwister64 _mt64 = new MersenneTwister64();
        public double NextDouble()
        {
            return _mt64.NextDouble();
        }
    }
}