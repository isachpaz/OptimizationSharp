using System;
using System.Diagnostics;
using System.Xml.Serialization;
using NUnit.Framework;

namespace OptimizationPSO.Tests
{
    [TestFixture]
    public class ParticleSwarmTests
    {
        [Test]
        public void Optimizer_Will_Stop_If_Accepted_Tolerance_Achieved_Test()
        {
            Func<double[], double> func = (x) =>100.0 + (x[0] * x[0] + x[1] * x[1]);
            PSOSolverConfig conf = PSOSolverConfig.CreateDefault(
                numberParticles: 50,
                maxEpochs: 100,
                lowerBound: new double[] {-10, -10},
                upperBound: new double[] {10, 10},
                acceptanceError: 1E-16,
                isStoppingCriteriaEnabled: true);
            ParticleSwarm ps = new ParticleSwarmMinimization(func, conf);
            var result = ps.Solve();
            Assert.Less(result.Iteration, 100);
        }
    }
}