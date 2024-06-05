namespace TTSBulkImporter.TabletopSimulator.GamePiece.Component
{
    public class ColorDiffuse
    {
        public readonly float r = 1f;
        public readonly float g = 1f;
        public readonly float b = 1f;

        public ColorDiffuse() {}

        public ColorDiffuse(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public static class Colors
        {
            public static ColorDiffuse Default => new ColorDiffuse();
            public static ColorDiffuse Pink => new ColorDiffuse(1.0f, 0.4f, 1.0f); // hex: ff66ff

            /// <summary>
            /// The default brown color used by bags in TTS.
            /// </summary>
            public static ColorDiffuse Brown => new ColorDiffuse(0.7058823f, 0.366520882f, 0.0f);
        }
    }
}
