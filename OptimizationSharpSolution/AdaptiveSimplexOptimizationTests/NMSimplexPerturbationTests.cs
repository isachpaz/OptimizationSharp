using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaptiveSimplexOptimization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using NUnit.Framework;

namespace AdaptiveSimplexOptimizationTests
{
    [TestFixture]
    public class NMSimplexPerturbationTests
    {
        [Test]
        public void Minimize2DQuadraticFunctionTest()
        {
            // Define the objective function to minimize
            Func<Vector<double>, double> objectiveFunction = v => Math.Pow(v[0] - 3, 2) + Math.Pow(v[1] - 2, 2);

            // Create an objective function object compatible with Math.NET
            var mathNetObjective = ObjectiveFunction.Value(objectiveFunction);

            // Define the initial guess
            var initialGuess = Vector<double>.Build.DenseOfArray(new double[] { 0.0, 0.0 });

            // Define the initial perturbation (perturbation size for each dimension)
            var initialPerturbation = Vector<double>.Build.DenseOfArray(new double[] { 0.00001, 0.00000110 });

            // Perform the minimization using Math.NET's Nelder-Mead optimizer
            var result = NelderMeadSimplex.Minimum(
                mathNetObjective,
                initialGuess,
                initialPerturbation,
                convergenceTolerance: 1e-8,
                maximumIterations: 1000);

            // Print the result
            Console.WriteLine($"Minimized at: ({result.MinimizingPoint[0]}, {result.MinimizingPoint[1]})");
            Console.WriteLine($"Minimum value: {result.FunctionInfoAtMinimum.Value}");
        }
        
        [Test]
        public void Minimize2DQuadraticWithMultiInitialGuessesTest()
        {
            // Define the objective function
            Func<Vector<double>, double> objectiveFunction = v =>
                Math.Pow(v[0] - 3, 2) + Math.Pow(v[1] - 2, 2);

            // Define valid bounds
            var bounds = new List<(double Min, double Max)> { (-10, 10), (-10, 10) };

            // Initialize the Nelder-Mead minimizer with the function and bounds
            var minimizer = new NelderMeadMinimizer(objectiveFunction, bounds);

            // Define multiple initial guesses
            var initialGuesses = new List<Vector<double>>
            {
                Vector<double>.Build.DenseOfArray(new double[] { 0.0, 0.0 }),
                Vector<double>.Build.DenseOfArray(new double[] { 1.0, 1.0 }),
                Vector<double>.Build.DenseOfArray(new double[] { -1.0, -1.0 })
            };

            var bestSolution = minimizer.MinimizeMultiple(initialGuesses);

            Assert.That(bestSolution.MinimizingPoint[0], Is.EqualTo(3.0).Within(1e-6));
            Assert.That(bestSolution.MinimizingPoint[1], Is.EqualTo(2.0).Within(1e-6));
            
        } 
        
        [Test]
        public void Minimize2DQuadraticWithHammersleySequenceTest()
        {
            Func<Vector<double>, double> objectiveFunction = v => Math.Pow(v[0] - 3, 2) + Math.Pow(v[1] - 2, 2);

            var bounds = new List<(double Min, double Max)>
            {
                (0, 10), // Bounds for the first dimension
                (0, 10), // Bounds for the second dimension
            };

            var minimizer = new NelderMeadMinimizer(objectiveFunction, bounds);
            int numInitialGuesses = 10;

            var bestSolution = minimizer.MinimizeMultipleWithPerturbations(numInitialGuesses);

            Console.WriteLine($"Best solution found at: ({bestSolution.MinimizingPoint[0]}, {bestSolution.MinimizingPoint[1]})");
        }
    }
}
