namespace Raspberry.IO.Components.Displays.Ht16k33
{
    using System;

    using Raspberry.IO.InterIntegratedCircuit;

    /// <summary>
    /// Represents an I2C connection to a HT16K33 (RAM Mapping 16*8 LED Controller Driver).
    /// </summary>
    /// <remarks>See <see cref="http://www.holtek.com/pdf/consumer/ht16K33v110.pdf"/> for specifications.</remarks>
    public class Ht16k33I2cConnection
    {
        // System Setup Command
        private const byte HT16K33_SYSTEM = 32;
        private const byte HT16K33_SYSTEM_OSCILLATOR_ON = 1;

        // Display Setup Command
        private const byte HT16K33_DISPLAY = 128;
        private const byte HT16K33_DISPLAY_ON = 1;
        private const byte HT16K33_DISPLAY_BLINKING_2HZ = 2;
        private const byte HT16K33_DISPLAY_BLINKING_1HZ = 4;
        private const byte HT16K33_DISPLAY_BLINKING_05HZ = 6;

        // Dimming Setup Command
        private const byte HT16K33_DIMMING = 224;
        private const byte DEFAULT_BRIGHTNESS = 15;

        private readonly byte[] ledStateData = new byte[16];
        private readonly II2cDeviceConnection i2cDeviceConnection;

        public Ht16k33I2cConnection(II2cDeviceConnection connection)
        {
            this.i2cDeviceConnection = connection;
            this.Initialize();
        }

        /// <summary>
        /// Refreshes the display so that it matches the state of the connection.
        /// </summary>
        public void Refresh()
        {
            byte i = 0;
            while ((int)i < this.ledStateData.Length)
            {
                this.i2cDeviceConnection.Write(new byte[]
                {
                    i,
                    this.ledStateData[(int)i]
                });
                i += 1;
            }
        }

        /// <summary>
        /// Sets the brightness of the display to the provided value. Valid values are between 0 and 15.
        /// </summary>
        /// <param name="brightness">The new brightness level.</param>
        public void SetBrightness(byte brightness)
        {
            if (brightness > 15)
            {
                throw new ArgumentOutOfRangeException("brightness", "Brightness values must be between 0 and 15");
            }

            this.i2cDeviceConnection.WriteByte((byte)(Ht16k33I2cConnection.HT16K33_DIMMING | brightness));
        }

        /// <summary>
        /// Sets the state of all LEDs.
        /// </summary>
        /// <param name="state">True if the LEDs will be lit, and False otherwise.</param>
        public void SetAllLedState(bool state)
        {
            const byte allOff = 0x00;
            const byte allOn = 0xFF;

            for (int i = 0; i < this.ledStateData.Length; i++)
            {
                this.ledStateData[i] = state ? allOn : allOff;
            }
        }

        /// <summary>
        /// Sets the state of a specific LED.
        /// </summary>
        /// <param name="led">The LED. Valid values are between 0 and 127.</param>
        /// <param name="state">True if the LED will be lit, and False otherwise.</param>
        public void SetLedState(byte led, bool state)
        {
            if (led > 127)
            {
                throw new ArgumentOutOfRangeException("led", "LEDs are numbered between 0 and 127");
            }

            int index = (int)(led / 8);
            int offset = (int)(led % 8);

            if (state)
            {
                this.ledStateData[index] |= (byte)(1 << offset);
            }
            else
            {
                this.ledStateData[index] &= (byte)~(1 << offset);
            }
        }

        /// <summary>
        /// Sets the state of the entire display.
        /// </summary>
        /// <param name="state">True if the display should be ok, and False otherwise.</param>
        public void SetDisplayState(bool state)
        {
            const byte displayOff = Ht16k33I2cConnection.HT16K33_DISPLAY;
            const byte displayOn = Ht16k33I2cConnection.HT16K33_DISPLAY | Ht16k33I2cConnection.HT16K33_DISPLAY_ON;

            this.i2cDeviceConnection.WriteByte(state ? displayOn : displayOff);
        }

        private void Initialize()
        {
            this.i2cDeviceConnection.WriteByte(33);
            this.i2cDeviceConnection.WriteByte(239);

            // Make sure the display is reset before first use.
            this.SetDisplayState(false);
            this.Refresh();
        }
    }
}