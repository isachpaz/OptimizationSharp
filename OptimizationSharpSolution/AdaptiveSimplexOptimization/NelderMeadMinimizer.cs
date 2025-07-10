using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Optimization;
using System.Threading.Tasks;

namespace AdaptiveSimplexOptimization
{
    public class NelderMeadMinimizer
    {
        private readonly ILogger<NelderMeadMinimizer> _logger;
        private readonly Func<Vector<double>, double> _objectiveFunction;
        private readonly ConcurrentBag<(Vector<double> Point, double Value, MinimizationResult MinimizationResult)> _solutions;
        private readonly List<(double Min, double Max)> _bounds;
        private readonly double _convergenceTolerance;
        private readonly int _maximumIterations;
        private readonly double _perturbationScale;

        public NelderMeadMinimizer(
            Func<Vector<double>, double> objectiveFunction,
            List<(double Min, double Max)> bounds,
            double convergenceTolerance = 1e-06,
            int maximumIterations = 10000,
            double perturbationScale = 0.05,
            ILogger<NelderMeadMinimizer> logger = null)
        {
            _objectiveFunction = objectiveFunction ?? throw new ArgumentNullException(nameof(objectiveFunction));
            _bounds = bounds ?? throw new ArgumentNullException(nameof(bounds));

            foreach (var bound in _bounds)
            {
                if (bound.Min > bound.Max)
                {
                    throw new ArgumentException("Each bound's Min value must be less than or equal to its Max value.");
                }
            }

            _solutions = new ConcurrentBag<(Vector<double> Point, double Value, MinimizationResult MinimizationResult)>();
            _convergenceTolerance = convergenceTolerance;
            _maximumIterations = maximumIterations;
            _perturbationScale = perturbationScale;
            _logger = logger;

            _logger?.LogInformation("NelderMeadMinimizer initialized successfully.");
        }

        public Vector<double> Minimize(Vector<double> initialGuess)
        {
            return MinimizeInternal(initialGuess, null);
        }

        public Vector<double> Minimize(Vector<double> initialGuess, Vector<double> perturbation)
        {
            return MinimizeInternal(initialGuess, perturbation);
        }

        public MinimizationResult MinimizeMultiple(IEnumerable<Vector<double>> initialGuesses)
        {
            foreach (var guess in initialGuesses)
            {
                Minimize(guess);
            }
            return GetBestSolution();
        }

        public MinimizationResult MinimizeMultipleWithPerturbations(int numInitialGuesses)
        {
            var initialGuesses = HammersleySequence.GeneratePoints(numInitialGuesses, _bounds);

            foreach (var guess in initialGuesses)
            {
                var perturbation = CreatePerturbationVector();
                Minimize(guess, perturbation);
            }

            return GetBestSolution();
        }
        public MinimizationResult MinimizeMultipleWithPerturbationsInParallel(int numInitialGuesses)
        {
            var initialGuesses = HammersleySequence.GeneratePoints(numInitialGuesses, _bounds);
            var perturbation = CreatePerturbationVector();

            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            Parallel.ForEach(initialGuesses, options, guess =>
            {
                Minimize(guess, perturbation);
            });

            return GetBestSolution();
        }

        private Vector<double> MinimizeInternal(Vector<double> initialGuess, Vector<double> perturbation)
        {
            if (perturbation != null && initialGuess.Count != perturbation.Count)
            {
                throw new ArgumentException("Initial guess and perturbation must have the same number of dimensions.");
            }

            var initialPoint = new DenseVector(initialGuess.ToArray());
            MinimizationResult result = new MinimizationResult(ObjectiveFunction.Value(_objectiveFunction), _maximumIterations,ExitCondition.ExceedIterations);
            
            try
            {
                result = perturbation == null
                    ? new NelderMeadSimplex(_convergenceTolerance, _maximumIterations)
                        .FindMinimum(ObjectiveFunction.Value(_objectiveFunction), initialPoint)
                    : NelderMeadSimplex.Minimum(
                        ObjectiveFunction.Value(_objectiveFunction),
                        initialPoint,
                        new DenseVector(perturbation.ToArray()),
                        _convergenceTolerance,
                        _maximumIterations);

                _solutions.Add((result.MinimizingPoint, _objectiveFunction(result.MinimizingPoint), result));

                _logger?.LogInformation(
                    $"Optimization succeeded: {result.ReasonForExit}, Iterations: {result.Iterations}, Function value: {result.FunctionInfoAtMinimum.Value}, At point: {string.Join(", ", result.MinimizingPoint.ToArray())}");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error during optimization.");
            }

            return result.MinimizingPoint;
        }

        private MinimizationResult GetBestSolution()
        {
            if (_solutions.IsEmpty)
            {
                _logger?.LogWarning("No solutions have been found.");
                throw new InvalidOperationException("No solutions have been found.");
            }

            var bestSolution = _solutions.Aggregate((best, current) =>
                current.Value < best.Value ? current : best);

            _logger?.LogInformation($"Best solution found with value {bestSolution.Value}.");

            return bestSolution.MinimizationResult;
        }

        private Vector<double> CreatePerturbationVector()
        {
            var perturbation = new double[_bounds.Count];
            for (int i = 0; i < _bounds.Count; i++)
            {
                perturbation[i] = (_bounds[i].Max - _bounds[i].Min) * _perturbationScale;
            }

            _logger?.LogDebug($"Perturbation vector created: {string.Join(", ", perturbation)}");
            return Vector<double>.Build.Dense(perturbation);
        }
    }
}
