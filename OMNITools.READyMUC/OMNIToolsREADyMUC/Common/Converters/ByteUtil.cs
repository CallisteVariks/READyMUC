using Java.Nio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMNIToolsREADyMUC.Common.Converters
{
  public class ByteUtil
  {
    public static byte[] ByteListToByteArray(List<Byte> list)
    {
      if (list == null)
        return null;
      else if (list.Count() == 0)
        return new byte[0];

      return list.ToArray();
    }

    /// <summary>
    /// Check if a specific bit is set in a byte
    /// </summary>
    /// <param name="byteToCheck"></param>
    /// <param name="bit"></param>
    /// <returns></returns>
    public static bool IsBitSet(byte byteToCheck, int bit)
    {
      return (byteToCheck & (1 << bit)) != 0;
    }

    /// <summary>
    /// Converts input to byte array in little endian byte order
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static byte[] ToLittleEndian(long input)
    {
      ByteBuffer bb = ByteBuffer.Allocate(8);
      bb.Order(ByteOrder.LittleEndian);
      bb.PutLong(input);
      return bb.ToArray<byte>();
    }

    /// <summary>
    /// Converts input to byte array in big endian byte order
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static byte[] ToBigEndian(long input)
    {
      ByteBuffer bb = ByteBuffer.Allocate(8);
      bb.Order(ByteOrder.BigEndian);
      bb.PutLong(input);
      return bb.ToArray<byte>();
    }

    /// <summary>
    /// Converts input to byte array in little endian byte order
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static byte[] ToLittleEndian(int input)
    {
      ByteBuffer bb = ByteBuffer.Allocate(4);
      bb.Order(ByteOrder.LittleEndian);
      bb.PutInt(input);
      return bb.ToArray<byte>();
    }

    /// <summary>
    /// Converts input to byte array in little endian byte order
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static byte[] ToLittleEndian(short input)
    {
      ByteBuffer bb = ByteBuffer.Allocate(2);
      bb.Order(ByteOrder.LittleEndian);
      bb.PutShort(input);
      return bb.ToArray<byte>();
    }

    /// <summary>
    /// Converts input to byte array in big endian byte order
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static byte[] ToBigEndian(int input)
    {
      ByteBuffer bb = ByteBuffer.Allocate(4);
      bb.Order(ByteOrder.BigEndian);
      bb.PutInt(input);
      return bb.ToArray<byte>();
    }

    /// <summary>
    /// Converts binary int to packet BCD format
    /// </summary>
    /// <param name="bin"></param>
    /// <returns></returns>
    public static int BinToBcd(int bin)
    {
      int luBcd = 0;
      int luMask = 10000000;

      while (luMask > 0)
      {
        luBcd <<= 4;

        while (bin >= luMask)
        {
          bin -= luMask;
          luBcd++;
        }
        luMask /= 10;
      }
      return luBcd;
    }

    /// <summary>
    /// Converts packed BCD to standard binary format
    /// </summary>
    /// <param name="bcd"></param>
    /// <returns></returns>
    public static int BcdToBin(int bcd)
    {
      int luBase10 = 0;

      // Start with most significant nibble first.
      for (int lcNibbleIdx = 7; lcNibbleIdx >= 0; --lcNibbleIdx)
      {
        luBase10 = luBase10 * 10;
        luBase10 += (bcd >> (lcNibbleIdx * 4)) & 0x0F;
      }

      return luBase10;
    }

    /// <summary>
    /// Converts unsigned byte to int
    /// </summary>
    /// <param name="b"> byte </param>
    /// <returns></returns>
    public static int UnsignedByteToInt(byte b)
    {
      return b & 0xFF;
    }

    public static byte[] GetBCDArray(int value)
    {
      byte[] ret = new byte[4];
      for (int i = 0; i < 4; i++)
      {
        ret[i] = (byte)(value % 10);
        value /= 10;
        ret[i] |= (byte)((value % 10) << 4);
        value /= 10;
      }
      return ret;
    }

    public static String ByteArrayToBinaryString(byte[] bytes)
    {
      StringBuilder sb = new StringBuilder();
      for (int j = 0; j < bytes.Length; j++)
      {
        sb.Append(String.Format("%8s", Java.Lang.Integer.ToBinaryString(bytes[j] & 0xFF)).Replace(' ', '0'));
        if (j < (bytes.Length - 1))
          sb.Append(" ");
      }
      return sb.ToString();
    }

    public static byte[] BinaryStringToByteArray(String binaryString)
    {
      return Enumerable.Range(0, int.MaxValue / 8)
                       .Select(i => i * 8)    // get the starting index of which char segment
                       .TakeWhile(i => i < binaryString.Length)
                       .Select(i => binaryString.Substring(i, 8)) // get the binary string segments
                       .Select(s => Convert.ToByte(s, 2)) // convert to byte
                       .ToArray();
    }

    public static ByteBuffer WrapHex(String hexString)
    {
      return ByteBuffer.Wrap(HexStringByteConverter.HexStringToByteArray(hexString));
    }
  }
}