namespace Objectivity.Bot.Utils
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class HtmlToPlainTextDecorator
    {
        public string Parse(string input)
        {
            var rawText = input;
            List<KeyValuePair<string, MatchEvaluator>> flow = new List<KeyValuePair<string, MatchEvaluator>>
            {
                new KeyValuePair<string, MatchEvaluator>("<br.?/>", match => "\n"),
                new KeyValuePair<string, MatchEvaluator>("<b>(?<text>.+)</b>", match => match.Groups["text"].Value),
                new KeyValuePair<string, MatchEvaluator>(
                    "<a.*?href=\"(?<link>.+?)\".*?>(?<text>.+?)</a>",
                    match => $"[{match.Groups["text"]}] {match.Groups["link"].Value} "),
                new KeyValuePair<string, MatchEvaluator>(
                    "<li.*?><span.*?>(?<text>.+?)</span></li>",
                    match => "\n" + match.Groups["text"].Value),
                new KeyValuePair<string, MatchEvaluator>("<[^>]*>", match => string.Empty),
                new KeyValuePair<string, MatchEvaluator>("&nbsp;", match => string.Empty)
            };

            foreach (var item in flow)
            {
                rawText = Regex.Replace(rawText, item.Key, item.Value);
            }

            return rawText;
        }
    }
}