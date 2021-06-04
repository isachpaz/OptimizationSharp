using System;
using NUnit.Framework;

namespace OptimizationPSO.Tests
{
    public class FunctionMinimizationTests
    {
        //[Test]
        //public void Square2DFunctionMinimization_Test()
        //{
        //    var solverConfig = PSOSolverConfig.CreateDefault(numberParticles: 50000,
        //        maxEpochs: 500,
        //        lowerBound: new double[] {-10, -10},
        //        upperBound: new double[] {10, 10},
        //        acceptanceError: 0.00000000);

        //    Func<double[], double> func = x => (x[0] * x[0] + x[1] * x[1]);

        //    var solver = new PSOSolver(func, solverConfig);

        //    solver.OnAfterEpoch += (s, d) =>
        //    {
        //        Console.WriteLine($"Error function: {d.BestGlobalFitness:##.000} \n" +
        //                          $"Best global position: " +
        //                          $"x1: {d.BestGlobalPosition[0]:##.000}" +
        //                          $"x2: {d.BestGlobalPosition[1]:##.000}");
        //    };

        //    var res = solver.Minimize();
        //    Assert.AreEqual(0, res.BestPosition[0], 1E-9);
        //    Assert.AreEqual(0, res.BestPosition[1], 1E-9);
        //}

        [Test]
        public void Opt_Func_Test()
        {
            var solverConfig = PSOSolverConfig.CreateDefault(
                numberParticles: 50000,
                maxEpochs: 500,
                lowerBound: new double[] {-10, -10},
                upperBound: new double[] {10, 10},
                acceptanceError: 1E-30);

            Func<double[], double> func = x => - (x[0] * x[0] + x[1] * x[1]);

            var solver = new PSOSolver(solverConfig, func);

            solver.OnAfterEpoch += (s, d) =>
            {
                Console.WriteLine($"Iteration: {d.Iterations}\n" +
                                  $"Error function: {d.BestFitness:##.000} \n" +
                                  $"Best global position: " +
                                  $"x1: {d.BestPosition[0]:##.000}" +
                                  $"x2: {d.BestPosition[1]:##.000}");
            };

            var result = solver.Minimize();
            Assert.AreEqual(0, result.BestPosition[0], 1E-9);
            Assert.AreEqual(0, result.BestPosition[1], 1E-9);


        }
    }
}