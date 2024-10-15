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
            var lowerBound = new double[] { -10.0, -11.0, -20.0 };
            var upperBound = new double[] { 10.0, 11.0, 20.0 };

            var config = PSOSolverConfig.CreateDefault(numberParticles: 100,
                maxEpochs: 20,
                lowerBound: new double[] { -10.0, -11.0, -20.0 },
                upperBound: new double[] { 10.0, 11.0, 20.0 });

            Assert.That(config.NumDimensions, Is.EqualTo(upperBound.Length));
            Assert.That(config.NumParticles, Is.EqualTo(100));
            Assert.That(config.MaxEpochs, Is.EqualTo(20));
            Assert.That(config.AcceptanceError, Is.EqualTo(1E-09));
            Assert.That(config.InertiaWeight, Is.EqualTo(0.729));
            Assert.That(config.C1CognitiveWeight, Is.EqualTo(1.49445));
            Assert.That(config.C2SocialWeight, Is.EqualTo(1.49445));
            Assert.That(config.VelocityInitialAttenuation, Is.EqualTo(0.1));
            Assert.That(config.ParticleResetProbability, Is.EqualTo(0.001));
        }

        [Test]
        public void PSOSolverConfig_ThrowsException_If_Lower_Upper_Bound_DoNot_Match_Test()
        {
            Assert.Throws<ArgumentException>(() => PSOSolverConfig.CreateDefault(numberParticles: 100,
                maxEpochs: 20,
                lowerBound: new double[] { -10.0, -11.0, -20.0 },
                upperBound: new double[] { 10.0, 11.0 })
            );
        }
    }
}