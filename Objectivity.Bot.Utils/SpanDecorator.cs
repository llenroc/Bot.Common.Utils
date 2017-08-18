namespace Objectivity.Bot.Utils
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    public class SpanDecorator
    {
        public string Parse(string input)
        {
            var openingSpan = "<span style=\"font-family:Segoe UI;font-size: 13px;\">";
            input = string.Concat(openingSpan, input, "</span>");
            var pattern = ">([^<>]+)<";
            var spannedString = Regex.Replace(input, pattern,
                match =>
                    match.Value[0] + openingSpan + match.Value.Substring(1, match.Value.Length - 2) +
                    "</span><");
            return spannedString;
        }
    }
}