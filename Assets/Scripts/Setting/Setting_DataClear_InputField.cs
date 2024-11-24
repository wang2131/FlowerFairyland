﻿using System;
using System.Collections;
using System.Collections.Generic;
using GlobalNamaspaces;
using UnityEngine;
using UnityEngine.UI;

public class Setting_DataClear_InputField : MonoBehaviour
{
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _unselectedColor = Color.white;
    private Image[] _images;
    private Text[] _texts;
    private string password;
    private int currentIndex = 0;
    private void Awake()
    {
        _images = new Image[7];
        _texts = new Text[7];
        password = "0000000";
        for (int i = 0; i < _images.Length; i++)
        {
            _images[i] = transform.Find((i + 1).ToString()).GetComponent<Image>();
            _texts[i] = _images[i].GetComponentInChildren<Text>();
        }
    }

    private void ChangeIndexFromInt(int index)
    {
        if (index >= 0 && index < 7)
        {
            currentIndex = index;
        }
        for (int i = 0; i < _images.Length; i++)
        {
            if (i == currentIndex) _images[i].color = _selectedColor;
            else _images[i].color = _unselectedColor;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction">1是向后，-1向前</param>
    public void ChangeIndex(int direction)
    {
        if (direction == 1)
        {
            ChangeIndexFromInt((currentIndex+1)%7);
        }
        else if (direction == -1)
        {   
            ChangeIndexFromInt((currentIndex+7-1)%7);
        }
        
        
    }

    /// <summary>
    /// 改变值
    /// </summary>
    /// <param name="direction">1为增加，-1为减少</param>
    public void ChangeValue(int direction)
    {
        if (direction == 1)
        {
            int value = int.Parse(password[currentIndex].ToString());
            int changedValue = (value + 1) % 10;
            password = password.Remove(currentIndex, 1);
            password = password.Insert(currentIndex, changedValue.ToString());
        }
        else if (direction == -1)
        {
            int value = int.Parse(password[currentIndex].ToString());
            int changedValue = (value + 10 - 1) % 10;
            password = password.Remove(currentIndex, 1);
            password = password.Insert(currentIndex, changedValue.ToString());
            _texts[currentIndex].text = (changedValue).ToString();
        }

        StartCoroutine(ShowPassword(currentIndex));
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <returns></returns>
    public bool VerifyPassword()
    {
        
        if (password == Save.gameData.gs.sourceCleanCode)
        {
            return true;
        }

        return false;
    }

    public void Reset()
    {
        password = "0000000";
        ChangeIndexFromInt(0);
        foreach (var VARIABLE in _texts)
        {
            VARIABLE.text = "*";
        }
        for (int i = 0; i < _texts.Length; i++)
        {
            StartCoroutine(ShowPassword(i));
        }
    }
    
    public IEnumerator ShowPassword(int index)
    {
        _texts[index].text = password[index].ToString();
        yield return new WaitForSeconds(0.5f);
        _texts[index].text = "*";
    }

    public void SavePassword()
    {
        Save.gameData.gs.sourceCleanCode = password;
        Save.SetGameData(Save.gameData);
    }
}
