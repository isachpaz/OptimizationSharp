namespace OptimizationPSO.StoppingCriteria
{
    public abstract class BaseStoppingCriterion
    {
        public string Description { get; }
        public abstract bool CanStop();

        protected BaseStoppingCriterion(string description)
        {
            Description = description;
        }
    }
}