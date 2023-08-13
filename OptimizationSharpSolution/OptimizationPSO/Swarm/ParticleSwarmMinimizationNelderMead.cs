using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Optimization;
using OptimizationPSO.Particles;

namespace OptimizationPSO.Swarm
{
    public class ParticleSwarmMinimizationNelderMead : ParticleSwarm
    {
        public ParticleSwarmMinimizationNelderMead(
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
            Array.Sort(Particles, new ParticleComparer());
        }

        protected override void RunNMOpt(int n)
        {
            var dimension = Particles.FirstOrDefault().bestPosition.Length;
            Vector<double> bestPosition = new DenseVector(Particles.FirstOrDefault()?.position);
            double bestFitness = Double.MaxValue;

            // For the first to n+1 particles, run NM
            for (int i = 0; i <= n + 1; i++)
            {
                var particle = Particles[i];
                var f1 = new Func<Vector<double>, double>(
                    x => FitnessFunc(x.ToArray()));
                var obj = ObjectiveFunction.Value(f1);

                var solver = new NelderMeadSimplex(1e-10, maximumIterations: 500);
                var initialGuess = new DenseVector(particle.bestPosition);

                var result = solver.FindMinimum(obj, initialGuess);
                var position = result.FunctionInfoAtMinimum.Point;
                var newBestValue = result.FunctionInfoAtMinimum.Value;

                if (newBestValue < bestFitness)
                {
                    bestFitness = newBestValue;
                    bestPosition = position;
                }

            }

            Particles[n + 1].bestFitness = bestFitness;
            Array.Copy(bestPosition.ToArray(), Particles[n + 1].bestPosition, bestPosition.Count);

            if (bestFitness < BestFitness)
            {
                BestFitness = bestFitness;
                Array.Copy(bestPosition.ToArray(), BestPosition, bestPosition.Count);
            }

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