namespace OptimizationPSO.StoppingCriteria
{
    public abstract class BaseStoppingCriterium
    {
        public string Description { get; }
        public abstract bool CanStop();

        protected BaseStoppingCriterium(string description)
        {
            Description = description;
        }
    }
}