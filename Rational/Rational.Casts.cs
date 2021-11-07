using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShInUeXx.Numerics
{
    public readonly partial struct Rational
    {
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

        public static implicit operator Rational(BigInteger other) => new(other);
        public static implicit operator Rational(int other) => new(other);
        public static implicit operator Rational(uint other) => new(other);
        public static implicit operator Rational(long other) => new(other);
        public static implicit operator Rational(ulong other) => new(other);

        public static explicit operator BigInteger(Rational value) => value.Numerator / value.Denominator;
        public static explicit operator long(Rational value) => (long)(BigInteger)value;
        public static explicit operator ulong(Rational value) => (ulong)(BigInteger)value;
        public static explicit operator int(Rational value) => (int)(BigInteger)value;
        public static explicit operator uint(Rational value) => (uint)(BigInteger)value;

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
            double _ = di + dr / dd;
            return s ? -_ : _;
        }
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
            float _ = di + dr / dd;
            return s ? -_ : _;
        }
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

        public static Rational FromContinuedForm(IEnumerable<int> values)
        {
            var f = PositiveInfinity;
            foreach (var i in values.Reverse())
            {
                f = i + Inverse(f);
            }
            return f;
        }
        public static Rational Parse(string value)
        {
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
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
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
