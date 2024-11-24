using System;
using System.Collections;
using System.Collections.Generic;
using GlobalNamaspaces;
using UnityEngine;
using UnityEngine.UI;

public class Setting_DataClear : MonoBehaviour, ISetting
{
    private Setting setting;
    private Setting_DataClear_InputField _inputField;
    private bool isCleared = false;
    private bool isReseting;
    private Text _resultText;
    private Text _idText;
    private Text _clearCode;
    private GameObject _buttons;
    private Text _buttonsText;
    
    

    private void Awake()
    {
        setting = GetComponentInParent<Setting>();
        _inputField = transform.Find("InputField").GetComponent<Setting_DataClear_InputField>();
        _resultText = transform.Find("ResultText").GetComponent<Text>();
        _idText = transform.Find("ID").GetComponent<Text>();
        _clearCode = transform.Find("ClearCode").GetComponent<Text>();
        _buttons = transform.Find("Buttons").gameObject;
        _buttonsText = transform.Find("Buttons").Find("ButtonText").GetComponent<Text>();

    }

    private void OnEnable()
    {
        isReseting = false;
        _buttonsText.text = "";
        _buttons.SetActive(false);
        AddInputFieldListener();
    }

    private void returnToMainSetting()
    {
        setting.SwitchIndex(typeof(Setting_MainSetting));
    }

    private void OnDisable()
    {
        
    }

    private void AddInputFieldListener()
    {
        
        isCleared = false;
        _idText.text = "ID:" + Save.gameData.gs.deviceId;
        _clearCode.text = "清除码：" + Save.gameData.gs.sourceCleanCode;
        _resultText.text = "";
        
        _inputField.Reset();
        EventCenter.AddListener((EventType)EventName.bet, () =>
        {
            if (!isCleared) setting.SwitchIndex(typeof(Setting_MainSetting));
        });
        EventCenter.AddListener((EventType)EventName.hold1, () =>
        {
            if (!isCleared) _inputField.ChangeIndex(-1);
        });
        EventCenter.AddListener((EventType)EventName.hold5, () =>
        {
            if (!isCleared) _inputField.ChangeIndex(1);
        });
        EventCenter.AddListener((EventType)EventName.rollCredits, () =>
        {
            if (!isCleared) _inputField.ChangeValue(1);
        });        
        EventCenter.AddListener((EventType)EventName.guess, () =>
        {
            if (!isCleared) _inputField.ChangeValue(-1);
        });
        EventCenter.AddListener((EventType)EventName.confirm, ()=>{

                if (!isReseting)
                {


                    if (_inputField.VerifyPassword())
                    {
                        RemoveInputFieldListener();
                        _buttons.SetActive(true);
                        AddButtonsListener();

                    }
                    else
                    {
                        _resultText.text = "密码错误，请重试";
                        _inputField.Reset();
                    }
                }
                else
                {
                    _inputField.SavePassword();
                    _resultText.text = "密码修改成功！";
                    Save.SetGameData(Save.gameData);
                    isCleared = true;
                    RemoveInputFieldListener();
                    Invoke("returnToMainSetting", 2f);
                }

        });

    }

    private void RemoveInputFieldListener()
    {
        EventCenter.RemoveAllListener((EventType)EventName.bet);
        EventCenter.RemoveAllListener((EventType)EventName.hold1);
        EventCenter.RemoveAllListener((EventType)EventName.hold5);
        EventCenter.RemoveAllListener((EventType)EventName.rollCredits);
        EventCenter.RemoveAllListener((EventType)EventName.guess);
        EventCenter.RemoveAllListener((EventType)EventName.confirm);
    }

    private void AddButtonsListener()
    {
        EventCenter.AddListener((EventType)EventName.hold1, () =>
        {
            isCleared = true;
            
            //Save.ResetDataInSetting();
            Save.Reset2DefaultStationData();
            Save.Reset2DefaultBookkeeping();
            Save.ResetGameRecord();
            
            _buttonsText.text = "数据清除成功";
            Save.SetGameData(Save.gameData);
            Invoke("returnToMainSetting", 2f);
        });
        EventCenter.AddListener((EventType)EventName.hold2, () =>
        {
            _buttons.SetActive(false);
            isReseting = true;
            RemoveButtonsListener();
            AddInputFieldListener();
        });
        EventCenter.AddListener((EventType)EventName.hold3, () =>
        {
            setting.SwitchIndex(typeof(Setting_HardSetting));
            RemoveButtonsListener();
        });
        EventCenter.AddListener((EventType)EventName.bet, () =>
        {
            if (!isCleared)
            {
                setting.SwitchIndex(typeof(Setting_MainSetting));
                RemoveButtonsListener();
            }
                
        });
    }

    private void RemoveButtonsListener()
    {

        EventCenter.RemoveAllListener((EventType)EventName.bet);
        EventCenter.RemoveAllListener((EventType)EventName.hold1);
        EventCenter.RemoveAllListener((EventType)EventName.hold2);
        EventCenter.RemoveAllListener((EventType)EventName.hold3);
    }
}
