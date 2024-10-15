using System;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Optimization;
using OptimizationPSO.Particles;

namespace OptimizationPSO.Swarm
{
    public class ParticleSwarmMinimizationNelderMead : ParticleSwarm
    {
        public NMSolverConfig NmConfig { get; }

        public ParticleSwarmMinimizationNelderMead(
            Func<double[], double> evalFunc,
            PSOSolverConfig psoConfig,
            NMSolverConfig nmConfig,
            Action<Particle> updateParticlePositionFunc = null)
            : base(evalFunc, psoConfig, psoConfig.RandomEngine, updateParticlePositionFunc)
        {
            NmConfig = nmConfig;
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

        protected override void RunNMOptAndMoveParticles(int n)
        {
            Console.WriteLine("RunNMOptAndMoveParticles");
            var dimension = Particles?.FirstOrDefault()?.bestPosition.Length;
            Vector<double> bestPosition = new DenseVector(Particles?.FirstOrDefault()?.position);
            double bestFitness = Double.MaxValue;

            if (Particles != null)
            {
                // For the first to n+1 particles, run NM
                foreach (var particle in Particles.Take(n + 2))
                {
                    var f1 = new Func<Vector<double>, double>(
                        x => FitnessFunc(x.ToArray()));
                    var obj = ObjectiveFunction.Value(f1);
                    
                    try
                    {
                        var solver = new NelderMeadSimplex(
                            NmConfig.ConvergenceTolerance, 
                            NmConfig.MaximumIterations);

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
                    catch (MaximumIterationsException e)
                    {
                        Trace.WriteLine(e.Message);
                    }
                    
                }

                Particles[n + 1].bestFitness = bestFitness;
                Particles[n + 1].fitness = bestFitness;

                Array.Copy(bestPosition.ToArray(), Particles[n + 1].bestPosition, bestPosition.Count);
                Array.Copy(bestPosition.ToArray(), Particles[n + 1].position, bestPosition.Count);

                if (bestFitness < BestFitness)
                {
                    BestFitness = bestFitness;
                    Array.Copy(bestPosition.ToArray(), BestPosition, bestPosition.Count);
                }

                Console.WriteLine("RunNMOptAndMoveParticles ended");
                foreach (var t in Particles)
                {
                    MoveParticle(t);
                }
            }
        }

        protected override void EvaluateParticle(Particle p)
        {
            p.fitness = FitnessFunc(p.position);

            if (p.fitness < p.bestFitness)
            {
                p.bestFitness = p.fitness;
                Array.Copy(p.position, p.bestPosition, p.position.Length);

                //lock (_lock)
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