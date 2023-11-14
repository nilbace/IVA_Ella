using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_StatProperty : UI_Popup
{
    public static UI_StatProperty instance;
    enum Buttons
    {
        CloseBTN,
    }
    enum Texts
    {
        BigStatValueTMP, StatInfoTMP
    }

    enum GameObjects
    { 
        //Empty�� ������ ���ڵ� ������ �����ؾ���
        TextGroup,
        StatInfo_SelectBox
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
        GetText((int)Texts.StatInfoTMP).text = UI_MainBackUI.instance.NowSelectStatProperty.ToString();

        StatName SelectedStat = UI_MainBackUI.instance.NowSelectStatProperty;
        float nowStatValue = Managers.Data._myPlayerData.SixStat[(int)SelectedStat];
        int SelectedStatTier = GetStatTier_div_20(nowStatValue);
        Bonus nowBonus = Managers.Data.GetMainProperty(SelectedStat);


        //��ġ ����
        GetGameObject((int)GameObjects.StatInfo_SelectBox).transform.localPosition += new Vector3(0, -14 * SelectedStatTier, 0);
        //���� ���� ����
        GetText((int)Texts.BigStatValueTMP).text = (SelectedStatTier * 20).ToString();
        if (SelectedStat == StatName.Game || SelectedStat == StatName.Song || SelectedStat == StatName.Draw)
            GetText((int)Texts.StatInfoTMP).text = $"������ �� + {nowBonus.SubBonus}%, �� ȹ�淮 +{nowBonus.IncomeBonus}%";


        int tempTier = 1; 
        int indexofEmptyText = IndexofEmptyPlace(SelectedStatTier);
        TMPro.TMP_Text[] StatInfoTexts = GetGameObject((int)GameObjects.TextGroup).GetComponentsInChildren<TMPro.TMP_Text>();
        for (int i = 0;i<11;i++)
        {
            if (i == indexofEmptyText)
            {
                StatInfoTexts[i].text = ""; continue; 
            }
            StatInfoTexts[i].text = GetMainStatText(tempTier);
            tempTier++;
        }


        

        GetButton((int)Buttons.CloseBTN).onClick.AddListener(Close);
    }

 

    int GetStatTier_div_20(float Value)
    {
        return (int)(Math.Floor(Value / 20));
    }

    string GetMainStatText(int tier)
    {
        int subBonus = Math.Max(5, tier) * Managers.instance.MainStat_ValuePerLevel;
        int incomeBonus = Math.Max(0, tier) * Managers.instance.MainStat_ValuePerLevel;
        int nowgrade = tier * 20;

        string temp = $"{nowgrade} : ������ �� + {subBonus}%, �� ȹ�淮 +{incomeBonus}%";

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
