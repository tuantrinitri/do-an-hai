using System.Text.RegularExpressions;
using System;
using System.Text;
using System.Diagnostics;
using System.Web;
using System.Linq;
using System.Collections.Generic;

namespace CMS.Helpers
{
    public static class StringExtensions
    {
        /// <summary>
        /// Remove spaces at begin, end and middle of string
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns></returns>
        public static string FullTrim(this string str)
        {
            return str != null ? Regex.Replace(str.Trim(), @"\s+", " ").ToString() : null;
        }
        /// <summary>
        /// Encode to Base64 format
        /// </summary>
        /// <param name="str">Input string to Encode</param>
        /// <returns></returns>
        public static string Base64Encode(this string str)
        {
            return str != null ? Convert.ToBase64String(Encoding.UTF8.GetBytes(str)) : null;
        }
        /// <summary>
        /// Decode from Base64 format
        /// </summary>
        /// <param name="str">Input string to Decode</param>
        /// <returns></returns>
        public static string Base64Decode(this string str)
        {
            return str != null ? Encoding.UTF8.GetString(Convert.FromBase64String(str)) : null;
        }
        public static string EncodePath(this string path)
        {
            return UrlTokenEncode(Encoding.UTF8.GetBytes(path));
        }
        private static string UrlTokenEncode(byte[] input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (input.Length < 1)
            {
                return string.Empty;
            }

            string base64Str = null;
            int endPos = 0;
            char[] base64Chars = null;

            ////////////////////////////////////////////////////////
            // Step 1: Do a Base64 encoding
            base64Str = Convert.ToBase64String(input);
            if (base64Str == null)
            {
                return null;
            }

            ////////////////////////////////////////////////////////
            // Step 2: Find how many padding chars are present in the end
            for (endPos = base64Str.Length; endPos > 0; endPos--)
            {
                if (base64Str[endPos - 1] != '=') // Found a non-padding char!
                {
                    break; // Stop here
                }
            }

            ////////////////////////////////////////////////////////
            // Step 3: Create char array to store all non-padding chars,
            //      plus a char to indicate how many padding chars are needed
            base64Chars = new char[endPos + 1];
            base64Chars[endPos] = (char)('0' + base64Str.Length - endPos); // Store a char at the end, to indicate how many padding chars are needed

            ////////////////////////////////////////////////////////
            // Step 3: Copy in the other chars. Transform the "+" to "-", and "/" to "_"
            for (int iter = 0; iter < endPos; iter++)
            {
                char c = base64Str[iter];

                switch (c)
                {
                    case '+': base64Chars[iter] = '-'; break;
                    case '/': base64Chars[iter] = '_'; break;
                    case '=': Debug.Assert(false); base64Chars[iter] = c; break;
                    default: base64Chars[iter] = c; break;
                }
            }
            return new string(base64Chars);
        }
        /// <summary>
        /// Get Raw content of HTML string
        /// </summary>
        /// <param name="str">Input HTML string to decode</param>
        /// <returns></returns>
        public static string GetRawContent(this string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            string decode = HttpUtility.HtmlDecode(str).TrimEnd();
            string tagsreplaced = Regex.Replace(decode, "<.*?>", "");
            string normalized = tagsreplaced.Normalize();
            return normalized;
        }

        /// <summary>
        /// Check type file to validate
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <param name="checkTypes"></param>
        /// <returns></returns>
        public static bool CheckTypeFile(this string pathToFile, List<string> checkTypes)
        {
            if (!String.IsNullOrEmpty(pathToFile))
            {
                var type = pathToFile.Split('.').Last().ToLower();
                if (!checkTypes.Contains(type))
                {
                    return false;
                }
                return true;
            }
            return false;

        }

        public static bool CheckNamePerson(this string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                if(name.Length != name.FullTrim().Length)
                {
                    return false;
                }
                if (name.Contains("  "))
                {
                    return false;
                }
                if (Regex.IsMatch(name, "[0-9]"))
                {
                    return false;
                }
                return true;
            }
            return true;

        }
    }
}
