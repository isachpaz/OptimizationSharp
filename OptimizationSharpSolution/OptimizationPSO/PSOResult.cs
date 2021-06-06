namespace OptimizationPSO
{
    public class PSOResult
    {
        public double[] BestPosition;
        public double BestFitness;
        public bool Success;
        public int Iteration { get; internal set; }

        public override string ToString()
        {
            return $"{nameof(BestPosition)}: {BestPosition}, {nameof(BestFitness)}: {BestFitness}, {nameof(Success)}: {Success}, {nameof(Iteration)}: {Iteration}";
        }
    }
}