using System;
using System.Collections.Generic;
using System.Numerics;

namespace ShInUeXx.Numerics
{
    /// <summary>
    /// Representing arbitrary rational as BigIntegers ratio
    /// </summary>
    public readonly partial struct Rational
    {
        /// <summary>
        /// Indicates numerator of rational
        /// </summary>
        public BigInteger Numerator { get; private init; }
        /// <summary>
        /// Indicates denominator of rational
        /// </summary>
        public BigInteger Denominator { get; private init; }

        /// <summary>
        /// Gets a value that represents the number 0 (zero)
        /// </summary>
        public static Rational Zero { get; } = new();
        /// <summary>
        /// Gets a value that represents the number 1 (one)
        /// </summary>
        public static Rational One { get; } = new(BigInteger.One);
        /// <summary>
        /// Gets a value that represents the number -1 (minus one)
        /// </summary>
        public static Rational MinusOne { get; } = new(BigInteger.MinusOne);
        /// <summary>
        /// Gets a value that represents the number 1/2 (half)
        /// </summary>
        public static Rational Half { get; } = new(BigInteger.One, 2);
        /// <summary>
        /// Gets a value that represents the number -1/2 (minus half)
        /// </summary>
        public static Rational MinusHalf { get; } = new(BigInteger.MinusOne, 2);
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
        /// Indicates whether the value of current <see cref="Rational"/> is <see cref="Zero"/>
        /// </summary>
        public bool IsZero => (!Denominator.IsZero) && (Numerator.IsZero);
        /// <summary>
        /// Indicates whether the value of current <see cref="Rational"/> is <see cref="NaN"/>
        /// </summary>
        public bool IsNan => (Denominator.IsZero) && (Numerator.IsZero);
        /// <summary>
        /// Indicates whether the value of current <see cref="Rational"/> is <see cref="PositiveInfinity"/>
        /// </summary>
        public bool IsPositiveInfinity => (Denominator.IsZero) && (Numerator.Sign > 0);
        /// <summary>
        /// Indicates whether the value of current <see cref="Rational"/> is <see cref="NegativeInfinity"/>
        /// </summary>
        public bool IsNegativeInfinity => (Denominator.IsZero) && (Numerator.Sign < 0);
        /// <summary>
        /// Indicates whether the value of current <see cref="Rational"/> is <see cref="NegativeInfinity"/> or <see cref="PositiveInfinity"/>
        /// </summary>
        public bool IsInfinity => (Denominator.IsZero) && (!Numerator.IsZero);

        /// <summary>
        /// Indicates whether the value of current <see cref="Rational"/> is integer (int/1)
        /// </summary>
        public bool IsInteger => Denominator.IsOne;
        /// <summary>
        /// Indicates whether the value of current <see cref="Rational"/> is valid rational (<see cref="Denominator"/> != 0)
        /// </summary>
        public bool IsValid => !Denominator.IsZero;

        /// <summary>
        /// Gets a number that indicates the sign (negative, positive, or zero) of the current <see cref="Rational"/> object.
        /// </summary>
        public int Sign => Numerator.Sign;

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
            (Numerator, Denominator) = Simplify(MoveSign((numerator, denominator)));
        }
        /// <summary>
        /// Converts the numeric value of the current <see cref="Rational"/> object to its equivalent string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString() => string.Format("{0}/{1}", Numerator, Denominator);
        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Numerator, Denominator);
        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is not Rational r) return false;
            return Equals(r);
        }
        /// <summary>
        /// Gets a continued fraction form of the current <see cref="Rational"/> object
        /// </summary>
        /// <returns></returns>
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
