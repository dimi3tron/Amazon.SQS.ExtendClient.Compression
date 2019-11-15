using Newtonsoft.Json;

namespace Amazon.SQS.ExtendClient.Compression.Test.Extensions
{
    internal static class ObjectExtensions
    {
        public static string ToJson(this object me)
        {
            if (me == null) return null;

            return JsonConvert.SerializeObject(
                me,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.None,
                    Error = (sender, errorArgs) => { errorArgs.ErrorContext.Handled = true; }
                }
            );
        }
    }
}
