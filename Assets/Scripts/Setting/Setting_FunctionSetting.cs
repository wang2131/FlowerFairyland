using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class Setting_FunctionSetting : MonoBehaviour, ISetting
{
    private Setting_FunctionSetting_gamesettings _gamesettings;
    private Setting setting;

    private Text saveTip;

    private void Awake()
    {
        saveTip = transform.Find("SaveTip").GetComponent<Text>();
        _gamesettings = GetComponentInChildren<Setting_FunctionSetting_gamesettings>();
        setting = GetComponentInParent<Setting>();
        
    }

    private void OnEnable()
    {
        saveTip.gameObject.SetActive(false);
        EventCenter.AddListener((EventType)EventName.hold1, ()=>{_gamesettings.chooseIndex(-1);});
        EventCenter.AddListener((EventType)EventName.hold5, ()=>{_gamesettings.chooseIndex(1);});
        EventCenter.AddListener((EventType)EventName.rollCredits, ()=>{_gamesettings.changeValue();});
        EventCenter.AddListener((EventType)EventName.confirm, () => { StartCoroutine(OnSave());});
        EventCenter.AddListener((EventType)EventName.bet, ()=>{setting.SwitchIndex(typeof(Setting_MainSetting));});
    }

    private void OnDisable()
    {
        EventCenter.RemoveAllListener((EventType)EventName.hold1);
        EventCenter.RemoveAllListener((EventType)EventName.hold5);
        EventCenter.RemoveAllListener((EventType)EventName.rollCredits);
        EventCenter.RemoveAllListener((EventType)EventName.confirm);
        EventCenter.RemoveAllListener((EventType)EventName.bet);
    }

    private IEnumerator OnSave()
    {
        _gamesettings.OnSave();
        saveTip.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        saveTip.gameObject.SetActive(false);
    }
    
    
}
