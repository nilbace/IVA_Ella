using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static Define;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    public static Managers instance {get{Init(); return s_instance;}}
    

    InputManager _input = new InputManager();
    ResourceManager _resource = new ResourceManager();
    UI_Manager _ui_manager = new UI_Manager();
    SoundManager _sound = new SoundManager();
    PoolManager _pool = new PoolManager();
    DataManager _data = new DataManager();

    [Header("스텟 관련")]
    public int MainStat_ValuePerLevel;
    public float Str_Men_ValuePerLevel;

    REventManager _RE = new REventManager();
    MultiSpriteManager _MSM = new MultiSpriteManager();
    public static InputManager Input {get {return instance._input;}}
    public static ResourceManager Resource{get{return instance._resource;}}
    public static UI_Manager UI_Manager{get{return instance._ui_manager;}}
    public static SoundManager Sound{get{return instance._sound;}}
    public static PoolManager Pool{get{return instance._pool;}}
    public static DataManager Data { get { return instance._data; } }
    public static REventManager RandEvent { get { return instance._RE; } }
    public static MultiSpriteManager MSM { get { return instance._MSM; } }


    void Awake()
    {
        Init();
        StartCoroutine(LoadDatas());
    }


    void Update()
    {
        _input.OnUpdate();
    }

    static void Init(){
        if(s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if(go==null)
            {
                go = new GameObject{name = "@Managers"};
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._sound.Init();
            s_instance._pool.Init();
            s_instance._data.Init();
            s_instance._MSM.Init();
        }
    }

    public static void Clear()
    {
        Sound.Clear();
        Input.Clear();
        UI_Manager.Clear();
        
        Pool.Clear();
    }

    const string DayDatasURL = "https://docs.google.com/spreadsheets/d/1WjIWPgya-w_QcNe6pWE_iug0bsF6uwTFDRY8j2MkO3o/export?format=tsv&gid=1890750354&range=B2:Q";
    const string RandEventURL = "https://docs.google.com/spreadsheets/d/1WjIWPgya-w_QcNe6pWE_iug0bsF6uwTFDRY8j2MkO3o/export?format=tsv&gid=185260022&range=A2:W";
    const string MerchantURL = "https://docs.google.com/spreadsheets/d/1WjIWPgya-w_QcNe6pWE_iug0bsF6uwTFDRY8j2MkO3o/export?format=tsv&gid=1267834452&range=A2:J";

    IEnumerator LoadDatas()
    {
        Coroutine cor1 = StartCoroutine(s_instance._data.RequestAndSetDayDatas(DayDatasURL));
        Coroutine cor2 = StartCoroutine(s_instance._data.RequestAndSetRandEventDatas(RandEventURL)); 
        Coroutine cor3 = StartCoroutine(s_instance._data.RequestAndSetItemDatas(MerchantURL));

        yield return cor1;
        yield return cor2;
        yield return cor3;

        Debug.Log("시작 가능");
    }


    #region UI
    public void ShowReceipt()
    {
        StartCoroutine(ShowReceiptCor());
    }
    IEnumerator ShowReceiptCor()
    {
        UI_Manager.CloseALlPopupUI();
        yield return new WaitForEndOfFrame();
        UI_Manager.ShowPopupUI<UI_Reciept>();
    }

    public void StartSchedule()
    {
        StartCoroutine(ScheduleExecuter.Inst.StartSchedule());
    }

    public void ShowDefualtPopUP(string Text)
    {
        StartCoroutine(ShowDefaultPopupCor(Text));
    }

    IEnumerator ShowDefaultPopupCor(string Text)
    {
        UI_Manager.ShowPopupUI<UI_DefaultPopup>();
        yield return new WaitForEndOfFrame();
        UI_DefaultPopup.instance.SetText(Text);
    }

    public void ShowMainStory()
    {
        StartCoroutine(ShowMainStoryCor());
    }
    IEnumerator ShowMainStoryCor()
    {
        UI_Manager.ShowPopupUI<UI_Communication>();
        yield return new WaitForEndOfFrame();
        MainStoryParser.Inst.StartStory(ChooseMainStory());
    }

    public MainStory ChooseMainStory()
    {
        MainStory mainStory;
        string temp = Data._myPlayerData.GetHigestStatName().ToString();
        temp += (Data._myPlayerData.NowWeek / 4).ToString();

        Debug.Log(temp);
        Enum.TryParse(temp, out mainStory);

        return mainStory;
    }


    #endregion


    #region Event

    public delegate void WeekEnd();

    public static event WeekEnd OnWeekEnd;

    public static void FinishWeek()
    {
        OnWeekEnd?.Invoke();
    }
    #endregion
}
