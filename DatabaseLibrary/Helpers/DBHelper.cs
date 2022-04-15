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
            return isNotAlpha(true, args);
        }

        protected static bool isNotAlpha(bool allowSpaces, params string[] args)
        {
            foreach (string arg in args)
                if (string.IsNullOrEmpty(arg) || Regex.IsMatch(arg, "([^A-Za-z&/" + (allowSpaces ? "\\s" : "") + "])+"))
                    return true;

            return false;
        }
        protected static bool isNotAlphaNumeric(params string[] args)
        {
            return isNotAlphaNumeric(true, args);
        }

        protected static bool isNotAlphaNumeric(bool allowSpaces, params string[] args)
        {
            foreach (string arg in args)
                if (string.IsNullOrEmpty(arg) || Regex.IsMatch(arg, "[^\\w" + (allowSpaces ? "\\s" : "") + "]+"))
                    return true;

            return false;
        }

        protected static bool isValidColor(string color)
        {
            return Regex.IsMatch(color, "#[a-z0-9]{6}");
        }

        // Reference: https://www.codeproject.com/Tips/483763/Equivalent-function-of-mysql-real-escape-string-in
        protected static string MySQLEscape(string str)
        {
            return Regex.Replace(str, @"[\x00'""\b\n\r\t\cZ\\%_]",
                delegate (Match match)
                {
                    string v = match.Value;
                    switch (v)
                    {
                        case "\x00":            // ASCII NUL (0x00) character
                            return "\\0";
                        case "\b":              // BACKSPACE character
                            return "\\b";
                        case "\n":              // NEWLINE (linefeed) character
                            return "\\n";
                        case "\r":              // CARRIAGE RETURN character
                            return "\\r";
                        case "\t":              // TAB
                            return "\\t";
                        case "\u001A":          // Ctrl-Z
                            return "\\Z";
                        default:
                            return "\\" + v;
                    }
                });
        }
    }
}
