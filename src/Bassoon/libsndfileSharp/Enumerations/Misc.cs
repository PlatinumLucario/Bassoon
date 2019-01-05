// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

namespace libsndfileSharp
{
    public enum SFBool
    {
        False = 0,
        True  = 1,
    }

    /// <summary>
    /// Modes for opening files.
    /// </summary>
    public enum SFMode
    {
        Read  = 0x10,
        Write = 0x20,
        RDWR  = 0x30,
    }

    public enum Ambisonic
    {
        None =    0x40,
        BFormat = 0x41,
    }
}
