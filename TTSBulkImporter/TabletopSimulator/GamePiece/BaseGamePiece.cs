using TTSBulkImporter.TabletopSimulator.GamePiece.Component;

namespace TTSBulkImporter.TabletopSimulator.GamePiece
{
    public abstract class BaseGamePiece
    {
        // Custom Object Type
        public abstract string Name { get; }    // Custom_Tile, Custom_Model, etc.

        // User Name/Description
        public string Nickname     = "";
        public string Description  = "";

        // Physical Properties
        public Transform Transform         = new Transform();
        public ColorDiffuse ColorDiffuse   = new ColorDiffuse();

        // Toggles
        public bool Locked         = false;
        public bool Grid           = true;
        public bool Snap           = true;
        public bool Autoraise      = true;
        public bool Sticky         = true;
        public bool Tooltip        = true;
        public bool GridProjection = false;
        public bool Hands          = false;

        // Scripting
        public string XmlUI            = "";
        public string LuaScript        = "";
        public string LuaScriptState   = "";
        public string GUID             = GuidProvider.NewGuid();
    }
}
