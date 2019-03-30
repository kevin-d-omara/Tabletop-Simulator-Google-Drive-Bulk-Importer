namespace TTSBulkImporter.TabletopSimulator.GamePiece.Component.Image
{
    public class TileImageProperties : BaseImageProperties
    {
        public TileType Type = TileType.Rounded;    // I'm cheating, don't look at me!!
        public bool Stretch = true;

        public enum TileType
        {
            Box,
            Hex,
            Circle,
            Rounded
        }
    }
}
