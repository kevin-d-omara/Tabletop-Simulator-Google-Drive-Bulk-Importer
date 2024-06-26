﻿namespace TTSBulkImporter.TabletopSimulator.GamePiece.Component
{
    public class Transform
    {
        public float posX = 0f;
        public float posY = 0f;
        public float posZ = 0f;

        public float rotX = 0f;
        public float rotY = 180f; // Defaults to 180 instead of 0 so that the image will be face-up to the default TTS camera.
        public float rotZ = 0f;

        public float scaleX = 1f;
        public float scaleY = 1f;
        public float scaleZ = 1f;

        public Transform() {}

        public Transform(
            float posX, float posY, float posZ,
            float rotX, float rotY, float rotZ,
            float scaleX, float scaleY, float scaleZ
            )
        {
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;

            this.rotX = rotX;
            this.rotY = rotY;
            this.rotZ = rotZ;

            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.scaleZ = scaleZ;
        }
    }
}
