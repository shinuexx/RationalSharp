using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;

namespace ShInUeXx.Numerics
{
    public readonly partial struct Rational
    {
        public BigInteger Numerator { get; private init; }
        public BigInteger Denominator { get; private init; }

        public static Rational Zero { get; } = new(BigInteger.Zero);
        public static Rational One { get; } = new(BigInteger.One);
        public static Rational MinusOne { get; } = new(BigInteger.MinusOne);
        public static Rational Half { get; } = new(BigInteger.One, 2);
        public static Rational MinusHalf { get; } = new(BigInteger.MinusOne, 2);
        public static Rational NaN { get; } = new();
        public static Rational PositiveInfinity { get; } = new(BigInteger.One, BigInteger.Zero);
        public static Rational NegativeInfinity { get; } = new(BigInteger.MinusOne, BigInteger.Zero);

        public bool IsZero => (!Denominator.IsZero) && (Numerator.IsZero);
        public bool IsNan => (Denominator.IsZero) && (Numerator.IsZero);
        public bool IsPositiveInfinity => (Denominator.IsZero) && (Numerator.Sign > 0);
        public bool IsNegativeInfinity => (Denominator.IsZero) && (Numerator.Sign < 0);
        public bool IsInfinity => (Denominator.IsZero) && (!Numerator.IsZero);

        public bool IsInteger => Denominator.IsOne;
        public bool IsValid => !Denominator.IsZero;

        public int Sign => Numerator.Sign;

        public Rational(BigInteger integer)
        {
            Numerator = integer;
            Denominator = BigInteger.One;
        }
        public Rational(BigInteger numerator, BigInteger denominator)
        {
            (Numerator, Denominator) = Simplify(MoveSign((numerator, denominator)));
        }
        public override string ToString() => string.Format("{0}/{1}", Numerator, Denominator);
        public override int GetHashCode() => HashCode.Combine(Numerator, Denominator);
        public override bool Equals(object obj)
        {
            if (obj is not Rational r) return false;
            return Equals(r);
        }
        public IEnumerable<int> GetContinuedForm()
        {
            var t = this;
            while (!t.IsZero)
            {
                BigInteger big = (BigInteger)t;
                yield return (int)big;
                t -= big;
                if (t.IsZero) break;
                t = Inverse(t);
            }
        }
    }
}
