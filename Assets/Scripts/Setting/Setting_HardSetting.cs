using System;
using System.Collections;
using System.Collections.Generic;
using Notification;
using UnityEngine;
using UnityEngine.UI;

public class Setting_HardSetting : MonoBehaviour
{
    [SerializeField]private Setting_HardSetting_gamesettings _gamesettings;
    [SerializeField]private Setting setting;

    [SerializeField]private Text saveTip;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        saveTip.gameObject.SetActive(false);
        OnLoad();
    }

    private void OnDisable()
    {
        RemoveListener();
    }

    private void AddListener()
    {
        EventCenter.AddListener((EventType)EventName.hold1, ()=>{_gamesettings.chooseIndex(-1);});
        EventCenter.AddListener((EventType)EventName.hold5, ()=>{_gamesettings.chooseIndex(1);});
        EventCenter.AddListener((EventType)EventName.rollCredits, ()=>{_gamesettings.changeValue();});
        EventCenter.AddListener((EventType)EventName.confirm, () => { StartCoroutine(OnSave());});
        EventCenter.AddListener((EventType)EventName.bet, ()=>{setting.SwitchIndex(typeof(Setting_MainSetting));});
    }

    private void RemoveListener()
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

    private void OnLoad()
    {
        UsbEventCenter.AddListener((EventType)EventName.getHardSetting, (USBCommunication.SettingInformation information) =>
        {
            UsbEventCenter.RemoveAllListener((EventType)EventName.getHardSetting);
            _gamesettings.GetValue(information);
            AddListener();
            
        });
        List<byte> dataList = new List<byte> { 0x00, 0x00, 0x01, 0x00, 0x08, 0x00, 0x01, 0x02 };
        byte[] outputResult = EncodingCenter.Encode(dataList.ToArray());
        
        List<byte> data = new List<byte>();
        byte[] head = new byte[] { 0xfe, 0x01, 0x0a, 0x00 };
        data.AddRange(head);
        data.AddRange(outputResult);
        
        JavaToData sender = GameObject.FindObjectOfType<JavaToData>().GetComponent<JavaToData>();
        sender.jo.Call("SendData", data.ToArray());
    }
}
