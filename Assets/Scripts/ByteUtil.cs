using UnityEngine;
using System.Collections;

public static class ByteUtil {
    public static int byteArray2Int(byte[] bt, int starIndex)//byte转int
    {
        int i = System.BitConverter.ToInt32(bt, starIndex);
        return i;
    }
    public static byte[] int2ByteArray(int num)//int转byte
    {
        byte[] bt = System.BitConverter.GetBytes(num);
        return bt;
    }
    public static float byteArray2Float(byte[] bt, int starIndex)//byte转float
    {
        float f = System.BitConverter.ToSingle(bt, starIndex);
        return f;
    }
    public static byte[] float2ByteArray(float f)//float转byte
    {
        byte[] bt = System.BitConverter.GetBytes(f);
        return bt;
    }
}
