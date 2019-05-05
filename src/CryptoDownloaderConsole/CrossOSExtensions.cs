using System;

namespace CryptoDownloaderConsole
{
    /// <summary>
    /// Extension methods to use od different OSes
    /// </summary>
    public static class CrossOSExtension
    {
        /// <summary>
        /// Handles different types of line breaks
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string[] SplitByNewLine(this string input)
        {
            return input.Split(new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None);
        }
        public static string TrimAndReduce(this string str)
        {
            return (str).Trim();
        }
    }
}