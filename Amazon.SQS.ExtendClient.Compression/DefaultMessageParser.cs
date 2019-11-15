using System;
using System.Text.RegularExpressions;

namespace Amazon.SQS.ExtendClient.Compression
{
    public class DefaultMessageParser : IMessageParser
    {
        private static readonly Regex CompressionMarkerPattern = new Regex(@"^(?<is_compressed>0|1)\|(?<message>.*)$", RegexOptions.Compiled);

        public MessageBody Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return default;

            var match = CompressionMarkerPattern.Match(input);

            if (!match.Success) throw new FormatException("The provided input does not represent a valid compressed message");

            return new MessageBody(
                int.Parse(match.Groups["is_compressed"].Value) == 1,
                match.Groups["message"].Value
            );
        }
    }
}
