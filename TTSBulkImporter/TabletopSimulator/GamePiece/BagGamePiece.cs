using System.Collections.Generic;
using TTSBulkImporter.TabletopSimulator.GamePiece.Component;

namespace TTSBulkImporter.TabletopSimulator.GamePiece
{
    public class BagGamePiece : BaseGamePiece
    {
        public override string Name => "Bag";

        public ICollection<BaseGamePiece> ContainedObjects = new List<BaseGamePiece>();

        public BagGamePiece()
        {
            ColorDiffuse = ColorDiffuse.Colors.Brown;
        }
    }
}
