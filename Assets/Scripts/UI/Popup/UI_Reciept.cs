using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using DG.Tweening;
using System;

public class UI_Reciept : UI_Popup
{
    public Ease EaseStatus;
    public float periodScale;
    public float Duration;
    enum Buttons
    {
        FinishBTN
    }
    enum Texts
    {
        NWeekTMP,
        SuccessTimeTMP,
        ReceieptDetailUpperTMP,
        ReceieptDetailLowerTMP
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TMPro.TMP_Text>(typeof(Texts));
        Managers.Sound.Play(Define.Sound.Receipt);
        SetReceipt();
        transform.DOLocalMoveY(0, Duration).SetEase(EaseStatus, 0, periodScale);
        GetButton((int)Buttons.FinishBTN).onClick.AddListener(FinishBTN);
    }

    public void SetReceipt()
    {
        GetText((int)Texts.NWeekTMP).text = $"{Managers.Data.PlayerData.NowWeek}���� ������";
        GetText((int)Texts.SuccessTimeTMP).text = $"{ScheduleExecuter.Inst.SuccessTimeContainer[0]}ȸ"
                                                   + $"\n{ScheduleExecuter.Inst.SuccessTimeContainer[1]}ȸ"
                                                   + $"\n{ScheduleExecuter.Inst.SuccessTimeContainer[2]}ȸ";
        WeekReceiptData temp = ScheduleExecuter.Inst.BeforeScheduleData;

        int sub = Managers.Data.PlayerData.nowSubCount - temp.Subs;
        int income = Managers.Data.PlayerData.nowGoldAmount - temp.Gold;
        float[] stats = new float[6];
        for (int i = 0; i < 6; i++)
        {
            stats[i] = Managers.Data.PlayerData.SixStat[i] - temp.SixStat[i];
        }

        GetText((int)Texts.ReceieptDetailUpperTMP).text = $"+{sub}��"
                                                           + $"\n{income}��";
        GetText((int)Texts.ReceieptDetailLowerTMP).text = "";
        for (int i = 0; i < 6; i++)
        {
            GetText((int)Texts.ReceieptDetailLowerTMP).text += (stats[i] >= 0 ? (stats[i] == 0 ? "-" : "+") : "") + (stats[i] != 0 ? stats[i].ToString("F0") : "") + "\n";
        }
        GetText((int)Texts.ReceieptDetailLowerTMP).text.TrimEnd('\n');
    }



    void FinishBTN()
    {
        //������ �κ�
        var data = Managers.Data.PlayerData;
        temp += $"{data.NowWeek}���� : ����{data.SixStat[0]},����{data.nowSubCount},��{data.nowGoldAmount},�ٷ�{data.SixStat[3]},��Ż{data.SixStat[4]},���{data.SixStat[5]}\n";
        Debug.Log(temp);
        //�������

        //if (Managers.Data.PlayerData.NowWeek == 20)
        //{
        //    Managers.UI_Manager.ShowPopupUI<UI_Ending>();
        //    return;
        //}

        Managers.UI_Manager.CloseALlPopupUI();
        Managers.Sound.Play(Define.Sound.NextWeekBTN);

        ScheduleExecuter.Inst.FinishWeek();
    }

    static string temp = "";
}
