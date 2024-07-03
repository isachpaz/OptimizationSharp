using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimization.Helpers
{
    public static class OptHelpers
    {

        public static List<double> DeepClone(this List<double> source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(List<double>);
            }

            return new List<double>(source);

        }

        public static List<List<double>> GetAllPossibleCombos(List<List<double>> data)
        {
            IEnumerable<List<double>> combos = new List<List<double>>() { new List<double>() };

            foreach (var inner in data)
            {
                combos = combos.SelectMany(r => inner
                    .Select(x => {
                        var n = r.DeepClone();
                        if (x != null)
                        {
                            n.Add(x);
                        }
                        return n;
                    }).ToList());
            }

            return combos.ToList();
        }
    }
}