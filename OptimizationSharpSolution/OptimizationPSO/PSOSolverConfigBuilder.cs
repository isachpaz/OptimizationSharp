using OptimizationPSO.RandomEngines;

namespace OptimizationPSO
{
    public class PSOSolverConfigBuilder
    {
        private PSOSolverConfig _config = new PSOSolverConfig();

        public static PSOSolverConfigBuilder Init() => new PSOSolverConfigBuilder();
        public PSOSolverConfig Build() => _config;

        public PSOSolverConfigBuilder WithNumDimensions(int numDimentions)
        {
            _config.NumDimensions = numDimentions;
            return this;
        }

        public PSOSolverConfigBuilder WithNumParticles(int numParticles)
        {
            _config.NumParticles = numParticles;
            return this;
        }

        public PSOSolverConfigBuilder WithMaxEpochs(int maxEpochs)
        {
            _config.MaxEpochs = maxEpochs;
            return this;
        }

        public PSOSolverConfigBuilder WithAcceptanceError(double acceptanceError)
        {
            _config.AcceptanceError = acceptanceError;
            return this;
        }


        public PSOSolverConfigBuilder WithLowerBound(double[] lowerBound)
        {
            _config.LowerBound = lowerBound;
            return this;
        }


        public PSOSolverConfigBuilder WithUpperBound(double[] upperBound)
        {
            _config.UpperBound = upperBound;
            return this;
        }

        public PSOSolverConfigBuilder WithInertiaWeight(double inertiaWeight)
        {
            _config.InertiaWeight = inertiaWeight;
            return this;
        }


        public PSOSolverConfigBuilder WithC1CognitiveWeight(double c1CognitiveWeight)
        {
            _config.C1CognitiveWeight = c1CognitiveWeight;
            return this;
        }

        public PSOSolverConfigBuilder WithC2SocialWeight(double c2SocialWeight)
        {
            _config.C2SocialWeight = c2SocialWeight;
            return this;
        }

        public PSOSolverConfigBuilder WithVelocityInitialAttenuation(double velocityInitialAttenuation)
        {
            _config.VelocityInitialAttenuation = velocityInitialAttenuation;
            return this;
        }

        public PSOSolverConfigBuilder WithParticleResetProbability(double particleResetProbability)
        {
            _config.ParticleResetProbability = particleResetProbability;
            return this;
        }


        public PSOSolverConfigBuilder WithRandomSeed(int randomSeed)
        {
            _config.RandomSeed = randomSeed;
            return this;
        }

        public PSOSolverConfigBuilder WithRandomEngine(IRandomEngine re)
        {
            _config.RandomEngine = re;
            return this;
        }

        public PSOSolverConfigBuilder WithStoppingCriteriaEnabled(bool canKeepSolutionsHistory)
        {
            _config.IsStoppingCriteriaEnabled = canKeepSolutionsHistory;
            return this;
        }

    }
}