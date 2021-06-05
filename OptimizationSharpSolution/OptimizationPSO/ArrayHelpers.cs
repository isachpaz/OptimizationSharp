namespace OptimizationPSO
{
    public static class ArrayHelpers
    {
        public static double[] DeepCopy(this double[] source)
        {
            var result = new double[source.Length];
            source.CopyTo(result, 0);
            return result;
        }
    }
}