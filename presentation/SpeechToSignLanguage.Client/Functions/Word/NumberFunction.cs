using System;
using System.Collections.Generic;
using System.Linq;

namespace SpeechToSignLanguage.Client.Functions.Word
{
    public class NumberFunction
    {
        public List<int> findDigitsOfaNumber(string number)
        {
            List<int> values = new List<int>();
            string[] digits = number.ToCharArray().Select(c => c.ToString()).ToArray();
            var len = digits.Length - 1;
            var val = digits[1];
            for (int i = 0; i < digits.Length; i++)
            {
                if (i == digits.Length - 2)
                {

                    if (Int32.Parse(digits[i] + digits[i + 1]) < 20)
                    {
                        values.Add(Int32.Parse(digits[i] + digits[i + 1]));
                        break;
                    }
                }
                int value = (int)(Int32.Parse(digits[i]) * Math.Pow(10, len - i));
                if (value > 0)
                {
                    values.Add(value);
                }
            }

            return values;
        }


        public List<int> numberdigits(int number)
        {
            List<int> digits = new List<int>();
            digits.Add(number % 10);
            return digits;
        }
    }
}
