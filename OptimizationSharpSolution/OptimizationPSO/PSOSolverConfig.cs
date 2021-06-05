using System;
using OptimizationPSO.RandomEngines;

namespace OptimizationPSO
{
    public partial class PSOSolverConfig
    {
        public int NumDimensions { get; internal set; }
        public int NumParticles { get; internal set; }
        public int MaxEpochs { get; internal set; }
        public double AcceptanceError { get; internal set; }
        public double[] UpperBound { get; internal set; }
        public double[] LowerBound { get; internal set; }

        // https://en.wikipedia.org/wiki/Particle_swarm_optimization
        public double InertiaWeight { get; internal set; } = 0.729;
        public double C1CognitiveWeight { get; internal set; } = 1.49445;
        public double C2SocialWeight { get; internal set; } = 1.49445;

        public double VelocityInitialAttenuation { get; internal set; } = 0.1;

        public double ParticleResetProbability { get; internal set; } = 0.0;

        public int RandomSeed { get; internal set; } = 0;

        public IRandomEngine RanadomEngine { get; internal set; }

        internal PSOSolverConfig()
        {
        }

        public static PSOSolverConfig CreateDefault(int numberParticles, 
            int maxEpochs,
            double[] lowerBound,
            double[] upperBound,
            double acceptanceError = 1E-09,
            double particleResetProbability = 0.001)
        {
            if (lowerBound.Length != upperBound.Length)
                throw new ArgumentException("Dimensions of lower and upper bound do not match");


            return PSOSolverConfigBuilder.Init()
                .WithNumParticles(numberParticles)
                .WithNumDimensions(lowerBound.Length)
                .WithMaxEpochs(maxEpochs)
                .WithAcceptanceError(acceptanceError)
                .WithInertiaWeight(0.729)
                .WithC1CognitiveWeight(1.49445)
                .WithC2SocialWeight(1.49445)
                .WithVelocityInitialAttenuation(0.1)
                .WithParticleResetProbability(particleResetProbability)
                .WithLowerBound(lowerBound)
                .WithUpperBound(upperBound)
                .Build();
        }
    }
}