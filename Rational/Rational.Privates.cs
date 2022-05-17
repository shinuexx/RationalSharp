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
        /// <summary>
        /// Regular expression for rational in form <c>i n/d</c> where <c>i</c> ,<c>n</c>, <c>d</c> are integers
        /// </summary>
        private static readonly Regex REGEXP_INT_FRACTION = new(@"^([+-]?\d+)\s(\d+)\/(\d+)$");
        /// <summary>
        /// Regular expression for rational in integer form
        /// </summary>
        private static readonly Regex REGEXP_INT = new(@"^([+-]?\d+)$");
        /// <summary>
        /// Regular expression for rational in scientific and decimal notation (eg. <c>1.1e2</c>)
        /// </summary>
        private static readonly Regex REGEXP_DECIMAL = new(@"^([0-9]+)\.?([0-9]*)[eE]?([+-]?[0-9]*)$");
        /// <summary>
        /// Regular expression for rational in fraction form (<c>n/d</c> where <c>n</c>, <c>d</c> are integers)
        /// </summary>
        private static readonly Regex REGEXP_FRACTION = new(@"^([+-]?\d+)\/(\d+)$");

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
        /// Move sign of from last to first <see cref="BigInteger"/> object.
        /// </summary>
        /// <param name="args">Tuple of two <see cref="BigInteger"/> objects <c>(first, last)</c></param>
        /// <returns>Tuple of <c>(first, last)</c> <see cref="BigInteger"/> with non negative <c>last</c></returns>
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

        /// <summary>
        /// Simplify two <see cref="BigInteger"/> by its <see cref="BigInteger.GreatestCommonDivisor(BigInteger, BigInteger)"/>
        /// </summary>
        /// <param name="args">Tuple of two <see cref="BigInteger"/> objects <c>(first, last)</c></param>
        /// <returns>Tuple of <c>(first, last)</c> with simplified <see cref="BigInteger"/> objects</returns>
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
