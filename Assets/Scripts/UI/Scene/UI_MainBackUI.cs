using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using DG.Tweening;

public class UI_MainBackUI : UI_Scene
{
    [SerializeField] float AniSpeed;
    enum Texts
    {
        HeartTMP,  //���� �ǰ� ����
        StarTMP,  //���� ���� ����
        MyMoneyTMP, //���� ���� ���
        MySubsTMP,  //���� ���� �����ڼ�
        NowWeekTMP,
        TempGameTMP,
        TempSongTMP,
        TempDrawTMP,
        TempStrTMP,
        TempMenTMP,
        TempLuckTMP,
    }

    enum Buttons
    {
        CreateScheduleBTN,
        GameStatBTN,
        SongStatBTN,
        DrawStatBTN,
        StrStatBTN, 
        MentalStatBTN,
        LuckStatBTN,
        SettingBTN,
        PlayerSB_BTN,
        StartScheduleBTN, BackBTN
    }

    enum GameObjects
    {
        HeartBar, StarBar, HeartCover, StarCover,
        GameStat_Cover, SongStat_Cover, DrawStat_Cover, StrStat_Cover, MenStat_Cover, LuckStat_Cover,
        Stats, Days7, CallenderBottom, BroadCastTitle
    }


    Animator[] IconBaseAnis = new Animator[6];
    Image[] DayResultSeals = new Image[7];

    public static UI_MainBackUI instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Init();
    }

    public StatName NowSelectStatProperty;
    private void Init()
    {
        base.Init();
        Bind<TMPro.TMP_Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        Button CreateScheduleBTN = Get<Button>((int)Buttons.CreateScheduleBTN);

        CreateScheduleBTN.onClick.AddListener(ShowSchedulePopup);
        GetButton((int)Buttons.GameStatBTN).onClick.AddListener(() => ShowStatProperty(StatName.Game));
        GetButton((int)Buttons.SongStatBTN).onClick.AddListener(() => ShowStatProperty(StatName.Song));
        GetButton((int)Buttons.DrawStatBTN).onClick.AddListener(() => ShowStatProperty(StatName.Draw));
        GetButton((int)Buttons.StrStatBTN).onClick.AddListener(() => ShowStatProperty(StatName.Health));
        GetButton((int)Buttons.MentalStatBTN).onClick.AddListener(() => ShowStatProperty(StatName.Mental));
        GetButton((int)Buttons.LuckStatBTN).onClick.AddListener(() => ShowStatProperty(StatName.Luck));
        GetButton((int)Buttons.StartScheduleBTN).onClick.AddListener(StartScheduleBTN);
        GetButton((int)Buttons.BackBTN).onClick.AddListener(BackBTN);
        GetButton((int)Buttons.StartScheduleBTN).gameObject.SetActive(false);
        GetButton((int)Buttons.BackBTN).gameObject.SetActive(false);

        for (int i = 0;i<6;i++)
        {
            IconBaseAnis[i] = GetGameObject((int)GameObjects.Stats).transform.GetChild(i).GetChild(0).GetComponent<Animator>();
            IconBaseAnis[i].speed = AniSpeed;
        }
        for (int i = 0; i < 7; i++)
        {
            DayResultSeals[i] = GetGameObject((int)GameObjects.Days7).transform.GetChild(i).GetChild(1).GetComponent<Image>();
        }
        GetButton((int)Buttons.SettingBTN).onClick.AddListener(SettingBTN);

        //��� Ÿ��Ʋ ���������� ���� ����
        GetGameObject((int)GameObjects.BroadCastTitle).transform.localPosition += new Vector3(XOffset,0,0);


        UpdateUItexts();
        Managers.Sound.Play("bgm1", Sound.Bgm);
    }

    void ShowStatProperty(StatName statName)
    {
        var Go = Managers.UI_Manager.ShowPopupUI<UI_StatProperty>();
        NowSelectStatProperty = statName;
    }

    /// <summary>
    /// ����ȭ�� �ؽ�Ʈ�� ����
    /// </summary>
    public void UpdateUItexts()
    {
        foreach (Texts textType in System.Enum.GetValues(typeof(Texts)))
        {
            TMPro.TMP_Text tmpText = Get<TMPro.TMP_Text>((int)textType);
            tmpText.text = GetInitialTextForType(textType);
        }

        GetGameObject((int)GameObjects.HeartCover).transform.localScale =
            new Vector3( 1 - (float)Managers.Data._myPlayerData.NowHeart/100f, 1, 1);
        GetGameObject((int)GameObjects.StarCover).transform.localScale =
            new Vector3( 1 - (float)Managers.Data._myPlayerData.NowStar / 100f, 1, 1);

        for(int i = 0; i<6;i++)
        {
            GetGameObject((int)GameObjects.GameStat_Cover+i).transform.localScale =
           new Vector3(1 - (float)Managers.Data._myPlayerData.SixStat[i] / 200f, 1, 1);
            GetText((int)Texts.TempGameTMP+i).text = Managers.Data._myPlayerData.SixStat[i].ToString();
        }
    }

    private string GetInitialTextForType(Texts textType)
    {
        switch (textType)
        {
            case Texts.HeartTMP:
                return GetNowConditionToString(Managers.Data._myPlayerData.NowHeart);
            case Texts.StarTMP:
                return GetNowConditionToString(Managers.Data._myPlayerData.NowStar);
            case Texts.MyMoneyTMP:
                return Util.FormatMoney(Managers.Data._myPlayerData.nowGoldAmount);
            case Texts.MySubsTMP:
                return Util.FormatMoney(Managers.Data._myPlayerData.nowSubCount);
            case Texts.NowWeekTMP:
                return "��� " +Managers.Data._myPlayerData.NowWeek.ToString()+"����";
            default:
                return "";
        }
    }

    string GetNowConditionToString(int n)
    {
        string temp = "";
        if (n >= 75)
        {
            temp = "�ǰ�";
        }
        else if (n >= 50)
        {
            temp = "����";
        }
        else if (n >= 25)
        {
            temp = "����";
        }
        else temp = "�ɰ�";
        return temp;
    }

    
    float moveDuration = 0.52f;
    float XOffset = 350;
    [Header("��Ʈ�� �ִϸ��̼�")]
    [SerializeField] Ease ease;
    /// <summary>
    /// ������ ���� ���۽� ȣ��Ǿ� UI�� �ٲ���
    /// </summary>
    public void StartScheduleAndSetUI()
    {
        StartCoroutine(StartScheduleAndSetUICor());
    }

    IEnumerator StartScheduleAndSetUICor()
    {
        CleanSealsOnCallenderBottom();

        Transform BroadCastTitle_tr = GetGameObject((int)GameObjects.BroadCastTitle).transform;
        Transform callenderB_tr = GetGameObject((int)GameObjects.CallenderBottom).transform;
        Transform PlayerSB_BTN_tr = GetButton((int)Buttons.PlayerSB_BTN).transform;
        Transform CreateScheduleBTN_tr = GetButton((int)Buttons.CreateScheduleBTN).transform;
        Transform StartScheduleBTN_TR = GetButton((int)Buttons.StartScheduleBTN).transform;

        callenderB_tr.DOMoveY(callenderB_tr.localPosition.y + 55, moveDuration).SetEase(ease);
        PlayerSB_BTN_tr.DOMoveY(PlayerSB_BTN_tr.localPosition.y - 55, moveDuration).SetEase(ease);
        CreateScheduleBTN_tr.DOMoveX(CreateScheduleBTN_tr.localPosition.x - XOffset, moveDuration).SetEase(ease);
        StartScheduleBTN_TR.DOMoveX(StartScheduleBTN_TR.localPosition.x - XOffset, moveDuration).SetEase(ease);
        var tween = BroadCastTitle_tr.DOMoveX(BroadCastTitle_tr.localPosition.x - XOffset, moveDuration).SetEase(ease);

        yield return tween;
    }

    /// <summary>
    /// ������ ����� UI�� �ٽ� ��ȯ
    /// </summary>
    public void EndScheduleAndSetUI()
    {
        StartCoroutine(EndScheduleAndSetUICor());
    }

    IEnumerator EndScheduleAndSetUICor()
    {
        CleanSealsOnCallenderBottom();
        GetButton((int)Buttons.CreateScheduleBTN).gameObject.SetActive(true);
        GetButton((int)Buttons.StartScheduleBTN).gameObject.SetActive(false);

        Transform BroadCastTitle_tr = GetGameObject((int)GameObjects.BroadCastTitle).transform;
        Transform callenderB_tr = GetGameObject((int)GameObjects.CallenderBottom).transform;
        Transform PlayerSB_BTN_tr = GetButton((int)Buttons.PlayerSB_BTN).transform;
        Transform CreateScheduleBTN_tr = GetButton((int)Buttons.CreateScheduleBTN).transform;
        Transform StartScheduleBTN_TR = GetButton((int)Buttons.StartScheduleBTN).transform;

        callenderB_tr.DOMoveY(callenderB_tr.localPosition.y - 55, moveDuration).SetEase(ease);
        PlayerSB_BTN_tr.DOMoveY(PlayerSB_BTN_tr.localPosition.y + 55, moveDuration).SetEase(ease);
        CreateScheduleBTN_tr.DOMoveX(CreateScheduleBTN_tr.localPosition.x + XOffset, moveDuration).SetEase(ease);
        StartScheduleBTN_TR.DOMoveX(StartScheduleBTN_TR.localPosition.x + XOffset, moveDuration).SetEase(ease);
        var tween = BroadCastTitle_tr.DOMoveX(BroadCastTitle_tr.localPosition.x + XOffset, moveDuration).SetEase(ease);

        yield return tween;
    }

    public void GlitterStat(int i)
    {
        IconBaseAnis[i].CrossFade("Shine", 0);
    }

    public void CleanSealsOnCallenderBottom()
    {
        for(int i = 0; i<7;i++)
        {
            DayResultSeals[i].sprite = null;
            DayResultSeals[i].color = new Color(0, 0, 0, 0);
        }
    }

    public void StampSeal(int day, int SealType)
    {
        DayResultSeals[day].color = new Color(1, 1, 1, 1);
        DayResultSeals[day].sprite = Managers.MSM.DayResultSeal[SealType];
    }

    public void ShowSchedulePopup()
    {
        Managers.UI_Manager.ShowPopupUI<UI_SchedulePopup>();
        GetButton((int)Buttons.StartScheduleBTN).gameObject.SetActive(true);
        GetButton((int)Buttons.BackBTN).gameObject.SetActive(true);
        Get<Button>((int)Buttons.CreateScheduleBTN).gameObject.SetActive(false);
    }

    void StartScheduleBTN()
    {
        StartScheduleAndSetUI();
        Managers.instance.StartSchedule();
        Managers.UI_Manager.ClosePopupUI();
    }
    void BackBTN()
    {
        if (UI_SchedulePopup.instance.SubContentSelectPhase)
        {
            UI_SchedulePopup.instance.Show3Contents();
        }
        else
        {
            Get<Button>((int)Buttons.CreateScheduleBTN).gameObject.SetActive(true);
            GetButton((int)Buttons.StartScheduleBTN).gameObject.SetActive(false);
            GetButton((int)Buttons.BackBTN).gameObject.SetActive(false);
            Managers.UI_Manager.ClosePopupUI();

        }
    }

    public Button GetStartScheduleBTN()
    {
        return GetButton((int)Buttons.StartScheduleBTN);
    }

    public Button GetBackBTN()
    {
        return GetButton((int)Buttons.BackBTN);
    }

    public void SettingBTN()
    {
        Managers.UI_Manager.ShowPopupUI<UI_Setting>();
    }
}
