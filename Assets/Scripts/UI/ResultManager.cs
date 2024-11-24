using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using DG.Tweening;

public class ResultManager : SerializedMonoBehaviour
{
    [SerializeField] private Sprite littleJoker;
    [SerializeField] private Sprite bigJoker;
    [SerializeField] private GameObject doubleSprite;
    // private Dictionary<int, string> card_table = new Dictionary<int, string>
    // {
    //     [1] = "A",
    //     [2] = "2",
    //     [3] = "3",
    //     [4] = "4",
    //     [5] = "5",
    //     [6] = "6",
    //     [7] = "7",
    //     [8] = "8",
    //     [9] = "9",
    //     [10] = "10",
    //     [11] = "J",
    //     [12] = "Q",
    //     [13] = "K"
    //
    // };
    [SerializeField]
    private Sprite _tempSprite;
    public SerializableDictionary<byte, Sprite> table = new SerializableDictionary<byte, Sprite>();
    [SerializeField] private float tempspeed = 0.2f;
    [SerializeField] private float cardspeed = 0.2f;

    private WaitForSeconds _tempWaitForSeconds;
    private WaitForSeconds _cardWaitForSeconds;
    private SpriteRenderer[] _spriteRenderers;
    public SpriteRenderer[] _holdSpriteRenderers;
    [SerializeField] private TextMeshProUGUI[] _texts;
    

    private void Awake()
    {
        doubleSprite.SetActive(false);
        _tempWaitForSeconds = new WaitForSeconds(tempspeed);
        _cardWaitForSeconds = new WaitForSeconds(cardspeed);
        _spriteRenderers = new SpriteRenderer[5];
        _holdSpriteRenderers = new SpriteRenderer[5];
        for (int i = 0; i < 5; i++)
        {
            _spriteRenderers[i] = transform.Find("S" + i).GetComponent<SpriteRenderer>();
            _holdSpriteRenderers[i] = _spriteRenderers[i].transform.GetChild(0).GetComponent<SpriteRenderer>();
        }
    }

    /// <summary>
    /// 展示一组牌
    /// </summary>
    /// <param name="card"></param>
    /// <param name="holdcard"></param>
    /// <returns></returns>
    public IEnumerator OnShowCard(byte[] card, byte[] holdcard, byte award)
    {
        if (card.Length != holdcard.Length || card.Length!=5)
        {
            yield return null;
        }


        for (int i = 0; i <= 5; i++)
        {
            if (i - 1 >= 0)
            {
                if (card[i - 1] == 0x3E)
                {
                    _spriteRenderers[i - 1].sprite = littleJoker;
                    _texts[i - 1].text = "";
                    _holdSpriteRenderers[i - 1].enabled = holdcard[i - 1] != 0;
                }
                else if (card[i - 1] == 0x3F)
                {
                    _spriteRenderers[i - 1].sprite = bigJoker;
                    _texts[i - 1].text = "";
                    _holdSpriteRenderers[i - 1].enabled = holdcard[i - 1] != 0;
                }
                else
                {
                    _spriteRenderers[i - 1].sprite = table[(byte)ByteScript.getHeight4(card[i-1])];
                    _texts[i - 1].text = ByteScript.getLow4(card[i - 1]).ToString();
                    _holdSpriteRenderers[i - 1].enabled = holdcard[i - 1] != 0;
                }
                MainController.play("helo" + i);
                yield return _cardWaitForSeconds;

            }

            if (i < 5)
            {
                _spriteRenderers[i].sprite = _tempSprite;
                _texts[i].text = "";
                yield return _tempWaitForSeconds;
            }

            
        }
        if(award>0) MainController.play("snd_item_" + ((int)award).ToString());
    }
    
    public IEnumerator OnShowSecondCard(byte[] card, byte[] holdcard, byte award)
    {
        if (card.Length != holdcard.Length || card.Length!=5)
        {
            yield return null;
        }


        int i = -1;
        while (i < 5)
        {
            int j = i+1;
            while (j < 5)
            {
                if (holdcard[j] == 0x01) j++;
                else break;
            }

            if (i >= 0 && i < 5)
            {
                if (card[i] == 0x3E)
                {
                    _spriteRenderers[i].sprite = littleJoker;
                    _texts[i].text = "";
                    _holdSpriteRenderers[i].enabled = holdcard[i] != 0;
                }
                else if (card[i] == 0x3F)
                {
                    _spriteRenderers[i].sprite = bigJoker;
                    _texts[i].text = "";
                    _holdSpriteRenderers[i].enabled = holdcard[i] != 0;
                }
                else
                {
                    _spriteRenderers[i].sprite = table[(byte)ByteScript.getHeight4(card[i])];
                    _texts[i].text = ByteScript.getLow4(card[i]).ToString();
                    _holdSpriteRenderers[i].enabled = holdcard[i] != 0;
                }
                MainController.play("helo" + (i+1).ToString());
                yield return _cardWaitForSeconds;
            }

            if (j >= 0 && j < 5)
            {
                _spriteRenderers[j].sprite = _tempSprite;
                _texts[j].text = "";
                yield return _tempWaitForSeconds;
                
            }

            i = j;
            
            
        }
        if(award>0) MainController.play("snd_item_" + ((int)award).ToString());
    }

    public IEnumerator ShowJoker(int index)
    {
        doubleSprite.SetActive(true);
        Vector3 position = _spriteRenderers[index].transform.position;

        yield return doubleSprite.transform.DOMove(position + new Vector3(0, 0, 10f), 1.0f);
    }

    public void ResetEmpty()
    {
        foreach (var VARIABLE in _spriteRenderers)
        {
            VARIABLE.sprite = null;
        }

        foreach (var VARIABLE in _holdSpriteRenderers)
        {
            VARIABLE.enabled = false;
        }

        foreach (var VARIABLE in _texts)
        {
            VARIABLE.text = "";
        }
    }
    
    
}
