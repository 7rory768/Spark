using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DatabaseLibrary.Helpers
{
    public class DBHelper
    {
        protected static bool isNotAlpha(params string[] args)
        {
            foreach (string arg in args)
                if (string.IsNullOrEmpty(arg) || Regex.IsMatch(arg, "([^A-Za-z&/])+"))
                    return true;

            return false;
        }
        protected static bool isNotAlphaNumeric(params string[] args)
        {
            foreach (string arg in args)
                if (string.IsNullOrEmpty(arg) || Regex.IsMatch(arg, "[^\\w]+"))
                    return true;

            return false;
        }

        protected static bool isValidColor(string color)
        {
            return Regex.IsMatch(color, "#[a-z0-9]{6}");
        }
    }
}
