using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using GlobalNamaspaces;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Setting_GameRecord : SerializedMonoBehaviour
{
    private Setting setting;
    private Image[] images;
    private Text[] texts;
    
    [SerializeField] private Sprite littleJoker;
    [SerializeField] private Sprite bigJoker;
    public SerializableDictionary<byte, Sprite> table = new SerializableDictionary<byte, Sprite>();
    
    private Text bet;
    private Text win;
    private Text indexText;

    private int currentIndex = 0;
    private void Awake()
    {
        images = new Image[10];
        texts = new Text[10];
        for (int i = 0; i <= 1; i++)
        {
            for (int j = 1; j <= 5; j++)
            {
                images[(i*5)+j - 1] = transform.Find("RecordCount").Find((i+1).ToString() + "_" + j.ToString())
                    .GetComponentInChildren<Image>();
                texts[(i*5)+j - 1] = transform.Find("RecordCount").Find((i+1).ToString() + "_" + j.ToString())
                    .GetComponentInChildren<Text>();
            }
        }
        setting = GetComponentInParent<Setting>();
        bet = transform.Find("Bet").GetComponent<Text>();
        win = transform.Find("Win").GetComponent<Text>();
        indexText = transform.Find("Tips").Find("IndexText").GetComponent<Text>();
    }
    /// <summary>
    /// 切换记录
    /// </summary>
    /// <param name="direction">1上一个， -1下一个</param>
    public void ChangeIndex(int direction)
    {
        switch (direction)
        {
            case -1:
                ChangeIndexFromInt((currentIndex+Save.gameData.gr.Length-1)%Save.gameData.gr.Length);
                break;
            case 1:
                ChangeIndexFromInt((currentIndex+1)%Save.gameData.gr.Length);
                break;
        }
    }

    private void ChangeIndexFromInt(int index)
    {
        if (index >= 0 && index < Save.gameData.gr.Length)
        {
            currentIndex = index;
            ChangeUI(ref Save.gameData.gr[index]);
            indexText.text = (index + 1).ToString() + " / 20";
        }
    }

    private void ChangeUI(ref Save.GameRecord data)
    {

        for (int i = 0; i < 5; i++)
        {
            if (data.arrFirstCard[i] == 0x3E)
            {
                images[i].sprite = littleJoker;
                texts[i].text = "";
            }
            else if (data.arrFirstCard[i] == 0x3F)
            {
                images[i].sprite = bigJoker;
                texts[i].text = "";
            }
            else if (data.arrFirstCard[i] == 0x0)
            {
                images[i].sprite = null;
                texts[i].text = "";
            }
            else
            {
                images[i].sprite = table[(byte)ByteScript.getHeight4(data.arrFirstCard[i])];
                texts[i].text = ByteScript.getLow4(data.arrFirstCard[i]).ToString();
            }
        }
        for (int i = 5; i < 10; i++)
        {
            if (data.arrSecondCard[i-5] == 0x3E)
            {
                images[i].sprite = littleJoker;
                texts[i].text = "";
            }
            else if (data.arrSecondCard[i-5] == 0x3F)
            {
                images[i].sprite = bigJoker;
                texts[i].text = "";
            }
            else if (data.arrSecondCard[i-5] == 0x0)
            {
                images[i].sprite = null;
                texts[i].text = "";
            }
            else
            {
                images[i].sprite = table[(byte)ByteScript.getHeight4(data.arrSecondCard[i-5])];
                texts[i].text = ByteScript.getLow4(data.arrSecondCard[i-5]).ToString();
            }
        }

        bet.text = data.bet.ToString();
        win.text = data.win.ToString();
    }
    private void OnEnable()
    {
        ChangeIndexFromInt(0);
        EventCenter.AddListener((EventType)EventName.bet, ()=>{setting.SwitchIndex(typeof(Setting_MainSetting));});
        EventCenter.AddListener((EventType)EventName.hold1, ()=>{ChangeIndex(-1);});
        EventCenter.AddListener((EventType)EventName.hold5, ()=>{ChangeIndex(1);});
    }

    private void OnDisable()
    {
        EventCenter.RemoveAllListener((EventType)EventName.bet);
        EventCenter.RemoveAllListener((EventType)EventName.hold1);
        EventCenter.RemoveAllListener((EventType)EventName.hold5);
    }
}
