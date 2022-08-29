namespace CommunAxiom.Commons.Ingestion.Extentions
{
    public static class StringExtensions
    {
        public static T ToEnum<T>(this string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }
    }
}
