using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ShInUeXx.Numerics
{
    public readonly partial struct Rational
    {
        /// <summary>
        /// Invert the value of <see cref="Rational"/> object
        /// </summary>
        /// <param name="value">A value to invert</param>
        /// <returns>Returns a inverted value of <paramref name="value"/>. Effectively: <c>1/<paramref name="value"/></c></returns>
        public static Rational Inverse(Rational value) => new(value.Denominator, value.Numerator);
        /// <summary>
        /// Gets the absolute value of a <see cref="Rational"/> object.
        /// </summary>
        /// <param name="value">A number.</param>
        /// <returns>The absolute value of <paramref name="value"/></returns>
        public static Rational Abs(Rational value) => new(BigInteger.Abs(value.Numerator), value.Denominator);

        /// <summary>
        /// Negates a specified <see cref="Rational"/> value.
        /// </summary>
        /// <param name="value">The value to negate</param>
        /// <returns><inheritdoc cref="operator -(Rational)"/></returns>
        public static Rational Negate(Rational value) => -value;

        /// <summary>
        /// Returns the largest integral value less than or equal to the specified <see cref="Rational"/> number.
        /// </summary>
        /// <param name="value">A <see cref="Rational"/> number</param>
        /// <returns>The largest integral value less than or equal to <paramref name="value"/>. Preserves non valid values.</returns>
        public static Rational Floor(Rational value)
        {
            if (!value.IsValid) return value;
            if (value.IsInteger) return value.Numerator;
            var v = BigInteger.Abs(value.Numerator) / value.Denominator;
            var s = value.Numerator < 0;
            if (s) v++;
            return s ? -v : v;
        }
        /// <summary>
        /// Returns the smallest integral value greater than or equal to the specified <see cref="Rational"/> number.
        /// </summary>
        /// <param name="value">A <see cref="Rational"/> number</param>
        /// <returns>The smallest integral value greater than or equal to <paramref name="value"/>. Preserves non valid values.</returns>
        public static Rational Ceiling(Rational value)
        {
            if (!value.IsValid) return value;
            if (value.IsInteger) return value.Numerator;
            var v = BigInteger.Abs(value.Numerator) / value.Denominator;
            var s = value.Numerator < 0;
            if (!s) v++;
            return s ? -v : v;
        }

        /// <summary>
        /// Rounds a <see cref="Rational"/> value to nearest integral value.
        /// </summary>
        /// <param name="value">A <see cref="Rational"/> number to be rounded.</param>
        /// <returns>The integer nearest <paramref name="value"/>. Implemented as <c><see cref="Floor"/>(<paramref name="value"/> + <see cref="Half"/>)</c></returns>
        public static Rational Round(Rational value) => Floor(value + Half);

        /// <summary>
        /// Calculates the integral part of specified <see cref="Rational"> number.
        /// </summary>
        /// <param name="value">A number to truncate.</param>
        /// <returns>
        /// The integral part of <paramref name="value"/>. 
        /// Discard any fractional part of number. 
        /// If <paramref name="value"/> is valid number, after call result satisfy <see cref="IsInteger"/>. 
        /// Preserves non valid values.
        /// </returns>
        public static Rational Truncate(Rational value)
        {
            if (!value.IsValid) return value;
            return value.Numerator / value.Denominator;
        }

        /// <summary>
        /// Returns the smaller of two <see cref="Rational"/> numbers.
        /// </summary>
        /// <param name="left">The first of two <see cref="Rational"/> number to compare.</param>
        /// <param name="right">The second of two <see cref="Rational"/> number to compare.</param>
        /// <returns>
        /// Parameter <paramref name="left"/> or <paramref name="right"/>, whichever is smaller.
        /// </returns>
        public static Rational Min(Rational left, Rational right) => left < right ? left : right;
        /// <summary>
        /// Returns the larger of two <see cref="Rational"/> numbers.
        /// </summary>
        /// <param name="left">The first of two <see cref="Rational"/> number to compare.</param>
        /// <param name="right">The second of two <see cref="Rational"/> number to compare.</param>
        /// <returns>
        /// Parameter <paramref name="left"/> or <paramref name="right"/>, whichever is larger.
        /// </returns>
        public static Rational Max(Rational left, Rational right) => left < right ? right : left;

        /// <inheritdoc cref="operator +(Rational, Rational)"/>
        public static Rational Add(Rational left, Rational right) => left + right;

        /// <inheritdoc cref="operator -(Rational, Rational)"/>
        public static Rational Subtract(Rational left, Rational right) => left - right;

        /// <inheritdoc cref="operator *(Rational, Rational)"/>
        public static Rational Multiply(Rational left, Rational right) => left * right;

        /// <inheritdoc cref="operator /(Rational, Rational)"/>
        public static Rational Divide(Rational dividend, Rational divisor) => dividend / divisor;

        /// <inheritdoc cref="operator %(Rational, Rational)"/>
        public static Rational Remainder(Rational dividend, Rational divisor) => dividend % divisor;

        /// <summary>
        /// Calculate the modulo of two <see cref="Rational"/> values.
        /// </summary>
        /// <param name="left">The dividend.</param>
        /// <param name="right">The divisor</param>
        /// <returns>
        /// The modulo result of dividing <paramref name="left"/> by <paramref name="right"/>.
        /// Effective calls:<code><paramref name="left"/> - <paramref name="right"/> * <see cref="Floor"/>(<paramref name="left"/> / <paramref name="right"/>)</code>
        /// </returns>
        public static Rational Modulo(Rational left, Rational right) => left - right * Floor(left / right);

        /// <summary>
        /// Raises a <see cref="Rational"/> value to the power of specific value.
        /// </summary>
        /// <param name="value">The number to raise to the <paramref name="exponent"/> power.</param>
        /// <param name="exponent">The exponent to raise <paramref name="value"/> by.</param>
        /// <returns>The result of raising <paramref name="value"/> to the <paramref name="exponent"/> power. Preserves non valid values.</returns>
        public static Rational Pow(Rational value, int exponent)
        {
            if (value.IsNan || (exponent == 1)) return value;
            if (exponent < 0) return Pow(Inverse(value), -exponent);
            if (exponent == 0) return 1;
            else
            {
                var n = BigInteger.Pow(value.Numerator, exponent);
                var d = BigInteger.Pow(value.Denominator, exponent);
                return new Rational(n, d);
            }
        }

        /// <summary>
        /// Returns the logarithm of a specified number in specified base.
        /// </summary>
        /// <param name="value">A number whose logarithm is to be found.</param>
        /// <param name="base">The base of the logarithm</param>
        /// <returns>The base <paramref name="base"/> logarithm of <paramref name="value"/>.</returns>
        /// <seealso cref="BigInteger.Log(BigInteger,double)"/>
        public static double Log(Rational value, double @base) => BigInteger.Log(value.Numerator, @base) - BigInteger.Log(value.Denominator, @base);

        /// <summary>
        /// Returns the natural (base <c>e</c>) logarithm of a specified number.
        /// </summary>
        /// <param name="value">A number whose logarithm is to be found.</param>
        /// <returns>The natural (base <c>e</c>) log of <paramref name="value"/></returns>
        /// <seealso cref="BigInteger.Log(BigInteger)"/>
        public static double Log(Rational value) => BigInteger.Log(value.Numerator) - BigInteger.Log(value.Denominator);
        /// <summary>
        /// Returns the logarithm of a specified number in specified base.
        /// </summary>
        /// <param name="value">A number whose logarithm is to be found.</param>
        /// <param name="base">The base of the logarithm</param>
        /// <returns>The base <paramref name="base"/> logarithm of <paramref name="value"/>.</returns>
        /// <seealso cref="BigInteger.Log(BigInteger)"/>
        public static double Log(Rational value, Rational @base) => Log(value) / Log(@base);
    }
}
