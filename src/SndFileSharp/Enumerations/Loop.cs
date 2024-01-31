// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

namespace SndFileSharp
{
    /// <summary>
    /// The loop mode field in SF_INSTRUMENT will be one of the following.
    /// </summary>
    public enum Loop
    {
        None = 800,
        Forward,
        Backward,
        Alternating
    }
}
