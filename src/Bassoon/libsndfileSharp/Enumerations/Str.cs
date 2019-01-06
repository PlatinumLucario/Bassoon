// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

namespace libsndfileSharp
{
    /// <summary>
    /// The following file types can be read and written.
    /// A file type would consist of a major type (ie WAV) bitwise
    /// ORed with a minor type (ie PCM). TYPEMASK and
    /// SUBMASK can be used to separate the major and minor file
    /// types.
    /// </summary>
    public enum Str
    {
        Title       = 0x01,
        Copyright   = 0x02,
        Software    = 0x03,
        Artist      = 0x04,
        Comment     = 0x05,
        Date        = 0x06,
        Album       = 0x07,
        License     = 0x08,
        TrackNumber = 0x09,
        Genre       = 0x10,

        /*
        ** Use the following as the start and end index when doing metadata
        ** transcoding.
        */
//        First = Title,
//        Last = Genre,
    }
}
