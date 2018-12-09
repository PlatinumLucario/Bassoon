using System;

namespace Bassoon
{
    /// <summary>
    /// Various helpful math related utilies.
    /// </summary>
    internal static class MathHelper
    {
        /// <summary>
        /// Clamp a value between a range (inclusive).
        ///
        /// This exists in .NET Core, but not in .NET standard
        /// </summary>
        /// <param name="value">Valume to clamp</param>
        /// <param name="min">minimum possible value</param>
        /// <param name="max">maximum possible value</param>
        /// <returns>The orignal value (clamped between the range)</returns>
        public static float Clamp(float value, float min, float max) =>
            Math.Max(Math.Min(value, max), min);

        /// <summary>
        /// Clamp a value between a range (inclusive).
        ///
        /// This exists in .NET Core, but not in .NET standard
        /// </summary>
        /// <param name="value">Valume to clamp</param>
        /// <param name="min">minimum possible value</param>
        /// <param name="max">maximum possible value</param>
        /// <returns>The orignal value (clamped between the range)</returns>
        public static long Clamp(long value, long min, long max) =>
            Math.Max(Math.Min(value, max), min);
    }
}
