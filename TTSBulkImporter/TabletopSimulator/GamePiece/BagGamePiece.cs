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
            // Default brown colored bag in TSS.
            ColorDiffuse = new ColorDiffuse(0.7058823f, 0.366520882f, 0.0f);
        }
    }
}
