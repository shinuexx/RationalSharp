using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

[assembly: CLSCompliant(true)]

namespace ShInUeXx.Numerics
{
    /// <summary>
    /// Representing arbitrary rational as <see cref="BigInteger">BigIntegers</see> ratio
    /// </summary>
    [CLSCompliant(true)]
    public readonly partial struct Rational : INumber<Rational>
    {
        /// <summary>
        /// Regular expression for rational in form <c>i n/d</c> where <c>i</c> ,<c>n</c>, <c>d</c> are integers
        /// </summary>
        private static readonly Regex REGEXP_INT_FRACTION = IntFractionRegex();
        /// <summary>
        /// Regular expression for rational in integer form
        /// </summary>
        private static readonly Regex REGEXP_INT = IntegerRegex();
        /// <summary>
        /// Regular expression for rational in scientific and decimal notation (eg. <c>1.1e2</c>)
        /// </summary>
        private static readonly Regex REGEXP_DECIMAL = DecimalRegex();
        /// <summary>
        /// Regular expression for rational in fraction form (<c>n/d</c> where <c>n</c>, <c>d</c> are integers)
        /// </summary>
        private static readonly Regex REGEXP_FRACTION = FractionRegex();

        /// <summary>
        /// Bitmask for double exponent
        /// </summary>
        private const long double_exp_mask = (1L << 11) - 1;
        /// <summary>
        /// Bitmask for double mantissa
        /// </summary>
        private const long double_mantissa_mask = (1L << 52) - 1;
        /// <summary>
        /// Default double denominator
        /// </summary>
        private const long double_default_denominator = (1L << 52);

        /// <summary>
        /// Bitmask for float exponent
        /// </summary>
        private const int float_exp_mask = 0xff;
        /// <summary>
        /// Bitmast for float mantissa
        /// </summary>
        private const int float_mantissa_mask = (1 << 23) - 1;
        /// <summary>
        /// Default float denominator
        /// </summary>
        private const int float_default_denominator = (1 << 23);

        /// <summary>
        /// Bitmask for half exponent
        /// </summary>
        private const short half_exp_mask = 0x1f;
        /// <summary>
        /// Bitmask for half mantissa
        /// </summary>
        private const short half_matissa_mask = (1 << 10) - 1;
        /// <summary>
        /// Default half denominator
        /// </summary>
        private const short half_default_denominator = (1 << 10);

        /// <summary>
        /// Indicates numerator of rational
        /// </summary>
        public BigInteger Numerator { get; }
        /// <summary>
        /// Indicates denominator of rational
        /// </summary>
        public BigInteger Denominator { get; }

        /// <summary>
        /// Gets a number that indicates the sign (negative, positive, or zero) of the current <see cref="Rational"/> object.
        /// </summary>
        public int Sign => Numerator.Sign;

        /// <summary>
        /// Gets a value that is not a number (NaN)(0/0)
        /// </summary>
        public static Rational NaN { get; } = new(BigInteger.Zero, BigInteger.Zero);
        /// <summary>
        /// Gets positive infinity (1/0)
        /// </summary>
        public static Rational PositiveInfinity { get; } = new(BigInteger.One, BigInteger.Zero);
        /// <summary>
        /// Gets negative infinity (-1/0)
        /// </summary>
        public static Rational NegativeInfinity { get; } = new(BigInteger.MinusOne, BigInteger.Zero);
        /// <summary>
        /// Gets a value that represents the number -1 (minus one)
        /// </summary>
        public static Rational MinusOne { get; } = new(BigInteger.MinusOne);
        /// <summary>
        /// Gets a value that represents the number 1/2 (half)
        /// </summary>
        public static Rational Half { get; } = new(BigInteger.One, 2);
        /// <summary>
        /// Gets a value that is not a number (NaN)(0/0)
        /// </summary>
        public static Rational MinusHalf { get; } = new(BigInteger.MinusOne, 2);

        /// <summary>
        /// Initializes a new instance of the <see cref="Rational"/> structure with 0
        /// </summary>
        public Rational()
        {
            Numerator = BigInteger.Zero;
            Denominator = BigInteger.One;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Rational"/> structure using <see cref="BigInteger"/> value
        /// </summary>
        /// <param name="integer">A BigInteger value</param>
        public Rational(BigInteger integer)
        {
            Numerator = integer;
            Denominator = BigInteger.One;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Rational"/> structure using <see cref="BigInteger"/>s ratio
        /// </summary>
        /// <param name="numerator">A BigInteger value</param>
        /// <param name="denominator">A BigInteger value</param>
        public Rational(BigInteger numerator, BigInteger denominator)
        {
            (Numerator, Denominator) = Simplify(MoveSign(numerator, denominator));
        }

        /// <summary>
        /// Move sign of from last to first <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="n">Numerator value as <see cref="BigInteger"/></param>
        /// <param name="d">Denominator value as <see cref="BigInteger"/></param>
        /// <returns>Tuple of <c>(first, last)</c> <see cref="BigInteger"/> with non negative <c>last</c></returns>
        private static (BigInteger, BigInteger) MoveSign(BigInteger n, BigInteger d)
        {
            if (BigInteger.IsNegative(d))
            {
                n = -n;
                d = -d;
            }
            return (n, d);
        }
        /// <summary>
        /// Simplify two <see cref="BigInteger"/> by its <see cref="BigInteger.GreatestCommonDivisor(BigInteger, BigInteger)"/>
        /// </summary>
        /// <param name="nd">Tuple of two <see cref="BigInteger"/> objects <c>(first, last)</c></param>
        /// <returns>Tuple of <c>(first, last)</c> with simplified <see cref="BigInteger"/> objects</returns>
        private static (BigInteger, BigInteger) Simplify((BigInteger, BigInteger) nd)
        {
            var (n, d) = nd;
            if (d > 1)
            {
                var gcd = BigInteger.GreatestCommonDivisor(n, d);
                if (gcd > 1)
                {
                    n /= gcd;
                    d /= gcd;
                }
            }
            if (d.IsZero)
            {
                n = n.Sign;
            }
            return (n, d);
        }

        /// <inheritdoc/>
        public static Rational One { get; } = new(BigInteger.One);

        /// <inheritdoc/>
        public static int Radix => 2;

        /// <inheritdoc/>
        public static Rational Zero { get; } = new();

        /// <inheritdoc/>
        public static Rational AdditiveIdentity => Zero;

        /// <inheritdoc/>
        public static Rational MultiplicativeIdentity => One;

        /// <inheritdoc/>
        public static Rational Abs(Rational value) => new(BigInteger.Abs(value.Numerator), value.Denominator);
        /// <inheritdoc/>
        public static bool IsCanonical(Rational value) => IsFinite(value);
        /// <inheritdoc/>
        public static bool IsComplexNumber(Rational value) => false;
        /// <inheritdoc/>
        public static bool IsEvenInteger(Rational value) => IsInteger(value) && BigInteger.IsEvenInteger(value.Numerator);
        /// <inheritdoc/>
        public static bool IsFinite(Rational value) => !value.Denominator.IsZero;
        /// <inheritdoc/>
        public static bool IsImaginaryNumber(Rational value) => false;
        /// <inheritdoc/>
        public static bool IsInfinity(Rational value) => value.Denominator.IsZero && !value.Numerator.IsZero;
        /// <inheritdoc/>
        public static bool IsInteger(Rational value) => value.Denominator.IsOne;
        /// <inheritdoc/>
        public static bool IsNaN(Rational value) => value.Denominator.IsZero && value.Numerator.IsZero;
        /// <inheritdoc/>
        public static bool IsNegative(Rational value) => BigInteger.IsNegative(value.Numerator);
        /// <inheritdoc/>
        public static bool IsNegativeInfinity(Rational value) => value.Denominator.IsZero && BigInteger.IsNegative(value.Numerator);
        /// <inheritdoc/>
        public static bool IsNormal(Rational value) => IsFinite(value);
        /// <inheritdoc/>
        public static bool IsOddInteger(Rational value) => IsInteger(value) && BigInteger.IsOddInteger(value.Numerator);
        /// <inheritdoc/>
        public static bool IsPositive(Rational value) => BigInteger.IsPositive(value.Numerator);
        /// <inheritdoc/>
        public static bool IsPositiveInfinity(Rational value) => value.Denominator.IsZero && BigInteger.IsPositive(value.Numerator);
        /// <inheritdoc/>
        public static bool IsRealNumber(Rational value) => true;
        /// <inheritdoc/>
        public static bool IsSubnormal(Rational value) => !IsFinite(value);
        /// <inheritdoc/>
        public static bool IsZero(Rational value) => !value.Denominator.IsZero && value.Numerator.IsZero;
        /// <inheritdoc/>
        public static Rational MaxMagnitude(Rational x, Rational y)
        {
            var l = BigInteger.Abs(x.Numerator) * y.Denominator;
            var r = BigInteger.Abs(y.Numerator) * x.Denominator;
            return r.CompareTo(l) < 0 ? y : x;
        }
        /// <inheritdoc/>
        public static Rational MaxMagnitudeNumber(Rational x, Rational y)
        {
            if (IsNaN(x)) return y;
            if (IsNaN(y)) return x;
            return MaxMagnitude(x, y);
        }
        /// <inheritdoc/>
        public static Rational MinMagnitude(Rational x, Rational y)
        {
            var l = BigInteger.Abs(x.Numerator) * y.Denominator;
            var r = BigInteger.Abs(y.Numerator) * x.Denominator;
            return r.CompareTo(l) < 0 ? x : y;
        }
        /// <inheritdoc/>
        public static Rational MinMagnitudeNumber(Rational x, Rational y)
        {
            if (IsNaN(x)) return y;
            if (IsNaN(y)) return x;
            return MinMagnitude(x, y);
        }
        /// <inheritdoc/>
        public static Rational Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
        {

            var trimmed = s.Trim();
            Match m;
            if (REGEXP_INT.IsMatch(trimmed))
            {
                m = REGEXP_INT.Match(trimmed.ToString());
                var n = BigInteger.Parse(m.Groups[1].Value);
                return new(n);
            }
            else if (REGEXP_FRACTION.IsMatch(trimmed))
            {
                m = REGEXP_FRACTION.Match(trimmed.ToString());
                var n = BigInteger.Parse(m.Groups[1].Value);
                var d = BigInteger.Parse(m.Groups[2].Value);
                return new(n, d);
            }
            else if (REGEXP_DECIMAL.IsMatch(trimmed))
            {
                m = REGEXP_DECIMAL.Match(trimmed.ToString());
                var iStr = m.Groups[1].Value;
                var fStr = m.Groups[2].Value;
                var eStr = m.Groups[3].Value;
                var ps = iStr + fStr;
                var e = int.Parse(eStr);
                var n = BigInteger.Parse(ps);
                BigInteger d;
                if (e > 0)
                {
                    n *= BigInteger.Pow(10, e);
                    d = 1;
                }
                else
                {
                    d = BigInteger.Pow(10, -e);
                }
                return new(n, d);
            }
            else if (REGEXP_INT_FRACTION.IsMatch(trimmed))
            {
                m = REGEXP_INT_FRACTION.Match(trimmed.ToString());
                var i = BigInteger.Parse(m.Groups[1].Value);
                var n = BigInteger.Parse(m.Groups[2].Value);
                var d = BigInteger.Parse(m.Groups[3].Value);
                var sign = i.Sign;
                n += BigInteger.Abs(i) * d;
                return new(sign * n, d);
            }
            throw new FormatException(string.Format("Parameter {0} is in unknown format: {1}", nameof(s), s.ToString()));
        }
        /// <inheritdoc/>
        public static Rational Parse(string s, NumberStyles style, IFormatProvider? provider)
        {
            if (s is null) throw new ArgumentNullException(nameof(s));
            return Parse(s.AsSpan(), style, provider);
        }
        /// <inheritdoc/>
        public static Rational Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, NumberStyles.Any, provider);
        /// <inheritdoc/>
        public static Rational Parse(string s, IFormatProvider? provider) => Parse(s, NumberStyles.Any, provider);
        /// <inheritdoc/>
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out Rational result)
        {
            try
            {
                result = Parse(s, style, provider);
                return true;
            }
            catch (Exception)
            {
                result = default;
                return false;
            }
        }
        /// <inheritdoc/>
        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out Rational result) => TryParse(s.AsSpan(), style, provider, out result);
        /// <inheritdoc/>
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Rational result) => TryParse(s, NumberStyles.Any, provider, out result);
        /// <inheritdoc/>
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Rational result) => TryParse(s, NumberStyles.Any, provider, out result);
        /// <inheritdoc/>
        static bool INumberBase<Rational>.TryConvertFromChecked<TOther>(TOther value, out Rational result)
        {
            result = NaN;
            bool converted = true;
            if (value is byte b) result = new((BigInteger)b);
            else if (value is sbyte sb) result = new((BigInteger)sb);
            else if (value is short s) result = new((BigInteger)s);
            else if (value is ushort us) result = new((BigInteger)us);
            else if (value is int i) result = new((BigInteger)i);
            else if (value is uint ui) result = new((BigInteger)ui);
            else if (value is long l) result = new((BigInteger)l);
            else if (value is ulong ul) result = new((BigInteger)ul);
            else if (value is Int128 i128) result = new((BigInteger)i128);
            else if (value is UInt128 u128) result = new((BigInteger)u128);
            else if (value is BigInteger bi) result = new(bi);
            else if (value is Half h) result = From(h);
            else if (value is float f) result = From(f);
            else if (value is double d) result = From(d);
            else if (value is decimal de) result = From(de);
            else if (value is Complex c)
            {
                if (c.Imaginary != 0d)
                    throw new OverflowException(string.Format("Value {0} cannot be safely convert to Rational", value));
                else
                    result = From(c);
            }
            else if (value is char ch) result = new((BigInteger)ch);
            else converted = false;

            return converted;
        }
        /// <inheritdoc/>
        static bool INumberBase<Rational>.TryConvertFromSaturating<TOther>(TOther value, out Rational result)
        {
            result = NaN;
            bool converted = true;
            if (value is byte b) result = new((BigInteger)b);
            else if (value is sbyte sb) result = new((BigInteger)sb);
            else if (value is short s) result = new((BigInteger)s);
            else if (value is ushort us) result = new((BigInteger)us);
            else if (value is int i) result = new((BigInteger)i);
            else if (value is uint ui) result = new((BigInteger)ui);
            else if (value is long l) result = new((BigInteger)l);
            else if (value is ulong ul) result = new((BigInteger)ul);
            else if (value is Int128 i128) result = new((BigInteger)i128);
            else if (value is UInt128 u128) result = new((BigInteger)u128);
            else if (value is BigInteger bi) result = new(bi);
            else if (value is Half h) result = From(h);
            else if (value is float f) result = From(f);
            else if (value is double d) result = From(d);
            else if (value is decimal de) result = From(de);
            else if (value is Complex c) result = From(c);
            else if (value is char ch) result = new((BigInteger)ch);
            else converted = false;
            return converted;
        }
        /// <inheritdoc/>
        static bool INumberBase<Rational>.TryConvertFromTruncating<TOther>(TOther value, out Rational result)
        {
            result = NaN;
            bool converted = true;
            if (value is byte b) result = new((BigInteger)b);
            else if (value is sbyte sb) result = new((BigInteger)sb);
            else if (value is short s) result = new((BigInteger)s);
            else if (value is ushort us) result = new((BigInteger)us);
            else if (value is int i) result = new((BigInteger)i);
            else if (value is uint ui) result = new((BigInteger)ui);
            else if (value is long l) result = new((BigInteger)l);
            else if (value is ulong ul) result = new((BigInteger)ul);
            else if (value is Int128 i128) result = new((BigInteger)i128);
            else if (value is UInt128 u128) result = new((BigInteger)u128);
            else if (value is BigInteger bi) result = new(bi);
            else if (value is Half h) result = From(h);
            else if (value is float f) result = From(f);
            else if (value is double d) result = From(d);
            else if (value is decimal de) result = From(de);
            else if (value is Complex c) result = From(c);
            else if (value is char ch) result = new((BigInteger)ch);
            else converted = false;
            return converted;
        }
        /// <inheritdoc/>
        static bool INumberBase<Rational>.TryConvertToChecked<TOther>(Rational value, out TOther result)
        {
            try
            {
                result = (dynamic)value;
                return true;
            }
            catch (OverflowException e)
            {
                throw e;
            }
            catch (Exception)
            {
#pragma warning disable CS8601 // Możliwe przypisanie odwołania o wartości null.
                result = default;
#pragma warning restore CS8601 // Możliwe przypisanie odwołania o wartości null.
                return false;
            }
        }
        /// <inheritdoc/>
        static bool INumberBase<Rational>.TryConvertToSaturating<TOther>(Rational value, out TOther result)
        {
            try
            {
                result = (dynamic)value;
                return true;
            }
            catch (Exception)
            {
#pragma warning disable CS8601 // Możliwe przypisanie odwołania o wartości null.
                result = default;
#pragma warning restore CS8601 // Możliwe przypisanie odwołania o wartości null.
                return false;
            }
        }
        /// <inheritdoc/>
        static bool INumberBase<Rational>.TryConvertToTruncating<TOther>(Rational value, out TOther result)
        {
            try
            {
                result = (dynamic)value;
                return true;
            }
            catch (Exception)
            {
#pragma warning disable CS8601 // Możliwe przypisanie odwołania o wartości null.
                result = default;
#pragma warning restore CS8601 // Możliwe przypisanie odwołania o wartości null.
                return false;
            }
        }
        /// <inheritdoc/>
        public int CompareTo(object? obj)
        {
            if (obj is null) return 1;
            else if (obj is byte b) return CompareTo((ulong)b);
            else if (obj is sbyte sb) return CompareTo(sb);
            else if (obj is short s) return CompareTo(s);
            else if (obj is ushort us) return CompareTo((ulong)us);
            else if (obj is int i) return CompareTo(i);
            else if (obj is uint ui) return CompareTo((ulong)ui);
            else if (obj is long l) return CompareTo(l);
            else if (obj is ulong ul) return CompareTo(ul);
            else if (obj is Int128 i128) return CompareTo(i128);
            else if (obj is UInt128 u128) return CompareTo(u128);
            else if (obj is BigInteger bi) return CompareTo(bi);
            else if (obj is Half h) return CompareTo(From(h));
            else if (obj is float f) return CompareTo(From(f));
            else if (obj is double d) return CompareTo(From(d));
            else if (obj is decimal de) return CompareTo(From(de));
            else if (obj is Complex c) return CompareTo(From(c));
            else if (obj is char ch) return CompareTo(ch);
            else if (obj is Rational r) return CompareTo(r);
            else throw new ArgumentException(string.Format("Cannot compare Rational to {0}", obj.GetType().Name), nameof(obj));
        }
        /// <summary>
        /// <inheritdoc cref="CompareTo(Rational)"/>
        /// </summary>
        /// <param name="other"></param>
        /// <returns><inheritdoc cref="CompareTo(Rational)"/></returns>
        public int CompareTo(Rational other)
        {
            var l = Numerator * other.Denominator;
            var r = other.Numerator * Denominator;
            return l.CompareTo(r);
        }
        /// <summary>
        /// <inheritdoc cref="CompareTo(Rational)"/>
        /// </summary>
        /// <param name="other"></param>
        /// <returns><inheritdoc cref="CompareTo(Rational)"/></returns>
        public int CompareTo(long other) => Numerator.CompareTo(other * Denominator);
        /// <summary>
        /// <inheritdoc cref="CompareTo(Rational)"/>
        /// </summary>
        /// <param name="other"></param>
        /// <returns><inheritdoc cref="CompareTo(Rational)"/></returns>
        [CLSCompliant(false)]
        public int CompareTo(ulong other) => Numerator.CompareTo(other * Denominator);
        /// <summary>
        /// <inheritdoc cref="CompareTo(Rational)"/>
        /// </summary>
        /// <param name="other"></param>
        /// <returns><inheritdoc cref="CompareTo(Rational)"/></returns>
        public int CompareTo(BigInteger other) => Numerator.CompareTo(other * Denominator);

        /// <inheritdoc/>
        public bool Equals(Rational other) => Numerator == other.Numerator && Denominator == other.Denominator;

        /// <inheritdoc/>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (format == null || format == "F")
                return string.Format("{0}/{1}", Numerator, Denominator);
            else if (format.StartsWith('D'))
            {
                int dec = format[1..].Length > 0 ? int.Parse(format[1..]) : 15;
                var pow10 = BigInteger.Pow(10, dec);
                var w = Numerator / Denominator;
                var f = BigInteger.Abs(Numerator) % Denominator;
                f = f * pow10 / Denominator;
                return string.Format("{0}.{1}", w, f.ToString().PadLeft(dec, '0'));
            }
            else if (format.StartsWith('W'))
            {
                var w = Numerator / Denominator;
                var f = BigInteger.Abs(Numerator) % Denominator;
                return string.Format("{0} {1}/{2}", w, f, Denominator);
            }
            throw new FormatException();
        }
        /// <inheritdoc/>
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public static Rational operator +(Rational value) => value;
        /// <inheritdoc/>
        public static Rational operator +(Rational left, Rational right)
        {
            var n = left.Numerator * right.Denominator + left.Denominator * right.Numerator;
            var d = left.Denominator * right.Denominator;
            return new(n, d);
        }
        /// <inheritdoc/>
        public static Rational operator -(Rational value) => new(-value.Numerator, value.Denominator);
        /// <inheritdoc/>
        public static Rational operator -(Rational left, Rational right)
        {
            var n = left.Numerator * right.Denominator - left.Denominator * right.Numerator;
            var d = left.Denominator * right.Denominator;
            return new(n, d);
        }
        /// <inheritdoc/>
        public static Rational operator ++(Rational value) => new(value.Numerator + value.Denominator, value.Denominator);
        /// <inheritdoc/>
        public static Rational operator --(Rational value) => new(value.Numerator - value.Denominator, value.Denominator);
        /// <inheritdoc/>
        public static Rational operator *(Rational left, Rational right)
        {
            var n = right.Numerator * left.Numerator;
            var d = right.Denominator * left.Denominator;
            return new(n, d);
        }
        /// <inheritdoc/>
        public static Rational operator /(Rational left, Rational right)
        {
            var n = left.Numerator * right.Denominator;
            var d = left.Denominator * right.Numerator;
            return new(n, d);
        }
        /// <inheritdoc/>
        public static Rational operator %(Rational left, Rational right)
        {
            // left - right * Truncate(left / right);
            var ln = left.Numerator;
            var ld = left.Denominator;
            var rn = right.Numerator;
            var rd = right.Denominator;
            var a = (ln * rd) / (ld * rn);
            var n = rn * a;
            var d = rd * ld;
            n = ln * rd - n * ld;
            return new(n, d);
        }


        /// <summary>
        /// Adds the values of <see cref="Rational"/> object and <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="left">The first value to add</param>
        /// <param name="right">The second value to add</param>
        /// <returns>The sum of <paramref name="left"/> and <paramref name="right"/></returns>
        public static Rational operator +(Rational left, BigInteger right) => new(left.Numerator + right * left.Denominator, left.Denominator);
        /// <summary>
        /// Subtracts a <see cref="BigInteger"/> value from another <see cref="Rational"/> value.
        /// </summary>
        /// <param name="left">The value to subtract from (the minuend).</param>
        /// <param name="right">The value to subtract (the subtrahend)</param>
        /// <returns>The result of subtracting <paramref name="right"/> from <paramref name="left"/></returns>
        public static Rational operator -(Rational left, BigInteger right) => new(left.Numerator - right * left.Denominator, left.Denominator);
        /// <summary>
        /// Multiplies <see cref="Rational"/> value and <see cref="BigInteger"/> value.
        /// </summary>
        /// <param name="left">The first value to multiply</param>
        /// <param name="right">The second value to multiply</param>
        /// <returns>The product of <paramref name="left"/> and <paramref name="right"/></returns>
        public static Rational operator *(Rational left, BigInteger right) => new(left.Numerator * right, left.Denominator);
        /// <summary>
        /// Divides a specified <see cref="Rational"/> value by another specified <see cref="BigInteger"/> value.
        /// </summary>
        /// <param name="left">The dividend</param>
        /// <param name="right">The divisor</param>
        /// <returns>The result of dividing <paramref name="left"/> by <paramref name="right"/></returns>
        public static Rational operator /(Rational left, BigInteger right) => new(left.Numerator, left.Denominator * right);
        /// <summary>
        /// Returns the remainder resulting from dividing <see cref="Rational"/> value by <see cref="BigInteger"/> value.
        /// </summary>
        /// <param name="left">The dividend</param>
        /// <param name="right">The divisor</param>
        /// <returns>The remainder resulting from dividing <paramref name="left"/> by <paramref name="right"/></returns>
        public static Rational operator %(Rational left, BigInteger right)
        {
            // left - right * Truncate(left / right);
            var ln = left.Numerator;
            var ld = left.Denominator;
            var a = ln / (ld * right);
            var n = right * a;
            return new(ln - n * ld, ld);
        }

        /// <summary>
        /// Adds the values of <see cref="BigInteger"/> object and <see cref="Rational"/> object.
        /// </summary>
        /// <param name="left">The first value to add</param>
        /// <param name="right">The second value to add</param>
        /// <returns>The sum of <paramref name="left"/> and <paramref name="right"/></returns>
        public static Rational operator +(BigInteger left, Rational right) => new(left * right.Denominator + right.Numerator, right.Denominator);
        /// <summary>
        /// Subtracts a <see cref="Rational"/> value from another <see cref="BigInteger"/> value.
        /// </summary>
        /// <param name="left">The value to subtract from (the minuend).</param>
        /// <param name="right">The value to subtract (the subtrahend)</param>
        /// <returns>The result of subtracting <paramref name="right"/> from <paramref name="left"/></returns>
        public static Rational operator -(BigInteger left, Rational right) => new(left * right.Denominator - right.Numerator, right.Denominator);
        /// <summary>
        /// Multiplies <see cref="BigInteger"/> value and <see cref="Rational"/> value.
        /// </summary>
        /// <param name="left">The first value to multiply</param>
        /// <param name="right">The second value to multiply</param>
        /// <returns>The product of <paramref name="left"/> and <paramref name="right"/></returns>
        public static Rational operator *(BigInteger left, Rational right) => new(left * right.Numerator, right.Denominator);
        /// <summary>
        /// Divides a specified <see cref="BigInteger"/> value by another specified <see cref="Rational"/> value.
        /// </summary>
        /// <param name="left">The dividend</param>
        /// <param name="right">The divisor</param>
        /// <returns>The result of dividing <paramref name="left"/> by <paramref name="right"/></returns>
        public static Rational operator /(BigInteger left, Rational right) => new(left * right.Denominator, right.Numerator);
        /// <summary>
        /// Returns the remainder resulting from dividing <see cref="BigInteger"/> value by <see cref="Rational"/> value.
        /// </summary>
        /// <param name="left">The dividend</param>
        /// <param name="right">The divisor</param>
        /// <returns>The remainder resulting from dividing <paramref name="left"/> by <paramref name="right"/></returns>
        public static Rational operator %(BigInteger left, Rational right)
        {
            // left - right * Truncate(left / right);
            var rn = right.Numerator;
            var rd = right.Denominator;
            var a = left * rd / rn;
            var n = a * rn;
            return new(left * rd - n, rd);
        }

        /// <inheritdoc/>
        public static bool operator ==(Rational left, Rational right) => left.Equals(right);
        /// <inheritdoc/>
        public static bool operator !=(Rational left, Rational right) => !left.Equals(right);
        /// <inheritdoc/>
        public static bool operator <(Rational left, Rational right) => left.CompareTo(right) < 0;
        /// <inheritdoc/>
        public static bool operator >(Rational left, Rational right) => left.CompareTo(right) > 0;
        /// <inheritdoc/>
        public static bool operator <=(Rational left, Rational right) => left.CompareTo(right) <= 0;
        /// <inheritdoc/>
        public static bool operator >=(Rational left, Rational right) => left.CompareTo(right) >= 0;

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            else if (obj is byte b) return Equals((ulong)b);
            else if (obj is sbyte sb) return Equals(sb);
            else if (obj is short s) return Equals(s);
            else if (obj is ushort us) return Equals((ulong)us);
            else if (obj is int i) return Equals(i);
            else if (obj is uint ui) return Equals((ulong)ui);
            else if (obj is long l) return Equals(l);
            else if (obj is ulong ul) return Equals(ul);
            else if (obj is Int128 i128) return Equals(i128);
            else if (obj is UInt128 u128) return Equals(u128);
            else if (obj is BigInteger bi) return Equals(bi);
            else if (obj is Half h) return Equals(From(h));
            else if (obj is float f) return Equals(From(f));
            else if (obj is double d) return Equals(From(d));
            else if (obj is decimal de) return Equals(From(de));
            else if (obj is Complex c) return Equals(From(c));
            else if (obj is char ch) return Equals(ch);
            else if (obj is Rational r) return Equals(r);
            else return base.Equals(obj);
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance and <see cref="long"/> have the same value.
        /// </summary>
        /// <param name="other">A <see cref="long"/> value</param>
        /// <returns><see langword="true"/> if the current instance and <see cref="long"/> have the same value; otherwise, <see langword="false"/></returns>
        public bool Equals(long other) => Denominator.IsOne && Numerator.Equals(other);
        /// <summary>
        /// Returns a value that indicates whether the current instance and <see cref="ulong"/> have the same value.
        /// </summary>
        /// <param name="other">A <see cref="ulong"/> value</param>
        /// <returns><see langword="true"/> if the current instance and <see cref="ulong"/> have the same value; otherwise, <see langword="false"/></returns>
        [CLSCompliant(false)]
        public bool Equals(ulong other) => Denominator.IsOne && Numerator.Equals(other);
        /// <summary>
        /// Returns a value that indicates whether the current instance and <see cref="BigInteger"/> object have the same value.
        /// </summary>
        /// <param name="other">A <see cref="BigInteger"/> object to compare</param>
        /// <returns><see langword="true"/> if the current instance and <see cref="BigInteger"/> have the same value; otherwise, <see langword="false"/></returns>
        public bool Equals(BigInteger other) => Denominator.IsOne && Numerator.Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Numerator, Denominator);

        [GeneratedRegex("^([+-]?\\d+)\\s(\\d+)\\/(\\d+)$")]
        private static partial Regex IntFractionRegex();
        [GeneratedRegex("^([+-]?\\d+)$")]
        private static partial Regex IntegerRegex();
        [GeneratedRegex("^([0-9]+)\\.?([0-9]*)[eE]?([+-]?[0-9]*)$")]
        private static partial Regex DecimalRegex();
        [GeneratedRegex("^([+-]?\\d+)\\/(\\d+)$")]
        private static partial Regex FractionRegex();

        /// <summary>
        /// Converts <see cref="System.Half"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static Rational From(Half x)
        {
            short hval = BitConverter.HalfToInt16Bits(x);
            bool sign = (hval < 0);
            short exp = (short)((hval >> 10) & half_exp_mask);
            short fraction = (short)(hval & half_matissa_mask);
            BigInteger n, d;
            if (exp == 0)
            {
                if (fraction == 0)
                {
                    n = 0;
                    d = 1;
                }
                else
                {
                    n = fraction;
                    d = BigInteger.Pow(2, 24);
                }
            }
            else if (exp == 0x1f)
            {
                n = fraction == 0 ? 1 : 0;
                d = 0;
            }
            else
            {
                n = fraction + half_default_denominator;
                d = half_default_denominator;
                exp -= 25;
                if (exp < 0)
                {
                    d <<= (-exp);
                }
                else
                {
                    n <<= exp;
                }
            }
            return new(sign ? -n : n, d);
        }
        /// <summary>
        /// Converts <see cref="float"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static Rational From(float x)
        {
            bool sign = (x < 0);
            int fval = BitConverter.SingleToInt32Bits(System.Math.Abs(x));
            int exp = (fval >> 23) & float_exp_mask;
            int fraction = fval & float_mantissa_mask;
            BigInteger n, d;
            if (exp == 0)
            {
                if (fraction == 0)
                {
                    n = 0;
                    d = 1;
                }
                else
                {
                    n = fraction;
                    d = BigInteger.Pow(2, 126);
                }
            }
            else if (exp == 0xff)
            {
                n = fraction == 0 ? 1 : 0;
                d = 0;
            }
            else
            {
                n = fraction + float_default_denominator;
                d = float_default_denominator;
                exp -= 127;
                if (exp < 0)
                {
                    d <<= (-exp);
                }
                else
                {
                    n <<= exp;
                }
            }
            return new Rational(sign ? -n : n, d);
        }
        /// <summary>
        /// Converts <see cref="double"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static Rational From(double x)
        {
            bool sign = (x < 0);
            long dval = BitConverter.DoubleToInt64Bits(System.Math.Abs(x));
            int exp = (int)((dval >> 52) & double_exp_mask);
            long fraction = dval & double_mantissa_mask;
            BigInteger n, d;
            if (exp == 0)
            {
                if (fraction == 0)
                {
                    n = 0;
                    d = 1;
                }
                else
                {
                    n = fraction;
                    d = BigInteger.Pow(2, 1022);
                }
            }
            else if (exp == 0x7ff)
            {
                n = fraction == 0 ? 1 : 0;
                d = 0;
            }
            else
            {
                n = fraction + double_default_denominator;
                d = double_default_denominator;
                exp -= 1023;
                if (exp < 0)
                {
                    d <<= (-exp);
                }
                else
                {
                    n <<= exp;
                }
            }
            return new Rational(sign ? -n : n, d);
        }
        /// <summary>
        /// Converts <see cref="decimal"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static Rational From(decimal x)
        {
            int[] parts = decimal.GetBits(x);
            bool sign = x < 0;
            int scale = (parts[3] >> 16) & 0x7f;
            BigInteger n = (uint)parts[2];
            n = (n << 32) | (uint)parts[1];
            n = (n << 32) | (uint)parts[0];
            BigInteger d = BigInteger.Pow(10, scale);
            return new Rational(sign ? -n : n, d);
        }
        /// <summary>
        /// Converts <see cref="Complex"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="complex">The value to convert to <see cref="Rational"/></param>
        public static Rational From(Complex complex) => From(complex.Real);

        /// <summary>
        /// Converts <see cref="Rational">this</see> to <see cref="BigInteger"/> value
        /// </summary>
        /// <returns>The value of this as <see cref="BigInteger"/>. Truncate if necessary</returns>
        /// <exception cref="OverflowException"></exception>
        public BigInteger ToBigInteger()
        {
            if (Denominator.IsZero) throw new OverflowException();
            return Numerator / Denominator;
        }
        /// <summary>
        /// Converts <see cref="Rational">this</see> to <see cref="Half"/> value
        /// </summary>
        /// <returns>The value of this as <see cref="Half"/>. Truncate if necessary</returns>
        public Half ToHalf()
        {
            var s = Numerator < 0;
            var a = BigInteger.Abs(Numerator);
            BigInteger q;
            q = BigInteger.DivRem(a, Denominator, out BigInteger r);
            BigInteger d = Denominator;
            var noOfBits = BigInteger.Log(d, 2);
            if (noOfBits > 25)
            {
                var move = (int)(noOfBits - 12);
                r >>= move;
                d >>= move;
            }
            float di, dr, dd;
            di = (float)q;
            dr = (float)r;
            dd = (float)d;
            float _;
            unchecked
            {
                _ = di + dr / dd;
            }
            return (Half)(s ? -_ : _);
        }
        /// <summary>
        /// Converts <see cref="Rational">this</see> to <see cref="float"/> value
        /// </summary>
        /// <returns>The value of this as <see cref="float"/>. Truncate if necessary</returns>
        public float ToFloat()
        {
            var s = Numerator < 0;
            var a = BigInteger.Abs(Numerator);
            BigInteger q;
            q = BigInteger.DivRem(a, Denominator, out BigInteger r);
            BigInteger d = Denominator;
            var noOfBits = BigInteger.Log(d, 2);
            if (noOfBits > 127)
            {
                var move = (int)(noOfBits - 64);
                r >>= move;
                d >>= move;
            }
            float di, dr, dd;
            di = (float)q;
            dr = (float)r;
            dd = (float)d;
            float _;
            unchecked
            {
                _ = di + dr / dd;
            }
            return s ? -_ : _;
        }
        /// <summary>
        /// Converts <see cref="Rational">this</see> to <see cref="double"/> value
        /// </summary>
        /// <returns>The value of this as <see cref="double"/>. Truncate if necessary</returns>
        public double ToDouble()
        {
            var s = Numerator < 0;
            var a = BigInteger.Abs(Numerator);
            BigInteger q;
            q = BigInteger.DivRem(a, Denominator, out BigInteger r);
            BigInteger d = Denominator;
            var noOfBits = BigInteger.Log(d, 2);
            if (noOfBits > 1023)
            {
                var move = (int)(noOfBits - 512);
                r >>= move;
                d >>= move;
            }
            double di, dr, dd;
            di = (double)q;
            dd = (double)d;
            dr = (double)r;
            double _;
            unchecked
            {
                _ = di + dr / dd;
            }
            return s ? -_ : _;
        }
        /// <summary>
        /// Converts <see cref="Rational">this</see> to <see cref="decimal"/> value
        /// </summary>
        /// <returns>The value of this as <see cref="decimal"/>. Truncate if necessary</returns>
        public decimal ToDecimal()
        {
            var s = Numerator < 0;
            var a = BigInteger.Abs(Numerator);
            BigInteger q;
            q = BigInteger.DivRem(a, Denominator, out BigInteger r);
            BigInteger d = Denominator;
            var noOfDec = BigInteger.Log10(d);
            if (noOfDec > 28)
            {
                var e = (int)(noOfDec - 27);
                var div = BigInteger.Pow(10, e);
                r /= div;
                d /= div;
            }
            decimal di, dr, dd;
            di = (decimal)q;
            dr = (decimal)r;
            dd = (decimal)d;
            decimal _ = di + dr / dd;
            return s ? -_ : _;
        }
        /// <summary>
        /// Converts <see cref="Rational">this</see> to <see cref="Complex"/> value
        /// </summary>
        /// <returns>The value of this as <see cref="Complex"/>. Truncate if necessary</returns>
        public Complex ToComplex() => new(ToDouble(), 0d);

        /// <summary>
        /// Deconstructs <see cref="Rational">this</see> into numerator and denominator.
        /// </summary>
        /// <param name="n">Value of numerator as <see cref="BigInteger"/></param>
        /// <param name="d">Value of denominator as <see cref="BigInteger"/></param>
        public void Deconstruct(out BigInteger n, out BigInteger d)
        {
            n = Numerator;
            d = Denominator;
        }

        /// <summary>
        /// Create new instance of <see cref="Rational"/> from continued fraction form
        /// </summary>
        /// <param name="values">A continued fraction</param>
        /// <returns></returns>
        public static Rational FromContinuedForm<T>(IEnumerable<T> values) where T : IBinaryInteger<T>
        {
            var f = PositiveInfinity;
            foreach (var i in values.Reverse())
                f = BigInteger.CreateChecked(i) + Inverse(f);
            return f;
        }

        /// <summary>
        /// Invert the value of <see cref="Rational"/> object
        /// </summary>
        /// <param name="value">A value to invert</param>
        /// <returns>Returns a inverted value of <paramref name="value"/>. Effectively: <c>1/<paramref name="value"/></c></returns>

        public static Rational Inverse(Rational value) => new(value.Denominator, value.Numerator);

        /// <summary>
        /// Defines an implicit conversion of <see cref="System.Half"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(Half x) => From(x);
        /// <summary>
        /// Defines an implicit conversion of <see cref="float"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(float x) => From(x);
        /// <summary>
        /// Defines an implicit conversion of <see cref="double"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(double x) => From(x);
        /// <summary>
        /// Defines an implicit conversion of <see cref="decimal"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(decimal x) => From(x);
        /// <summary>
        /// Defines an explicit conversion of <see cref="Complex"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static explicit operator Rational(Complex x) => From(x);
        /// <summary>
        /// Defines an implicit conversion of <see cref="BigInteger"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="i">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(BigInteger i) => new(i);
        /// <summary>
        /// Defines an implicit conversion of <see cref="int"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="i">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(int i) => new(i);
        /// <summary>
        /// Defines an implicit conversion of <see cref="uint"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="i">The value to convert to <see cref="Rational"/></param>
        [CLSCompliant(false)]
        public static implicit operator Rational(uint i) => new(i);
        /// <summary>
        /// Defines an implicit conversion of <see cref="long"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="i">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(long i) => new(i);
        /// <summary>
        /// Defines an implicit conversion of <see cref="ulong"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="i">The value to convert to <see cref="Rational"/></param>
        [CLSCompliant(false)]
        public static implicit operator Rational(ulong i) => new(i);

        /// <summary>
        /// Defines an implicit conversion of <see cref="Int128"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="i">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(Int128 i) => new(i);
        /// <summary>
        /// Defines an implicit conversion of <see cref="UInt128"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="i">The value to convert to <see cref="Rational"/></param>
        [CLSCompliant(false)]
        public static implicit operator Rational(UInt128 i) => new(i);

        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="BigInteger"/> object
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="BigInteger"/></param>
        /// <exception cref="DivideByZeroException"><paramref name="value"/> is not valid <see cref="Rational"/></exception>
        public static explicit operator BigInteger(Rational value) => value.ToBigInteger();
        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="int"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="int"/></param>
        /// <exception cref="DivideByZeroException"><paramref name="value"/> is not valid <see cref="Rational"/></exception>
        /// <exception cref="OverflowException"><paramref name="value"/> is less than <see cref="int.MinValue"/> or greater than <see cref="int.MaxValue"/></exception>
        public static explicit operator int(Rational value) => (int)value.ToBigInteger();

        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="uint"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="uint"/></param>
        /// <exception cref="DivideByZeroException"><paramref name="value"/> is not valid <see cref="Rational"/></exception>
        /// <exception cref="OverflowException"><paramref name="value"/> is less than <see cref="uint.MinValue"/> or greater than <see cref="uint.MaxValue"/></exception>
        [CLSCompliant(false)]
        public static explicit operator uint(Rational value) => (uint)value.ToBigInteger();
        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="long"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="long"/></param>
        /// <exception cref="DivideByZeroException"><paramref name="value"/> is not valid <see cref="Rational"/></exception>
        /// <exception cref="OverflowException"><paramref name="value"/> is less than <see cref="long.MinValue"/> or greater than <see cref="long.MaxValue"/></exception>
        public static explicit operator long(Rational value) => (long)value.ToBigInteger();

        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="ulong"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="ulong"/></param>
        /// <exception cref="DivideByZeroException"><paramref name="value"/> is not valid <see cref="Rational"/></exception>
        /// <exception cref="OverflowException"><paramref name="value"/> is less than <see cref="ulong.MinValue"/> or greater than <see cref="ulong.MaxValue"/></exception>
        [CLSCompliant(false)]
        public static explicit operator ulong(Rational value) => (ulong)value.ToBigInteger();

        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="Int128"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="Int128"/></param>
        /// <exception cref="DivideByZeroException"><paramref name="value"/> is not valid <see cref="Rational"/></exception>
        /// <exception cref="OverflowException"><paramref name="value"/> is less than <see cref="Int128.MinValue"/> or greater than <see cref="Int128.MaxValue"/></exception>
        public static explicit operator Int128(Rational value) => (Int128)value.ToBigInteger();

        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="UInt128"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="UInt128"/></param>
        /// <exception cref="DivideByZeroException"><paramref name="value"/> is not valid <see cref="Rational"/></exception>
        /// <exception cref="OverflowException"><paramref name="value"/> is less than <see cref="UInt128.MinValue"/> or greater than <see cref="UInt128.MaxValue"/></exception>
        [CLSCompliant(false)]
        public static explicit operator UInt128(Rational value) => (UInt128)value.ToBigInteger();

        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="System.Half"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="System.Half"/></param>
        public static explicit operator Half(Rational value) => value.ToHalf();

        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="float"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="float"/></param>
        public static explicit operator float(Rational value) => value.ToFloat();

        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="double"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="double"/></param>
        public static explicit operator double(Rational value) => value.ToDouble();

        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="decimal"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="decimal"/></param>
        /// <exception cref="OverflowException"><paramref name="value"/> is less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/> </exception>
        public static explicit operator decimal(Rational value) => value.ToDecimal();

        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="Complex"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="Complex"/></param>
        public static explicit operator Complex(Rational value) => value.ToComplex();
        /// <summary>
        /// Converts the numeric value of the current <see cref="Rational"/> object to its equivalent string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(null, null);

        /// <summary>
        /// Calculates the integral part of specified <see cref="Rational"/> number.
        /// </summary>
        /// <param name="value">A number to truncate.</param>
        /// <returns>
        /// The integral part of <paramref name="value"/>. 
        /// Discard any fractional part of number.
        /// Preserves non valid values.
        /// </returns>
        public static Rational Truncate(Rational value)
        {
            if (!IsFinite(value)) return value;
            if (IsInteger(value)) return value.Numerator;
            return new(value.ToBigInteger());
        }
        /// <summary>
        /// Returns the largest integral value less than or equal to the specified <see cref="Rational"/> number.
        /// </summary>
        /// <param name="value">A <see cref="Rational"/> number</param>
        /// <returns>The largest integral value less than or equal to <paramref name="value"/>. Preserves non valid values.</returns>
        public static Rational Floor(Rational value)
        {
            if (!IsFinite(value)) return value;
            if (IsInteger(value)) return value.Numerator;
            var v = BigInteger.Abs(value.Numerator) / value.Denominator;
            var s = value.Numerator < 0;
            if (s) v++;
            return new(s ? -v : v);
        }
        /// <summary>
        /// Returns the smallest integral value greater than or equal to the specified <see cref="Rational"/> number.
        /// </summary>
        /// <param name="value">A <see cref="Rational"/> number</param>
        /// <returns>The smallest integral value greater than or equal to <paramref name="value"/>. Preserves non valid values.</returns>
        public static Rational Ceiling(Rational value)
        {
            if (!IsFinite(value)) return value;
            if (IsInteger(value)) return value.Numerator;
            var v = BigInteger.Abs(value.Numerator) / value.Denominator;
            var s = value.Numerator < 0;
            if (!s) v++;
            return new(s ? -v : v);
        }
        /// <summary>
        /// Rounds a <see cref="Rational"/> value to nearest integral value.
        /// </summary>
        /// <param name="value">A <see cref="Rational"/> number to be rounded.</param>
        /// <returns>The integer nearest <paramref name="value"/>. Implemented as <c><see cref="Floor"/>(<paramref name="value"/> + <see cref="Half"/>)</c></returns>
        public static Rational Round(Rational value) => Floor(value + Half);

        /// <summary>
        /// Returns the larger of two <see cref="Rational"/> numbers.
        /// </summary>
        /// <param name="left">The first of two <see cref="Rational"/> number to compare.</param>
        /// <param name="right">The second of two <see cref="Rational"/> number to compare.</param>
        /// <returns>
        /// Parameter <paramref name="left"/> or <paramref name="right"/>, whichever is larger.
        /// </returns>
        public static Rational Max(Rational left, Rational right) => left.CompareTo(right) > 0 ? left : right;

        /// <summary>
        /// Returns the smaller of two <see cref="Rational"/> numbers.
        /// </summary>
        /// <param name="left">The first of two <see cref="Rational"/> number to compare.</param>
        /// <param name="right">The second of two <see cref="Rational"/> number to compare.</param>
        /// <returns>
        /// Parameter <paramref name="left"/> or <paramref name="right"/>, whichever is smaller.
        /// </returns>
        public static Rational Min(Rational left, Rational right) => left.CompareTo(right) < 0 ? left : right;

        /// <summary>
        /// Raises a <see cref="Rational"/> value to the power of specific value.
        /// </summary>
        /// <param name="value">The number to raise to the <paramref name="exp"/> power.</param>
        /// <param name="exp">The exponent to raise <paramref name="value"/> by.</param>
        /// <returns>The result of raising <paramref name="value"/> to the <paramref name="exp"/> power. Preserves non valid values.</returns>
        public static Rational Pow(Rational value, int exp)
        {
            if (IsNaN(value) || (exp == 1)) return value;
            else if (exp < 0) return Pow(Inverse(value), -exp);
            else if (exp == 0) return One;
            else
            {
                var n = BigInteger.Pow(value.Numerator, exp);
                var d = BigInteger.Pow(value.Denominator, exp);
                return new(n, d);
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
        /// <summary>
        /// Converts the string representation of a number to its <see cref="Rational"/> equivalent
        /// </summary>
        /// <param name="s">A string that contains the number to convert</param>
        /// <returns>A value that is equivalent to het number specified in the <paramref name="s"/> parameter</returns>
        /// <exception cref="FormatException"><paramref name="s"/> is not in known format</exception>
        public static Rational Parse(string s)
        {
            if (TryParse(s, null, out var r)) return r;
            throw new FormatException();
        }
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
        /// Gets a continued fraction form of the current <see cref="Rational"/> object
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> ToContinuedForm<T>() where T : IBinaryInteger<T>
        {
            var t = this;
            while (!IsZero(t))
            {
                BigInteger big = t.ToBigInteger();
                yield return T.CreateChecked(big);
                t -= big;
                if (IsZero(t)) break;
                t = Inverse(t);
            }
        }
    }
}
