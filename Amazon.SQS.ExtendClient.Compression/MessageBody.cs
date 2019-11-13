namespace Amazon.SQS.ExtendClient.Compression
{
    public struct MessageBody
    {
        public MessageBody(bool isCompressed, string value, bool implicitCompression = false)
        {
            IsCompressed = isCompressed;
            Value = value;
            ImplicitCompression = implicitCompression;
            HasValue = true;
        }

        public bool HasValue { get; }

        /// <summary>
        /// Indicates whether the value of this message is compressed
        /// </summary>
        public bool IsCompressed { get; }


        /// <summary>
        /// Indicates whether a compression marker should be added tot the message
        /// </summary>
        public bool ImplicitCompression { get; }


        /// <summary>
        /// Value of the message
        /// </summary>
        public string Value { get; }

        public override string ToString()
        {
            if (!HasValue) return string.Empty;

            return ImplicitCompression ?
                Value :
                $"{(this.IsCompressed ? "1" : "0")}|{Value}";
        }
    }
}