//#if USE_DOUBLE
//using Dec = System.Double;
//#else
//using Dec = System.Single;
//#endif

//namespace Nonno.Numerics;

//public readonly struct Fraction<TNumber> : INumber<Fraction<TNumber>> where TNumber : INumber<TNumber>, IComparable<TNumber>
//{
//    readonly TNumber n;
//    readonly TNumber d;

//    public TNumber Numerator => n;
//    public TNumber Denominator => d;
//    public double Value => n / (double)d;
//    public bool IsZero => n == 0 && d != 0;

//    public Fraction(TNumber numerator, TNumber denominator)
//    {
//        n = numerator;
//        d = denominator;
//    }

//    public override bool Equals(object? obj) => obj is Fraction<TNumber> fraction && Equals(fraction);
//    public bool Equals(Fraction<TNumber> other)
//    {
//        if (d == other.d) return n == other.n;

//        var a = Reduce(this);
//        var b = Reduce(other);
//        return a.n == b.n && a.d == b.d;
//    }
//    public override int GetHashCode() => HashCode.Combine(n, d);
//    public override string ToString() => $"{n}/{d}={Value}";

//    public static Fraction<TNumber> Zero => new(TNumber.Zero, TNumber.Unit);
//    public static Fraction<TNumber> operator *(TNumber left, Fraction<TNumber> right) => new(left * right.n, right.d);
//    public static Fraction<TNumber> operator /(TNumber left, Fraction<TNumber> right) => new(left * right.d, right.n);
//    public static Fraction<TNumber> operator *(Fraction<TNumber> left, TNumber right) => new(left.n * right, left.d);
//    public static Fraction<TNumber> operator /(Fraction<TNumber> left, TNumber right) => new(left.n, left.d * right);
//    public static Fraction<TNumber> operator *(Fraction<TNumber> left, Fraction<TNumber> right) => new(left.n * right.n, left.d * right.d);
//    public static Fraction<TNumber> operator /(Fraction<TNumber> left, Fraction<TNumber> right) => new(left.n * right.d, left.d * right.n);
//    public static bool operator ==(Fraction<TNumber> left, Fraction<TNumber> right) => left.Equals(right);
//    public static bool operator !=(Fraction<TNumber> left, Fraction<TNumber> right) => !left.Equals(right);
//    public static Fraction<TNumber> Reduce(Fraction<TNumber> fraction)
//    {
//        if (fraction.n == 0)
//        {
//            return Zero;
//        }
//        else
//        {
//            // 符号に注意。入力の分母は常に非正で出力の分母は常に非負である。
//            var v = -GetGreatestCommonDivisor(fraction.n < 0 ? -fraction.n : fraction.n, -fraction.d);
//            return new Fraction<TNumber>(fraction.n / v, fraction.d / v);
//        }
//    }
//    public static int GetGreatestCommonDivisor(int integar1, int integar2)
//    {
//#if DEBUG
//        if (integar1 <= 0) throw new ArgumentException("正の値が必要です。", nameof(integar1));
//        if (integar2 <= 0) throw new ArgumentException("正の値が必要です。", nameof(integar2));
//#endif

//        while (integar1 > 0)
//        {
//            int integar1_ = integar2 % integar1;
//            integar2 = integar1;
//            integar1 = integar1_;
//        }
//        return integar2;
//    }
//    public static Fraction<TNumber> Inverse(Fraction<TNumber> fraction)
//    {
//        if (fraction.IsReduced)
//        {
//            if (fraction.numerator < 0) return new Fraction<TNumber>(-fraction.denominator, -fraction.numerator);
//            else return new Fraction<TNumber>(fraction.denominator, fraction.numerator);
//        }
//        else
//        {
//            if (fraction.numerator < 0) return new Fraction<TNumber>(fraction.denominator, fraction.numerator);
//            else return new Fraction<TNumber>(-fraction.denominator, -fraction.numerator);
//        }
//    }
//    public static Fraction<TNumber> GetApproximation(float value, int deepness = sizeof(float), float errorRange = 0)
//    {
//#if DEBUG
//        if (errorRange < 0) throw new ArgumentException("誤差幅は非負の値である必要があります。", nameof(errorRange));
//#endif

//#if DEBUG
//        int[] span = new int[deepness];
//#else
//        Span<int> span = stackalloc int[deepness];
//#endif

//        int i = 0;
//        for (; i < span.Length; i++)
//        {
//            var floor = (int)value;
//            span[i] = floor;
//            var fP = value - floor;
//            if (-errorRange <= fP && fP <= errorRange) { i++; break; }
//            value = 1 / fP;
//        }

//        i--;

//        int numerator = span[i], denominator = 1;
//        for (int j = i - 1; j >= 0; j--)
//        {
//            int numerator_ = numerator * span[j] + denominator;
//            denominator = numerator;
//            numerator = numerator_;
//        }

//        return new Fraction<TNumber>(numerator, denominator);
//    }
//    public static Fraction<TNumber> GetApproximation(double value, int deepness = sizeof(double), double errorRange = 0)
//    {
//#if DEBUG
//        if (errorRange < 0) throw new ArgumentException("誤差幅は非負の値である必要があります。", nameof(errorRange));
//#endif

//#if DEBUG
//        int[] span = new int[deepness];
//#else
//        Span<int> span = stackalloc int[deepness];
//#endif

//        int i = 0;
//        for (; i < span.Length; i++)
//        {
//            var floor = (int)value;
//            span[i] = floor;
//            var fP = value - floor;
//            if (-errorRange <= fP && fP <= errorRange) { i++; break; }
//            value = 1 / fP;
//        }

//        i--;

//        int numerator = span[i], denominator = 1;
//        for (int j = i - 1; j >= 0; j--)
//        {
//            int numerator_ = numerator * span[j] + denominator;
//            denominator = numerator;
//            numerator = numerator_;
//        }

//        return new Fraction<TNumber>(numerator, denominator);
//    }
//    public static explicit operator float(Fraction<TNumber> fraction) => (float)fraction.numerator / fraction.denominator;
//    public static explicit operator double(Fraction<TNumber> fraction) => (double)fraction.numerator / fraction.denominator;
//    public static explicit operator decimal(Fraction<TNumber> fraction) => (decimal)fraction.numerator / fraction.denominator;
//}
