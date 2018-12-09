namespace libsndfileSharp
{
    public enum ChannelMap
    {
        Invalid = 0,
        Mono = 1,
        Left,               /* Apple calls this 'Left' */
        Right,              /* Apple calls this 'Right' */
        Center,             /* Apple calls this 'Center' */
        FrontLeft,
        FrontRight,
        FrontCenter,
        ReadCenter,         /* Apple calls this 'Center Surround', Msft calls this 'Back Center' */
        ReadLeft,           /* Apple calls this 'Left Surround', Msft calls this 'Back Left' */
        ReadRight,          /* Apple calls this 'Right Surround', Msft calls this 'Back Right' */
        LFE,                /* Apple calls this 'LFEScreen', Msft calls this 'Low Frequency'  */
        FrontLeftOfCenter,  /* Apple calls this 'Left Center' */
        FrontRightOfCenter, /* Apple calls this 'Right Center */
        SideLeft,           /* Apple calls this 'Left Surround Direct' */
        SideRight,          /* Apple calls this 'Right Surround Direct' */
        TopCenter,          /* Apple calls this 'Top Center Surround' */
        TopFrontLeft,       /* Apple calls this 'Vertical Height Left' */
        TopFrontRight,      /* Apple calls this 'Vertical Height Right' */
        TopFrontCenter,     /* Apple calls this 'Vertical Height Center' */
        TopReadLeft,        /* Apple and MS call this 'Top Back Left' */
        TopReadRight,       /* Apple and MS call this 'Top Back Right' */
        TopReadCenter,      /* Apple and MS call this 'Top Back Center' */

        AMbisonicBW,
        AMbisonicBX,
        AMbisonicBY,
        AMbisonicBZ,

        Max
    }
}
