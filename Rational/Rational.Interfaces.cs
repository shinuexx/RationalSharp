using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShInUeXx.Numerics
{
    public readonly partial struct Rational : IEquatable<Rational>, IComparable, IComparable<Rational>
    {
        public bool Equals(Rational other)
        {
            return Numerator.Equals(other.Numerator) && Denominator.Equals(other.Denominator);
        }
        public bool Equals(BigInteger obj)
        {
            return Denominator.Equals(1) && Numerator.Equals(obj);
        }
        public bool Equals(long obj)
        {
            return Denominator.IsOne && Numerator.Equals(obj);
        }
        public bool Equals(int obj)
        {
            return Denominator.IsOne && Numerator.Equals(obj);
        }
        public bool Equals(ulong obj)
        {
            return Denominator.IsOne && Numerator.Equals(obj);
        }
        public bool Equals(uint obj)
        {
            return Denominator.IsOne && Numerator.Equals(obj);
        }

        public int CompareTo(object obj)
        {
            if (obj is null) return 1;
            if (obj is not Rational r) throw new ArgumentException("Argument must be Rational");
            return CompareTo(r);
        }

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

        public int CompareTo(BigInteger integer)
        {
            return CompareTo(new Rational(integer));
        }
        public int CompareTo(long integer)
        {
            return CompareTo(new Rational(integer));
        }
        public int CompareTo(int integer)
        {
            return CompareTo(new Rational(integer));
        }
        public int CompareTo(ulong integer)
        {
            return CompareTo(new Rational(integer));
        }
        public int CompareTo(uint integer)
        {
            return CompareTo(new Rational(integer));
        }
    }
}
