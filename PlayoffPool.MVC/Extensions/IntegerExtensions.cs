using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;

namespace PlayoffPool.MVC.Extensions
{
    public static class IntegerExtensions
    {
        public static string ToOrdinal(this int number)
        {
            var numberAsString = number.ToString();
            // Negative and zero have no ordinal representation
            if (number < 1)
            {
                return numberAsString;
            }
            number %= 100;
            if ((number >= 11) && (number <= 13))
            {
                return numberAsString + "th";
            }
            switch (number % 10)
            {
                case 1: return numberAsString + "st";
                case 2: return numberAsString + "nd";
                case 3: return numberAsString + "rd";
                default: return numberAsString + "th";
            }
        }
    }
}
