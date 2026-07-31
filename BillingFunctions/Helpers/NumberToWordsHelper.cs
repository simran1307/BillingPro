namespace BillingFunctions.Helpers;

public static class NumberToWordsHelper
{
    public static string Convert(int number)
    {
        if (number == 0)
            return "Zero";

        if (number < 0)
            return "Minus " + Convert(Math.Abs(number));

        string words = "";

        if ((number / 1000) > 0)
        {
            words += Convert(number / 1000) +
                     " Thousand ";

            number %= 1000;
        }

        if ((number / 100) > 0)
        {
            words += Convert(number / 100) +
                     " Hundred ";

            number %= 100;
        }

        if (number > 0)
        {
            if (words != "")
                words += "";

            string[] unitsMap =
            {
                "Zero","One","Two","Three",
                "Four","Five","Six","Seven",
                "Eight","Nine","Ten","Eleven",
                "Twelve","Thirteen","Fourteen",
                "Fifteen","Sixteen","Seventeen",
                "Eighteen","Nineteen"
            };

            string[] tensMap =
            {
                "Zero","Ten","Twenty",
                "Thirty","Forty",
                "Fifty","Sixty",
                "Seventy","Eighty",
                "Ninety"
            };

            if (number < 20)
            {
                words += unitsMap[number];
            }
            else
            {
                words +=
                    tensMap[number / 10];

                if ((number % 10) > 0)
                {
                    words += " " +
                             unitsMap[number % 10];
                }
            }
        }

        return words.Trim();
    }
}