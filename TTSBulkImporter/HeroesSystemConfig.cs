using TTSBulkImporter.TabletopSimulator.GamePiece.Component;

namespace TTSBulkImporter
{
    public static class HeroesSystemConfig
    {
        public static class Colors
        {
            public static ColorDiffuse Default => ColorDiffuse.Colors.Default;

            public static ColorDiffuse Pink => ColorDiffuse.Colors.Pink;

            public static ColorDiffuse US => new ColorDiffuse(0.39607808f, 0.5058824f, 0.286274165f); // hex: 648148

            public static ColorDiffuse GE => new ColorDiffuse(0.2666619f, 0.294113219f, 0.3097992f); // hex: 434A4E

            public static ColorDiffuse CW => new ColorDiffuse(0.443134367f, 0.3725461f, 0.2784285f); // hex: 705E46

            public static ColorDiffuse FFI => new ColorDiffuse(0.576470554f, 0.545097947f, 0.509800732f); // hex: 928A81

            public static ColorDiffuse Civilian => FFI;

            public static ColorDiffuse Customization => new ColorDiffuse(0.2941174f, 0.450980365f, 0.407842875f); // hex: 4A7267

            public static ColorDiffuse TerrainBoard => new ColorDiffuse(0.407842726f, 0.443136871f, 0.1882349f); // hex: 67702F
        }
    }
}
