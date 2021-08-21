using UnityEngine;

namespace Extensions
{
    public static class MathExtension
    {
        public static bool IsBetweenRange(this float thisValue, float value1, float value2)
        {
            return thisValue >= Mathf.Min(value1, value2) && thisValue <= Mathf.Max(value1, value2);
        }

        public static float RoundTo(this float value, float multipleOf)
        {
            return Mathf.Round(value / multipleOf) * multipleOf;
        }
    }
}