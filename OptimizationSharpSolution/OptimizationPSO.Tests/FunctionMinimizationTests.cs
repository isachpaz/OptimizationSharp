using System;
using NUnit.Framework;
using OptimizationPSO.StoppingCriteria;
using OptimizationPSO.Swarm;

namespace OptimizationPSO.Tests
{
    public class FunctionMinimizationTests
    {

        public void Test1()
        {

        }

        [Test]
        public void Square2DFunctionMinimization_Test()
        {
            var solverConfig = PSOSolverConfig.CreateDefault(
                numberParticles: 500,
                maxEpochs: 1000,
                seed: 100,
                lowerBound: new double[] {-20, -20},
                upperBound: new double[] {10, 10},
                isStoppingCriteriaEnabled: true);
            
            solverConfig.StoppingCriteria.Add(new StandardDeviationOfNBestSolutions(acceptanceError:1E-12));

            Func<double[], double> func = x => 100.0 + ((x[0]-10.0) * (x[0]-10.0) + x[1] * x[1]);

            //var solver = new ParticleSwarmMinimizationNelderMead(func, solverConfig, NMSolverConfig.Default());
            var solver = new ParticleSwarmMaximization(func, solverConfig);


            solver.OnAfterEpoch += (s, d) =>
            {
                Console.WriteLine($"Iteration: {d.Iteration}\n" +
                                  $"Error function: {d.BestFitness:##.000} \n" +
                                  $"Best global position: " +
                                  $"x1: {d.BestPosition[0]:##.000}" +
                                  $"x2: {d.BestPosition[1]:##.000}");
            };

            var result = solver.Solve();
            Assert.That(result.BestPosition[0], Is.EqualTo(10).Within(1E-6));
            Assert.That(result.BestPosition[1], Is.EqualTo(0).Within(1E-6));
            

        }
    }
}