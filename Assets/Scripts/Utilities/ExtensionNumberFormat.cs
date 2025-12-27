namespace Utilities
{
    public static class ExtensionNumberFormat
    {
        public static string ToMoneyFormat(this double balance)
        {
            return $"${balance:N1}";
        }
    }
}