namespace OptimizationPSO
{
    using System;
    using System.Threading.Tasks;

    public delegate void EpochDelegate(PSOSolver sender, PSOResult result);

    public class PSOSolver
    {
        public PSOSolverConfig Config { get; }
        public event EpochDelegate OnAfterEpoch;

        // Particle swarm parameters.
        // https://en.wikipedia.org/wiki/Particle_swarm_optimization
        public double Omega { get; set; } = 0.729;
        public double Phi_G { get; set; } = 1.49445;
        public double Phi_P { get; set; } = 1.49445;

        /// <summary>
        /// Gets the best fitness of all Particles.
        /// </summary>
        /// <value>The best fitness.</value>
        public double BestFitness { get; private set; }

        /// <summary>
        /// Gets the best position of all Particles.
        /// </summary>
        /// <value>The best position.</value>
        public double[] BestPosition { get; private set; }
        private Random _random = new Random();
        private Particle[] Particles { get; set; }
        private Func<double[], double> FitnessFunc { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ParticleSwarmOptimization.ParticleSwarm"/> class.
        /// The particle swarm maximizes the particle fitness.
        /// </summary>
        /// <param name="config">PSOSolverConfig .</param>
        /// <param name="evalFunc">Evaluation function which takes the particle positions and returns its fitness.</param>
        public PSOSolver(PSOSolverConfig config, Func<double[], double> evalFunc)
        {
            if (config.LowerBound.Length != config.UpperBound.Length)
                throw new ArgumentException("Dimensions of lower and upper bound do not match");

            Config = config;
            this.FitnessFunc = evalFunc;
        }


        private void Initialize()
        {
            var numDimensions = Config.LowerBound.Length;
            var numParticles = Config.NumParticles;

            BestFitness = -double.MaxValue;
            BestPosition = new double[numDimensions];
            Particles = new Particle[numParticles];

            for (int i = 0; i < numParticles; i++)
            {
                var p = new Particle(numDimensions);

                for (int j = 0; j < numDimensions; j++)
                {
                    var diff = Config.UpperBound[j] - Config.LowerBound[j];
                    p.position[j] = NextDoubleInRange(Config.LowerBound[j], Config.UpperBound[j]);
                    p.velocity[j] = NextDoubleInRange(-diff, +diff);
                }

                Particles[i] = p;
            }
        }

        private double NextDoubleInRange(double min, double max)
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
                        BestPosition = this.BestPosition.Clone2(), 
                        Success = true,
                        Iterations = this.EpochsTillToSolution,
                    });
            }
        }

        private void EvaluateParticle(Particle p)
        {
            p.fitness = FitnessFunc(p.position);

            if (p.fitness > p.bestFitness)
            {
                p.bestFitness = p.fitness;
                Array.Copy(p.position, p.bestPosition, p.position.Length);

                lock (this)
                {
                    if (p.bestFitness > BestFitness)
                    {
                        BestFitness = p.bestFitness;
                        Array.Copy(p.bestPosition, BestPosition, p.bestPosition.Length);
                    }
                }
            }
        }

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

        public PSOResult Minimize()
        {
            Initialize();
            this.Step(Config.MaxEpochs, i => Math.Abs(BestFitness) < Config.AcceptanceError);
            return new PSOResult()
            {
                BestFitness = this.BestFitness,
                BestPosition = this.BestPosition.Clone2(),
                Success = true,
                Iterations = this.EpochsTillToSolution,
            };
        }
    }
}