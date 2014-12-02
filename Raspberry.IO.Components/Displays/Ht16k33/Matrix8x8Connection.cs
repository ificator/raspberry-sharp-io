namespace Raspberry.IO.Components.Displays.Ht16k33
{
    using System;

    using Raspberry.IO.InterIntegratedCircuit;

    public class Matrix8x8Connection : Ht16k33I2cConnection
    {
        private readonly Rotation rotation;

        public enum Rotation
        {
            Rotate0,
            Rotate90,
            Rotate180,
            Rotate270,
        }

        public Matrix8x8Connection(II2cDeviceConnection connection, Rotation rotation = Rotation.Rotate0)
            : base(connection)
        {
            this.rotation = rotation;
        }

        /// <summary>
        /// Sets the state of the specified pixel.
        /// </summary>
        /// <param name="x">The x coordinate. Valid values are between 0 and 7.</param>
        /// <param name="y">The y coordinate. Valid values are between 0 and 7.</param>
        /// <param name="state">True if the pxiel will be lit, and False otherwise.</param>
        public void SetPixelState(byte x, byte y, bool state)
        {
            if (x >= 8)
            {
                throw new ArgumentOutOfRangeException("x", "X must be between 0 and 7");
            }

            if (y >= 8)
            {
                throw new ArgumentOutOfRangeException("y", "Y must be between 0 and 7");
            }

            byte actualX = x;
            byte actualY = y;

            switch (this.rotation)
            {
                case Rotation.Rotate90:
                    actualX = (byte)(7 - y);
                    actualY = (byte)(7 - x);
                    break;

                case Rotation.Rotate180:
                    actualX = (byte)(7 - x);
                    actualY = (byte)(7 - y);
                    break;

                case Rotation.Rotate270:
                    actualX = y;
                    actualY = x;
                    break;
            }

            this.SetLedState((byte)(actualY * 16 + (actualX + 7) % 8), state);
        }
    }
}
