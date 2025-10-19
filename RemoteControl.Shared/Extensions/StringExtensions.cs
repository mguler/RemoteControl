using System.Text.RegularExpressions;

namespace RemoteControl.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string[] Matches(this string s, string pattern) => Regex.Matches(s, pattern).Select(match => match.Value).ToArray();
        public static string Match(this string s, string pattern) => Regex.Match(s, pattern).Value;
        public static bool IsMatch(this string s, string pattern) => Regex.IsMatch(s, pattern);
    }
}
