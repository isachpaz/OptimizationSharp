using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Optimization;
using OptimizationPSO.Particles;

namespace OptimizationPSO.Swarm
{
    public class ParticleSwarmMinimization : ParticleSwarm
    {
        public ParticleSwarmMinimization(
            Func<double[], double> evalFunc, 
            PSOSolverConfig config,
            Action<Particle> updateParticlePositionFunc = null)
            : base(evalFunc, config, config.RandomEngine, updateParticlePositionFunc)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            var numDimensions = base.NumDimensions;
            var numParticles = base.NumParticles;
            BestFitness = double.MaxValue;
            BestPosition = new double[numDimensions];
            
            for (int i = 0; i < numParticles; i++)
            {
                var p = new ParticleMinimization(numDimensions);

                for (int j = 0; j < numDimensions; j++)
                {
                    var diff = Config.UpperBound[j] - Config.LowerBound[j];
                    p.position[j] = NextDoubleInRange(Config.LowerBound[j], Config.UpperBound[j]);
                    p.velocity[j] = NextDoubleInRange(-diff, +diff);
                }

                UpdateParticlePositionFunc?.Invoke(p);
                Particles[i] = p;
            }
        }

        protected override void SortParticles()
        {
           
        }

        protected override void RunNMOptAndMoveParticles(int n)
        {

        }

        protected override void EvaluateParticle(Particle p)
        {
            p.fitness = FitnessFunc(p.position);

            if (p.fitness < p.bestFitness)
            {
                p.bestFitness = p.fitness;
                Array.Copy(p.position, p.bestPosition, p.position.Length);

                lock (_lock)
                {
                    if (p.bestFitness < BestFitness)
                    {
                        BestFitness = p.bestFitness;
                        Array.Copy(p.bestPosition, BestPosition, p.bestPosition.Length);
                    }
                }
            }
        }
    }
}