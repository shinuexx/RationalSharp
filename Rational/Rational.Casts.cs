using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace ShInUeXx.Numerics
{
    public readonly partial struct Rational
    {
        /// <summary>
        /// Defines an implicit conversion of <see cref="float"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(float x)
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
        /// Defines an implicit conversion of <see cref="double"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(double x)
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
        /// Defines an implicit conversion of <see cref="decimal"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(decimal value)
        {
            int[] parts = decimal.GetBits(value);
            bool sign = value < 0;
            int scale = (parts[3] >> 16) & 0x7f;
            BigInteger n = (uint)parts[2];
            n = (n << 32) | (uint)parts[1];
            n = (n << 32) | (uint)parts[0];
            BigInteger d = BigInteger.Pow(10, scale);
            return new Rational(sign ? -n : n, d);
        }

        /// <summary>
        /// Defines an implicit conversion of <see cref="BigInteger"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(BigInteger other) => new(other);

        /// <summary>
        /// Defines an implicit conversion of <see cref="int"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(int other) => new(other);

        /// <summary>
        /// Defines an implicit conversion of <see cref="uint"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(uint other) => new(other);

        /// <summary>
        /// Defines an implicit conversion of <see cref="long"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(long other) => new(other);

        /// <summary>
        /// Defines an implicit conversion of <see cref="ulong"/> value to <see cref="Rational"/> object
        /// </summary>
        /// <param name="x">The value to convert to <see cref="Rational"/></param>
        public static implicit operator Rational(ulong other) => new(other);


        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="BigInteger"/> object
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="BigInteger"/></param>
        /// <exception cref="DivideByZeroException"><paramref name="value"/> is not valid <see cref="Rational"/></exception>
        public static explicit operator BigInteger(Rational value) => value.Numerator / value.Denominator;

        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="long"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="long"/></param>
        /// <exception cref="DivideByZeroException"><paramref name="value"/> is not valid <see cref="Rational"/></exception>
        /// <exception cref="OverflowException"><paramref name="value"/> is less than <see cref="long.MinValue"/> or greater than <see cref="long.MaxValue"/></exception>
        public static explicit operator long(Rational value) => (long)(BigInteger)value;

        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="ulong"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="ulong"/></param>
        /// <exception cref="DivideByZeroException"><paramref name="value"/> is not valid <see cref="Rational"/></exception>
        /// <exception cref="OverflowException"><paramref name="value"/> is less than <see cref="ulong.MinValue"/> or greater than <see cref="ulong.MaxValue"/></exception>
        public static explicit operator ulong(Rational value) => (ulong)(BigInteger)value;

        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="int"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="int"/></param>
        /// <exception cref="DivideByZeroException"><paramref name="value"/> is not valid <see cref="Rational"/></exception>
        /// <exception cref="OverflowException"><paramref name="value"/> is less than <see cref="int.MinValue"/> or greater than <see cref="int.MaxValue"/></exception>
        public static explicit operator int(Rational value) => (int)(BigInteger)value;

        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="uint"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="uint"/></param>
        /// <exception cref="DivideByZeroException"><paramref name="value"/> is not valid <see cref="Rational"/></exception>
        /// <exception cref="OverflowException"><paramref name="value"/> is less than <see cref="uint.MinValue"/> or greater than <see cref="uint.MaxValue"/></exception>
        public static explicit operator uint(Rational value) => (uint)(BigInteger)value;

        /// <summary>
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="double"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="double"/></param>
        public static explicit operator double(Rational value)
        {
            var s = value.Numerator < 0;
            var a = BigInteger.Abs(value.Numerator);
            BigInteger q;
            q = BigInteger.DivRem(a, value.Denominator, out BigInteger r);
            BigInteger d = value.Denominator;
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
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="float"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="float"/></param>
        public static explicit operator float(Rational value)
        {
            var s = value.Numerator < 0;
            var a = BigInteger.Abs(value.Numerator);
            BigInteger q;
            q = BigInteger.DivRem(a, value.Denominator, out BigInteger r);
            BigInteger d = value.Denominator;
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
        /// Defines an explicit conversion of a <see cref="Rational"/> object to a <see cref="decimal"/> value
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="decimal"/></param>
        /// <exception cref="OverflowException"><paramref name="value"/> is less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/> </exception>
        public static explicit operator decimal(Rational value)
        {
            var s = value.Numerator < 0;
            var a = BigInteger.Abs(value.Numerator);
            BigInteger q;
            q = BigInteger.DivRem(a, value.Denominator, out BigInteger r);
            BigInteger d = value.Denominator;
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
        /// Create new instance of <see cref="Rational"/> from continued fraction form
        /// </summary>
        /// <param name="values">A continued fraction</param>
        /// <returns></returns>
        public static Rational FromContinuedForm(IEnumerable<int> values)
        {
            var f = PositiveInfinity;
            foreach (var i in values.Reverse())
            {
                f = i + Inverse(f);
            }
            return f;
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="Rational"/> equivalent
        /// </summary>
        /// <param name="value">A string that contains the number to convert</param>
        /// <returns>A value that is equivalent to het number specified in the <paramref name="value"/> parameter</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null</exception>
        /// <exception cref="FormatException"><paramref name="value"/> is not in known format</exception>
        public static Rational Parse(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            var trimmed = value.Trim();
            Match m;
            if (REGEXP_INT.IsMatch(trimmed))
            {
                m = REGEXP_INT.Match(trimmed);
                var n = BigInteger.Parse(m.Groups[1].Value);
                return new Rational(n);
            }
            else if (REGEXP_FRACTION.IsMatch(trimmed))
            {
                m = REGEXP_FRACTION.Match(trimmed);
                var n = BigInteger.Parse(m.Groups[1].Value);
                var d = BigInteger.Parse(m.Groups[2].Value);
                return new Rational(n, d);
            }
            else if (REGEXP_DECIMAL.IsMatch(trimmed))
            {
                m = REGEXP_DECIMAL.Match(trimmed);
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
                return new Rational(n, d);
            }
            else if (REGEXP_INT_FRACTION.IsMatch(trimmed))
            {
                m = REGEXP_INT_FRACTION.Match(trimmed);
                var i = BigInteger.Parse(m.Groups[1].Value);
                var n = BigInteger.Parse(m.Groups[2].Value);
                var d = BigInteger.Parse(m.Groups[3].Value);
                var s = i.Sign;
                n += BigInteger.Abs(i) * d;
                return new Rational(s * n, d);
            }
            else
            {
                throw new FormatException(string.Format("Parameter {0} is in unknown format: {1}", nameof(value), value));
            }
        }

        /// <summary>
        /// Tries to convert the string representation of a number to its <see cref="Rational"/> equivalent, and returns a value that indicates whether the conversion succeeded
        /// </summary>
        /// <param name="value">The string representation of a number</param>
        /// <param name="rational">
        /// When this method returns, contains the <see cref="Rational"/> equivalent to the number that is contained in <paramref name="value"/>, 
        /// or <see cref="NaN"/> is the convertion fails. 
        /// The convertion fails if the <paramref name="value"/> parametes is <c>null</c> or is not of the correct format.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns><c>true</c> if <paramref name="value"/> was converted successfully; otherwise, <c>false</c></returns>
        public static bool TryParse(string value, out Rational rational)
        {
            try
            {
                rational = Parse(value);
                return true;
            }
            catch (Exception)
            {
                rational = NaN;
                return false;
            }
        }
    }
}
