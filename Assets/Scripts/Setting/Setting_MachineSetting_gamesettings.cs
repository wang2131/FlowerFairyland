using System;
using System.Collections;
using System.Collections.Generic;
using GlobalNamaspaces;
using UnityEngine;
using UnityEngine.UI;

public class Setting_MachineSetting_gamesettings : MonoBehaviour
{
    public Color selectedColor;
    public Color unselectedColor;

    private Image[] options;
    private Text[] values;

    private int currentIndex = 0;

    private int stationId;
    private int volumn;
    private int openCloudBonus;

    private void Awake()
    {
        options = GetComponentsInChildren<Image>();
        values = new Text[options.Length];
        for (int i = 0; i < options.Length; i++)
        {
            values[i] = options[i].gameObject.transform.Find("value").GetComponent<Text>();
        }
    }
    
    /// <summary>
    /// 切换选项
    /// </summary>
    /// <param name="value">1向下，-1向上</param>
    public void chooseIndex(int value)
    {
        if (value == 1)
        {
            if (currentIndex < options.Length - 1)
            {
                pri_chooseIndex(currentIndex+1);
            }
        }
        else if (value == -1)
        {
            if (currentIndex > 0)
            {
                pri_chooseIndex(currentIndex-1);
            }
        }

    }
    
    private void pri_chooseIndex(int value)
    {
        options[currentIndex].color = unselectedColor;
        currentIndex = value;
        options[currentIndex].color = selectedColor;
    }

    /// <summary>
    /// 切换当前选项的值
    /// </summary>
    public void changeValue()
    {
        switch (currentIndex)
        {
            case 0:
                stationId = (stationId + 1) % Save.stationId_tbl.Length;
                values[0].text = Save.stationId_tbl[stationId].ToString();
                break;
            case 1:
                volumn = (volumn + 1) % Save.volumn_tbl.Length;
                values[1].text = Save.volumn_tbl[volumn].ToString();
                break;
            case 2:
                openCloudBonus = (openCloudBonus + 1) % Save.openCloudBonus_tbl.Length;
                values[2].text = Save.openCloudBonus_tbl[openCloudBonus].ToString();
                break;
        }
    }

    public void OnSave()
    {
        Save.gameData.gs.stationId = stationId;
        Save.gameData.gs.volumn = volumn;
        Save.gameData.gs.openCloudBonus = openCloudBonus;
        Save.SetGameData(Save.gameData);
    }

    private void OnEnable()
    {
        stationId = Save.gameData.gs.stationId;
        values[0].text = Save.stationId_tbl[stationId].ToString();
        volumn = Save.gameData.gs.volumn;
        values[1].text = Save.volumn_tbl[volumn].ToString();
        openCloudBonus = Save.gameData.gs.openCloudBonus;
        values[2].text = Save.openCloudBonus_tbl[openCloudBonus].ToString();
        
        pri_chooseIndex(0);
    }

    private void OnDisable()
    {
        
    }
}
