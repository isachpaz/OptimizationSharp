namespace OptimizationPSO
{
    public class NMSolverConfig
    {
        public double ConvergenceTolerance { get; }
        public int MaximumIterations { get; }

        public NMSolverConfig(double convergenceTolerance, int maximumIterations)
        {
            ConvergenceTolerance = convergenceTolerance;
            MaximumIterations = maximumIterations;
        }

        public override string ToString()
        {
            return $"ConvergenceTolerance: {ConvergenceTolerance}, MaximumIterations: {MaximumIterations}";
        }

        public static NMSolverConfig Default()
        {
            return new NMSolverConfig(1e-10, maximumIterations: 500);
        }
    }
}