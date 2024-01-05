using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_StatProperty : UI_Popup
{
    public float Tier0Poz;
    public float Tier1to10Poz;
    public float StatInfoInitalPoz;
    public float StatInfoInterval;

    

    public static UI_StatProperty instance;
    enum Buttons
    {
        CloseBTN,
    }
    enum Texts
    {
        BigStatValueTMP, StatInfoTMP, StatValueTMP, StatNameTMP, ExtraInfoTMP
    }

    enum GameObjects
    { 
        TextGroup,
        StatInfo_SelectBox
    }

    enum Images
    {
        UpperStatIcon, Stat_Cover
    }





    private void Start()
    {
        instance = this;
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TMPro.TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        GetButton((int)Buttons.CloseBTN).onClick.AddListener(Close);
    }

    public void Setting(StatName stat)
    {
        GetGameObject((int)GameObjects.StatInfo_SelectBox).transform.localPosition = new Vector3(0, StatInfoInitalPoz, 0);

        float nowStatValue = Managers.Data._myPlayerData.SixStat[(int)stat];
        int SelectedStatTier = GetStatTier_div_20(nowStatValue);
        Bonus nowBonus = Managers.Data.GetMainProperty(stat);

        //아이콘 세팅
        GetImage((int)Images.UpperStatIcon).sprite = Resources.Load<Sprite>($"Icon/{stat}");
        GetText((int)Texts.StatValueTMP).text = Managers.Data._myPlayerData.SixStat[(int)stat].ToString("F0");
        GetImage((int)Images.Stat_Cover).transform.localScale = new Vector3(1 - (float)Managers.Data._myPlayerData.SixStat[(int)stat] / 200f, 1, 1);


        //위치 조절
        GetGameObject((int)GameObjects.StatInfo_SelectBox).transform.localPosition += new Vector3(0, StatInfoInterval * SelectedStatTier, 0);
        //내부 글자 조절
        GetText((int)Texts.BigStatValueTMP).text = (SelectedStatTier * 20).ToString();
        if (stat == StatName.Game || stat == StatName.Song || stat == StatName.Draw)
        {
            GetText((int)Texts.StatInfoTMP).text = $"{GetIconString(StatIcons.Sub)} + {nowBonus.SubBonus}%, {GetIconString(StatIcons.Gold)} +{nowBonus.IncomeBonus}%";
            GetText((int)Texts.StatNameTMP).text = GetStatKorName(stat) + " 실력";
        }
        else if(stat == StatName.Strength)
        {
            GetText((int)Texts.StatInfoTMP).text = $"{GetIconString(StatIcons.Heart)} 감소량 -{SelectedStatTier * 10}%";
            GetText((int)Texts.StatNameTMP).text = GetStatKorName(stat);
        }
        else if(stat == StatName.Mental)
        {
            GetText((int)Texts.StatInfoTMP).text = $"{GetIconString(StatIcons.Star)} 감소량 -{SelectedStatTier * 10}%";
            GetText((int)Texts.StatNameTMP).text = GetStatKorName(stat);
        }
        else
        {
            GetText((int)Texts.StatInfoTMP).text = $"{GetIconString(StatIcons.BigSuccess)}대성공 확률 {SelectedStatTier * 10}%";
            GetText((int)Texts.StatNameTMP).text = GetStatKorName(stat);
        }
        GetText((int)Texts.ExtraInfoTMP).text = SetExtraInfoTMP(stat);

        int tempTier = 1;
        int indexofEmptyText = IndexofEmptyPlace(SelectedStatTier);
        TMPro.TMP_Text[] StatInfoTexts = GetGameObject((int)GameObjects.TextGroup).GetComponentsInChildren<TMPro.TMP_Text>();
        for (int i = 0; i < 11; i++)
        {
            if (i == indexofEmptyText)
            {
                StatInfoTexts[i].text = "";
                StatInfoTexts[i].rectTransform.sizeDelta = new Vector2(200, 10);
                continue;
            }
            StatInfoTexts[i].text = GetStatText(tempTier, stat);
            tempTier++;
        }

        if (SelectedStatTier != 0)
        {
            GetGameObject((int)GameObjects.StatInfo_SelectBox).SetActive(true);
            GetGameObject((int)GameObjects.TextGroup).transform.localPosition = new Vector3(-46, Tier1to10Poz, 0);
        }
        else
        {
            GetGameObject((int)GameObjects.StatInfo_SelectBox).SetActive(false);
            GetGameObject((int)GameObjects.TextGroup).transform.localPosition = new Vector3(-46, Tier0Poz, 0);
        }

        
    }

    string SetExtraInfoTMP(StatName stat)
    {
        string temp = "";
        switch (stat)
        {
            case StatName.Game:
                temp = "힐링 게임, 대전 게임, 공포 게임, 켠김에 왕까지 방송 진행시 아래에 표기된 보너스 수치를 획득합니다.";
                break;
            case StatName.Song:
                temp = "노래 부르기, 악기 연주, 작곡 방송 진행시 아래에 표기된 보너스 수치를 획득합니다.";
                break;
            case StatName.Draw:
                temp = "낙서 방송, 커미션 방송 진행시 아래에 표기된 보너스 수치를 획득합니다.";
                break;
            case StatName.Strength:
                temp = "일정 진행시 아래에 표기된 수치만큼 <sprite=11>의 소모량이 감소합니다.";
                break;
            case StatName.Mental:
                temp = "일정 진행시 아래에 표기된 수치만큼 <sprite=12>의 소모량이 감소합니다.";
                break;
            case StatName.Luck:
                temp = "일정 진행시 아래에 표기된 수치의 확률로 대성공합니다. 대성공시 얻는 모든 수치가 50% 추가됩니다.";
                break;
        }

        return temp;
    }


    int GetStatTier_div_20(float Value)
    {
        return (int)(Math.Floor(Value / 20));
    }

    string GetStatText(int tier, StatName stat)
    {
        int nowgrade = tier * 20;
        Bonus temp2 = Managers.Data.GetMainProperty(tier * 20);

        string temp = "";
        if (stat == StatName.Game || stat == StatName.Song || stat == StatName.Draw)
            temp = $"{nowgrade} :{GetIconString(StatIcons.Sub)}+ {temp2.SubBonus}%,{GetIconString(StatIcons.Gold)} +{temp2.IncomeBonus}%";
        else if (stat == StatName.Strength )
            temp = $"{nowgrade} :{GetIconString(StatIcons.Heart)} 감소량 -{tier*10}%";
        else if (stat == StatName.Mental)
            temp = $"{nowgrade} :{GetIconString(StatIcons.Star)} 감소량 -{tier * 10}%";
        else
            temp = $"{nowgrade} :{GetIconString(StatIcons.BigSuccess)}대성공 확률 {tier * 10}%";

        return temp;
    }

    int IndexofEmptyPlace(int RealTier)
    {
        int temp = RealTier - 1;
        if (temp == -1) return 0;
        return temp;
    }


    void Close()
    {
        Managers.UI_Manager.ClosePopupUI();
    }


}
