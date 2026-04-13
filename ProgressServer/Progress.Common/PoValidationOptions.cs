namespace Progress.Common
{
    public class PoValidationOptions
    {
        public const string SectionName = "PoValidation";
        /// <summary>Regex；空表示不校验。</summary>
        public string Pattern { get; set; } = "";
    }
}
