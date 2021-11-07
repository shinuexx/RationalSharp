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
        public static bool operator ==(Rational left, Rational right)
        {
            return !left.IsNan && !right.IsNan && left.Equals(right);
        }

        public static bool operator !=(Rational left, Rational right)
        {
            return !(left == right);
        }

        public static bool operator <(Rational left, Rational right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Rational left, Rational right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Rational left, Rational right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Rational left, Rational right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static Rational operator +(Rational value) => value;
        public static Rational operator -(Rational value) => new(-value.Numerator, value.Denominator);
        public static Rational operator ++(Rational value) => new(value.Numerator + value.Denominator, value.Denominator);
        public static Rational operator --(Rational value) => new(value.Numerator - value.Denominator, value.Denominator);

        public static Rational operator +(Rational left, Rational right)
        {
            var n = left.Numerator * right.Denominator + right.Numerator * left.Denominator;
            var d = left.Denominator * right.Denominator;
            return new Rational(n, d);
        }
        public static Rational operator -(Rational left, Rational right)
        {
            var n = left.Numerator * right.Denominator - right.Numerator * left.Denominator;
            var d = left.Denominator * right.Denominator;
            return new Rational(n, d);
        }
        public static Rational operator *(Rational left, Rational right)
        {
            var n = left.Numerator * right.Numerator;
            var d = left.Denominator * right.Denominator;
            return new Rational(n, d);
        }
        public static Rational operator /(Rational left, Rational right)
        {
            var n = left.Numerator * right.Denominator;
            var d = left.Denominator * right.Numerator;
            return new Rational(n, d);
        }
        public static Rational operator %(Rational left, Rational right)
        {
            return left - right * Rational.Truncate(left / right);
        }

        public static Rational operator +(Rational left, BigInteger right)
        {
            var n = left.Numerator + right * left.Denominator;
            return new Rational(n, left.Denominator);
        }
        public static Rational operator +(BigInteger left, Rational right)
        {
            var n = left * right.Denominator + right.Numerator;
            return new Rational(n, right.Denominator);
        }
        public static Rational operator -(Rational left, BigInteger right)
        {
            var n = left.Numerator - right * left.Denominator;
            return new Rational(n, left.Denominator);
        }
        public static Rational operator -(BigInteger left, Rational right)
        {
            var n = left * right.Denominator - right.Numerator;
            return new Rational(n, right.Denominator);
        }
        public static Rational operator *(Rational left, BigInteger right) => new(left.Numerator * right, left.Denominator);
        public static Rational operator *(BigInteger left, Rational right) => new(left * right.Numerator, right.Denominator);
        public static Rational operator /(Rational left, BigInteger right) => new(left.Numerator, left.Denominator * right);
        public static Rational operator /(BigInteger left, Rational right) => new(left * right.Denominator, right.Numerator);
        public static Rational operator %(Rational left, BigInteger right)
        {
            return left - right * Truncate(left / right);
        }
        public static Rational operator %(BigInteger left, Rational right)
        {
            return left - right * Truncate(left / right);
        }
    }
}
