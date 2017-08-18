namespace Objectivity.Bot.Utils.DirectLine
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class DirectLineMessage
    {
        public const string ContentType = "application/objectivity.bot.dl.message";

        public string Base64Attachment { get; set; }

        public string HtmlBody { get; set; }

        public string ConvertToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}