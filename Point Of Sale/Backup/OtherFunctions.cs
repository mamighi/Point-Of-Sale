using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMVExampleDotNet
{
    class OtherFunctions
    {
        public OtherFunctions()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        ushort hex2code(ushort h)
        {
            if (h <= '9') return (ushort)(h - '0');
            else return (ushort)(h - 'A' + 10);
        }
        byte code2hex(byte c)
        {
            if (c <= 9) return (byte)(c + '0');
            else return (byte)(c - 10 + 'A');
        }
        public unsafe void hex2bin(byte* hex, byte* bin, uint byte_amn)
        {
            ushort x, y;
            while (byte_amn > 0)
            {
                x = (ushort)(hex2code((ushort)*hex++) << 4);
                y = hex2code((ushort)*hex++);
                *bin++ = (byte)(x + y);
                byte_amn--;
            }
        }

        public unsafe void bin2hex(byte* bin, byte* hex, uint byte_amn)
        {
            while (byte_amn > 0)
            {
                *hex++ = code2hex((byte)(*bin >> 4));
                *hex++ = code2hex((byte)(*bin & 0xf));
                bin++;
                byte_amn--;
            }
        }
        public unsafe void BinToHex(byte[] bin, byte[] hex, int count)
        {
            fixed (byte* p1 = bin)
            {
                fixed (byte* p2 = hex)
                {
                    bin2hex(p1, p2, (uint)count);
                }
            }
        }
        public unsafe void HexToBin(byte[] hex, byte[] bin, int count)
        {
            fixed (byte* p1 = hex)
            {
                fixed (byte* p2 = bin)
                {
                    hex2bin(p1, p2, (uint)count);
                }
            }
        }
        public ushort BYTE2_TO_WORD(byte[] array)
        {
            return (ushort)((array[0] << 8) | array[1]);
        }

        public void DWORD_TO_BYTE4(byte[] array, UInt32 VALUE)
        {
            UInt32 value = VALUE;
            array[0] = (byte)(value >> 24);
            array[1] = (byte)(value >> 16);
            array[2] = (byte)(value >> 8);
            array[3] = (byte)value;
        }

        public unsafe void BinToHex(byte[] bin, int indexBin, byte[] hex, int indexHex, int count)
        {
            byte[] binArray = new byte[count];
            byte[] hexArray = new byte[count * 2];
            Array.Copy(bin, indexBin, binArray, 0, count);
            BinToHex(binArray, hexArray, count);
            Array.Copy(hexArray, 0, hex, indexHex, hexArray.Length);
        }
    }
}
