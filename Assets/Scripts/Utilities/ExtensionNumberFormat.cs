using UnityEngine;

namespace Utilities
{
    public static class ExtensionNumberFormat
    {
        public static string ToMoneyFormat(this int amount)
        {
            // 0 - 999
            if (amount < 1000)
            {
                return $"${Mathf.FloorToInt(amount).ToString()}";
            }

            // 1,000 - 999,999 (K)
            if (amount < 1000000)
            {
                return (amount / 1000f).ToString("$0.##") + "K";
            }

            // 1,000,000 - 999,999,999 (M)
            if (amount < 1000000000)
            {
                return (amount / 1000000f).ToString("$0.##") + "M";
            }

            // 1,000,000,000+ (B)
            return (amount / 1000000000f).ToString("$0.##") + "B";
        }
        
        public static string ToPowerFormat(this int amount)
        {
            string negative = amount < 0 ? "-" : "";
            
            amount = Mathf.Abs(amount);
            
            // 0 - 999
            if (amount < 1000)
            {
                return $"{negative}{Mathf.FloorToInt(amount).ToString()}";
            }

            // 1,000 - 999,999 (K)
            if (amount < 1000000)
            {
                return (amount / 1000f).ToString( $"{negative}0.##") + "K";
            }

            // 1,000,000 - 999,999,999 (M)
            if (amount < 1000000000)
            {
                return (amount / 1000000f).ToString($"{negative}0.##") + "M";
            }

            // 1,000,000,000+ (B)
            return (amount / 1000000000f).ToString($"{negative}0.##") + "B";
        }
    }
}