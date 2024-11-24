using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByteScript : MonoBehaviour
{
    public static int getHeight4(byte data){//获取高四位
        int height;
        height = ((data & 0xf0) >> 4);
        return height;
    }
  
    public static int getLow4(byte data){//获取低四位
        int low;
        low = (data & 0x0f);//0x0f(00001111)
        return low;    
    }
}
