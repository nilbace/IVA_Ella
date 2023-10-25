using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameManager
{
    public IEnumerator StartSchedule()
    {
        int beforeSubsAmount = Managers.Data._myPlayerData.nowSubCount;
        int beforeHeart = Managers.Data._myPlayerData.NowHeart;
        int beforeStar = Managers.Data._myPlayerData.NowStar;
        for (int i = 0; i < 7; i++)
        {
            CarryOutOneDayWork(Managers.Data._SevenDayScheduleDatas[i]);
            Debug.Log($"{i + 1}���� ������ ����");
            UI_MainBackUI.instance.UpdateUItexts();
            yield return new WaitForSeconds(0.1f);
        }
        int aftersubsAmount = Managers.Data._myPlayerData.nowSubCount;
        int afterHeart = Managers.Data._myPlayerData.NowHeart;
        int afterStar = Managers.Data._myPlayerData.NowStar;
        Debug.Log($"1���� �� ������ ��ȭ�� :     {aftersubsAmount - beforeSubsAmount}");
        Debug.Log($"1���� ��Ʈ ������ ��ȭ�� :   {afterHeart - beforeHeart}");
        Debug.Log($"1���� �� ������ ��ȭ�� :     {afterStar - beforeStar}");

        UI_MainBackUI.instance.UpdateUItexts();

        if (Managers.Data._myPlayerData.NowWeek % 5 != 0) Managers.UI_Manager.ShowPopupUI<UI_RandomEvent>();
        else Managers.UI_Manager.ShowPopupUI<UI_Merchant>();

    }

    void CarryOutOneDayWork(OneDayScheduleData oneDay)
    {
        float nowWeekmag = Managers.Data.GetNowWeekBonusMag();

        int newSubs = CalculateSubAfterDay(Managers.Data._myPlayerData.nowSubCount,
            oneDay.FisSubsUpValue, oneDay.PerSubsUpValue, nowWeekmag);

        Managers.Data._myPlayerData.nowGoldAmount += Mathf.CeilToInt(Managers.Data._myPlayerData.nowSubCount * oneDay.InComeMag);
        Debug.Log($"��� ������ : {Mathf.CeilToInt(Managers.Data._myPlayerData.nowSubCount * oneDay.InComeMag)}");

        if (oneDay.scheduleType == ScheduleType.BroadCast)
        {
            Managers.Data._myPlayerData.nowSubCount += newSubs;
            Debug.Log($"������ ������ : {newSubs}");
        }

        if (oneDay.broadcastType == BroadCastType.Game || oneDay.broadcastType == BroadCastType.Song || oneDay.broadcastType == BroadCastType.Chat)
        {
            CalculateBonus((StatName)Enum.Parse(typeof(StatName), oneDay.broadcastType.ToString()), newSubs, Mathf.CeilToInt(Managers.Data._myPlayerData.nowSubCount * oneDay.InComeMag));
        }

        Debug.Log($"��Ʈ ��ȭ�� : {Mathf.Clamp(Mathf.CeilToInt(oneDay.HealthPointChangeValue) + Managers.Data._myPlayerData.NowHeart, 0, 100) - Managers.Data._myPlayerData.NowHeart}" +
            $"���� ��Ʈ : {Mathf.Clamp(Mathf.CeilToInt(oneDay.HealthPointChangeValue) + Managers.Data._myPlayerData.NowHeart, 0, 100)}");

        Debug.Log($"�� ��ȭ�� : {Mathf.Clamp(Mathf.CeilToInt(oneDay.MentalPointChageValue) + Managers.Data._myPlayerData.NowStar, 0, 100) - Managers.Data._myPlayerData.NowStar}" +
            $"���� �� : {Mathf.Clamp(Mathf.CeilToInt(oneDay.MentalPointChageValue) + Managers.Data._myPlayerData.NowStar, 0, 100)}");

        Managers.Data._myPlayerData.NowHeart = Mathf.Clamp(Mathf.CeilToInt(oneDay.HealthPointChangeValue) + Managers.Data._myPlayerData.NowHeart, 0, 100);
        Managers.Data._myPlayerData.NowStar = Mathf.Clamp(Mathf.CeilToInt(oneDay.MentalPointChageValue) + Managers.Data._myPlayerData.NowStar, 0, 100);


        Managers.Data._myPlayerData.SixStat[0] += oneDay.Six_Stats[0];
        Managers.Data._myPlayerData.SixStat[1] += oneDay.Six_Stats[1];
        Managers.Data._myPlayerData.SixStat[2] += oneDay.Six_Stats[2];
        Managers.Data._myPlayerData.SixStat[3] += oneDay.Six_Stats[3];
        Managers.Data._myPlayerData.SixStat[4] += oneDay.Six_Stats[4];
        Managers.Data._myPlayerData.SixStat[5] += oneDay.Six_Stats[5];
    }

    int CalculateSubAfterDay(int now, float fix, float per, float bonus)
    {
        float temp = (now + fix) * ((float)(100 + per) / 100f) * bonus;
        int result = Mathf.CeilToInt(temp);
        return result - now;
    }

    void CalculateBonus(StatName statname, int DaySub, int DayIncome)
    {
        Bonus tempBonus = Managers.Data.GetProperty(statname);

        Managers.Data._myPlayerData.nowGoldAmount += Mathf.CeilToInt(DayIncome * (tempBonus.IncomeBonus) / 100f);

        Managers.Data._myPlayerData.nowSubCount += Mathf.CeilToInt(DaySub * (tempBonus.IncomeBonus) / 100f);
        Debug.Log($"��� ���ʽ� ������ : {Mathf.CeilToInt(DayIncome * (tempBonus.IncomeBonus) / 100f)}  ������ ���ʽ� ������ : {Mathf.CeilToInt(DaySub * (tempBonus.IncomeBonus) / 100f)}");
    }

}
