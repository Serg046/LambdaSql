namespace LambdaSql
{
    internal static class Extensions
    {
        internal static bool IsNotEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }
    }
}
