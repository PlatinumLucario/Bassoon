// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

namespace SndFileSharp
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
