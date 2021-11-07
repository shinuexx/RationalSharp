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
        public static Rational Inverse(Rational value) => new(value.Denominator, value.Numerator);
        public static Rational Abs(Rational value) => new(BigInteger.Abs(value.Numerator), value.Denominator);
        public static Rational Negate(Rational value) => -value;
        public static Rational Floor(Rational value)
        {
            if (value.IsInteger) return value.Numerator;
            var v = BigInteger.Abs(value.Numerator) / value.Denominator;
            var s = value.Numerator < 0;
            if (s) v++;
            return s ? -v : v;
        }
        public static Rational Ceiling(Rational value)
        {
            if (value.IsInteger) return value.Numerator;
            var v = BigInteger.Abs(value.Numerator) / value.Denominator;
            var s = value.Numerator < 0;
            if (!s) v++;
            return s ? -v : v;
        }
        public static Rational Round(Rational value) => Floor(value + Half);
        public static Rational Truncate(Rational value)
        {
            if (!value.IsValid) return value;
            return value.Numerator / value.Denominator;
        }
        public static Rational Min(Rational left, Rational right) => left < right ? left : right;
        public static Rational Max(Rational left, Rational right) => left < right ? right : left;

        public static Rational Add(Rational left, Rational right) => left + right;
        public static Rational Subtract(Rational left, Rational right) => left - right;
        public static Rational Multiply(Rational left, Rational right) => left * right;
        public static Rational Divide(Rational dividend, Rational divisor) => dividend / divisor;
        public static Rational Remainder(Rational dividend, Rational divisor) => dividend % divisor;
        public static Rational Modulo(Rational left, Rational right) => left - right * Floor(left / right);
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
        public static double Log(Rational value, double @base) => BigInteger.Log(value.Numerator, @base) - BigInteger.Log(value.Denominator, @base);
        public static double Log(Rational value) => BigInteger.Log(value.Numerator) - BigInteger.Log(value.Denominator);
        public static double Log(Rational value, Rational @base) => Log(value) / Log(@base);
    }
}
