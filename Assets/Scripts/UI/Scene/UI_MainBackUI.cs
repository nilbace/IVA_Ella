using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_MainBackUI : UI_Scene
{
    enum Texts
    {
        HeartTMP,  //���� �ǰ� ����
        StarTMP,  //���� ���� ����
        MyMoneyTMP, //���� ���� ���
        MySubsTMP,  //���� ���� �����ڼ�
        NowWeekTMP,
        TempGameTMP,

    }

    enum Buttons
    {
        CreateScheduleBTN,
        GameStatBTN,
        SongStatBTN,
        DrawStatBTN,
        StrStatBTN, 
        MentalStatBTN,
        LuckStatBTN
    }

    enum GameObjects
    {
        HeartBar, StarBar, HeartCover, StarCover,
        GameStat_Cover,
    }


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

        UpdateUItexts();
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

        GetGameObject((int)GameObjects.GameStat_Cover).transform.localScale =
            new Vector3(1 - (float)Managers.Data._myPlayerData.SixStat[0] / 200f, 1, 1);
        GetText((int)Texts.TempGameTMP).text = Managers.Data._myPlayerData.SixStat[0].ToString();
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

    public void ShowCreateScheduleBTN()
    {
        Get<Button>((int)Buttons.CreateScheduleBTN).gameObject.SetActive(true);
    }

    public void ShowSchedulePopup()
    {
         Managers.UI_Manager.ShowPopupUI<UI_SchedulePopup>();
         Get<Button>((int)Buttons.CreateScheduleBTN).gameObject.SetActive(false);
    }
}
