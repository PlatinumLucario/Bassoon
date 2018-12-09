using System.Runtime.InteropServices;

using sf_count_t = System.Int64;

namespace libsndfileSharp
{
    /// <summary>
    /// Struct used to retrieve information about a file embedded within a
    /// larger file. See SFC_GET_EMBED_FILE_INFO.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EmbedFileInfo
    {
        public sf_count_t offset;
        public sf_count_t length;
    }
}
