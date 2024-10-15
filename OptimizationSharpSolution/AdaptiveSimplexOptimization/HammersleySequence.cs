using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace AdaptiveSimplexOptimization
{
    public class HammersleySequence
    {

        // Generate the Hammersley sequence for a given number of points, dimensions, and bounds
        public static List<Vector<double>> GeneratePoints(int numPoints, List<(double Min, double Max)> bounds)
        {
            int dimensions = bounds.Count; // Number of dimensions
            List<Vector<double>> points = new List<Vector<double>>();

            for (int i = 0; i < numPoints; i++)
            {
                double[] point = new double[dimensions];
                point[0] = (double)i / numPoints;  // The first dimension uses a simple fraction

                // Compute remaining dimensions using the radical inverse with different prime bases
                for (int j = 1; j < dimensions; j++)
                {
                    point[j] = RadicalInverse(i, GetPrime(j));
                }

                // Scale and shift points to fit within bounds
                for (int j = 0; j < dimensions; j++)
                {
                    double min = bounds[j].Min;
                    double max = bounds[j].Max;
                    point[j] = min + point[j] * (max - min);
                }

                points.Add(Vector<double>.Build.Dense(point));
            }

            return points;
        }

        // Generate the Hammersley sequence for a given number of points and 3 dimensions
        public static List<Vector<double>> Generate3DPoints(int numPoints)
        {
            int dimensions = 3; // Set to 3 for 3D points
            List<Vector<double>> points = new List<Vector<double>>();

            for (int i = 0; i < numPoints; i++)
            {
                double[] point = new double[dimensions];
                point[0] = (double)i / numPoints;  // The first dimension uses a simple fraction

                // Compute remaining dimensions using the radical inverse with different prime bases
                for (int j = 1; j < dimensions; j++)
                {
                    point[j] = RadicalInverse(i, GetPrime(j));
                }

                points.Add(Vector<double>.Build.Dense(point));
            }

            return points;
        }

        // Compute the radical inverse of an integer in a given base
        private static double RadicalInverse(int n, int baseValue)
        {
            double inverse = 0.0;
            double baseInv = 1.0 / baseValue;
            double basePow = baseInv;

            while (n > 0)
            {
                int digit = n % baseValue;
                inverse += digit * basePow;
                n /= baseValue;
                basePow *= baseInv;
            }

            return inverse;
        }

        // Get the nth prime number (0-indexed)
        private static int GetPrime(int index)
        {
            int[] primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71 };
            if (index < primes.Length)
                return primes[index];

            throw new ArgumentException("Prime index out of bounds. Increase the array size if needed.");
        }
    }
}