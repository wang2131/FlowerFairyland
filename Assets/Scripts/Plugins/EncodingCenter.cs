using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncodingCenter : MonoBehaviour
{
    public static byte[] Decode(byte[] data)
    {
        bool isAdded = false;
        
        List<byte> result = new List<byte>();
        result.AddRange(data);
        byte[] usTypeBytes = result.GetRange(0, 2).ToArray();
        result.RemoveRange(0, 2);
        if (result.Count % 2 != 0)
        {
            result.Add((byte)0x00);
            isAdded = true;
        }
        byte[] tempData = result.ToArray();
        DecodeData_UseKeyNo(0xaa01, tempData);
        //Console.WriteLine("解码：" + BitConverter.ToString(tempData));
        result.Clear();
        result.AddRange(tempData);
        if (isAdded)
        {
            result.RemoveAt(result.Count - 1);
        }
        byte[] checksumData = result.GetRange(result.Count - 2, 2).ToArray();
        //byte temp = checksumData[0];
        //checksumData[0] = checksumData[1];
        //checksumData[1] = temp;
        //short checksum = BitConverter.ToInt16(checksumData);
        result.RemoveRange(result.Count - 2, 2);
        //Console.WriteLine("校验码：" + checksumData);
        
        result.InsertRange(0, usTypeBytes);
        bool flag = VerifyCheckSum(result.ToArray(), checksumData);
        //Console.WriteLine("校验码验证结果：" + flag);
        return result.ToArray();

    } 

    public static byte[] Encode(byte[] data)
    {
        bool isAdded = false;

        List<byte> result = new List<byte>();
        result.AddRange(data);
        byte[] usTypeBytes = result.GetRange(0, 2).ToArray();
        result.RemoveRange(0, 2);
        ushort checkSum = CalculateChecksum(result.ToArray());
        byte[] checksumData = BitConverter.GetBytes(checkSum);
        result.AddRange(checksumData);
        if (result.Count % 2 != 0)
        {
            result.Add((byte)0x00);
            isAdded = true;
        }
        byte[] EncodeResult = result.ToArray();
        Debug.Log("添加校验码后");
        EncodeData_UseKeyNo(0xaa01, EncodeResult);
        //Console.WriteLine("位数：" + EncodeResult.Length);
        result.Clear();
        result.AddRange(EncodeResult);
        result.InsertRange(0, usTypeBytes);
        if(isAdded)
        {
            result.RemoveAt(result.Count-1);
        }
        return result.ToArray();
    }

    public static ushort CalculateChecksum(byte[] data)
    {
        if (data == null || data.Length < 2)
        {
            throw new ArgumentException("Input data is invalid.");
        }

        
        ushort checksum = 0;

        
        for (int i = 0; i < data.Length; i++)
        {
            checksum += data[i];
        }


        checksum = CalculateInverseAddOne(checksum);

        return checksum;
    }

    public static bool VerifyCheckSum(byte[] data, byte[] checksum)
    {
        byte[] TargetCheckSum = BitConverter.GetBytes(CalculateChecksum(data));
        byte temp = TargetCheckSum[0];
        TargetCheckSum[0] = TargetCheckSum[1];
        TargetCheckSum[1] = temp;
        return TargetCheckSum == checksum;

    }
    public static void DecodeData_UseKeyNo(ushort usKeyNo, byte[] data)
    {
        ushort usTemp = usKeyNo;
        int length = data.Length;
        int i = 0;
        for (i = 0; i < length; i += sizeof(ushort))
        {

            byte[] chunk = new byte[sizeof(ushort)];
            Array.Copy(data, i, chunk, 0, sizeof(ushort));

            ushort chunkAsUshort = BitConverter.ToUInt16(chunk, 0);
            ushort xorResult = (ushort)(chunkAsUshort ^ usTemp);

            usTemp = chunkAsUshort;

            byte[] xorResultAsBytes = BitConverter.GetBytes(xorResult);
            Array.Copy(xorResultAsBytes, 0, data, i, sizeof(ushort));
        }
    }
    public static void EncodeData_UseKeyNo(ushort usKeyNo, byte[] data)
    {
        ushort usTemp = usKeyNo;
        int length = data.Length;
        int i = 0;
        for (i = 0; i < length; i += sizeof(ushort))
        {

            byte[] chunk = new byte[sizeof(ushort)];
            Array.Copy(data, i, chunk, 0, sizeof(ushort));

            ushort chunkAsUshort = BitConverter.ToUInt16(chunk, 0);
            ushort xorResult = (ushort)(chunkAsUshort ^ usTemp);

            usTemp = xorResult;

            byte[] xorResultAsBytes = BitConverter.GetBytes(xorResult);
            Array.Copy(xorResultAsBytes, 0, data, i, sizeof(ushort));
        }
    }
    private static ushort CalculateInverseAddOne(ushort value)
    {
        if (value == short.MinValue)
        {
            // 当值为最小值时，需要特殊处理，因为反码加一后不是最大值
            return ushort.MaxValue;
        }
        // 计算反码
        ushort usValue = value;
        ushort usInverse = (ushort)(~usValue);
        ushort usAddOne = (ushort)(usInverse + 1);
        
        ushort result = (ushort)usAddOne;
        return result;
    }
}
