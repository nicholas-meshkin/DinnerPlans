using System.Globalization;

namespace DinnerPlans.Shared.Util
{
    public static class Utils
    {

        public static double FlOzToMl = 29.5735;
        //TODO figure out what to do about all the different kinds of cups, apparently
        public static double CupToMl = 236.5882365;
        //TODO also differences in imperial vs us teaspoons / tablespoons
        public static double TspToMl = 4.92892;
        public static double TbspToMl = 14.7868;
        public static double OzToG = 28.3495;
        public static double LbToG = 453.592;

        //need to remove units before using this
        public static double? ParseAmount(string line)
        {
            try
            {
                if (!line.Any(c => char.IsDigit(c) || CharUnicodeInfo.GetNumericValue(c) != -1)) return 0;

                //for some reason thirds are not a special char
                //need 4ths and 8ths for jpgs
                //TODO figure out if need 16ths
                if (line.Contains("1/3"))
                {
                    line = line.Replace("1/3", ".33");
                }
                if (line.Contains("2/3"))
                {
                    line = line.Replace("2/3", ".66");
                }
                if (line.Contains("1/4"))
                {
                    line = line.Replace("1/4", ".25");
                }
                if (line.Contains("3/4"))
                {
                    line = line.Replace("3/4", ".75");
                }
                if (line.Contains("1/8"))
                {
                    line = line.Replace("1/8", ".125");
                }
                if (line.Contains("3/8"))
                {
                    line = line.Replace("3/8", ".375");
                }
                if (line.Contains("5/8"))
                {
                    line = line.Replace("5/8", ".625");
                }
                if (line.Contains("7/8"))
                {
                    line = line.Replace("7/8", ".775");
                }
                string numbers = new string(line.Where(c => char.IsDigit(c) || c.Equals('.') || CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.OtherNumber).ToArray());
                double number = 0.0;

                if (numbers.Any(a => CharUnicodeInfo.GetUnicodeCategory(a) == UnicodeCategory.OtherNumber))
                {
                    foreach (char d in numbers)
                    {
                        if (CharUnicodeInfo.GetUnicodeCategory(d) == UnicodeCategory.OtherNumber)
                            number += CharUnicodeInfo.GetNumericValue(d);
                        else number += double.Parse(d.ToString());
                    }
                }
                else
                {
                    number = double.Parse(numbers);
                }
                return number;
            }
            catch
            {
                //TODO figure out why I was returning null here before
                return 0;
            }

        }

        public static string? ParseUnit(string line)
        {
            try
            {
                switch (line)
                {
                    case var _ when string.IsNullOrEmpty(line):
                        return "UNIT";
                    case var _ when line.ToUpper().Contains("FL. OZ."):
                        return "FL. OZ.";
                    case var _ when line.ToUpper().Contains("FL OZ"):
                        return "FL OZ";
                    case var _ when line.ToUpper().Contains("FLUID OUNCES"):
                        return "FLUID OUNCES";
                    case var _ when line.ToUpper().Contains("FLUID OUNCE"):
                        return "FLUID OUNCE";
                    case var _ when line.ToUpper().Contains("OZ."):
                        return "OZ.";
                    case var _ when line.ToUpper().Contains("OZ "):
                        return "OZ";
                    case var _ when line.ToUpper().Contains("OUNCES"):
                        return "OUNCES";
                    case var _ when line.ToUpper().Contains("OUNCE"):
                        return "OUNCE";
                    case var _ when line.ToUpper().Contains("TSP."):
                        return "TSP.";
                    case var _ when line.ToUpper().Contains("TSP"):
                        return "TSP";
                    case var _ when line.ToUpper().Contains("TEASPOONS"):
                        return "TEASPOONS";
                    case var _ when line.ToUpper().Contains("TEASPOON"):
                        return "TEASPOON";
                    case var _ when line.ToUpper().Contains("TBSP."):
                        return "TBSP.";
                    case var _ when line.ToUpper().Contains("TBSP"):
                        return "TBSP";
                    case var _ when line.ToUpper().Contains("TABLESPOONS"):
                        return "TABLESPOONS";
                    case var _ when line.ToUpper().Contains("TABLESPOON"):
                        return "TABLESPOON";
                    case var _ when line.ToUpper().Contains(" CUP "):
                        return "CUP";
                    case var _ when line.ToUpper().Contains(" CUPS "):
                        return "CUPS";
                    //TODO metric, none of my recipies currently use it 

                    default: return "UNIT";
                }
            }
            catch
            {
                return "";
            }

        }

        public static string? GetStandardizedUnit(string line)
        {
            try
            {
                switch (line)
                {
                    case var _ when line.ToUpper().Contains("FL. OZ."):
                    case var _ when line.ToUpper().Contains("FL OZ"):
                    case var _ when line.ToUpper().Contains("FLUID OUNCES"):
                    case var _ when line.ToUpper().Contains("FLUID OUNCE"):
                        return "FL. OZ.";
                    case var _ when line.ToUpper().Contains("OZ."):
                    case var _ when line.ToUpper().Contains("OZ "):
                    case var _ when line.ToUpper().Contains("OUNCES"):
                    case var _ when line.ToUpper().Contains("OUNCE"):
                        return "OZ.";
                    case var _ when line.ToUpper().Contains("TSP."):
                    case var _ when line.ToUpper().Contains("TSP"):
                    case var _ when line.ToUpper().Contains("TEASPOONS"):
                    case var _ when line.ToUpper().Contains("TEASPOON"):
                        return "TSP.";
                    case var _ when line.ToUpper().Contains("TBSP."):
                    case var _ when line.ToUpper().Contains("TBSP"):
                    case var _ when line.ToUpper().Contains("TABLESPOONS"):
                    case var _ when line.ToUpper().Contains("TABLESPOON"):
                        return "TBSP.";
                    case var _ when line.ToUpper().Contains(" CUP "):
                    case var _ when line.ToUpper().Contains(" CUPS "):
                        return "CUP";
                    case var _ when line.ToUpper().Contains(" LB. "):
                    case var _ when line.ToUpper().Contains(" LB "):
                    case var _ when line.ToUpper().Contains(" LBS. "):
                    case var _ when line.ToUpper().Contains(" LBS "):
                    case var _ when line.ToUpper().Contains(" POUND "):
                    case var _ when line.ToUpper().Contains(" POUNDS "):
                        return "LB.";
                    //TODO metric, none of my recipies currently use it 

                    default: return "UNIT";
                }
            }
            catch
            {
                return "";
            }

        }
    }
}
