using System.Numerics;

namespace ShInUeXx.Numerics
{
    public readonly partial struct Rational
    {
        /// <summary>
        /// Returns a value that indicates whether the values of two <see cref="Rational"/> objects are equal
        /// </summary>
        /// <param name="left">The first value to compare</param>
        /// <param name="right">The seconds value to compare</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> and <paramref name="right"/> parameters have the same value; otherwise <see langword="false"/></returns>
        public static bool operator ==(Rational left, Rational right)
        {
            return !left.IsNan && !right.IsNan && left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether the values of two <see cref="Rational"/> objects are not equal
        /// </summary>
        /// <param name="left">The first value to compare</param>
        /// <param name="right">The seconds value to compare</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> and <paramref name="right"/> parameters have the different values; otherwise <see langword="false"/></returns>
        public static bool operator !=(Rational left, Rational right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a value that indicates whether a <see cref="Rational"/> value is less than another <see cref="Rational"/> value
        /// </summary>
        /// <param name="left">The first value to compare</param>
        /// <param name="right">The seconds value to compare</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> parametes is less than <paramref name="right"/> parameter; otherwise <see langword="false"/></returns>
        public static bool operator <(Rational left, Rational right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Returns a value that indicates whether a <see cref="Rational"/> value is less than or equal another <see cref="Rational"/> value
        /// </summary>
        /// <param name="left">The first value to compare</param>
        /// <param name="right">The seconds value to compare</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> parametes is less than or equal <paramref name="right"/> parameter; otherwise <see langword="false"/></returns>
        public static bool operator <=(Rational left, Rational right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Returns a value that indicates whether a <see cref="Rational"/> value is greater than another <see cref="Rational"/> value
        /// </summary>
        /// <param name="left">The first value to compare</param>
        /// <param name="right">The seconds value to compare</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> parametes is greater than <paramref name="right"/> parameter; otherwise <see langword="false"/></returns>
        public static bool operator >(Rational left, Rational right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Returns a value that indicates whether a <see cref="Rational"/> value is greater than or equal another <see cref="Rational"/> value
        /// </summary>
        /// <param name="left">The first value to compare</param>
        /// <param name="right">The seconds value to compare</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> parametes is greater than or equal <paramref name="right"/> parameter; otherwise <see langword="false"/></returns>
        public static bool operator >=(Rational left, Rational right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        /// Returns the value of <see cref="Rational"/> operand. (The sign of the operand is unchanged.)
        /// </summary>
        /// <param name="value">A <see cref="Rational"/> value</param>
        /// <returns>The value of the <paramref name="value"/> operand</returns>
        public static Rational operator +(Rational value) => value;

        /// <summary>
        /// Negates a specified <see cref="Rational"/> value.
        /// </summary>
        /// <param name="value">A value to negate</param>
        /// <returns>The result of the <paramref name="value"/> parameter multiplied by negative one (-1)</returns>
        public static Rational operator -(Rational value) => new(-value.Numerator, value.Denominator);

        /// <summary>
        /// Increments a <see cref="Rational"/> value by 1.
        /// </summary>
        /// <param name="value">The value to increment</param>
        /// <returns>The value of the <paramref name="value"/> parameter incremented by 1.</returns>
        public static Rational operator ++(Rational value) => new(value.Numerator + value.Denominator, value.Denominator);

        /// <summary>
        /// Decrements a <see cref="Rational"/> value by 1.
        /// </summary>
        /// <param name="value">The value to decrement</param>
        /// <returns>The value of the <paramref name="value"/> parameter decremented by 1.</returns>
        public static Rational operator --(Rational value) => new(value.Numerator - value.Denominator, value.Denominator);

        /// <summary>
        /// Adds the values of two specified <see cref="Rational"/> objects.
        /// </summary>
        /// <param name="left">The first value to add</param>
        /// <param name="right">The second value to add</param>
        /// <returns>The sum of <paramref name="left"/> and <paramref name="right"/></returns>
        public static Rational operator +(Rational left, Rational right)
        {
            var n = left.Numerator * right.Denominator + right.Numerator * left.Denominator;
            var d = left.Denominator * right.Denominator;
            return new Rational(n, d);
        }

        /// <summary>
        /// Subtracts a <see cref="Rational"/> value from another <see cref="Rational"/> value.
        /// </summary>
        /// <param name="left">The value to subtract from (the minuend).</param>
        /// <param name="right">The value to subtract (the subtrahend)</param>
        /// <returns>The result of subtracting <paramref name="right"/> from <paramref name="left"/></returns>
        public static Rational operator -(Rational left, Rational right)
        {
            var n = left.Numerator * right.Denominator - right.Numerator * left.Denominator;
            var d = left.Denominator * right.Denominator;
            return new Rational(n, d);
        }

        /// <summary>
        /// Multiplies two <see cref="Rational"/> values.
        /// </summary>
        /// <param name="left">The first value to multiply</param>
        /// <param name="right">The second value to multiply</param>
        /// <returns>The product of <paramref name="left"/> and <paramref name="right"/></returns>
        public static Rational operator *(Rational left, Rational right)
        {
            var n = left.Numerator * right.Numerator;
            var d = left.Denominator * right.Denominator;
            return new Rational(n, d);
        }

        /// <summary>
        /// Divides a specified <see cref="Rational"/> value by another specified <see cref="Rational"/> value.
        /// </summary>
        /// <param name="left">The dividend</param>
        /// <param name="right">The divisor</param>
        /// <returns>The result of dividing <paramref name="left"/> by <paramref name="right"/></returns>
        public static Rational operator /(Rational left, Rational right)
        {
            var n = left.Numerator * right.Denominator;
            var d = left.Denominator * right.Numerator;
            return new Rational(n, d);
        }
        /// <summary>
        /// Returns the remainder resulting from dividing two specified <see cref="Rational"/> values.
        /// </summary>
        /// <param name="left">The dividend</param>
        /// <param name="right">The divisor</param>
        /// <returns>The remainder resulting from dividing <paramref name="left"/> by <paramref name="right"/></returns>
        public static Rational operator %(Rational left, Rational right)
        {
            return left - right * Truncate(left / right);
        }

        /// <summary>
        /// Adds the values of <see cref="Rational"/> object and <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left">The first value to add</param>
        /// <param name="right">The second value to add</param>
        /// <returns>The sum of <paramref name="left"/> and <paramref name="right"/></returns>
        public static Rational operator +(Rational left, BigInteger right)
        {
            var n = left.Numerator + right * left.Denominator;
            return new Rational(n, left.Denominator);
        }

        /// <summary>
        /// Adds the values of <see cref="BigInteger"/> object and <see cref="Rational"/> object.
        /// </summary>
        /// <param name="left">The first value to add</param>
        /// <param name="right">The second value to add</param>
        /// <returns>The sum of <paramref name="left"/> and <paramref name="right"/></returns>
        public static Rational operator +(BigInteger left, Rational right)
        {
            var n = left * right.Denominator + right.Numerator;
            return new Rational(n, right.Denominator);
        }

        /// <summary>
        /// Subtracts a <see cref="BigInteger"/> value from another <see cref="Rational"/> value.
        /// </summary>
        /// <param name="left">The value to subtract from (the minuend).</param>
        /// <param name="right">The value to subtract (the subtrahend)</param>
        /// <returns>The result of subtracting <paramref name="right"/> from <paramref name="left"/></returns>
        public static Rational operator -(Rational left, BigInteger right)
        {
            var n = left.Numerator - right * left.Denominator;
            return new Rational(n, left.Denominator);
        }

        /// <summary>
        /// Subtracts a <see cref="Rational"/> value from another <see cref="BigInteger"/> value.
        /// </summary>
        /// <param name="left">The value to subtract from (the minuend).</param>
        /// <param name="right">The value to subtract (the subtrahend)</param>
        /// <returns>The result of subtracting <paramref name="right"/> from <paramref name="left"/></returns>
        public static Rational operator -(BigInteger left, Rational right)
        {
            var n = left * right.Denominator - right.Numerator;
            return new Rational(n, right.Denominator);
        }

        /// <summary>
        /// Multiplies <see cref="Rational"/> value and <see cref="BigInteger"/> value.
        /// </summary>
        /// <param name="left">The first value to multiply</param>
        /// <param name="right">The second value to multiply</param>
        /// <returns>The product of <paramref name="left"/> and <paramref name="right"/></returns>
        public static Rational operator *(Rational left, BigInteger right) => new(left.Numerator * right, left.Denominator);

        /// <summary>
        /// Multiplies <see cref="BigInteger"/> value and <see cref="Rational"/> value.
        /// </summary>
        /// <param name="left">The first value to multiply</param>
        /// <param name="right">The second value to multiply</param>
        /// <returns>The product of <paramref name="left"/> and <paramref name="right"/></returns>
        public static Rational operator *(BigInteger left, Rational right) => new(left * right.Numerator, right.Denominator);

        /// <summary>
        /// Divides a specified <see cref="Rational"/> value by another specified <see cref="BigInteger"/> value.
        /// </summary>
        /// <param name="left">The dividend</param>
        /// <param name="right">The divisor</param>
        /// <returns>The result of dividing <paramref name="left"/> by <paramref name="right"/></returns>
        public static Rational operator /(Rational left, BigInteger right) => new(left.Numerator, left.Denominator * right);

        /// <summary>
        /// Divides a specified <see cref="BigInteger"/> value by another specified <see cref="Rational"/> value.
        /// </summary>
        /// <param name="left">The dividend</param>
        /// <param name="right">The divisor</param>
        /// <returns>The result of dividing <paramref name="left"/> by <paramref name="right"/></returns>
        public static Rational operator /(BigInteger left, Rational right) => new(left * right.Denominator, right.Numerator);

        /// <summary>
        /// Returns the remainder resulting from dividing <see cref="Rational"/> value by <see cref="BigInteger"/> value.
        /// </summary>
        /// <param name="left">The dividend</param>
        /// <param name="right">The divisor</param>
        /// <returns>The remainder resulting from dividing <paramref name="left"/> by <paramref name="right"/></returns>
        public static Rational operator %(Rational left, BigInteger right)
        {
            return left - right * Truncate(left / right);
        }

        /// <summary>
        /// Returns the remainder resulting from dividing <see cref="BigInteger"/> value by <see cref="Rational"/> value.
        /// </summary>
        /// <param name="left">The dividend</param>
        /// <param name="right">The divisor</param>
        /// <returns>The remainder resulting from dividing <paramref name="left"/> by <paramref name="right"/></returns>
        public static Rational operator %(BigInteger left, Rational right)
        {
            return left - right * Truncate(left / right);
        }
    }
}
