namespace Raspberry.IO.InterIntegratedCircuit
{
    public interface II2cDeviceConnection
    {
        /// <summary>
        /// Reads the specified number of bytes.
        /// </summary>
        /// <param name="byteCount">The byte count.</param>
        /// <returns>The buffer.</returns>
        byte[] Read(int byteCount);

        /// <summary>
        /// Reads a byte.
        /// </summary>
        /// <returns>The byte.</returns>
        byte ReadByte();

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        void Write(params byte[] buffer);

        /// <summary>
        /// Writes the specified byte.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteByte(byte value);
    }
}
