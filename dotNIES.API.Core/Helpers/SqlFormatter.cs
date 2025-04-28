using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace dotNIES.API.Core.Helpers;

public static class SqlFormatter
{
    /// <summary>
    /// Replace multiple spaces with a single space
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string SimplifySpaces(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return Regex.Replace(input, @"\s{2,}", " ");
    }
}
