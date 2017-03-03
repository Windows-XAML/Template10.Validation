using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmSample.TypeExtentions
{
    public static class StringExtentions
    {
        public static string GetDiff(this string first, string second)
        {
            var firstCodes = DiffCharCodes(first);
            var secondCodes = DiffCharCodes(second);

            var results = my.utils.Diff.DiffInt(secondCodes, firstCodes);
            var delta = string.Empty;
            foreach (var result in results)
            {
                if (result.insertedB > 0)
                    delta += first.Substring(result.StartB, result.insertedB);
            }

            return delta;
        }


        private static int[] DiffCharCodes(string aText, bool ignoreCase = false)
        {
            if (ignoreCase)
                aText = aText.ToUpperInvariant();

            var codes = new int[aText.Length];

            for (int n = 0; n < aText.Length; n++)
                codes[n] = (int)aText[n];

            return (codes);
        }
    }
}
