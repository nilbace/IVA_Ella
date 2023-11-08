using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DataManager;
using static Define;
using static REventManager;

public class UI_RandomEvent : UI_Popup
{
    public static UI_RandomEvent instance;
    EventData _eventData;
    enum Buttons
    {
        ResultBTN,
    }
    enum Texts
    {
        EventText,
    }

    enum Images
    {
        CutScene,
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        _eventData = Managers.RandEvent.GetProperEvent();
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        Bind<TMPro.TMP_Text>(typeof(Texts));

        Debug.Log(_eventData.EventName);

        GetText((int)Texts.EventText).text = _eventData.EventInfoString;
        GetButton((int)Buttons.ResultBTN).onClick.AddListener(Close);
    }

    void Close()
    {
        Managers.Data._myPlayerData.NowWeek++;
        ProcessData();
        UI_MainBackUI.instance.UpdateUItexts();
        UI_MainBackUI.instance.EndScheduleAndSetUI();
        Managers.Data.SaveData();
        Managers.UI_Manager.ClosePopupUI();
    }

    void ProcessData()
    {
        //�� ��ȭ
        Managers.Data._myPlayerData.nowGoldAmount += _eventData.Change[0];

        //������ ��ȭ
        Managers.Data._myPlayerData.nowSubCount = Mathf.CeilToInt((0.01f)*(float)(_eventData.Change[1]+100)*Managers.Data._myPlayerData.nowSubCount);

        //��Ʈ �� ��ȭ��
        Managers.Data._myPlayerData.NowHeart += _eventData.Change[2];
        Managers.Data._myPlayerData.NowStar += _eventData.Change[3];

        //���� ��ȭ��
        float[] eventStatValues = new float[6];
        for(int i = 0; i<6;i++)
        {
            eventStatValues[i] = _eventData.Change[i + 4];
        }

        Managers.Data._myPlayerData.UpStat(eventStatValues);
    }
   
}
