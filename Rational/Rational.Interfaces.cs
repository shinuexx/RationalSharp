using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ShInUeXx.Numerics
{
    public readonly partial struct Rational : IEquatable<Rational>, IComparable, IComparable<Rational>
    {
        /// <inheritdoc/>
        public bool Equals(Rational other)
        {
            return Numerator.Equals(other.Numerator) && Denominator.Equals(other.Denominator);
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance and <see cref="BigInteger"/> object have the same value.
        /// </summary>
        /// <param name="obj">A <see cref="BigInteger"/> object to compare</param>
        /// <returns><see langword="true"/> if the current instance and <see cref="BigInteger"/> have the same value; otherwise, <see langword="false"/></returns>
        public bool Equals(BigInteger obj)
        {
            return Denominator.Equals(1) && Numerator.Equals(obj);
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance and <see cref="long"/> have the same value.
        /// </summary>
        /// <param name="obj">A <see cref="long"/> value</param>
        /// <returns><see langword="true"/> if the current instance and <see cref="long"/> have the same value; otherwise, <see langword="false"/></returns>
        public bool Equals(long obj)
        {
            return Denominator.IsOne && Numerator.Equals(obj);
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance and <see cref="int"/> have the same value.
        /// </summary>
        /// <param name="obj">A <see cref="int"/> value</param>
        /// <returns><see langword="true"/> if the current instance and <see cref="int"/> have the same value; otherwise, <see langword="false"/></returns>
        public bool Equals(int obj)
        {
            return Denominator.IsOne && Numerator.Equals(obj);
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance and <see cref="ulong"/> have the same value.
        /// </summary>
        /// <param name="obj">A <see cref="ulong"/> value</param>
        /// <returns><see langword="true"/> if the current instance and <see cref="ulong"/> have the same value; otherwise, <see langword="false"/></returns>
        public bool Equals(ulong obj)
        {
            return Denominator.IsOne && Numerator.Equals(obj);
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance and <see cref="uint"/> have the same value.
        /// </summary>
        /// <param name="obj">A <see cref="uint"/> value</param>
        /// <returns><see langword="true"/> if the current instance and <see cref="uint"/> have the same value; otherwise, <see langword="false"/></returns>
        public bool Equals(uint obj)
        {
            return Denominator.IsOne && Numerator.Equals(obj);
        }

        /// <inheritdoc/>
        public int CompareTo(object obj)
        {
            if (obj is null) return 1;
            if (obj is not Rational r) throw new ArgumentException("Argument must be Rational");
            return CompareTo(r);
        }
        /// <inheritdoc/>
        public int CompareTo(Rational other)
        {
            if (IsNan)
            {
                return other.IsNan ? 0 : -1;
            }
            else if (other.IsNan)
            {
                return 1;
            }
            else
            {
                var l = Numerator * other.Denominator;
                var r = Denominator * other.Numerator;
                return l.CompareTo(r);
            }
        }

        /// <summary>
        /// <inheritdoc cref="CompareTo(Rational)"/>
        /// </summary>
        /// <param name="integer"></param>
        /// <returns><inheritdoc cref="CompareTo(Rational)"/></returns>
        public int CompareTo(BigInteger integer)
        {
            return CompareTo(new Rational(integer));
        }

        /// <summary>
        /// <inheritdoc cref="CompareTo(Rational)"/>
        /// </summary>
        /// <param name="integer"></param>
        /// <returns><inheritdoc cref="CompareTo(Rational)"/></returns>
        public int CompareTo(long integer)
        {
            return CompareTo(new Rational(integer));
        }

        /// <summary>
        /// <inheritdoc cref="CompareTo(Rational)"/>
        /// </summary>
        /// <param name="integer"></param>
        /// <returns><inheritdoc cref="CompareTo(Rational)"/></returns>
        public int CompareTo(int integer)
        {
            return CompareTo(new Rational(integer));
        }

        /// <summary>
        /// <inheritdoc cref="CompareTo(Rational)"/>
        /// </summary>
        /// <param name="integer"></param>
        /// <returns><inheritdoc cref="CompareTo(Rational)"/></returns>
        public int CompareTo(ulong integer)
        {
            return CompareTo(new Rational(integer));
        }

        /// <summary>
        /// <inheritdoc cref="CompareTo(Rational)"/>
        /// </summary>
        /// <param name="integer"></param>
        /// <returns><inheritdoc cref="CompareTo(Rational)"/></returns>
        public int CompareTo(uint integer)
        {
            return CompareTo(new Rational(integer));
        }
    }
}
