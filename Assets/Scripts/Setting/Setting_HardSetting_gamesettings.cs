using System;
using System.Collections;
using System.Collections.Generic;
using GlobalNamaspaces;
using Notification;
using UnityEngine;
using UnityEngine.UI;

public class Setting_HardSetting_gamesettings : MonoBehaviour
{
    public Color selectedColor;
    public Color unselectedColor;

    private Image[] options;
    private Text[] values;

    private int currentIndex = 0;

    private int bZongNandu = 1; //总难度
    private int b4KNandu = 1; //4同难度
    private int bxiaoshunNandu = 1; //小顺难度
    private int bdashunNandu = 1; //大难度
    private int b5KNandu = 1; //5同难度
    private int bBibeiNandu = 1; //比倍难度

    private void Awake()
    {
        options = GetComponentsInChildren<Image>(true);
        values = new Text[options.Length];
        for (int i = 0; i < options.Length; i++)
        {
            values[i] = options[i].GetComponentsInChildren<Text>()[1];
        }
    }

    public void chooseIndex(int value)
    {
        if (value == 1)
        {
            if (currentIndex < options.Length - 1)
            {
                pri_chooseIndex(currentIndex + 1);
                Debug.Log("index+1");
            }
        }
        else if (value == -1)
        {
            if (currentIndex > 0)
            {
                pri_chooseIndex(currentIndex - 1);
                Debug.Log("index-1");
            }
        }
    }

    private void pri_chooseIndex(int value)
    {
        options[currentIndex].color = unselectedColor;
        currentIndex = value;
        options[currentIndex].color = selectedColor;
    }

    public void changeValue()
    {
        switch (currentIndex)
        {
            case 0:
                bZongNandu = (bZongNandu + 1) % 8;
                values[0].text = bZongNandu.ToString();
                break;
            case 1:
                b4KNandu = (b4KNandu + 1) % 4;
                values[1].text = b4KNandu.ToString();
                break;
            case 2:
                bxiaoshunNandu = (bxiaoshunNandu + 1) % 4;
                values[2].text = bxiaoshunNandu.ToString();
                break;
            case 3:
                bdashunNandu = (bdashunNandu + 1) % 4;
                values[3].text = bdashunNandu.ToString();
                break;
            case 4:
                b5KNandu = (b5KNandu + 1) % 4;
                values[4].text = b5KNandu.ToString();
                break;
            case 5:
                bBibeiNandu = (bBibeiNandu + 1) % 8;
                values[5].text = bBibeiNandu.ToString();
                break;
        }
        Debug.Log("ChangeValue");


    }
    public void OnSave()
    {
        JavaToData sender = GameObject.FindObjectOfType<JavaToData>().GetComponent<JavaToData>();
        List<byte> dataList = new List<byte>(new byte[]{0x00, 0x00, 0x01, 0x00 ,0x0e, 0x00, 0x02, 0x02});
        
        dataList.Add((byte)bZongNandu);
        dataList.Add((byte)b4KNandu);
        dataList.Add((byte)bxiaoshunNandu);
        dataList.Add((byte)bdashunNandu);
        dataList.Add((byte)b5KNandu);
        dataList.Add((byte)bBibeiNandu);

        byte[] outputResult = EncodingCenter.Encode(dataList.ToArray());
        
        List<byte> data = new List<byte>();
        byte[] head = new byte[] { 0xfe, 0x01, 0x10, 0x00 };
        data.AddRange(head);
        data.AddRange(outputResult);
        
        sender.jo.Call("SendData", data.ToArray());
    }

    public void GetValue(USBCommunication.SettingInformation information)
    {
        bZongNandu = information.bZongNandu;
        b4KNandu = information.b4KNandu;
        bxiaoshunNandu = information.bxiaoshunNandu;
        bdashunNandu = information.bDashunNandu;
        b5KNandu = information.b5KNandu;
        bBibeiNandu = information.bBibeiNandu;
        
        values[0].text = bZongNandu.ToString();
        values[1].text = b4KNandu.ToString();
        values[2].text = bxiaoshunNandu.ToString();
        values[3].text = bdashunNandu.ToString();
        values[4].text = b5KNandu.ToString();
        values[5].text = bBibeiNandu.ToString();


    }

    public void OnEnable()
    {
        pri_chooseIndex(0);
    }

    public void OnDisable()
    {
        
    }
}
