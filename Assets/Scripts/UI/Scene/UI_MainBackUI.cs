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
        HealthTMP,  //���� �ǰ� ����
        MentalTMP,  //���� ���� ����
        MyMoneyTMP, //���� ���� ���
        MySubsTMP,  //���� ���� �����ڼ�
        NowWeekTMP,
        GameStatTMP,
        SongStatTMP,
        ChatStatTMP,
        StrStatTMP,
        MentalStatTMP,
        LuckStatTMP
    }

    enum Buttons
    {
        CreateScheduleBTN,
        GameStatBTN,
        SongStatBTN,
        ChatStatBTN,
        StrStatBTN, MentalStatBTN, LuckStatBTN
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

        Button CreateScheduleBTN = Get<Button>((int)Buttons.CreateScheduleBTN);

        CreateScheduleBTN.onClick.AddListener(ShowOrCloseCreateSchedulePopup);
        GetButton((int)Buttons.GameStatBTN).onClick.AddListener(() => ShowStatProperty(StatName.Game));
        GetButton((int)Buttons.SongStatBTN).onClick.AddListener(() => ShowStatProperty(StatName.Song));
        GetButton((int)Buttons.ChatStatBTN).onClick.AddListener(() => ShowStatProperty(StatName.Chat));
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

        Get<TMPro.TMP_Text>((int)Texts.GameStatTMP).text =   "���� : " + Managers.Data._myPlayerData.SixStat[0];
        Get<TMPro.TMP_Text>((int)Texts.SongStatTMP).text =   "�뷡 : " + Managers.Data._myPlayerData.SixStat[1];
        Get<TMPro.TMP_Text>((int)Texts.ChatStatTMP).text =   "��ê : " + Managers.Data._myPlayerData.SixStat[2];
        Get<TMPro.TMP_Text>((int)Texts.StrStatTMP).text =    "�ٷ� : " + Managers.Data._myPlayerData.SixStat[3];
        Get<TMPro.TMP_Text>((int)Texts.MentalStatTMP).text = "��Ż : " + Managers.Data._myPlayerData.SixStat[4];
        Get<TMPro.TMP_Text>((int)Texts.LuckStatTMP).text =   "��� : " + Managers.Data._myPlayerData.SixStat[5];
    }

    private string GetInitialTextForType(Texts textType)
    {
        switch (textType)
        {
            case Texts.HealthTMP:
                return GetNowConditionToString(Managers.Data._myPlayerData.NowHeart);
            case Texts.MentalTMP:
                return GetNowConditionToString(Managers.Data._myPlayerData.NowStar);
            case Texts.MyMoneyTMP:
                return Managers.Data._myPlayerData.nowGoldAmount.ToString();
            case Texts.MySubsTMP:
                return Managers.Data._myPlayerData.nowSubCount.ToString();
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

    bool isPopupOpen = false;
    public void ShowOrCloseCreateSchedulePopup()
    {
        TMP_Text CreateScheduleTMP = Get<Button>((int)Buttons.CreateScheduleBTN).GetComponentInChildren<TMP_Text>();
        if (isPopupOpen)
        {
            Managers.UI_Manager.ClosePopupUI();
            CreateScheduleTMP.text = "������ �ۼ��ϱ�";
        }
        else
        {
            Managers.UI_Manager.ShowPopupUI<UI_SchedulePopup>();
            CreateScheduleTMP.text = "������ ���ư���";
        }        
        isPopupOpen = !isPopupOpen;
    }
}
