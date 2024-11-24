using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GlobalNamaspaces;

public class Setting_ChangePwd : MonoBehaviour, ISetting
{
    private Setting setting;
    private Setting_ChangePwd_InputField _inputField;
    private bool isChanged = false;
    private Text _resultText;
    

    private void Awake()
    {
        setting = GetComponentInParent<Setting>();
        _inputField = transform.Find("InputField").GetComponent<Setting_ChangePwd_InputField>();
        _resultText = transform.Find("ResultText").GetComponent<Text>();
        
        
    }

    private void OnEnable()
    {
        isChanged = false;
        
        _resultText.text = "";
        _inputField.Reset();
        EventCenter.AddListener((EventType)EventName.bet, () =>
        {
            if(isChanged == false)setting.SwitchIndex(typeof(Setting_MainSetting));
        });
        EventCenter.AddListener((EventType)EventName.hold1, () =>
        {
            if(isChanged == false)_inputField.ChangeIndex(-1);
        });
        EventCenter.AddListener((EventType)EventName.hold5, () =>
        {
            if(isChanged == false)_inputField.ChangeIndex(1);
        });
        EventCenter.AddListener((EventType)EventName.rollCredits, () =>
        {
            if(isChanged == false)_inputField.ChangeValue(1);
        });        
        EventCenter.AddListener((EventType)EventName.guess, () =>
        {
            if(isChanged == false)_inputField.ChangeValue(-1);
        });
        EventCenter.AddListener((EventType)EventName.confirm, ()=>{
            if (isChanged == false)
            {
                int a = _inputField.VerifyPassword();
                if (a == 1)
                {
                    _resultText.text = "修改成功";
                    isChanged = true;
                    Save.SetGameData(Save.gameData);
                    Invoke("returnToMainSetting", 2f);
                }
                else if(a == 0)
                {
                    _resultText.text = "请再次确认一遍";
                }
                else if (a == -1)
                {
                    _resultText.text = "两次输入密码不同，请输入新密码";
                }
            }
        });
        
        
    }

    private void returnToMainSetting()
    {
        setting.SwitchIndex(typeof(Setting_MainSetting));
    }

    private void OnDisable()
    {
        EventCenter.RemoveAllListener((EventType)EventName.bet);
        EventCenter.RemoveAllListener((EventType)EventName.hold1);
        EventCenter.RemoveAllListener((EventType)EventName.hold5);
        EventCenter.RemoveAllListener((EventType)EventName.rollCredits);
        EventCenter.RemoveAllListener((EventType)EventName.guess);
        EventCenter.RemoveAllListener((EventType)EventName.confirm);
    }
}
