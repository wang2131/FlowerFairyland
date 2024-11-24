using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GlobalNamaspaces;
using DG.Tweening;
using Notification;
using Sirenix.OdinInspector;
using static USBCommunication;

public class Game : SerializedMonoBehaviour
{
    [SerializeField] private UnitySerialPort port;
    public GameState currentState;

    private ResultManager _resultManager;
    private GuessCardResultManager _guessCardResultManager;
    private PrizeNotes _prizeNotes;
    private TipTexts _tipTexts;
    private GuessTable _guessTable;
    private SingleBonus _singleBonus_10;
    private SingleBonus _singleBonus_9;
    private SingleBonus _singleBonus_8;
    private SingleBonus _singleBonus_7;
    private BonusCount _bonusCount;
    private SpriteRenderer _winUI;
    private Text _bet_txt; //能量与获得成果
    public Text _credits_txt; //总成果

    private bool isHoppering = false;
    public float lastHopperTime;

    public JavaToData java;
    public bool isSecondAwardEmpty = false;
    private bool isOnResult = false;
    public byte GuessTimes = 0;
    public bool guess;

    private Coroutine resultCoroutine;

    public Animator baihuaqifang;
    public Animator shijikaihua;
    public Animator wanziqianhong;
    public Animator yifanfengshun;

    private void OnEnable()
    {
        CloseAllBonusTips();
        EventCenter.AddListener((EventType)EventName.confirm, () => { });
        EventCenter.AddListener((EventType)EventName.bet, () => { });
        EventCenter.AddListener((EventType)EventName.gameSetting, () => { Main.main.SwitchIndex(typeof(Login)); });

        SwitchState(GameState.WaitForStart);
        _guessTable.gameObject.SetActive(false);
        CheckCaijin();
        ShowCaijinUI();
    }

    private void OnDisable()
    {
        RemoveListener(currentState);
        EventCenter.RemoveAllListener((EventType)EventName.gameSetting);
    }

    private void Start()
    {
        if (Save.gameData.sd.win > 0)
        {
            Save.gameData.bk.totalCredits += Save.gameData.sd.win;
            Debug.LogFormat(" Start-->totalCredits={0},win={1}", Save.gameData.bk.totalCredits, Save.gameData.sd.win);
            Save.gameData.sd.win = 0;
            Save.SetGameData(Save.gameData);
        }

        _credits_txt.text = Save.gameData.bk.totalCredits.ToString();
    }

    /// <summary>
    /// 第一组牌
    /// </summary>
    /// <param name="first"></param>
    private void OnGetCardFirst(FirstCardResult firstCard)
    {
        Debug.Log("OnGetCardFirst ");
        USBCommunication.CopyFirstCardResult(ref firstCard, ref Save.gameData.sd.firstCardResult);
        Save.SetGameData(Save.gameData);
    }

    private void OnGetCardSecond(SecondCardResult second)
    {
        Debug.Log("OnGetCardSecond ");
        USBCommunication.CopySecondCardResult(ref second, ref Save.gameData.sd.secondCardResult);
        Save.SetGameData(Save.gameData);
    }


    /// <summary>
    /// 结果
    /// </summary>
    private void OnResult()
    {
    }

    /// <summary>
    /// 猜大小
    /// </summary>
    /// <param name="guess">true大，false小</param>
    private void OnGuess(bool guess)
    {
        SendDoubleupRequest((guess ? (byte)0x01 : (byte)0x00));
    }

    private void Awake()
    {
        _resultManager = transform.Find("Results").GetComponent<ResultManager>();
        _guessCardResultManager = transform.Find("GuessCardResults").GetComponent<GuessCardResultManager>();
        _prizeNotes = transform.Find("Canvas").Find("PrizeNotes").GetComponent<PrizeNotes>();
        _tipTexts = transform.Find("Canvas").Find("TipTexts").GetComponent<TipTexts>();
        _bet_txt = transform.Find("Canvas").Find("Bet").GetComponent<Text>();
        _credits_txt = transform.Find("Canvas").Find("TotalWin").GetComponent<Text>();
        _guessTable = transform.Find("Canvas").Find("GuessTable").GetComponent<GuessTable>();
        _winUI = transform.Find("Canvas").Find("WinUI").GetComponent<SpriteRenderer>();
        _singleBonus_10 = transform.Find("SingleBonus_10").GetComponent<SingleBonus>();
        _singleBonus_9 = transform.Find("SingleBonus_9").GetComponent<SingleBonus>();
        _singleBonus_8 = transform.Find("SingleBonus_8").GetComponent<SingleBonus>();
        _singleBonus_7 = transform.Find("SingleBonus_7").GetComponent<SingleBonus>();
        _bonusCount = transform.Find("BonusCount").GetComponent<BonusCount>();
        _winUI.enabled = false;
        // Save.SetGameData(Save.gameData);//保存函数。
    }


    public IEnumerator ActiveBunusTip(int index)
    {
        switch (index)
        {
            case 7:
                shijikaihua.gameObject.SetActive(true);
                shijikaihua.Play("dasanyuan");
                break;
            case 8:
                yifanfengshun.gameObject.SetActive(true);
                yifanfengshun.Play("dasixi");
                break;
            case 9:
                baihuaqifang.gameObject.SetActive(true);
                baihuaqifang.Play("dasanyuan");
                break;
            case 10:
                wanziqianhong.gameObject.SetActive(true);
                wanziqianhong.Play("quanzhong");
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(3.5f);
        CloseAllBonusTips();
    }

    public void CloseAllBonusTips()
    {
        shijikaihua.gameObject.SetActive(false);
        yifanfengshun.gameObject.SetActive(false);
        wanziqianhong.gameObject.SetActive(false);
        baihuaqifang.gameObject.SetActive(false);
    }

    public void SwitchState(GameState state)
    {
        switch (state)
        {
            case GameState.WaitForStart:
                RemoveListener(currentState);
                StartCoroutine(OnWaitForStartState());
                break;


            case GameState.First:
                RemoveListener(currentState);
                UsbEventCenter.AddListener((EventType)EventName.getCardFirst,
                    (FirstCardResult e) => { StartCoroutine(OnFirstState(e)); });
                SendFirstCardMessage();

                break;


            case GameState.SecondCard:
                RemoveListener(currentState);
                UsbEventCenter.AddListener((EventType)EventName.getCardSecond,
                    (SecondCardResult e) => { StartCoroutine(OnSecondCardState(e)); });
                SendSecondCardMessage();
                break;


            case GameState.GuessWeather:
                RemoveListener(currentState);
                StartCoroutine(OnGuessWeatherState());
                break;


            case GameState.GuessWeatherContinue:
                RemoveListener(currentState);
                StartCoroutine(OnGuessWeatherContinueState());
                break;


            case GameState.Result:
                RemoveListener(currentState);
                StartCoroutine(OnResultState());
                break;
        }
    }

    public void AddListener(GameState state)
    {
        switch (state)
        {
            case GameState.WaitForStart:
                EventCenter.AddListener((EventType)EventName.bet, () =>
                {
                    //MainController.play("open");
                    Bet();

                    _bet_txt.text = Save.gameData.sd.bet.ToString();
                    if (Save.gameData.sd.bet == 0)
                    {
                        _prizeNotes.ChangeValue(Save.minBet_tbl[Save.gameData.gs.minBet]);
                        Save.SetGameData(Save.gameData);
                    }
                    else
                    {
                        _prizeNotes.ChangeValue(Save.gameData.sd.bet);
                        Save.SetGameData(Save.gameData);
                    }
                });
                //开始游戏
                EventCenter.AddListener((EventType)EventName.confirm, () =>
                {
                    RemoveListener(currentState);
                    //MainController.play("open");
                    if (Save.gameData.sd.bet > 0)
                    {
                        if (Save.gameData.bk.totalCredits <= 0)
                        {
                            AddListener(currentState);
                        }
                        else
                        {
                            if (Save.gameData.sd.bet > Save.gameData.bk.totalCredits)
                            {
                                Save.gameData.sd.bet = Save.gameData.bk.totalCredits;
                            }

                            Save.gameData.bk.totalCredits -= Save.gameData.sd.bet;
                            Save.gameData.bk.totalBet += Save.gameData.sd.bet;
                            if (Save.gameData.sd.bet >= 80)
                            {
                                Save.gameData.bk.totalCaijin += 80;
                                _bonusCount.changeValue(80);
                            }
                            else
                            {
                                Save.gameData.bk.totalCaijin += Save.gameData.sd.bet;
                                _bonusCount.changeValue(Save.gameData.sd.bet);
                            }

                            CheckTempCaijin();
                            CheckCaijin();
                            ShowCaijinUI();
                            _bet_txt.text = Save.gameData.sd.bet.ToString();
                            _credits_txt.text = Save.gameData.bk.totalCredits.ToString();
                            Save.SetGameData(Save.gameData);
                            SwitchState(GameState.First);
                            Debug.LogFormat(" 开始游戏-->totalCredits={0}", Save.gameData.bk.totalCredits);
                        }
                    }
                    else
                    {
                        AddListener(currentState);
                    }
                });

                //投币
                EventCenter.AddListener((EventType)EventName.addCredits, () =>
                {
                    //还需判断是否有币


                    if (Save.gameData.bk.totalCredits < Save.maxCoinIn_tbl[Save.gameData.gs.maxCoinIn] *
                        Save.creditsPerCoin_tbl[Save.gameData.gs.creditsPerCoin])
                    {
                        if ((Save.maxCoinIn_tbl[Save.gameData.gs.maxCoinIn] *
                             Save.creditsPerCoin_tbl[Save.gameData.gs.creditsPerCoin]) -
                            Save.gameData.bk.totalCredits >= Save.creditsPerCoin_tbl[Save.gameData.gs.creditsPerCoin])
                        {
                            Save.gameData.bk.totalCredits += Save.creditsPerCoin_tbl[Save.gameData.gs.creditsPerCoin];
                            Save.gameData.bk.totalAddCredits +=
                                Save.creditsPerCoin_tbl[Save.gameData.gs.creditsPerCoin];
                            Save.SetGameData(Save.gameData);
                        }
                        else
                        {
                            Save.gameData.bk.totalAddCredits += (Save.maxCoinIn_tbl[Save.gameData.gs.maxCoinIn] *
                                                                 Save.creditsPerCoin_tbl[
                                                                     Save.gameData.gs.creditsPerCoin]) -
                                                                Save.gameData.bk.totalCredits;
                            Save.gameData.bk.totalCredits = Save.maxCoinIn_tbl[Save.gameData.gs.maxCoinIn] *
                                                            Save.creditsPerCoin_tbl[Save.gameData.gs.creditsPerCoin];
                            Save.SetGameData(Save.gameData);
                        }

                        _credits_txt.text = Save.gameData.bk.totalCredits.ToString();
                        MainController.play("shot");
                    }
                });

                //退币
                EventCenter.AddListener((EventType)EventName.reduceCredits, () =>
                {
                    if (Save.gameData.bk.totalCredits >= Save.creditsPerCoin_tbl[Save.gameData.gs.creditsPerCoin])
                    {
                        Save.gameData.bk.totalCredits -= Save.creditsPerCoin_tbl[Save.gameData.gs.creditsPerCoin];
                        Save.gameData.bk.totalReduceCredits += Save.creditsPerCoin_tbl[Save.gameData.gs.creditsPerCoin];
                        Save.SetGameData(Save.gameData);
                        Debug.LogFormat(" 退币中-->totalCredits={0}", Save.gameData.bk.totalCredits);
                    }

                    _credits_txt.text = Save.gameData.bk.totalCredits.ToString();
                    MainController.play("shot");
                });

                EventCenter.AddListener((EventType)EventName.hopperOutKey, () =>
                {
                    if (Save.gameData.bk.totalCredits >= Save.creditsPerCoin_tbl[Save.gameData.gs.creditsPerCoin])
                    {
                        int count = Save.gameData.bk.totalCredits /
                                    (Save.creditsPerCoin_tbl[Save.gameData.gs.creditsPerCoin] *
                                     Save.coinsPerTicket_tbl[Save.gameData.gs.coinsPerTicket]);
                        port.MainBoardSend(true, (short)count);
                    }
                });
                break;

            case GameState.First:
                EventCenter.AddListener((EventType)EventName.confirm, () =>
                {
                    /*MainController.play("open");*/
                    SwitchState(GameState.SecondCard);
                });
                EventCenter.AddListener((EventType)EventName.hold1, () =>
                {
                    //MainController.play("open");
                    Save.gameData.sd.firstCardResult.arrHoldCard[0] =
                        Save.gameData.sd.firstCardResult.arrHoldCard[0] == (byte)1 ? (byte)0 : (byte)1;
                    _resultManager._holdSpriteRenderers[0].enabled =
                        Save.gameData.sd.firstCardResult.arrHoldCard[0] == 1;
                });
                EventCenter.AddListener((EventType)EventName.hold2, () =>
                {
                    //MainController.play("open");
                    Save.gameData.sd.firstCardResult.arrHoldCard[1] =
                        Save.gameData.sd.firstCardResult.arrHoldCard[1] == (byte)1 ? (byte)0 : (byte)1;
                    _resultManager._holdSpriteRenderers[1].enabled =
                        Save.gameData.sd.firstCardResult.arrHoldCard[1] == 1;
                });
                EventCenter.AddListener((EventType)EventName.hold3, () =>
                {
                    //MainController.play("open");
                    Save.gameData.sd.firstCardResult.arrHoldCard[2] =
                        Save.gameData.sd.firstCardResult.arrHoldCard[2] == (byte)1 ? (byte)0 : (byte)1;
                    _resultManager._holdSpriteRenderers[2].enabled =
                        Save.gameData.sd.firstCardResult.arrHoldCard[2] == 1;
                });
                EventCenter.AddListener((EventType)EventName.hold4, () =>
                {
                    //MainController.play("open");
                    Save.gameData.sd.firstCardResult.arrHoldCard[3] =
                        Save.gameData.sd.firstCardResult.arrHoldCard[3] == (byte)1 ? (byte)0 : (byte)1;
                    _resultManager._holdSpriteRenderers[3].enabled =
                        Save.gameData.sd.firstCardResult.arrHoldCard[3] == 1;
                });
                EventCenter.AddListener((EventType)EventName.hold5, () =>
                {
                    //MainController.play("open");
                    Save.gameData.sd.firstCardResult.arrHoldCard[4] =
                        Save.gameData.sd.firstCardResult.arrHoldCard[4] == (byte)1 ? (byte)0 : (byte)1;
                    _resultManager._holdSpriteRenderers[4].enabled =
                        Save.gameData.sd.firstCardResult.arrHoldCard[4] == 1;
                });
                break;

            case GameState.SecondCard:
                EventCenter.AddListener((EventType)EventName.rollCredits, () =>
                {
                    //MainController.play("win2");
                    SwitchState(GameState.Result);
                });
                EventCenter.AddListener((EventType)EventName.guess, () =>
                {
                    MainController.play("open");
                    Save.gameData.bk.totalGuessBet += Save.gameData.sd.win;
                    Save.SetGameData(Save.gameData);
                    SwitchState(GameState.GuessWeather);
                });
                EventCenter.AddListener((EventType)EventName.bet, () => { });
                break;

            case GameState.GuessWeather:
                EventCenter.AddListener((EventType)EventName.big, () =>
                {
                    RemoveListener(GameState.GuessWeather);
                    //MainController.play("open");
                    UsbEventCenter.AddListener((EventType)EventName.guess, (DoubleUpResult e) =>
                    {
                        byte card = e.bCard;
                        bool result = e.bResult > 0 ? true : false;
                        UsbEventCenter.RemoveAllListener((EventType)EventName.guess);
                        if (result)
                        {
                            MainController.play("doubleWin");
                            Save.gameData.sd.win *= 2;
                            Save.gameData.bk.totalGuessWin += Save.gameData.sd.win;
                            _guessTable.GuessWin.text = Save.gameData.sd.win.ToString();
                            Save.SetGameData(Save.gameData);
                            EventCenter.AddListener((EventType)EventName.big, () => { });
                            EventCenter.AddListener((EventType)EventName.small, () => { });
                            GuessTimes++;
                            _guessCardResultManager.Show(card);
                            Save.SetGuessCardRecord(card);
                            if (GuessTimes >= 5)
                            {
                                Invoke(nameof(GoToResultState), 0.5f);
                            }
                            else
                            {
                                SwitchState(GameState.GuessWeatherContinue);
                            }
                        }
                        else
                        {
                            MainController.play("doubleFail");
                            Save.gameData.sd.win = 0;
                            _guessTable.GuessWin.text = Save.gameData.sd.win.ToString();
                            _bet_txt.text = 0.ToString();
                            _guessCardResultManager.Show(card);
                            Save.SetGuessCardRecord(card);
                            Save.SetGameData(Save.gameData);
                            EventCenter.AddListener((EventType)EventName.big, () => { });
                            EventCenter.AddListener((EventType)EventName.small, () => { });
                            Invoke(nameof(GoToResultState), 2.5f);
                        }
                    });
                    guess = true;
                    OnGuess(true);
                });
                EventCenter.AddListener((EventType)EventName.small, () =>
                {
                    RemoveListener(GameState.GuessWeather);
                    //MainController.play("open");
                    UsbEventCenter.AddListener((EventType)EventName.guess, (DoubleUpResult e) =>
                    {
                        byte card = e.bCard;
                        UsbEventCenter.RemoveAllListener((EventType)EventName.guess);
                        bool result = e.bResult > 0 ? true : false;

                        if (result)
                        {
                            MainController.play("doubleWin");
                            Save.gameData.sd.win *= 2;
                            Save.gameData.bk.totalGuessWin += Save.gameData.sd.win;
                            _guessTable.GuessWin.text = Save.gameData.sd.win.ToString();
                            EventCenter.AddListener((EventType)EventName.big, () => { });
                            EventCenter.AddListener((EventType)EventName.small, () => { });
                            GuessTimes++;
                            _guessCardResultManager.Show(card);
                            Save.SetGuessCardRecord(card);

                            if (GuessTimes >= 5)
                            {
                                Invoke(nameof(GoToResultState), 0.5f);
                            }
                            else
                            {
                                SwitchState(GameState.GuessWeatherContinue);
                            }
                        }
                        else
                        {
                            MainController.play("doubleFail");
                            Save.gameData.sd.win = 0;
                            _guessTable.GuessWin.text = Save.gameData.sd.win.ToString();
                            _bet_txt.text = 0.ToString();
                            EventCenter.AddListener((EventType)EventName.big, () => { });
                            EventCenter.AddListener((EventType)EventName.small, () => { });
                            _guessCardResultManager.Show(card);
                            Save.SetGuessCardRecord(card);
                            Save.SetGameData(Save.gameData);
                            Invoke(nameof(GoToResultState), 2.5f);
                        }
                    });
                    guess = false;
                    OnGuess(false);
                });
                break;

            case GameState.GuessWeatherContinue:
                EventCenter.AddListener((EventType)EventName.rollCredits, () =>
                {
                    /*MainController.play("win2");*/
                    SwitchState(GameState.Result);
                });
                EventCenter.AddListener((EventType)EventName.guess, () =>
                {
                    MainController.play("open");
                    Save.gameData.bk.totalGuessBet += Save.gameData.sd.win;
                    Save.SetGameData(Save.gameData);
                    SwitchState(GameState.GuessWeather);
                });
                break;

            case GameState.Result:
                EventCenter.AddListener((EventType)EventName.rollCredits, () =>
                {
                    RemoveListener(currentState);
                    StopCoroutine(resultCoroutine);
                    _bet_txt.text = Save.gameData.sd.bet.ToString();
                    _credits_txt.text = Save.gameData.bk.totalCredits.ToString();
                    Invoke(nameof(ReturnToStart), 0.5f);
                });
                break;

            case GameState.Hopper:
                break;
        }
    }


    public void RemoveListener(GameState state)
    {
        switch (state)
        {
            case GameState.WaitForStart:
                EventCenter.RemoveAllListener((EventType)EventName.bet);
                EventCenter.RemoveAllListener((EventType)EventName.confirm);
                EventCenter.RemoveAllListener((EventType)EventName.addCredits);
                EventCenter.RemoveAllListener((EventType)EventName.reduceCredits);
                EventCenter.RemoveAllListener((EventType)EventName.hopperOutKey);
                break;
            case GameState.First:
                EventCenter.RemoveAllListener((EventType)EventName.confirm);
                EventCenter.RemoveAllListener((EventType)EventName.hold1);
                EventCenter.RemoveAllListener((EventType)EventName.hold2);
                EventCenter.RemoveAllListener((EventType)EventName.hold3);
                EventCenter.RemoveAllListener((EventType)EventName.hold4);
                EventCenter.RemoveAllListener((EventType)EventName.hold5);
                break;
            case GameState.SecondCard:
                EventCenter.RemoveAllListener((EventType)EventName.bet);
                EventCenter.RemoveAllListener((EventType)EventName.rollCredits);
                EventCenter.RemoveAllListener((EventType)EventName.guess);
                if (isSecondAwardEmpty)
                {
                    EventCenter.RemoveAllListener((EventType)EventName.confirm);
                }

                isSecondAwardEmpty = false;
                break;
            case GameState.GuessWeather:
                EventCenter.RemoveAllListener((EventType)EventName.big);
                EventCenter.RemoveAllListener((EventType)EventName.small);
                break;
            case GameState.GuessWeatherContinue:
                EventCenter.RemoveAllListener((EventType)EventName.rollCredits);
                EventCenter.RemoveAllListener((EventType)EventName.guess);
                break;
            case GameState.Result:
                EventCenter.RemoveAllListener((EventType)EventName.rollCredits);
                break;

            case GameState.Hopper:
                break;
        }
    }

    public void GoToResultState()
    {
        SwitchState(GameState.Result);
    }

    /// <summary>
    /// 重置面板
    /// </summary>
    public void Reset()
    {
        Save.Reset2DefaultStationData();
        _resultManager.ResetEmpty();
    }

    public void ReturnToStart()
    {
        Loom.QueueOnMainThread(() => SwitchState(GameState.WaitForStart));
    }

    IEnumerator ShowResult()
    {
        bool BetIsFinished = false;
        bool TotalWinIsFinished = false;
        int tempCredits = int.Parse(_credits_txt.text);
        int tempBet = Save.gameData.sd.win;
        int credits = Save.gameData.bk.totalCredits;
        float lastTime = Time.time;
        //int speed = (int)(Save.gameData.sd.win / (3/Time.deltaTime));
        while (true)
        {
            if (Time.time - lastTime >= 2f)
            {
                EventCenter.Broadcast((EventType)EventName.rollCredits);
            }

            if (tempBet != 0)
            {
                if (tempBet < 10)
                {
                    tempBet = 0;
                }
                else
                {
                    tempBet -= 10;
                }

                _bet_txt.text = tempBet.ToString();
            }
            else
            {
                BetIsFinished = true;
            }

            if (tempCredits != Save.gameData.bk.totalCredits)
            {
                if (Save.gameData.bk.totalCredits - tempCredits < 10)
                {
                    tempCredits = credits;
                }
                else
                {
                    tempCredits += 10;
                }

                _credits_txt.text = tempCredits.ToString();
            }
            else
            {
                TotalWinIsFinished = true;
            }


            if (BetIsFinished && TotalWinIsFinished) break;

            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(0.5f);
        ReturnToStart();
    }


    private void Bet()
    {
        int temp = 0;
        if (Save.gameData.sd.bet - Save.minBet_tbl[Save.gameData.gs.minBet] < 0)//如果当前能量小于最小能量
        {
            temp = Save.minBet_tbl[Save.gameData.gs.minBet];
        }
        else if (Save.maxBet_tbl[Save.gameData.gs.maxBet] - Save.gameData.sd.bet <= 10 &&
                 Save.maxBet_tbl[Save.gameData.gs.maxBet] - Save.gameData.sd.bet > 0)//如果当前能量小于最大且相差不到10
        {
            temp = Save.maxBet_tbl[Save.gameData.gs.maxBet];
        }
        else if (Save.maxBet_tbl[Save.gameData.gs.maxBet] - Save.gameData.sd.bet <= 0)//如果当前能量大于等于最大
        {
            temp = Save.minBet_tbl[Save.gameData.gs.minBet];
        }
        else//其他情况
        {
            if (Save.gameData.sd.bet % 10 != 0)
            {

                temp = Save.gameData.sd.bet - (Save.gameData.sd.bet % 10) + 10;
                
            }
            else
            {
                temp = Save.gameData.sd.bet + 10;
            }
            
        }

        if (temp < Save.gameData.bk.totalCredits)
        {
            Save.gameData.sd.bet = temp;
        }
        else if (temp == Save.gameData.bk.totalCredits)
        {
            Save.gameData.sd.bet = Save.gameData.bk.totalCredits;
        }

        Save.SetGameData(Save.gameData);
    }

    private void CheckTempCaijin()
    {
        if (Save.gameData.bk.totalCaijin >= 256)
        {
            int temp = Save.gameData.bk.totalCaijin / 256;
            Save.gameData.bk.Caijin_7 += temp;
            Save.gameData.bk.Caijin_8 += temp;
            Save.gameData.bk.Caijin_9 += temp;
            Save.gameData.bk.Caijin_10 += temp;
            Save.gameData.bk.totalCaijin %= 256;
            Save.SetGameData(Save.gameData);
        }
    }

    private void CheckCaijin()
    {
        if (Save.gameData.bk.Caijin_7 > 500)
        {
            Save.gameData.bk.Caijin_7 = 500;
        }
        else if (Save.gameData.bk.Caijin_7 < 200)
        {
            Save.gameData.bk.Caijin_7 = 200;
        }

        if (Save.gameData.bk.Caijin_8 > 2000)
        {
            Save.gameData.bk.Caijin_8 = 2000;
        }
        else if (Save.gameData.bk.Caijin_8 < 500)
        {
            Save.gameData.bk.Caijin_8 = 500;
        }

        if (Save.gameData.bk.Caijin_9 > 10000)
        {
            Save.gameData.bk.Caijin_9 = 10000;
        }
        else if (Save.gameData.bk.Caijin_9 < 2000)
        {
            Save.gameData.bk.Caijin_9 = 2000;
        }

        if (Save.gameData.bk.Caijin_10 > 20000)
        {
            Save.gameData.bk.Caijin_10 = 20000;
        }

        else if (Save.gameData.bk.Caijin_10 < 5000)
        {
            Save.gameData.bk.Caijin_10 = 5000;
        }

        Save.SetGameData(Save.gameData);
    }

    private void ShowCaijinUI()
    {
        _singleBonus_10.changeValue(Save.gameData.bk.Caijin_10);
        _singleBonus_9.changeValue(Save.gameData.bk.Caijin_9);
        _singleBonus_8.changeValue(Save.gameData.bk.Caijin_8);
        _singleBonus_7.changeValue(Save.gameData.bk.Caijin_7);
    }

    private void CheckAward(byte award)
    {
        int multiple = 1;
        switch (award)
        {
            case 0x0:
                multiple = 0;
                break;
            case 0x1:
                multiple = 1;
                break;
            case 0x2:
                multiple = 2;
                break;
            case 0x3:
                multiple = 3;
                break;
            case 0x4:
                multiple = 5;
                break;
            case 0x5:
                multiple = 7;
                break;
            case 0x6:
                multiple = 10;
                break;
            case 0x7:
                multiple = 60;
                Save.gameData.bk.prize7Count += 1;
                Save.SetGameData(Save.gameData);
                break;
            case 0x8:
                multiple = 120;
                Save.gameData.bk.prize8Count += 1;
                Save.SetGameData(Save.gameData);
                break;
            case 0x9:
                multiple = 250;
                Save.gameData.bk.prize9Count += 1;
                Save.SetGameData(Save.gameData);
                break;
            case 0xA:
                multiple = 750;
                Save.gameData.bk.prize10Count += 1;
                Save.SetGameData(Save.gameData);
                break;
        }

        Save.gameData.sd.win = Save.gameData.sd.bet * multiple;
    }

    private void AddEmptyListenersForSecondCardState()
    {
        isSecondAwardEmpty = true;
        EventCenter.AddListener((EventType)EventName.bet, () =>
        {
            StopCoroutine(ShowWinUI());
            _winUI.enabled = false;
            Bet();
            _bet_txt.text = Save.gameData.sd.bet.ToString();
            ReturnToStart();
        });
        EventCenter.AddListener((EventType)EventName.confirm, () =>
        {
            StopCoroutine(ShowWinUI());
            _winUI.enabled = false;
            RemoveListener(currentState);
            if (Save.gameData.bk.totalCredits == 0)
            {
                // Save.gameData.sd.bet = 0;
                // _bet_txt.text = "";
                ReturnToStart();
            }

            if (Save.gameData.sd.bet >0 && Save.gameData.bk.totalCredits != 0)
            {
                //MainController.play("open");
                if (Save.gameData.sd.bet > Save.gameData.bk.totalCredits)
                {
                    Save.gameData.sd.bet = Save.gameData.bk.totalCredits;
                }

                if (Save.gameData.sd.bet >= 80)
                {
                    Save.gameData.bk.totalCaijin += 80;
                    _bonusCount.changeValue(80);
                }
                else
                {
                    Save.gameData.bk.totalCaijin += Save.gameData.sd.bet;
                    _bonusCount.changeValue(Save.gameData.sd.bet);
                }

                CheckTempCaijin();
                CheckCaijin();
                ShowCaijinUI();

                Save.gameData.bk.totalCredits -= Save.gameData.sd.bet;
                Debug.LogFormat(" AddEmptyListenersForSecondCardState-->totalCredits={0},bet={1}",
                    Save.gameData.bk.totalCredits, Save.gameData.sd.bet);
                Save.gameData.bk.totalBet += Save.gameData.sd.bet;
                Save.SetGameData(Save.gameData);
                _bet_txt.text = Save.gameData.sd.bet.ToString();
                _credits_txt.text = Save.gameData.bk.totalCredits.ToString();
                SwitchState(GameState.First);
            }
        });
        EventCenter.AddListener((EventType)EventName.guess, () => { });
        EventCenter.AddListener((EventType)EventName.rollCredits, () => { });
    }

    #region IEnumerator

    private IEnumerator ShowWinUI()
    {
        _winUI.enabled = true;
        _bet_txt.text = 0.ToString();

        yield return new WaitForSeconds(1.0f);

        _bet_txt.text = Save.gameData.sd.bet.ToString();
        _winUI.enabled = false;
    }

    private IEnumerator OnWaitForStartState()
    {
        currentState = GameState.WaitForStart;
        _winUI.enabled = false;

        _resultManager.ResetEmpty();
        _guessCardResultManager.Hide();
        CloseAllBonusTips();
        GuessTimes = 0;
        if (Save.gameData.sd.bet == 0)
        {
            _bet_txt.text = "";
        }
        else
        {
            _bet_txt.text = Save.gameData.sd.bet.ToString();
        }

        _credits_txt.text = Save.gameData.bk.totalCredits.ToString();
        if (Save.gameData.sd.bet == 0)
        {
            _prizeNotes.ChangeValue(Save.minBet_tbl[Save.gameData.gs.minBet]);
        }
        else
        {
            _prizeNotes.ChangeValue(Save.gameData.sd.bet);
        }

        _prizeNotes.ClearHighLights();
        _tipTexts.gameObject.SetActive(true);
        _tipTexts.Tip(1);
        _tipTexts.MainSparkle();
        AddListener(GameState.WaitForStart);
        yield return null;
    }

    private IEnumerator OnFirstState(FirstCardResult firstCardResult)
    {
        //MainController.play("open");
        _winUI.enabled = false;
        currentState = GameState.First;
        GuessTimes = 0;
        guess = false;
        Save.gameData.sd.win = 0;
        _prizeNotes.ChangeValue(Save.gameData.sd.bet);
        _bet_txt.text = Save.gameData.sd.bet.ToString();
        OnGetCardFirst(firstCardResult);
        _tipTexts.Stop();
        yield return _resultManager.StartCoroutine(_resultManager.OnShowCard(Save.gameData.sd.firstCardResult.arrCard,
            Save.gameData.sd.firstCardResult.arrHoldCard, 0x0));
        _prizeNotes.StartCoroutine("StartHighlight",
            Save.gameData.sd.firstCardResult.cCurAwardResult);
        _tipTexts.Tip(2);
        UsbEventCenter.RemoveAllListener((EventType)EventName.getCardFirst);
        AddListener(GameState.First);

        yield return null;
    }

    private IEnumerator OnSecondCardState(SecondCardResult second)
    {
        _prizeNotes.StopAllCoroutines();
        currentState = GameState.SecondCard;
        OnGetCardSecond(second);
        yield return _resultManager.StartCoroutine(_resultManager.OnShowSecondCard(
            Save.gameData.sd.secondCardResult.arrCard, Save.gameData.sd.firstCardResult.arrHoldCard,
            Save.gameData.sd.secondCardResult.bAwardItem));
        _prizeNotes.HighLight(Save.gameData.sd.secondCardResult.bAwardItem);
        if (Save.gameData.sd.secondCardResult.bAwardItem == 0)
        {
            StartCoroutine(ShowWinUI());
            UsbEventCenter.RemoveAllListener((EventType)EventName.getCardSecond);
            Save.SaveGameRecord(Save.gameData.sd.firstCardResult.arrCard,
                Save.gameData.sd.firstCardResult.arrHoldCard, Save.gameData.sd.secondCardResult.arrCard,
                Save.gameData.sd.secondCardResult.bAwardItem, Save.gameData.sd.bet, Save.gameData.sd.win);
            AddEmptyListenersForSecondCardState();
            _tipTexts.Tip(1);
        }
        else
        {
            CheckAward(Save.gameData.sd.secondCardResult.bAwardItem);
            yield return StartCoroutine(ActiveBunusTip(Save.gameData.sd.secondCardResult.bAwardItem));
            Save.gameData.bk.totalWin += Save.gameData.sd.win; //总得能量
            Save.SetGameData(Save.gameData);
            _bet_txt.text = Save.gameData.sd.win.ToString();
            int index = -1;
            bool islittle = false, isbig = false;
            for (int i = 0; i < 5; i++)
            {
                if (Save.gameData.sd.secondCardResult.arrCard[i] == 0x3E)
                {
                    islittle = true;
                    if (index == -1) index = i;
                }
                else if (Save.gameData.sd.secondCardResult.arrCard[i] == 0x3E)
                {
                    isbig = true;
                    if (index == -1) index = i;
                }
            }

            if (islittle && isbig)
            {
                yield return _resultManager.StartCoroutine(_resultManager.ShowJoker(index));
                Save.gameData.sd.win *= 2;
                _bet_txt.text = Save.gameData.sd.win.ToString();
            }

            Save.SetGameData(Save.gameData);


            _winUI.enabled = true;
            _tipTexts.Tip(3);
            UsbEventCenter.RemoveAllListener((EventType)EventName.getCardSecond);
            AddListener(GameState.SecondCard);
        }

        yield return null;
    }

    private IEnumerator OnGuessWeatherState()
    {
        currentState = GameState.GuessWeather;
        _resultManager.ResetEmpty();
        _guessCardResultManager.Show(0x00);
        _guessTable.gameObject.SetActive(true);
        _guessTable.GuessBet.text = Save.gameData.sd.win.ToString();
        _guessTable.GuessWin.text = "";
        _tipTexts.Tip(4);
        AddListener(GameState.GuessWeather);

        yield return null;
    }

    private IEnumerator OnGuessWeatherContinueState()
    {
        currentState = GameState.GuessWeatherContinue;
        _bet_txt.text = Save.gameData.sd.win.ToString();
        _tipTexts.Tip(3);
        AddListener(GameState.GuessWeatherContinue);
        yield return null;
    }

    private IEnumerator OnResultState()
    {
        currentState = GameState.Result;
        _guessCardResultManager.Hide();
        _tipTexts.Stop();
        _guessTable.gameObject.SetActive(false);
        if (Save.gameData.sd.win > 0)
        {
            if (Save.gameData.sd.bet < 80)
            {
                int num;
                switch (Save.gameData.sd.secondCardResult.bAwardItem)
                {
                    case 7:
                        num = Save.gameData.bk.Caijin_7 * (Save.gameData.sd.bet / 80);
                        Save.gameData.sd.win += num;
                        Save.gameData.bk.Caijin_7 -= num;
                        Save.SetGameData(Save.gameData);
                        break;

                    case 8:
                        num = Save.gameData.bk.Caijin_8 * (Save.gameData.sd.bet / 80);
                        Save.gameData.sd.win += num;
                        Save.gameData.bk.Caijin_8 -= num;
                        Save.SetGameData(Save.gameData);
                        break;
                    case 9:
                        num = Save.gameData.bk.Caijin_9 * (Save.gameData.sd.bet / 80);
                        Save.gameData.sd.win += num;
                        Save.gameData.bk.Caijin_9 -= num;
                        Save.SetGameData(Save.gameData);
                        break;
                    case 10:
                        num = Save.gameData.bk.Caijin_10 * (Save.gameData.sd.bet / 80);
                        Save.gameData.sd.win += num;
                        Save.gameData.bk.Caijin_10 -= num;
                        Save.SetGameData(Save.gameData);
                        break;
                }
            }

            else
            {
                int num;
                switch (Save.gameData.sd.secondCardResult.bAwardItem)
                {
                    case 7:
                        Save.gameData.sd.win += Save.gameData.bk.Caijin_7;
                        Save.gameData.bk.Caijin_7 = 0;
                        Save.SetGameData(Save.gameData);
                        break;

                    case 8:
                        Save.gameData.sd.win += Save.gameData.bk.Caijin_8;
                        Save.gameData.bk.Caijin_8 = 0;
                        Save.SetGameData(Save.gameData);
                        break;
                    case 9:
                        Save.gameData.sd.win += Save.gameData.bk.Caijin_9;
                        Save.gameData.bk.Caijin_9 = 0;
                        Save.SetGameData(Save.gameData);
                        break;
                    case 10:
                        Save.gameData.sd.win += Save.gameData.bk.Caijin_10;
                        Save.gameData.bk.Caijin_10 = 0;
                        Save.SetGameData(Save.gameData);
                        break;
                }
            }

            CheckCaijin();
            ShowCaijinUI();
        }
        
        _bet_txt.text = Save.gameData.sd.win.ToString();

        Save.gameData.bk.totalCredits += Save.gameData.sd.win;

        Debug.LogFormat(" OnResultState-->totalCredits={0},win={1}", Save.gameData.bk.totalCredits,
            Save.gameData.sd.win);
        //Save.gameData.bk.totalWin += Save.gameData.sd.win;
        Save.SaveGameRecord(Save.gameData.sd.firstCardResult.arrCard,
            Save.gameData.sd.firstCardResult.arrHoldCard, Save.gameData.sd.secondCardResult.arrCard,
            Save.gameData.sd.secondCardResult.bAwardItem, Save.gameData.sd.bet, Save.gameData.sd.win);
        yield return new WaitForSeconds(0.5f);
        resultCoroutine = StartCoroutine(ShowResult());
        //Save.gameData.sd.bet = Save.minBet_tbl[Save.gameData.gs.minBet];
        Save.gameData.sd.win = 0;
        Save.SetGameData(Save.gameData);
        AddListener(GameState.Result);
        yield return null;
    }


    // private IEnumerator OnHopper()
    // {
    //     byte tuibiCount = port.tuibijishu;
    //     float previousHoppertime = (int)(Time.time * 1000);
    //     port.MainBoardSend(true, (short)(Save.gameData.bk.totalCredits/Save.creditsPerCoin_tbl[Save.gameData.gs.creditsPerCoin]));
    //     while (Save.gameData.bk.totalCredits>=Save.creditsPerCoin_tbl[Save.gameData.gs.creditsPerCoin])
    //     {
    //         if (tuibiCount == port.tuibijishu)
    //         {
    //             //超过1000ms没有收到信息，退币故障
    //             if ((int)(Time.time * 1000) - previousHoppertime >= 1000)
    //             {
    //                 MyDebugger.Log("退币故障，退出状态");
    //                 break;
    //             }
    //         }
    //         else if(port.tuibijishu > tuibiCount)
    //         {
    //             Save.gameData.bk.totalCredits -= (port.tuibijishu - tuibiCount)*Save.creditsPerCoin_tbl[Save.gameData.gs.creditsPerCoin];
    //             _credits_txt.text = Save.gameData.bk.totalCredits.ToString();
    //             tuibiCount = port.tuibijishu;
    //             previousHoppertime = (int)(Time.time * 1000);
    //             yield return new WaitForFixedUpdate();
    //         }
    //     }
    //     
    //     SwitchState(GameState.WaitForStart);
    // }

    #endregion


    #region test

    public void SendFirstCardMessage()
    {
        byte[] data = new byte[] { 0xfe, 0x01, 0x0a, 0x00, 0x00, 0x00, 0x00, 0xaa, 0x08, 0xaa, 0x09, 0xae, 0xfb, 0x51 };
        java.jo.Call("SendData", data);
    }

    public void SendSecondCardMessage()
    {
        List<byte> dataList = new List<byte>(new byte[] { 0x00, 0x00, 0x01, 0x00, 0x0d, 0x00, 0x02, 0x04 });
        for (int i = 0; i < 5; i++)
        {
            dataList.Add(Save.gameData.sd.firstCardResult.arrHoldCard[i]);
        }

        Debug.Log("加密前：" + BitConverter.ToString(dataList.ToArray()));
        byte[] outputResult = EncodingCenter.Encode(dataList.ToArray());
        Debug.Log("加密结果：" + BitConverter.ToString(outputResult));

        List<byte> data = new List<byte>();
        byte[] head = new byte[] { 0xfe, 0x01, 0x0f, 0x00 };
        data.AddRange(head);
        data.AddRange(outputResult);

        java.jo.Call("SendData", data.ToArray());
    }

    public void SendDoubleupRequest(byte Gguess)
    {
        List<byte> dataList = new List<byte>(new byte[] { 0x00, 0x00, 0x01, 0x00, 0x0a, 0x00, 0x05, 0x04 });
        Debug.Log("猜次数：" + GuessTimes);
        Debug.Log("猜测：" + Gguess);
        dataList.Add(GuessTimes);
        dataList.Add(Gguess);


        byte[] outputResult = EncodingCenter.Encode(dataList.ToArray());

        List<byte> data = new List<byte>();
        byte[] head = new byte[] { 0xfe, 0x01, 0x0c, 0x00 };
        data.AddRange(head);
        data.AddRange(outputResult);
        Debug.Log("比倍发送： " + BitConverter.ToString(data.ToArray()));

        java.jo.Call("SendData", data.ToArray());
    }

    #endregion
}


/// <summary>
/// 游戏状态
/// </summary>
public enum GameState
{
    WaitForStart,
    First,
    SecondCard,
    GuessWeather,
    WaitForGuessResult,
    GuessWeatherContinue,
    Result,
    Hopper //退币中
}