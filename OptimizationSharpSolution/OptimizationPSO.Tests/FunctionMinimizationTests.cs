using System;
using NUnit.Framework;

namespace OptimizationPSO.Tests
{
    public class FunctionMinimizationTests
    {
        [Test]
        public void Square2DFunctionMinimization_Test()
        {
            var solverConfig = PSOSolverConfig.CreateDefault(
                numberParticles: 500,
                maxEpochs: 1000,
                lowerBound: new double[] {-20, -20},
                upperBound: new double[] {10, 10},
                isStoppingCriteriaEnabled: false);

            Func<double[], double> func = x => 100.0 + ((x[0]-10.0) * (x[0]-10.0) + x[1] * x[1]);

            var solver = new ParticleSwarmMinimization(func, solverConfig);

            solver.OnAfterEpoch += (s, d) =>
            {
                Console.WriteLine($"Iteration: {d.Iteration}\n" +
                                  $"Error function: {d.BestFitness:##.000} \n" +
                                  $"Best global position: " +
                                  $"x1: {d.BestPosition[0]:##.000}" +
                                  $"x2: {d.BestPosition[1]:##.000}");
            };

            var result = solver.Solve();
            Assert.AreEqual(10, result.BestPosition[0], 1E-6);
            Assert.AreEqual(0, result.BestPosition[1], 1E-6);


        }
    }
}