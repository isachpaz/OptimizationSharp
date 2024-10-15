using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Optimization;

namespace AdaptiveSimplexOptimization
{
    public class NelderMeadMinimizer
    {
        private readonly Func<Vector<double>, double> _objectiveFunction;
        private readonly NelderMeadSimplex _optimizer;
        private List<(Vector<double> Point, double Value, MinimizationResult MinimizationResult)> _solutions;
        
        // Define bounds as a list of tuples (Min, Max) for each dimension
        private readonly List<(double Min, double Max)> _bounds;

        public NelderMeadMinimizer(
            Func<Vector<double>, double> objectiveFunction,
            List<(double Min, double Max)> bounds = null,
            double convergenceTolerance = 1e-06,
            int maximumIterations = 10000)
        {
            _objectiveFunction = objectiveFunction;
            _optimizer = new NelderMeadSimplex(convergenceTolerance, maximumIterations);
            _solutions = new List<(Vector<double> Point, double Value, MinimizationResult MinimizationResult)>();
            _bounds = bounds;
        }

        public Vector<double> Minimize(Vector<double> initialGuess)
        {
            var initialPoint = new DenseVector(initialGuess.ToArray());
            var result = _optimizer.FindMinimum(ObjectiveFunction.Value(_objectiveFunction), initialPoint);
            _solutions.Add((result.MinimizingPoint, _objectiveFunction(result.MinimizingPoint), result));
            return result.MinimizingPoint;
        }

        public MinimizationResult MinimizeMultiple(int numInitialGuesses)
        {
            var initialGuesses = HammersleySequence.GeneratePoints(numInitialGuesses, _bounds);
            initialGuesses.ForEach(x =>
            {
                Debug.WriteLine($"{string.Join(",", x.ToArray())}");
            });

            foreach (var guess in initialGuesses)
            {
                Minimize(guess);
            }
            return GetBestSolution();
        }

        public MinimizationResult MinimizeMultiple(IEnumerable<Vector<double>> initialGuesses)
        {
            foreach (var guess in initialGuesses)
            {
                Minimize(guess);
            }
            return GetBestSolution();
        }

        private MinimizationResult GetBestSolution()
        {
            if (_solutions.Count == 0)
            {
                throw new InvalidOperationException("No solutions have been found.");
            }

            // Find the best solution (the one with the lowest objective function value)
            var bestSolution = _solutions[0];
            foreach (var solution in _solutions)
            {
                if (solution.Value < bestSolution.Value)
                {
                    bestSolution = solution;
                }
            }

            return bestSolution.MinimizationResult;
        }

        public Vector<double> Minimize(Vector<double> initialGuess, Vector<double> perturbation)
        {
            // Ensure initial guess and perturbation are within the correct dimensions
            if (initialGuess.Count != perturbation.Count)
                throw new ArgumentException("Initial guess and perturbation must have the same number of dimensions.");

            var initialPoint = new DenseVector(initialGuess.ToArray());
            var perturbationVector = new DenseVector(perturbation.ToArray());

            // Use Math.NET's Nelder-Mead optimizer with perturbation
            var result = NelderMeadSimplex.Minimum(
                ObjectiveFunction.Value(_objectiveFunction),
                initialPoint,
                perturbationVector);

            // Store the result
            _solutions.Add((result.MinimizingPoint, _objectiveFunction(result.MinimizingPoint), result));
            Debug.WriteLine($"{result.ReasonForExit},  Iterations: {result.Iterations}, Function value: {result.FunctionInfoAtMinimum.Value}, At point: {string.Join(", ",result.MinimizingPoint.ToArray())}");
            return result.MinimizingPoint;
        }

        public MinimizationResult MinimizeMultipleWithPerturbations(int numInitialGuesses)
        {
            var initialGuesses = HammersleySequence.GeneratePoints(numInitialGuesses, _bounds);

            foreach (var guess in initialGuesses)
            {
                // Create a perturbation vector proportional to the size of the bounds
                var perturbation = CreatePerturbationVector();

                Minimize(guess, perturbation);
            }
            return GetBestSolution();
        }

        private Vector<double> CreatePerturbationVector()
        {
            // Generate a perturbation vector proportional to the size of the bounds
            var perturbation = new double[_bounds.Count];
            for (int i = 0; i < _bounds.Count; i++)
            {
                perturbation[i] = (_bounds[i].Max - _bounds[i].Min) * 0.05; // 5% of the range for perturbation
            }
            return Vector<double>.Build.Dense(perturbation);
        }

    }
}
