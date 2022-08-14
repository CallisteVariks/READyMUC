using System;
using System.Collections.Generic;
using System.Linq;

namespace OMNIToolsREADyMUC.Common.Converters
{
  public class HexStringByteConverter
  {
    public static byte[] HexStringToByteArray(String s)
    {
      s.Trim();

      int len = s.Length;
      byte[] data = new byte[len / 2];

      for (int i = 0; i < len; i += 2)
        data[i / 2] = (byte)((Java.Lang.Character.Digit(s.ElementAt(i), 16) << 4) + Java.Lang.Character.Digit(s.ElementAt(i + 1), 16));

      return data;
    }

    public static String ByteArrayToHexString(List<Byte> byteList)
    {
      return ByteArrayToHexString(ByteUtil.ByteListToByteArray(byteList));
    }

    public static String ByteArrayToHexString(byte[] bytes)
    {
      char[] hexArray = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
      char[] hexChars = new char[bytes.Length * 2];
      int v;
      for (int j = 0; j < bytes.Length; j++)
      {
        v = bytes[j] & 0xFF;
      /////  hexChars[j * 2] = hexArray[v >>> 4];
        hexChars[j * 2 + 1] = hexArray[v & 0x0F];
      }
      return new String(hexChars);
    }

  }
}