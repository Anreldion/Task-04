using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary.Utilities
{
    /// <summary>
    /// Provides guard clause methods for validating method arguments.
    /// Throws descriptive exceptions when invalid arguments are detected.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given value is <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type of the value being checked.</typeparam>
        /// <param name="value">The value to check for null.</param>
        /// <param name="name">The name of the parameter being checked.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
        public static void NotNull<T>(T value, string name)
        {
            if (value == null)
                throw new ArgumentNullException(name);
        }

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> if the given collection is empty.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The collection to check.</param>
        /// <param name="name">The name of the parameter being checked.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="collection"/> is empty.</exception>
        public static void NotEmpty<T>(IEnumerable<T> collection, string name)
        {
            if (!collection.Any())
                throw new InvalidOperationException($"Collection '{name}' must not be empty.");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if the given numeric value is negative.
        /// </summary>
        /// <typeparam name="T">A value type that implements <see cref="IComparable{T}"/>.</typeparam>
        /// <param name="value">The numeric value to check.</param>
        /// <param name="name">The name of the parameter being checked.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is less than zero.</exception>
        public static void AgainstNegative<T>(T value, string name) where T : struct, IComparable<T>
        {
            if (value.CompareTo(default) < 0)
                throw new ArgumentOutOfRangeException(name, "Value cannot be negative.");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if the given numeric value is zero.
        /// </summary>
        /// <param name="value">The numeric value to check.</param>
        /// <param name="name">The name of the parameter being checked.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is zero.</exception>
        public static void AgainstZero(double value, string name)
        {
            if (value == 0)
                throw new ArgumentOutOfRangeException(name, "Value cannot be zero.");
        }
    }
}
