using System.Collections;
using System.Collections.Generic;
using GlobalNamaspaces;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuessCardResultManager : SerializedMonoBehaviour
{
    [SerializeField] private SpriteRenderer[] cards;
    [SerializeField] private TextMeshProUGUI[] texts;
    [SerializeField] private Sprite littleJoker;
    [SerializeField] private Sprite bigJoker;
    [SerializeField] private Sprite tempSprite;
    

    public SerializableDictionary<byte, Sprite> table;

    public void Hide()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = "";
        }

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].sprite = null;
        }
    }

    public void Show(byte value)
    {
        for (int i = 1; i < cards.Length; i++)
        {
            if (Save.gameData.bk.guessCards[i - 1] == 0x3E)
            {
                cards[i].sprite = littleJoker;
                texts[i].text = "";
            }
            else if (Save.gameData.bk.guessCards[i - 1] == 0x3F)
            {
                cards[i].sprite = bigJoker;
                texts[i].text = "";
            }
            else if (Save.gameData.bk.guessCards[i - 1] == 0x00)
            {
                cards[i].sprite = null;
                texts[i].text = "";
            }
            else
            {
                cards[i].sprite = table[(byte)ByteScript.getHeight4(Save.gameData.bk.guessCards[i - 1])];
                texts[i].text = ByteScript.getLow4(Save.gameData.bk.guessCards[i - 1]).ToString();
            }
        }

        if (value == 0x00)
        {
            cards[0].sprite = tempSprite;
            texts[0].text = "";
        }
        else if(value == 0x3E)
        {
            cards[0].sprite = littleJoker;
            texts[0].text = "";
        }
        else if (value == 0x3F)
        {
            cards[0].sprite = bigJoker;
            texts[0].text = "";
        }
        else
        {
            cards[0].sprite = table[(byte)ByteScript.getHeight4(value)];
            texts[0].text = ByteScript.getLow4(value).ToString();
        }
    }
}