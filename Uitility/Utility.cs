using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Data;
using System.Text.RegularExpressions;
using System;
using Microsoft.Data.SqlClient;

namespace Tenant.Query.Uitility
{
    public class Utility
    {
        /// <summary>
        /// Helper class to prepare parameter for sp. 
        /// </summary>
        /// <param name="paratmeterName"></param>
        /// <param name="paremeterType"></param>
        /// <param name="typeName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SqlParameter PrepareParametersForStoreProcedure(string paratmeterName, SqlDbType paremeterType, String typeName, object value)
        {
            return new SqlParameter
            {
                ParameterName = paratmeterName,
                SqlDbType = paremeterType,
                TypeName = typeName,
                Value = value
            };
        }

        #region public variables

        public static ValueConverter longToStringConverter = new ValueConverter<string, long>(
        v => long.Parse(v),
        v => v.ToString());
        #endregion

        public static double FindBestMatch(string s1, string s2)
        {
            s1 = Regex.Replace(s1, @"\s+", " ").ToUpper();
            s2 = Regex.Replace(s2, @"\s+", " ").ToUpper();

            double valPhrase = Calculate(s1, s2) - 0.8 * Math.Abs(s2.Length - s1.Length);
            double valword = valuewords(s1, s2);
            return Math.Min(valPhrase, valword) * 0.8 + Math.Max(valPhrase, valword) * 0.2;
        }

        static int valuewords(string s1, string s2)
        {
            string[] Arr1 = s1.Split(' ');
            string[] Arr2 = s2.Split(' ');

            int wordstotal = 0;
            foreach (var item1 in Arr1)
            {
                int wordbest = s2.Length;
                foreach (var item2 in Arr2)
                {
                    int dist = Calculate(item1, item2);
                    wordbest = dist < wordbest ? dist : wordbest;
                    if (dist == 0)
                    {
                        wordstotal += wordbest;
                        break;
                    }
                }
                wordstotal += wordbest;
            }
            return wordstotal;
        }

        static int Calculate(string source1, string source2) //O(n*m)
        {
            var source1Length = source1.Length;
            var source2Length = source2.Length;

            var matrix = new int[source1Length + 1, source2Length + 1];

            // First calculation, if one entry is empty return full length
            if (source1Length == 0)
                return source2Length;

            if (source2Length == 0)
                return source1Length;

            // Initialization of matrix with row size source1Length and columns size source2Length
            for (var i = 0; i <= source1Length; matrix[i, 0] = i++) { }
            for (var j = 0; j <= source2Length; matrix[0, j] = j++) { }

            // Calculate rows and collumns distances
            for (var i = 1; i <= source1Length; i++)
            {
                for (var j = 1; j <= source2Length; j++)
                {
                    var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }
            // return result
            return matrix[source1Length, source2Length];
        }
    }
}
