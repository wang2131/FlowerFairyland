using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting_KeyTest : MonoBehaviour
{
    private Setting setting;
    private Setting_KeyTest_KeyManager _keyManager;

    //连按间隔
    private float delay = 0.3f;
    //上次按下时间
    private float PreviousButtonPressTime = -1f;

    private void Awake()
    {
        setting = GetComponentInParent<Setting>();
        _keyManager = transform.Find("Keys").GetComponent<Setting_KeyTest_KeyManager>();
    }

    private void OnEnable()
    {
        PreviousButtonPressTime = -1f;
        EventCenter.AddListener((EventType)EventName.hold1, ()=>{_keyManager.OnClickButton(EventName.hold1);});
        EventCenter.AddListener((EventType)EventName.hold2, ()=>{_keyManager.OnClickButton(EventName.hold2);});
        EventCenter.AddListener((EventType)EventName.hold3, ()=>{_keyManager.OnClickButton(EventName.hold3);});
        EventCenter.AddListener((EventType)EventName.hold4, ()=>{_keyManager.OnClickButton(EventName.hold4);});
        EventCenter.AddListener((EventType)EventName.hold5, ()=>{_keyManager.OnClickButton(EventName.hold5);});
        EventCenter.AddListener((EventType)EventName.bet, () =>
        {
            _keyManager.OnClickButton(EventName.bet);
            if (Time.time - PreviousButtonPressTime <= delay)
            {
                setting.SwitchIndex(typeof(Setting_MainSetting));
            }
            else
            {
                PreviousButtonPressTime = Time.time;
            }
        });
        EventCenter.AddListener((EventType)EventName.hopperOutKey, ()=>{_keyManager.OnClickButton(EventName.hopperOutKey);});
        EventCenter.AddListener((EventType)EventName.guess, ()=>{_keyManager.OnClickButton(EventName.guess);});
        EventCenter.AddListener((EventType)EventName.big, ()=>{_keyManager.OnClickButton(EventName.big);});
        EventCenter.AddListener((EventType)EventName.small, ()=>{_keyManager.OnClickButton(EventName.small);});
        EventCenter.AddListener((EventType)EventName.rollCredits, ()=>{_keyManager.OnClickButton(EventName.rollCredits);});
        EventCenter.AddListener((EventType)EventName.confirm, ()=>{_keyManager.OnClickButton(EventName.confirm);});
        EventCenter.AddListener((EventType)EventName.addCredits, ()=>{_keyManager.OnClickButton(EventName.addCredits);});
        EventCenter.AddListener((EventType)EventName.reduceCredits, ()=>{_keyManager.OnClickButton(EventName.reduceCredits);});
        EventCenter.AddListener((EventType)EventName.gameSetting, ()=>{_keyManager.OnClickButton(EventName.gameSetting);});
    }

    private void OnDisable()
    {
        EventCenter.RemoveAllListener((EventType)EventName.hold1);
        EventCenter.RemoveAllListener((EventType)EventName.hold2);
        EventCenter.RemoveAllListener((EventType)EventName.hold3);
        EventCenter.RemoveAllListener((EventType)EventName.hold4);
        EventCenter.RemoveAllListener((EventType)EventName.hold5);
        EventCenter.RemoveAllListener((EventType)EventName.bet);
        EventCenter.RemoveAllListener((EventType)EventName.hopperOutKey);
        EventCenter.RemoveAllListener((EventType)EventName.guess);
        EventCenter.RemoveAllListener((EventType)EventName.big);
        EventCenter.RemoveAllListener((EventType)EventName.small);
        EventCenter.RemoveAllListener((EventType)EventName.rollCredits);
        EventCenter.RemoveAllListener((EventType)EventName.confirm);
        EventCenter.RemoveAllListener((EventType)EventName.addCredits);
        EventCenter.RemoveAllListener((EventType)EventName.reduceCredits);
        EventCenter.RemoveAllListener((EventType)EventName.gameSetting);
    }

    private void Update()
    {
        
    }
}
