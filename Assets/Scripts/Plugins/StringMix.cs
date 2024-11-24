using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringMix
{
    /// <summary>
    /// 将数字转换为指定长度的字符串
    /// </summary>
    /// <param name="value"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string IntToString(int value, int number)
    {
        string valueStr = value.ToString();
        if (valueStr.Length < number)
        {
            while (valueStr.Length < number)
            {
                valueStr.Insert(0, "0");
            }

            return valueStr;
        }
        else if (valueStr.Length == number)
        {
            return valueStr;
        }
        else
        {
            return valueStr.Substring(0, number);
        }
        
    }
}
