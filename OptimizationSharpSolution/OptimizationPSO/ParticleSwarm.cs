using OptimizationPSO.RandomEngines;

namespace OptimizationPSO
{
    using System;
    using System.Threading.Tasks;

    public delegate void EpochDelegate(ParticleSwarm sender, PSOResult result);

    public abstract class ParticleSwarm
    {
        public PSOSolverConfig Config { get; }
        public event EpochDelegate OnAfterEpoch;
        protected readonly object _lock = new object();

        // Particle swarm parameters.
        // https://en.wikipedia.org/wiki/Particle_swarm_optimization
        public double Omega { get; set; } = 0.729;
        public double Phi_G { get; set; } = 1.49445;
        public double Phi_P { get; set; } = 1.49445;

        /// <summary>
        /// Gets the best fitness of all Particles.
        /// </summary>
        /// <value>The best fitness.</value>
        public double BestFitness { get; protected set; }

        /// <summary>
        /// Gets the best position of all Particles.
        /// </summary>
        /// <value>The best position.</value>
        public double[] BestPosition { get; protected set; }
        private IRandomEngine _random;
        protected Particle[] Particles { get; set; }
        protected Func<double[], double> FitnessFunc { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ParticleSwarmOptimization.ParticleSwarm"/> class.
        /// The particle swarm maximizes the particle fitness.
        /// </summary>
        /// <param name="config">PSOSolverConfig .</param>
        /// <param name="evalFunc">Evaluation function which takes the particle positions and returns its fitness.</param>
        public ParticleSwarm(PSOSolverConfig config, Func<double[], double> evalFunc)
        {
            if (config.LowerBound.Length != config.UpperBound.Length)
                throw new ArgumentException("Dimensions of lower and upper bound do not match");

            
            Config = config;
            this.FitnessFunc = evalFunc;

        }


        protected abstract void Initialize();

        protected double NextDoubleInRange(double min, double max)
        {
            return _random.NextDouble() * (max - min) + min;
        }

        private int EpochsTillToSolution { get; set; } = 0;

        /// <summary>
        /// Step the particle swarm for a given number of steps.
        /// </summary>
        /// <param name="epochs">Maximum number of steps.</param>
        /// <param name="stepFunc">Step function. Takes current iteration counter and returns true when the stepping should be aborted.</param>
        private void Step(int epochs, Func<int, bool> stepFunc)
        {
            for (int l = 0; l < epochs; l++)
            {
                ++EpochsTillToSolution;
                foreach (var t in Particles)
                {
                    EvaluateParticle(t);
                    MoveParticle(t);
                }

                //Parallel.For(0, Particles.Length, i =>
                //{
                //    EvaluateParticle(Particles[i]);
                //    MoveParticle(Particles[i]);
                //});

                if (stepFunc(l))
                    break;
                if (OnAfterEpoch != null)
                    OnAfterEpoch(this, new PSOResult()
                    {
                        BestFitness = this.BestFitness, 
                        BestPosition = this.BestPosition.DeepCopy(), 
                        Success = true,
                        Iterations = this.EpochsTillToSolution,
                    });
            }
        }

        protected abstract void EvaluateParticle(Particle p);

        private void MoveParticle(Particle p)
        {
            for (int i = 0; i < p.position.Length; i++)
            {
                var rp = _random.NextDouble();
                var rg = _random.NextDouble();

                p.velocity[i] = Omega * p.velocity[i]
                                + Phi_P * rp * (p.bestPosition[i] - p.position[i])
                                + Phi_G * rg * (BestPosition[i] - p.position[i]);

                p.position[i] += p.velocity[i];

                if (p.position[i] > Config.UpperBound[i])
                    p.position[i] = Config.UpperBound[i];
                if (p.position[i] < Config.LowerBound[i])
                    p.position[i] = Config.LowerBound[i];
            }
        }

        public PSOResult Solve()
        {
            Initialize();
            this.Step(Config.MaxEpochs, i => Math.Abs(BestFitness) < Config.AcceptanceError);
            return new PSOResult()
            {
                BestFitness = this.BestFitness,
                BestPosition = this.BestPosition.DeepCopy(),
                Success = true,
                Iterations = this.EpochsTillToSolution,
            };
        }
    }
}