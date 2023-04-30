namespace Nonno.Numerics;
public static class ConstantValues<TNumber> where TNumber : INumber<TNumber>
{
    public static readonly TNumber PI = (TNumber)Math.PI;
    public static readonly TNumber PI_RECIPRO = (TNumber)1 / (TNumber)Math.PI;
}
