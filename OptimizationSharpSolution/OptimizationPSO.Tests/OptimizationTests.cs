using System;
using System.Linq;
using NUnit.Framework;

namespace OptimizationPSO.Tests
{
    [TestFixture]
    public class OptimizationTests
    {
        [Test]
        public void SphereFunctionTest()
        {
            // https://en.wikipedia.org/wiki/Test_functions_for_optimization
            // Global minimum x(i)=0.0 for i=0,1,2. Search domain: -Inf <= x <= Inf
            Func<double[], double> sphereFunc = x => x[0] * x[0] + x[1] * x[1] + x[2] * x[2];
            
        }


        [Test]
        public void AckleyFunctionTest()
        {
            // https://en.wikipedia.org/wiki/Ackley_function
            // Global minimum x(i) = 0.0 for i=0,1. Search domain: -5 <= x <= 5
            Func<double[], double> sphereFunc = x => x[0] * x[0] + x[1] * x[1] + x[2] * x[2];
            


        }
    }
}