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
    WeekEventData _eventData;
    enum Buttons
    {
        ResultBTN, ResultBTN2
    }
    enum Texts
    {
        EventText,
    }

    enum Images
    {
        CutSceneIMG,
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

        GetImage((int)Images.CutSceneIMG).sprite = LoadIMG(_eventData);

        GetText((int)Texts.EventText).text = _eventData.EventInfoString;
        GetButton((int)Buttons.ResultBTN).onClick.AddListener(ChooseBTN1);
        GetButton((int)Buttons.ResultBTN2).onClick.AddListener(ChooseBTN2);
        if (_eventData.EventDataType == EventDataType.Main) GetButton((int)Buttons.ResultBTN2).interactable = false;
    }

    Sprite LoadIMG(WeekEventData Data)
    {
        Sprite sprite = null;
        if(Data.CutSceneName == "")
        {
            Debug.Log("미완성");
        }
        else
        {
            sprite = Resources.Load<Sprite>("CutScene/" + Data.CutSceneName);
        }

        return sprite;
    }

    void ChooseBTN1()
    {
        DoOption(true);
        Managers.instance.ShowReceipt();
    }

    void ChooseBTN2()
    {
        DoOption(false);
        Managers.instance.ShowReceipt();
    }


    void DoOption(bool isOption1)
    {
        float[] optionArray;
        if (isOption1) optionArray = _eventData.Option1;
        else
        {
            optionArray = _eventData.Option2;
        }
        

        //하트 별 변화량
        Managers.Data._myPlayerData.ChangeHeart(optionArray[0]);
        if (optionArray[0] != 0) Debug.Log($"하트{optionArray[0]} 변화 ");
        Managers.Data._myPlayerData.ChangeHeart(optionArray[1]);
        if (optionArray[1] != 0) Debug.Log($"별{optionArray[1]} 변화");
        //스텟 변화량
        float[] eventStatValues = new float[6];
        for(int i = 0; i<6;i++)
        {
            eventStatValues[i] = optionArray[i + 2];
            if(optionArray[i+2] != 0)
            {
                Debug.Log($"{(StatName)(i)} 스텟 {optionArray[i + 2]} 증가");
            }
        }

        Managers.Data._myPlayerData.ChangeStat(eventStatValues);
    }
}
