//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Nonno.Numerics;
//#error 広範な定義Numberに逆数を定義できない。
//public readonly struct Recipro<TNumber> : INumber<TNumber> where TNumber : INumber<TNumber>
//{
//    readonly TNumber _value;

//    public Recipro(TNumber value) => _value = value;

//    public override bool Equals(object? obj) => obj is Recipro<TNumber> recipro && Equals(recipro);
//    public bool Equals(Recipro<TNumber> other) => _value == other._value;
//    public override int GetHashCode() => _value.GetHashCode();
//    public override string ToString() => "1/" + _value;

//    public static explicit operator Recipro<TNumber>(TNumber scalar) => new(1 / scalar);
//    public static explicit operator TNumber(Recipro<TNumber> recipro) => 1 / recipro._value;

//    public static bool operator ==(Recipro<TNumber> left, Recipro<TNumber> right) => left.Equals(right);
//    public static bool operator !=(Recipro<TNumber> left, Recipro<TNumber> right) => !(left == right);
//    public static bool operator <=(Recipro<TNumber> left, Recipro<TNumber> right)
//    {
//        if (TNumber.IsNegative(left._value))
//        {
//            return !TNumber.IsNegative(right._value) || left._value >= right._value;
//        }
//        else
//        {
//            return !TNumber.IsNegative(right._value) && left._value >= right._value;
//        }
//    }
//    public static bool operator >=(Recipro<TNumber> left, Recipro<TNumber> right)
//    {
//        if (TNumber.IsNegative(left._value))
//        {
//            return TNumber.IsNegative(right._value) && left._value <= right._value;
//        }
//        else
//        {
//            return TNumber.IsNegative(right._value) || left._value <= right._value;
//        }
//    }
//    public static bool operator <(Recipro<TNumber> left, Recipro<TNumber> right)
//    {
//        if (TNumber.IsNegative(left._value))
//        {
//            return !TNumber.IsNegative(right._value) || left._value > right._value;
//        }
//        else
//        {
//            return !TNumber.IsNegative(right._value) && left._value > right._value;
//        }
//    }
//    public static bool operator >(Recipro<TNumber> left, Recipro<TNumber> right)
//    {
//        if (TNumber.IsNegative(left._value))
//        {
//            return TNumber.IsNegative(right._value) && left._value < right._value;
//        }
//        else
//        {
//            return TNumber.IsNegative(right._value) || left._value < right._value;
//        }
//    }

//    public static Recipro<TNumber> operator *(Recipro<TNumber> left, Recipro<TNumber> right) => new(left._value * right._value);
//    public static TNumber operator *(Recipro<TNumber> left, TNumber right) => right / left._value;
//    public static Recipro<TNumber> operator *(TNumber left, Recipro<TNumber> right) => new(left / right._value);

//    public static TNumber operator /(Recipro<TNumber> left, Recipro<TNumber> right) => right._value / left._value;
//    public static Recipro<TNumber> operator /(Recipro<TNumber> left, TNumber right) => new(left._value * right);
//    public static TNumber operator /(TNumber left, Recipro<TNumber> right) => left * right._value;
//}
