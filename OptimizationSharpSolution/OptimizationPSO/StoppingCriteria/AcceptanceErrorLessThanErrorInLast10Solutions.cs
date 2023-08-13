using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;

namespace OptimizationPSO.StoppingCriteria
{
    public class AcceptanceErrorLessThanErrorInLast10Solutions : BaseStoppingCriterion
    {
        private readonly List<PSOResult> _solutionsHistory;
        private readonly double _acceptanceError;

        public AcceptanceErrorLessThanErrorInLast10Solutions(List<PSOResult> solutionsHistory, double acceptanceError)
            : base("AcceptanceError is less than the error in last 10 calculated solutions")
        {
            _solutionsHistory = solutionsHistory ?? throw new ArgumentNullException(nameof(solutionsHistory));
            _acceptanceError = acceptanceError;
        }

        public override bool CanStop()
        {
            if (_solutionsHistory.Count < 10) return false;
            var last10Solutions = GetDeltasOfLast10Solutions();
            return last10Solutions.Average() < _acceptanceError;
        }

        private List<double> GetDeltasOfLast10Solutions()
        {
            var deltas = new List<double>();
            for (int i = _solutionsHistory.Count - 10; i < _solutionsHistory.Count - 2; i++)
            {
                var delta = _solutionsHistory[i].BestFitness - _solutionsHistory[i + 1].BestFitness;
                deltas.Add(delta);
            }
                
            return deltas;
        }
    }
}