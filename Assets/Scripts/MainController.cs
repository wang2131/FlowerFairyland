using GlobalNamaspaces;
using Sirenix.OdinInspector;
using UnityEngine;

public class MainController : SerializedMonoBehaviour
{
    public static AudioSource audioSource;
    [SerializeField]private AudioClip[] sounds;
    [SerializeField]private static SerializableDictionary<string, AudioClip> soundtable;
    
    private void Awake()
    {

        MyDebugger.Init();
        Save.InitData();
        Main.Init();
        Main.main.SwitchIndex(typeof(Game));
        Screen.SetResolution(1280, 800, FullScreenMode.Windowed);
        audioSource = GetComponent<AudioSource>();
        soundtable = new SerializableDictionary<string, AudioClip>();
        for (int i = 0; i < sounds.Length; i++)
        {
            soundtable.Add(sounds[i].name, sounds[i]);
        }


    }

    private void Update()
    {
        InputScript();
    }
    
    public static void play(string name)
    {
        MainController.audioSource.PlayOneShot(soundtable[name]);
    }

    private void InputScript()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EventCenter.Broadcast((EventType)EventName.hold1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            EventCenter.Broadcast((EventType)EventName.hold2);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            EventCenter.Broadcast((EventType)EventName.hold3);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            EventCenter.Broadcast((EventType)EventName.hold4);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            EventCenter.Broadcast((EventType)EventName.hold5);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            EventCenter.Broadcast((EventType)EventName.bet);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            EventCenter.Broadcast((EventType)EventName.hopperOutKey);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            EventCenter.Broadcast((EventType)EventName.guess);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            EventCenter.Broadcast((EventType)EventName.big);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            EventCenter.Broadcast((EventType)EventName.small);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            EventCenter.Broadcast((EventType)EventName.rollCredits);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            EventCenter.Broadcast((EventType)EventName.confirm);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EventCenter.Broadcast((EventType)EventName.gameSetting);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EventCenter.Broadcast((EventType)EventName.addCredits);
        }        
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EventCenter.Broadcast((EventType)EventName.reduceCredits);
        }
        
    }

    private void OnDisable()
    {
        Save.SetGameData(Save.gameData);
    }

    private void OnApplicationQuit()
    {
        Save.SetGameData(Save.gameData);
    }

    private void OnDestroy()
    {
        Save.SetGameData(Save.gameData);
    }
}
