using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Setting_KeyTest_KeyManager : SerializedMonoBehaviour
{
    [SerializeField] private SerializableDictionary<string, Image> Keys;

    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _unselectedColor;
    

    public void ResetAll()
    {
        foreach (var VARIABLE in Keys)
        {
            VARIABLE.Value.color = _unselectedColor;
        }
    }

    public void OnClickButton(EventName name)
    {
        foreach (var VARIABLE in Keys)
        {
            if (VARIABLE.Key == name.ToString())
            {
                VARIABLE.Value.color = _selectedColor;
            }
            else
            {
                VARIABLE.Value.color = _unselectedColor;
            }
        }
    }
}