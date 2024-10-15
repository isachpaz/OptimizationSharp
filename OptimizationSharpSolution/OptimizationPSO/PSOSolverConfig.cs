using System;
using System.Collections.Generic;
using OptimizationPSO.RandomEngines;
using OptimizationPSO.StoppingCriteria;

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

        public IRandomEngine RandomEngine { get; internal set; }
        public bool IsStoppingCriteriaEnabled { get; internal set; }
        public List<BaseStoppingCriterion> StoppingCriteria { get; set; } = new List<BaseStoppingCriterion>();

        internal PSOSolverConfig()
        {
        }

        public static PSOSolverConfig CreateDefault(
            int numberParticles,
            int maxEpochs,
            double[] lowerBound,
            double[] upperBound,
            int? seed = null,
            double acceptanceError = 1E-09,
            double particleResetProbability = 0.001,
            bool isStoppingCriteriaEnabled = true)
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
                .WithRandomEngine(RandomEngineFactory.Create(RandomEngines.RandomEngine.MersenneTwister,
                    seed ?? MathNet.Numerics.Random.RandomSeed.Robust()))
                .WithStoppingCriteriaEnabled(isStoppingCriteriaEnabled)
                .Build();
        }
    }
}