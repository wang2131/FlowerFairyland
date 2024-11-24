using UnityEngine;
using UnityEngine.Timeline;

public class Main
{
    public static Main main;
    
    public Game game;
    
    public Setting setting;
    
    public Login login;

    public MonoBehaviour[] pages;
    public static void Init()
    {
        if (main != null) return;
        Loom.Initialize();
        main = new Main();
        main.pages = new MonoBehaviour[3];
        main.game = GameObject.Find("Game").GetComponent<Game>();
        main.pages[0] = main.game;
        main.setting = GameObject.Find("Setting").GetComponent<Setting>();
        main.pages[1] = main.setting;
        main.login = GameObject.Find("Login").GetComponent<Login>();
        main.pages[2] = main.login;

    }

    public void SwitchIndex(System.Type type)
    {
        MonoBehaviour temp = null;
        foreach (var page in pages)
        {
            if (page.GetType() == type)
            {
                temp = page;
            }
            else
            {
                page.gameObject.SetActive(false);
            }
        }
        if(temp!=null)
            temp.gameObject.SetActive(true);
    }
}
