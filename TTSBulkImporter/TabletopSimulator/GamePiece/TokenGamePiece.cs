using TTSBulkImporter.TabletopSimulator.GamePiece.Component.Image;

namespace TTSBulkImporter.TabletopSimulator.GamePiece
{
    class TokenGamePiece : BaseGamePiece
    {
        public override string Name => "Custom_Token";

        public TokenImage CustomImage = new TokenImage();
    }
}
