using System;

namespace OptimizationPSO.RandomEngines
{
    public class DefaultWindowsRandomEngine : IRandomEngine
    {
        private Random _rnd;

        public DefaultWindowsRandomEngine(int seed)
        {
            _rnd = new Random(seed);
        }

        public double NextDouble()
        {
            return _rnd.NextDouble();
        }
    }
}