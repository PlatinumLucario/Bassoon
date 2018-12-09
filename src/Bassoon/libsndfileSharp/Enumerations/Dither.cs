namespace libsndfileSharp
{
    public enum Dither
    {
        DefaultLevel  = 0,
        CustomLevel   = 0x40000000,

        NoDither      = 500,
        White         = 501,
        TriangularPDF = 502
    }
}