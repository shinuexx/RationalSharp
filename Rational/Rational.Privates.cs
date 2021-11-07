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
        private static readonly Regex REGEXP_INT_FRACTION = new(@"^([+-]?\d+)\s(\d+)\/(\d+)$");
        private static readonly Regex REGEXP_INT = new(@"^([+-]?\d+)$");
        private static readonly Regex REGEXP_DECIMAL = new(@"^([0-9]+)\.?([0-9]*)[eE]?([+-]?[0-9]*)$");
        private static readonly Regex REGEXP_FRACTION = new(@"^([+-]?\d+)\/(\d+)$");

        private const long double_exp_mask = (1L << 11) - 1;
        private const long double_mantissa_mask = (1L << 52) - 1;
        private const long double_default_denominator = (1L << 52);

        private const int float_exp_mask = 0xff;
        private const int float_mantissa_mask = (1 << 23) - 1;
        private const int float_default_denominator = (1 << 23);
        private static (BigInteger, BigInteger) MoveSign((BigInteger, BigInteger) args)
        {
            var (n, d) = args;
            if (d.Sign < 0)
            {
                n = -n;
                d = -d;
            }
            return (n, d);
        }
        private static (BigInteger, BigInteger) Simplify((BigInteger, BigInteger) args)
        {
            var (n, d) = args;
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
    }
}
