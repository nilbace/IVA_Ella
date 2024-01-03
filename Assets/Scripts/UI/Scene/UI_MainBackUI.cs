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

    //���� ȭ�� �ִϸ��̼�
    public Animator ScreenAnimator;
    public float ScreenAniSpeed;
    public bool IsFastMode = false;
    public Sprite[] SpeedBTNSprite;

    //�ϴ� ������ �ִϸ��̼�
    public Sprite[] TempStampImg;
    public float StampResetTime;
    public Ease StampEase;
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
        StartScheduleBTN, BackBTN,
        SpeedBTN,
        ArchiveBTN

    }

    enum GameObjects
    {
        HeartBar, StarBar, HeartCover, StarCover,
        GameStat_Cover, SongStat_Cover, DrawStat_Cover, StrStat_Cover, MenStat_Cover, LuckStat_Cover,
        Stats, Days7, CallenderBottom, BroadCastTitle
    }

    enum Images
    {
        HeartBar, StarBar, StampIMG
    }


    Animator[] IconBaseAnis = new Animator[6];
    Image[] DayResultSeals = new Image[7];

    public static UI_MainBackUI instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        base.Init();
        Bind<TMPro.TMP_Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        Button CreateScheduleBTN = Get<Button>((int)Buttons.CreateScheduleBTN);

        CreateScheduleBTN.onClick.AddListener(ShowSchedulePopup);
        GetButton((int)Buttons.GameStatBTN).onClick.AddListener(() => ShowStatPropertyUI(StatName.Game));
        GetButton((int)Buttons.SongStatBTN).onClick.AddListener(() => ShowStatPropertyUI(StatName.Song));
        GetButton((int)Buttons.DrawStatBTN).onClick.AddListener(() => ShowStatPropertyUI(StatName.Draw));
        GetButton((int)Buttons.StrStatBTN).onClick.AddListener(() => ShowStatPropertyUI(StatName.Strength));
        GetButton((int)Buttons.MentalStatBTN).onClick.AddListener(() => ShowStatPropertyUI(StatName.Mental));
        GetButton((int)Buttons.LuckStatBTN).onClick.AddListener(() => ShowStatPropertyUI(StatName.Luck));
        GetButton((int)Buttons.StartScheduleBTN).onClick.AddListener(StartScheduleBTN);
        GetButton((int)Buttons.BackBTN).onClick.AddListener(BackBTN);
        GetButton((int)Buttons.ArchiveBTN).onClick.AddListener(ArchiveBTN);
        GetButton((int)Buttons.StartScheduleBTN).gameObject.SetActive(false);
        GetButton((int)Buttons.BackBTN).gameObject.SetActive(false);
        SpeedBTNInit();
        GetButton((int)Buttons.SpeedBTN).onClick.AddListener(SpeedBTN);

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

        ScreenAnimator.speed = ScreenAniSpeed;
        SetStamp(-1);
        UpdateUItexts();
        Managers.Sound.Play("bgm1", Sound.Bgm);
    }

    void ArchiveBTN()
    {
        Managers.Sound.Play(Define.Sound.SmallBTN);
        Managers.UI_Manager.ShowPopupUI<UI_Archive>();
    }

    #region SpeedBTN
    void SpeedBTNInit()
    {
        if (PlayerPrefs.HasKey("IsFastModeKey"))
        {
            IsFastMode = PlayerPrefs.GetInt("IsFastModeKey") == 1;
        }

        if (!IsFastMode)
        {
            GetButton((int)Buttons.SpeedBTN).GetComponent<Image>().sprite = SpeedBTNSprite[0];
        }
        else
        {
            GetButton((int)Buttons.SpeedBTN).GetComponent<Image>().sprite = SpeedBTNSprite[1];
        }
    }

    void SpeedBTN()
    {
        IsFastMode = !IsFastMode;
        SaveIsFastMode();
        if (!IsFastMode)
        {
            GetButton((int)Buttons.SpeedBTN).GetComponent<Image>().sprite = SpeedBTNSprite[0];
        }
        else
        {
            GetButton((int)Buttons.SpeedBTN).GetComponent<Image>().sprite = SpeedBTNSprite[1];
        }
    }
    void SaveIsFastMode()
    {
        // true�� 1��, false�� 0���� ����
        PlayerPrefs.SetInt("IsFastModeKey", IsFastMode ? 1 : 0);
        PlayerPrefs.Save();
    }

    #endregion


    void ShowStatPropertyUI(StatName statName)
    {
        Managers.Sound.Play("SmallBTN", Sound.Effect);
        StartCoroutine(ShowStatProperty(statName));
    }

    IEnumerator ShowStatProperty(StatName statName)
    {
        if(UI_StatProperty.instance == null)
        {
            var Go = Managers.UI_Manager.ShowPopupUI<UI_StatProperty>();
            yield return new WaitForEndOfFrame();
            Go.Setting(statName);
        }
        else
        {
            UI_StatProperty.instance.Setting(statName);
        }
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

        float nowHeart = Managers.Data._myPlayerData.NowHeart;
        float nowStar = Managers.Data._myPlayerData.NowStar;

        GetImage((int)Images.HeartBar).sprite =
            Managers.MSM.StatusBar[GetStatusBarImageIndex(nowHeart)];
        GetImage((int)Images.StarBar).sprite =
            Managers.MSM.StatusBar[GetStatusBarImageIndex(nowStar)];

        GetGameObject((int)GameObjects.HeartCover).transform.localScale =
            new Vector3( 1 - (float)Managers.Data._myPlayerData.NowHeart/100f, 1, 1);
        GetGameObject((int)GameObjects.StarCover).transform.localScale =
            new Vector3( 1 - (float)Managers.Data._myPlayerData.NowStar / 100f, 1, 1);

        GetText((int)Texts.HeartTMP).color =
            HeartStarTextColors[GetStatusBarImageIndex(nowHeart)];
        GetText((int)Texts.StarTMP).color =
            HeartStarTextColors[GetStatusBarImageIndex(nowStar)];

        for (int i = 0; i<6;i++)
        {
            GetGameObject((int)GameObjects.GameStat_Cover+i).transform.localScale =
           new Vector3(1 - (float)Managers.Data._myPlayerData.SixStat[i] / 200f, 1, 1);
            GetText((int)Texts.TempGameTMP+i).text = Managers.Data._myPlayerData.SixStat[i].ToString("F0");
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

    [Header("�ǰ� ���� ��")]
    [SerializeField] Color[] HeartStarTextColors;
    string GetNowConditionToString(float n)
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

    int GetStatusBarImageIndex(float n)
    {
        int temp = -1;
        if (n >= 75)
        {
            temp = 0;
        }
        else if (n >= 50)
        {
            temp = 1;
        }
        else if (n >= 25)
        {
            temp = 2;
        }
        else temp = 3;
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
    string templog = "";
    public void EndScheduleAndSetUI()
    {
        templog += $"\n{Managers.Data._myPlayerData.NowWeek - 1 } / ������_{Managers.Data._myPlayerData.nowSubCount} / ���� ����_{Managers.Data._myPlayerData.SixStat[0]}/{Managers.Data._myPlayerData.SixStat[3]}/{Managers.Data._myPlayerData.SixStat[4]}/{Managers.Data._myPlayerData.SixStat[5]}";
        Debug.Log(templog);
        StartCoroutine(EndScheduleAndSetUICor());
    }

    IEnumerator EndScheduleAndSetUICor()
    {
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

    public void BottomSeal(int day, int SealType)
    {
        DayResultSeals[day].color = new Color(1, 1, 1, 1);

        if (SealType == 0)
            DayResultSeals[day].GetComponent<Animator>().SetTrigger("StarAni");
        else if(SealType == 1)
            DayResultSeals[day].GetComponent<Animator>().SetTrigger("OAni");
        else if(SealType == 2)
        {
            Debug.Log("XXXXX");
            DayResultSeals[day].GetComponent<Animator>().SetTrigger("XAni");
        }
    }

    public void ShowSchedulePopup()
    {
        Managers.Sound.Play(Sound.BigBTN);
        Managers.UI_Manager.ShowPopupUI<UI_SchedulePopup>();
        GetButton((int)Buttons.StartScheduleBTN).gameObject.SetActive(true);
        GetButton((int)Buttons.BackBTN).gameObject.SetActive(true);
        Get<Button>((int)Buttons.CreateScheduleBTN).gameObject.SetActive(false);
    }

    void StartScheduleBTN()
    {
        Managers.Sound.Play(Sound.ScheduleBTN);
        StartScheduleAndSetUI();
        Managers.instance.StartSchedule();
        Managers.UI_Manager.ClosePopupUI();
    }
    void BackBTN()
    {
        Managers.Sound.Play("SmallBTN", Sound.Effect);
        if (UI_SchedulePopup.instance.IsShowing3ContentsUI())
        {
            Get<Button>((int)Buttons.CreateScheduleBTN).gameObject.SetActive(true);
            GetButton((int)Buttons.StartScheduleBTN).gameObject.SetActive(false);
            GetButton((int)Buttons.BackBTN).gameObject.SetActive(false);
            Managers.UI_Manager.ClosePopupUI();
        }
        else
        {
            UI_SchedulePopup.instance.Show3Contents();
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
        Managers.Sound.Play("SmallBTN", Sound.Effect);
        Managers.UI_Manager.ShowPopupUI<UI_Setting>();
    }


    #region ScheduleAnimation

    public void StartScreenAnimation(string KorName)
    {
        ScreenAnimator.StopPlayback();
        if(KorName == "")
        {
            Debug.Log("������");
        }
        else
        {
            ScreenAnimator.SetTrigger(KorName);
        }
    }
    /// <summary>
    /// -1��ĭ 0���� 1���� 2�뼺�� 
    /// </summary>
    /// <param name="Result"></param>
    public void SetStamp(int Result)
    {
        switch (Result)
        {
            case -1:
                GetImage((int)Images.StampIMG).gameObject.SetActive(false);
                break;
            case 0:
                GetImage((int)Images.StampIMG).gameObject.SetActive(true);
                GetImage((int)Images.StampIMG).sprite = TempStampImg[0];
                break;
            case 1:
                GetImage((int)Images.StampIMG).gameObject.SetActive(true);
                GetImage((int)Images.StampIMG).sprite = TempStampImg[1];
                break;
            case 2:
                GetImage((int)Images.StampIMG).gameObject.SetActive(true);
                GetImage((int)Images.StampIMG).sprite = TempStampImg[2];
                break;
        }
        GetImage((int)Images.StampIMG).transform.localScale = Vector3.one * 5f;
        GetImage((int)Images.StampIMG).transform.DOScale(1, StampResetTime).SetEase(StampEase);
    }

    #endregion
}
