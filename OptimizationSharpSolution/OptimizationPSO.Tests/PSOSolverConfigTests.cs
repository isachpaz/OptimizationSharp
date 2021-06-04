using System;
using NUnit.Framework;

namespace OptimizationPSO.Tests
{
    [TestFixture]
    public class PSOSolverConfigTests
    {
        [Test]
        public void PSOSolverConfig_CreateDefault_Test()
        {
            var lowerBound = new double[] {-10.0, -11.0, -20.0};
            var upperBound = new double[] {10.0, 11.0, 20.0};

            var config = PSOSolverConfig.CreateDefault(numberParticles: 100,
                maxEpochs: 20,
                lowerBound: new double[] {-10.0, -11.0, -20.0},
                upperBound: new double[] {10.0, 11.0, 20.0});

            Assert.AreEqual(config.NumDimensions, upperBound.Length);
            Assert.AreEqual(config.NumParticles, 100);
            Assert.AreEqual(config.MaxEpochs, 20);
            Assert.AreEqual(config.AcceptanceError, 1E-09);
            Assert.AreEqual(config.InertiaWeight, 0.729);
            Assert.AreEqual(config.C1CognitiveWeight, 1.49445);
            Assert.AreEqual(config.C2SocialWeight, 1.49445);
            Assert.AreEqual(config.VelocityInitialAttenuation, 0.1);
            Assert.AreEqual(config.ParticleResetProbability, 0.001);
        }

        [Test]
        public void PSOSolverConfig_ThrowsException_If_Lower_Upper_Bound_DoNot_Match_Test()
        {
            Assert.Throws<ArgumentException>(() => PSOSolverConfig.CreateDefault(numberParticles: 100,
                maxEpochs: 20,
                lowerBound: new double[] {-10.0, -11.0, -20.0},
                upperBound: new double[] {10.0, 11.0})
            );
        }
    }
}