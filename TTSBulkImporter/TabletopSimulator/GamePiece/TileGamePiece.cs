using TTSBulkImporter.TabletopSimulator.GamePiece.Component.Image;

namespace TTSBulkImporter.TabletopSimulator.GamePiece
{
    public class TileGamePiece : BaseGamePiece
    {
        public override string Name => "Custom_Tile";

        public TileImage CustomImage = new TileImage();
    }
}
