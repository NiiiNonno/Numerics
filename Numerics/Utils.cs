using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;
public static partial class Utils
{
    [MI(MIO.AggressiveInlining | MIO.AggressiveOptimization)]
    public static unsafe bool SequencialEquals<T>(T* a, T* b, int length) where T : unmanaged
    {
        return new Span<T>(a, length).SequenceEqual(new Span<T>(b, length));
    }

    [MI(MIO.AggressiveInlining | MIO.AggressiveOptimization)]
    public static unsafe void Copy<T>(nint source, T* to, int length) where T : unmanaged => Copy((T*)source, to, length);
    [MI(MIO.AggressiveInlining | MIO.AggressiveOptimization)]
    public static unsafe void Copy<T>(T* source, nint to, int length) where T : unmanaged => Copy(source, (T*)to, length);
    [MI(MIO.AggressiveInlining | MIO.AggressiveOptimization)]
    public static unsafe void Copy<T>(nint source, nint to, int length) where T : unmanaged => Copy((T*)source, (T*)to, length);
    [MI(MIO.AggressiveInlining | MIO.AggressiveOptimization)]
    public static unsafe void Copy<T>(T* source, T* to, int length) where T : unmanaged
    {
        new Span<T>(source, length).CopyTo(new Span<T>(to, length));
    }

    public static TFormatInfo? GetFormat<TFormatInfo>(this IFormatProvider @this) where TFormatInfo : class => @this.GetFormat(typeof(TFormatInfo)) as TFormatInfo;

    //public static D Solve<T, D, N, DD, DN, ND, NN>(DD leftSide, D rightSide) 
    //    where T : unmanaged, INumber<T> 
    //    where D : IVector<T, D, N> 
    //    where N : IVector<T, N, N> 
    //    where DD : ITensor<T, D, D, N, DD, DN, ND, DD, DN, ND, NN>
    //    where DN : ITensor<T, D, N, N, DN, DN, NN, ND, NN, ND, NN>
    //    where ND : ITensor<T, N, D, N, ND, NN, ND, DN, DN, NN, NN>
    //    where NN : ITensor<T, N, N, N, NN, NN, NN, NN, NN, NN, NN>
    //{
    //    var I = leftSide.Zero();
    //    I.Init(0, 0, (T)1);

    //    var d = rightSide.Dimension;
    //    var l = d - 1;
    //    for (int i = 0; i < l; i++)
    //    {
    //        var e = rightSide.Zero();
    //        e.Init(i, (T)1);
    //        var a = leftSide * e;
    //        for (int j = 0; j < i; j++)
    //            a.Init(j, T.Zero);
    //        var dif = (a, a).Sqrt() * e - a;
    //        var H = I - (T)2 * (dif.Column() * dif.Row()) / (dif, dif);
    //        leftSide = H * leftSide;
    //        rightSide = H * rightSide:
    //    }

        
    //}

    //public static TVector Solve<T, TVector, TMatrix>(TMatrix leftSide, TVector rightSide) where T : unmanaged, INumber<T> where TVector : IVector<TNumber, TVector> where TMatrix : ITensor<T, TVector, TVector, VectorN<T>, TMatrix, >
    //{
    //    var I = leftSide.Zero();
    //    I[0, 0] = I[1, 1] = I[2, 2] = (TNumber)1;

    //    for (int i = 0; i < 2; i++)
    //    {
    //        var e = TVector.Zero(3);
    //        e[i] = (TNumber)1;
    //        var a = TVector.Cast(leftSide * TVector.Cast(e));
    //        for (int j = 0; j < i; j++)
    //            a[j] = TNumber.Zero;
    //        var a_d = (a * a).Sqrt() * e - a;
    //        var H = I - (TNumber)2 * (a_d.Column() * a_d.Row()) / (a_d * a_d);
    //        leftSide = TMatrix.Cast(H * leftSide);
    //        rightSide = TVector.Cast(H * TVector.Cast(rightSide));
    //    }

    //    return SolveQR<TNumber, TVector, TMatrix>(leftSide, rightSide);
    //}
    //public static TVector SolveQR<TNumber, TVector, TMatrix>(TMatrix leftSideUTM, TVector rightSide) where TNumber : unmanaged, INumber<TNumber> where TVector : IVector<TNumber, TVector> where TMatrix : ITensor<TNumber, TMatrix>
    //{
    //    var r = rightSide.Copy();
    //    for (int i = rightSide.Dimension - 1; i >= 0; i--)
    //    {
    //        var x = r[i] /= leftSideUTM[i, i];
    //        for (int j = i - 1; j >= 0; j--)
    //        {
    //            r[j] -= x * leftSideUTM[j, i]; 
    //        }
    //    }
    //    return r;
    //}

    public static int[] BitReverse(Shift length)
    {
        var r = new int[length];
        var expo = length.Exponent;
        for (int i = 0; i < r.Length; i++)
        {
            for (int j = 0; j < expo; j++)
                r[i] |= ((i >> j) & 1) << (expo - j - 1);
        }
        return r;
    }

    public static float HammingWindow(float x)
    {
        if (x is <= 1 or >= 0) return 0.54f - 0.46f * MathF.Cos(2 * MathF.PI * x);
        else return float.NaN;
    }
    public static double HammingWindow(double x)
    {
        if (x is <= 1 or >= 0) return 0.54 - 0.46 * Math.Cos(2 * Math.PI * x);
        else return double.NaN;
    }

    [Author("ChatGPT3.5")]
    public static T IntegrateUsingRectangle<T>(Func<T, T> function, T lowerBound, T upperBound, int numIntervals) where T : INumber<T>
    {
        var intervalWidth = (upperBound - lowerBound) / (T)numIntervals;
        var sum = T.Zero;

        for (int i = 0; i < numIntervals; i++)
        {
            var x = lowerBound + (T)i * intervalWidth;
            var y = function(x);
            sum += y;
        }

        return sum * intervalWidth;
    }
    [Author("ChatGPT3.5")]
    public static T IntegrateUsingFourierTransform<T>(Func<T, T> function, T lowerBound, T upperBound, ShortFastFourierTransformer<T> transformer) where T : unmanaged, IDecimal<T>
    {
        var samples = transformer.GetSourceArray();
        var numSamples = (int)transformer.Range;
        var numSamples_ = (T)numSamples;
        var delta = (upperBound - lowerBound) / numSamples_;

        for (int i = 0; i < numSamples; i++)
        {
            var x = lowerBound + (T)i * delta;
            samples[i] = function(x);
        }

        var integral = T.Zero;
        var frequencyDelta = T.Unit / (numSamples_ * delta);
        var fft = (stackalloc Complex<T>[numSamples]);
        transformer.Run();
        transformer.Copy(to: fft);

        for (int i = 0; i < numSamples; i++)
        {
            var frequency = (T)i * frequencyDelta;
            var weight = delta * ((T)(2.0 * Math.PI)).Sqrt() * fft[i].r;

            integral += weight;
        }

        return integral;
    }
    [Author("ChatGPT3.5")]
    public static double TrapezoidalRule(Func<double, double> function, double lowerBound, double upperBound, int numIntervals)
    {
        double intervalWidth = (upperBound - lowerBound) / numIntervals;
        double sum = 0.0;

        for (int i = 0; i < numIntervals; i++)
        {
            double x0 = lowerBound + i * intervalWidth;
            double x1 = lowerBound + (i + 1) * intervalWidth;
            double y0 = function(x0);
            double y1 = function(x1);

            double area = (y0 + y1) * intervalWidth / 2.0;
            sum += area;
        }

        return sum;
    }
    [Author("ChatGPT3.5")]
    public static double LagrangeInterpolation(Func<double, double> function, double lowerBound, double upperBound, int numPoints)
    {
        double[] xValues = new double[numPoints];
        double[] yValues = new double[numPoints];

        double delta = (upperBound - lowerBound) / (numPoints - 1);

        for (int i = 0; i < numPoints; i++)
        {
            double x = lowerBound + i * delta;
            double y = function(x);

            xValues[i] = x;
            yValues[i] = y;
        }

        double integral = 0.0;

        for (int i = 0; i < numPoints - 1; i++)
        {
            double x0 = xValues[i];
            double x1 = xValues[i + 1];
            double y0 = yValues[i];
            double y1 = yValues[i + 1];

            double area = (y0 + y1) * (x1 - x0) / 2.0;
            integral += area;
        }

        return integral;
    }
    [Author("ChatGPT3.5")]
    public static double SimpsonRule(Func<double, double> function, double lowerBound, double upperBound, int numIntervals)
    {
        double intervalWidth = (upperBound - lowerBound) / numIntervals;
        double sum = 0.0;

        for (int i = 0; i < numIntervals; i++)
        {
            double x0 = lowerBound + i * intervalWidth;
            double x1 = lowerBound + (i + 0.5) * intervalWidth;
            double x2 = lowerBound + (i + 1) * intervalWidth;
            double y0 = function(x0);
            double y1 = function(x1);
            double y2 = function(x2);

            double area = (y0 + 4 * y1 + y2) * intervalWidth / 6.0;
            sum += area;
        }

        return sum;
    }
    [Author("ChatGPT3.5")]
    public static double Solve(Func<double, double, double> dydx, double x0, double y0, double x, int n)
    {
        double h = (x - x0) / n;
        double y = y0;

        for (int i = 0; i < n; i++)
        {
            double k1 = h * dydx(x0, y);
            double k2 = h * dydx(x0 + 0.5 * h, y + 0.5 * k1);
            double k3 = h * dydx(x0 + 0.5 * h, y + 0.5 * k2);
            double k4 = h * dydx(x0 + h, y + k3);

            y = y + (1.0 / 6.0) * (k1 + 2 * k2 + 2 * k3 + k4);
            x0 = x0 + h;
        }

        return y;
    }
    [Author("ChatGPT3.5")]
    public static double SolveN(Func<double, double, double> dydx, double x0, double y0, double x, int n)
    {
        double h = (x - x0) / n;
        double y = y0;

        for (int i = 0; i < n; i++)
        {
            double[] k = new double[n];

            double xi = x0 + i * h;
            double yi = y;

            for (int j = 0; j < n; j++)
            {
                double sum = 0.0;
                for (int l = 0; l < j; l++)
                {
                    sum += k[l] * Math.Pow(h, l + 1) / Factorial(l + 1);
                }

                k[j] = h * dydx(xi + sum, yi + sum);
            }

            double sum2 = 0.0;
            for (int j = 0; j < n; j++)
            {
                sum2 += k[j] * Math.Pow(h, j) / Factorial(j);
            }

            y = yi + sum2;
        }

        return y;
    }

    public static int Factorial(int n) => n <= 1 ? 1 : n * Factorial(n - 1);
}
