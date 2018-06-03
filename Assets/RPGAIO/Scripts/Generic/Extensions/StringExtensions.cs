using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicSpawn.RPGMaker.Generic
{
    public static class StringExtensions
    {
        public static string OxbridgeAnd(this IEnumerable<String> value, string seperator, string finalDelimeter)
        {
            string output = string.Empty;
            var enumerable = value as List<string> ?? value.ToList();
            List<string> input = enumerable.ToList();
            if (input.Count > 2)
            {
                string delimited = string.Join(seperator, input.Take(input.Count - 1).ToArray());
                output = string.Concat(delimited, finalDelimeter, input.LastOrDefault());
            } 
            else if (input.Count == 2)
            {
                output = string.Concat(input.FirstOrDefault(), finalDelimeter, input.LastOrDefault());
            } 
            else
            {
                output = enumerable.FirstOrDefault();
            } 
            return output;
        }
    }
}