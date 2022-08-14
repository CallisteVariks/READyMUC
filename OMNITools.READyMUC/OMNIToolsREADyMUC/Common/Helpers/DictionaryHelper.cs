using OMNIToolsREADyMUC.Common.Enums;
using System;
using System.Collections.Generic;

namespace OMNIToolsREADyMUC.Common.Helpers
{
  public class DictionaryHelper
  {
    #region Methods for MBusTransport
    public static Dictionary<Byte, MBusTransportEnum> MBusTransportDictionary = new Dictionary<byte, MBusTransportEnum>();

    public void MBusTransportReverseByteEnumMap(Type valueType)
    {
      foreach (MBusTransportEnum transport in Enum.GetValues(valueType))
        MBusTransportDictionary.Add((byte)transport, transport);
    }

    public static MBusTransportEnum MBusTransportGet(byte val)
    {
      return MBusTransportDictionary[val];
    }
    #endregion


    #region Methods for MBusMode
    public static Dictionary<Byte, MBusModeEnum> MBusModeDictionary = new Dictionary<byte, MBusModeEnum>();

    public void MBusModeReverseByteEnumMap(Type valueType)
    {
      foreach (MBusModeEnum transport in Enum.GetValues(valueType))
        MBusModeDictionary.Add((byte)transport, transport);
    }

    public static MBusModeEnum MBusModeGet(byte val)
    {
      return MBusModeDictionary[val];
    }
    #endregion


    #region Methods for KmRfCommand
    public static Dictionary<Byte, KmRfCommandEnum> KmRfCommandDictionary = new Dictionary<byte, KmRfCommandEnum>();

    public void KmRfCommandReverseByteEnumMap(Type valueType)
    {
      foreach (KmRfCommandEnum transport in Enum.GetValues(valueType))
        KmRfCommandDictionary.Add((byte)transport, transport);
    }

    public static KmRfCommandEnum KmRfCommandGet(byte val)
    {
      return KmRfCommandDictionary[val];
    }
    #endregion


  }
}