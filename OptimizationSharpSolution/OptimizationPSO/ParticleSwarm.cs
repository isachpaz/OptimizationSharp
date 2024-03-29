﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using MathNet.Numerics.LinearAlgebra.Solvers;
using OptimizationPSO.RandomEngines;
using OptimizationPSO.StoppingCriteria;

namespace OptimizationPSO
{
    using System;
    using System.Threading.Tasks;

    public delegate void EpochDelegate(ParticleSwarm sender, PSOResult result);

    public abstract class ParticleSwarm
    {
        public PSOSolverConfig Config { get; }
        public Action<Particle> UpdateParticlePositionFunc { get; }
        public event EpochDelegate OnAfterEpoch;
        protected readonly object _lock = new object();
        protected bool IsStoppingCriteriaEnabled { get; set; }
        protected List<PSOResult> SolutionsHistory { get; } = new List<PSOResult>();

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

        protected List<BaseStoppingCriterium> StoppingCretiria { get; set; } = new List<BaseStoppingCriterium>();

        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ParticleSwarmOptimization.ParticleSwarm"/> class.
        /// The particle swarm maximizes the particle fitness.
        /// </summary>
        /// <param name="config">PSOSolverConfig .</param>
        /// <param name="evalFunc">Evaluation function which takes the particle positions and returns its fitness.</param>
        /// <param name="randomEngine"></param>
        protected ParticleSwarm(Func<double[], double> evalFunc,
            PSOSolverConfig config,
            IRandomEngine randomEngine,
            Action<Particle> updateParticlePositionFunc)
        {
            if (config.LowerBound.Length != config.UpperBound.Length)
                throw new ArgumentException("Dimensions of lower and upper bound do not match");

            _random = randomEngine;
            Config = config;
            UpdateParticlePositionFunc = updateParticlePositionFunc;
            this.FitnessFunc = evalFunc;
            this.IsStoppingCriteriaEnabled = config.IsStoppingCriteriaEnabled;

            StoppingCretiria.Add(
                new AcceptanceErrorLessThanErrorInLast10Solutions(SolutionsHistory,
                    config.AcceptanceError));
        }


        protected abstract void Initialize();

        protected double NextDoubleInRange(double min, double max)
        {
            return _random.NextDouble() * (max - min) + min;
        }

        private int ElapsedEpochs { get; set; } = 0;

        /// <summary>
        /// Step the particle swarm for a given number of steps.
        /// </summary>
        /// <param name="epochs">Maximum number of steps.</param>
        /// <param name="stepFunc">Step function. Takes current iteration counter and returns true when the stepping should be aborted.</param>
        private void Step(int epochs, Func<int, bool> stepFunc)
        {
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                ++ElapsedEpochs;
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

                if (IsStoppingCriteriaEnabled)
                {
                    CopySolutionToHistory(epoch: epoch,
                        bestFitness: this.BestFitness,
                        bestPosition: this.BestPosition.DeepCopy());
                }

                if (stepFunc(epoch))
                    break;

                OnAfterEpoch?.Invoke(this, new PSOResult()
                {
                    BestFitness = this.BestFitness,
                    BestPosition = this.BestPosition.DeepCopy(),
                    Success = true,
                    Iteration = this.ElapsedEpochs,
                });
            }
        }

        private void CopySolutionToHistory(int epoch, double bestFitness, double[] bestPosition)
        {
            this.SolutionsHistory.Add(new PSOResult()
            {
                BestFitness = bestFitness,
                BestPosition = bestPosition.DeepCopy(),
                Success = true,
                Iteration = epoch,
            });
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

                UpdateParticlePositionFunc?.Invoke(p);

            }
        }

        

        public PSOResult Solve()
        {
            SolutionsHistory.Clear();

            Initialize();
            this.Step(Config.MaxEpochs,
                i =>
                {
                    Debug.WriteLine($"Math.Abs(BestFitness):{Math.Abs(BestFitness)}" +
                                    $"Config.AcceptanceError: {Config.AcceptanceError}");
                    return CanStop();
                });

            return new PSOResult()
            {
                BestFitness = this.BestFitness,
                BestPosition = this.BestPosition.DeepCopy(),
                Success = true,
                Iteration = this.ElapsedEpochs,
            };
        }

        private bool CanStop()
        {
            return IsStoppingCriteriaEnabled && StoppingCretiria.TrueForAll(x=>x.CanStop());
        }
    }
}