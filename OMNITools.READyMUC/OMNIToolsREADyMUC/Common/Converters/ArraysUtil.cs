using Java.Lang;

namespace OMNIToolsREADyMUC.Common.Converters
{
  public  class ArraysUtil
  {
    /// <summary>
    /// Reset the given bytes in a byte array
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="index"></param>
    /// <param name="length"></param>
    public static void Clear(byte[] bytes, int index, int length)
    {
      for (int i = index; i < length + index; i++)
      {
        bytes[i] = 0;
      }
    }

    public static void ReverseArrayOrder(byte[] array)
    {
      if (array == null) { return; }
      int i = 0;
      int j = array.Length - 1;
      byte tmp;
      while (j > i)
      {
        tmp = array[j];
        array[j] = array[i];
        array[i] = tmp;
        j--;
        i++;
      }
    }

    /// <summary>
    /// Converts a byte array into a long value - maximum length of 8 bytes
    /// </summary>
    /// <param name="byteArray"> the byte array to convert </param>
    /// <param name="lsbIsLast"> true if array is BigEndian, false if LittleEndian </param>
    /// <returns></returns>
    public static long ConvertByteArrayToLong(byte[] byteArray, bool lsbIsLast)
    {
      if (byteArray.Length > 8)
        throw new IllegalArgumentException();

      long value = 0;
      if (!lsbIsLast)
      {
        for (int i = 0; i < byteArray.Length; i++)
          value += (byteArray[i] & 0xffL) << (8 * i);
      }
      else
      {
        // Is the first byte the most significant?
        for (int i = 0; i < byteArray.Length; i++)
          value = (value << 8) + (byteArray[i] & 0xff);
      }
      return value;
    }
  }
}