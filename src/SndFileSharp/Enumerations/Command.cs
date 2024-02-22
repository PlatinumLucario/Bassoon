// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

namespace SndFile
{
    /// <summary>
    /// The following are the valid command numbers for the sf_command()
    /// interface.  The use of these commands is documented in the file
    /// command.html in the doc directory of the source code distribution.
    /// </summary>
    public enum Command
    {
        GetLibVersion           = 0x1000,
        GetLogInfo              = 0x1001,
        GetCurrentSFInfo        = 0x1002,


        GetNormDouble           = 0x1010,
        GetNormFloat            = 0x1011,
        SetNormDouble           = 0x1012,
        SetNormFloat            = 0x1013,
        SetScaleFloatIntRead    = 0x1014,
        SetScaleIntFloatWrite   = 0x1015,

        GetSimpleFormatCount    = 0x1020,
        GetSimpleFormat         = 0x1021,

        GetFormatInfo           = 0x1028,

        GetFormatMajorCount     = 0x1030,
        GetFormatMajor          = 0x1031,
        GetFormatSubtypeCount   = 0x1032,
        GetFormatSubtype        = 0x1033,

        CalcSignalMax           = 0x1040,
        CalcNormSignalMax       = 0x1041,
        CalcMaxAllChannels      = 0x1042,
        CalcNormMaxAllChannels  = 0x1043,
        GetSignalMax            = 0x1044,
        GetMaxAllChannels       = 0x1045,

        SetAddPeakChunk         = 0x1050,
        SetAddHeaderPadChunk    = 0x1051,

        UpdateHeaderNow         = 0x1060,
        SetUpdateHeaderAuto     = 0x1061,

        FileTruncate            = 0x1080,

        SetRawStartOffset       = 0x1090,

        SetDitherOnWrite        = 0x10A0,
        SetDitherOnRead         = 0x10A1,

        GetDitherInfoCount      = 0x10A2,
        GetDitherInfo           = 0x10A3,

        GetEmbedFileInfo        = 0x10B0,

        SetClipping             = 0x10C0,
        GetClipping             = 0x10C1,

        GetCueCount             = 0x10CD,
        GetCue                  = 0x10CE,
        SetCue                  = 0x10CF,

        GetInstrument           = 0x10D0,
        SetInstrument           = 0x10D1,

        GetLoopInfo             = 0x10E0,

        GetBroadcastInfo        = 0x10F0,
        SetBroadcastInfo        = 0x10F1,

        GetChannelMapInfo       = 0x1100,
        SetChannelMapInfo       = 0x1101,

        RawDataNeedsEndswap     = 0x1110,

        /* Support for Wavex Ambisonics Format */
        WavexSetAmbisonic       = 0x1200,
        WavexGetAmbisonic       = 0x1201,

        /*
        ** RF64 files can be set so that on-close, writable files that have less
        ** than 4GB of data in them are converted to RIFF/WAV, as per EBU
        ** recommendations.
        */
        RF64AutoDowngrade       = 0x1210,

        SetVBREncodingQuality   = 0x1300,
        SetCompressionLevel     = 0x1301,

        /* Cart Chunk support */
        SetCartInfo             = 0x1400,
        GetCartInfo             = 0x1401,

        /* Following commands for testing only. */
        TestIEEEFloatReplace    = 0x6001,

        /*
        ** SetAdd* values are deprecated and will disappear at some
        ** time in the future. They are guaranteed to be here up to and
        ** including version 1.0.8 to avoid breakage of existing software.
        ** They currently do nothing and will continue to do nothing.
        */
        SetAddDitherOnWrite     = 0x1070,
        SetAddDitherOnRead      = 0x1071
    }
}
