using System;

namespace OptimizationPSO.RandomEngines
{
    public class DefaultWindowsRandomEngine : IRandomEngine
    {

        Random _rnd = new Random();
        public double NextDouble()
        {
            return _rnd.NextDouble();
        }
    }
}