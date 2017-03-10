using System.Linq;

namespace ObjectsComparer.Examples.Example2
{
    public class PhoneNumberComparer: AbstractValueComparer<string>
    {
        public override bool Compare(string obj1, string obj2, ComparisonSettings settings)
        {
            return ExtractDigits(obj1) == ExtractDigits(obj2);
        }

        private string ExtractDigits(string str)
        {
            return string.Join(
                string.Empty, 
                (str ?? string.Empty)
                    .ToCharArray()
                    .Where(char.IsDigit));
        }
    }
}
