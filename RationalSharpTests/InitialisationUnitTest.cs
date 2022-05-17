using NUnit.Framework.Internal;
using ShInUeXx.Numerics;
using System.Numerics;

namespace RationalSharpTests
{
    [TestClass]
    public class InitialisationUnitTest
    {
        [TestMethod]
        public void CanInitializeFromRatio()
        {
            BigInteger n = 123;
            BigInteger d = -5321;

            var r = new Rational(n, d);

            Assert.AreEqual(-n, r.Numerator);
            Assert.AreEqual(-d, r.Denominator);
            Assert.AreEqual((n * d).Sign, r.Sign);
        }
        [TestMethod]
        public void CanInitializeFromBigInteger()
        {
            BigInteger value = 123;

            var r = new Rational(value);

            Assert.AreEqual(value, r.Numerator);
            Assert.AreEqual(BigInteger.One, r.Denominator);
            Assert.AreEqual(value.Sign, r.Sign);
            Assert.IsTrue(r.IsInteger);
            Assert.IsTrue(r.IsValid);
        }
        [TestMethod]
        public void CanCastFromBuiltinTypes()
        {
            int a = -123;
            uint b = 123;
            long c = int.MinValue * 2L;
            ulong d = ulong.MaxValue;
            BigInteger e = BigInteger.Pow(2, 100);
            float f = MathF.E;
            BigInteger fn = 2850325, fd = 1048576;
            double g = Math.PI;
            BigInteger gn = 884279719003555, gd = 281474976710656;
            decimal h = new(1, 0, 0, false, 28);
            BigInteger hd = BigInteger.Pow(10, 28);
            Complex i = new(Math.PI, 0d);

            Rational value1, value2, value3, value4, value5, value6, value7, value8, value9;

            value1 = a;
            value2 = b;
            value3 = c;
            value4 = d;
            value5 = e;
            value6 = f;
            value7 = g;
            value8 = h;
            value9 = (Rational)i;

            Assert.AreEqual(value1.Numerator, (BigInteger)a);
            Assert.AreEqual(value1.Denominator, BigInteger.One);

            Assert.AreEqual(value2.Numerator, (BigInteger)b);
            Assert.AreEqual(value2.Denominator, BigInteger.One);

            Assert.AreEqual(value3.Numerator, (BigInteger)c);
            Assert.AreEqual(value3.Denominator, BigInteger.One);

            Assert.AreEqual(value4.Numerator, (BigInteger)d);
            Assert.AreEqual(value4.Denominator, BigInteger.One);

            Assert.AreEqual(value5.Numerator, e);
            Assert.AreEqual(value5.Denominator, BigInteger.One);

            Assert.AreEqual(value6.Numerator, fn);
            Assert.AreEqual(value6.Denominator, fd);

            Assert.AreEqual(value7.Numerator, gn);
            Assert.AreEqual(value7.Denominator, gd);

            Assert.AreEqual(value8.Numerator, BigInteger.One);
            Assert.AreEqual(value8.Denominator, hd);

            Assert.AreEqual(value9.Numerator, gn);
            Assert.AreEqual(value9.Denominator, gd);

            Assert.AreEqual((int)value1, a);
            Assert.AreEqual((uint)value2, b);
            Assert.AreEqual((long)value3, c);
            Assert.AreEqual((ulong)value4, d);
            Assert.AreEqual((BigInteger)value5, e);
            Assert.AreEqual((float)value6, f, float.Epsilon);
            Assert.AreEqual((double)value7, g, double.Epsilon);
            Assert.AreEqual((decimal)value8, h);
            Assert.AreEqual((Complex)value9, i);
        }
        [TestMethod]
        public void StaticPropertiesAreCorrect()
        {
            BigInteger two = 2;

            Assert.AreEqual(Rational.One.Numerator, BigInteger.One);
            Assert.AreEqual(Rational.One.Denominator, BigInteger.One);

            Assert.AreEqual(Rational.MinusOne.Numerator, BigInteger.MinusOne);
            Assert.AreEqual(Rational.MinusOne.Denominator, BigInteger.One);

            Assert.AreEqual(Rational.Zero.Numerator, BigInteger.Zero);
            Assert.AreEqual(Rational.Zero.Denominator, BigInteger.One);

            Assert.AreEqual(Rational.Half.Numerator, BigInteger.One);
            Assert.AreEqual(Rational.Half.Denominator, two);

            Assert.AreEqual(Rational.MinusHalf.Numerator, BigInteger.MinusOne);
            Assert.AreEqual(Rational.MinusHalf.Denominator, two);

            Assert.AreEqual(Rational.NaN.Numerator, BigInteger.Zero);
            Assert.AreEqual(Rational.NaN.Denominator, BigInteger.Zero);

            Assert.AreEqual(Rational.PositiveInfinity.Numerator, BigInteger.One);
            Assert.AreEqual(Rational.PositiveInfinity.Denominator, BigInteger.Zero);

            Assert.AreEqual(Rational.NegativeInfinity.Numerator, BigInteger.MinusOne);
            Assert.AreEqual(Rational.NegativeInfinity.Denominator, BigInteger.Zero);
        }
        [TestMethod]
        public void StringRepresentationIsCorrect()
        {
            var a = Rational.One;
            var b = Rational.NaN;
            var c = new Rational(1234, 4321);

            Assert.AreEqual(a.ToString(), "1/1");
            Assert.AreEqual(b.ToString(), "0/0");
            Assert.AreEqual(c.ToString(), "1234/4321");
        }
        [TestMethod]
        public void IComparableInterfaceImplemenationIsCorrect()
        {
            Rational a = new(1, 3);
            Rational b = new(1, 4);

            Assert.AreEqual(Rational.Half.CompareTo(Rational.Zero), 1);
            Assert.AreEqual(Rational.MinusHalf.CompareTo(Rational.Zero), -1);
            Assert.AreEqual(Rational.One.CompareTo(BigInteger.One), 0);
            Assert.AreEqual(a.CompareTo(b), 1);
        }
        [TestMethod]
        public void OperatorsWorksCorrect()
        {
            var a = Rational.Half;
            Rational b = new(1, 3);
            Rational sum_expect = new(5, 6);
            Rational diff_expect = new(1, 6);
            Rational mul_expect = new(1, 6);
            Rational div_expect = new(3, 2);

            Assert.AreEqual(a + b, sum_expect);
            Assert.AreEqual(a - b, diff_expect);
            Assert.AreEqual(a * b, mul_expect);
            Assert.AreEqual(a / b, div_expect);
            Assert.AreEqual(++a, new(3, 2));
            a = Rational.Half;
            Assert.AreEqual(--a, Rational.MinusHalf);
            Assert.AreEqual(-Rational.Half, Rational.MinusHalf);
        }
        [TestMethod]
        public void ParseStringRepresentation()
        {
            var a = ("2/101", new Rational(2, 101));
            var b = ("1.234e-100", new Rational(1234, BigInteger.Pow(10, 100)));
            var c = ("3 1/2", new Rational(7, 2));
            var d = ("12345678", (Rational)12345678);

            Assert.AreEqual(Rational.Parse(a.Item1), a.Item2);
            Assert.AreEqual(Rational.Parse(b.Item1), b.Item2);
            Assert.AreEqual(Rational.Parse(c.Item1), c.Item2);
            Assert.AreEqual(Rational.Parse(d.Item1), d.Item2);
        }
        [TestMethod]
        public void StaticMethods()
        {
            Assert.AreEqual(Rational.Abs(Rational.MinusHalf), Rational.Half);
            Assert.AreEqual(Rational.Floor(Rational.MinusHalf), Rational.MinusOne);
            Assert.AreEqual(Rational.Ceiling(Rational.MinusHalf), Rational.Zero);
            Assert.AreEqual(Rational.Truncate(Rational.MinusHalf), Rational.Zero);
            Assert.AreEqual(Rational.Truncate(Rational.Half), Rational.Zero);
            Assert.AreEqual(Rational.Pow(Rational.Half, 100), new(1, BigInteger.Pow(2, 100)));
        }
    }
}