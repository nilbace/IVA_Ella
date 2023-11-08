using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameManager
{
    string[] daysOfWeek = { "������", "ȭ����", "������", "�����", "�ݿ���", "�����", "�Ͽ���" };
    public IEnumerator StartSchedule()
    {
        int beforeSubsAmount = Managers.Data._myPlayerData.nowSubCount;
        int beforeHeart = Managers.Data._myPlayerData.NowHeart;
        int beforeStar = Managers.Data._myPlayerData.NowStar;

        // �������� �Ⱦ���
        isSick = false; SickDayOne = false;

        for (int i = 0; i < 7; i++)
        {
            CarryOutOneDayWork(Managers.Data._SevenDayScheduleDatas[i], i);
            Debug.Log("-----------------------------------------");
            UI_MainBackUI.instance.UpdateUItexts();
            yield return new WaitForSeconds(1f);
        }

        //�� �� ������ ������ ���� ����
        if (Managers.Data._myPlayerData.NowWeek % 4 == 0)
        {
            Managers.Data._myPlayerData.nowGoldAmount -= Managers.Data.GetNowMonthExpense();
            Debug.Log($"���� {Managers.Data.GetNowMonthExpense() } �� ����");
        }

        int aftersubsAmount = Managers.Data._myPlayerData.nowSubCount;
        int afterHeart = Managers.Data._myPlayerData.NowHeart;
        int afterStar = Managers.Data._myPlayerData.NowStar;
        Debug.Log($"1���� �� ������ ��ȭ�� :     {aftersubsAmount - beforeSubsAmount}");
        Debug.Log($"1���� ��Ʈ ������ ��ȭ�� :   {afterHeart - beforeHeart}");
        Debug.Log($"1���� �� ������ ��ȭ�� :     {afterStar - beforeStar}");

        for(int i =0;i<7;i++)
        {
            Managers.Data._SevenDayScheduleDatas[i] = null;
            Managers.Data._SeveDayScrollVarValue[i] = 0;
        }

        UI_MainBackUI.instance.UpdateUItexts();

        if (Managers.Data._myPlayerData.NowWeek % 5 != 0) Managers.UI_Manager.ShowPopupUI<UI_RandomEvent>();
        else Managers.UI_Manager.ShowPopupUI<UI_Merchant>();
    }

    void CarryOutOneDayWork(OneDayScheduleData oneDay, int Day)
    {
        //�޽� �ϴ°� �ƴ϶�� ���� �� ����
        if(oneDay.scheduleType != ScheduleType.Rest)
        {
            CheckPossibilityOfCatching_Aya();
        }

        //������ �׳� �������� ����
        if(isSick)
        {
            Debug.Log($"{daysOfWeek[Day]} ����");
            CarryOutSickDay();
        }
        //�Ⱦ����� ��� ������ ���� �뼺���� �� �� ����
        else
        {
            Debug.Log($"{daysOfWeek[Day]} ������ {Managers.Data._SevenDayScheduleDatas[Day].KorName} ����");
            float bonusMultiplier = 1.0f;
            if (CheckPossibilityOfBigSuccess())
            {
                BigSuccess = true;
                bonusMultiplier = 1.5f;// 50% ����� ���� �����
                Debug.Log("�뼺��");
            }

            float nowWeekmag = Managers.Data.GetNowWeekBonusMag();

            int OneDayNewSubs = CalculateSubAfterDay(Managers.Data._myPlayerData.nowSubCount,
                oneDay.FisSubsUpValue, oneDay.PerSubsUpValue, nowWeekmag * bonusMultiplier);

            int OneDayIncome = Mathf.CeilToInt(Managers.Data._myPlayerData.nowSubCount * oneDay.InComeMag * bonusMultiplier);

            if (oneDay.scheduleType == ScheduleType.BroadCast)
            {
                Managers.Data._myPlayerData.nowSubCount += OneDayNewSubs;
                Managers.Data._myPlayerData.nowGoldAmount += OneDayIncome;
                Debug.Log($"����+ : {OneDayNewSubs}" + $" / ��� + : {OneDayIncome}");
            }

            CalculateBonusWithType(oneDay.broadcastType, OneDayNewSubs, OneDayIncome);

            float HeartVariance; float StarVariance;
            if (oneDay.scheduleType == ScheduleType.Rest)
            {
                HeartVariance = oneDay.HeartVariance * bonusMultiplier;
                StarVariance = oneDay.StarVariance * bonusMultiplier;
            }
            else
            {
                HeartVariance = oneDay.HeartVariance * StrengthBonus();
                StarVariance = oneDay.StarVariance * MentalBonus();
            }

            Debug.Log($"�ǰ� ��ȭ�� : ({Mathf.Clamp((HeartVariance) + Managers.Data._myPlayerData.NowHeart, 0, 100) - Managers.Data._myPlayerData.NowHeart}," +
            $" {Mathf.Clamp((StarVariance) + Managers.Data._myPlayerData.NowStar, 0, 100) - Managers.Data._myPlayerData.NowStar}), " +
            $"��� ( {Mathf.Clamp((HeartVariance) + Managers.Data._myPlayerData.NowHeart, 0, 100)}, " +
            $"{Mathf.Clamp((StarVariance) + Managers.Data._myPlayerData.NowStar, 0, 100)})");

            Managers.Data._myPlayerData.NowHeart = Mathf.Clamp(Mathf.CeilToInt(HeartVariance) + Managers.Data._myPlayerData.NowHeart, 0, 100);
            Managers.Data._myPlayerData.NowStar = Mathf.Clamp(Mathf.CeilToInt(StarVariance) + Managers.Data._myPlayerData.NowStar, 0, 100);

            float[] tempstat = new float[6];
            for(int i = 0;i<6;i++)
            {
                tempstat[i] = oneDay.Six_Stats[i] * bonusMultiplier;
            }

            Managers.Data._myPlayerData.UpStat(tempstat);
        }

        if (isSick)         UI_MainBackUI.instance.StampSeal(Day, 2);
        else if(BigSuccess) UI_MainBackUI.instance.StampSeal(Day, 0);
        else                UI_MainBackUI.instance.StampSeal(Day, 1);
    }


    #region Calculate
    int CalculateSubAfterDay(int now, float fix, float per, float bonus)
    {
        float temp = (now + fix) * ((float)(100 + per) / 100f) * bonus;
        int result = Mathf.CeilToInt(temp);
        return result - now;
    }

    void CalculateBonusWithType(BroadCastType broadCastType, int DaySub, int DayIncome)
    {
        StatName temp;
        if (broadCastType == BroadCastType.Healing || broadCastType == BroadCastType.LOL
            || broadCastType == BroadCastType.Horror || broadCastType == BroadCastType.Challenge)
            temp = StatName.Game;
        else if (broadCastType == BroadCastType.Sing || broadCastType == BroadCastType.PlayInst || broadCastType == BroadCastType.Compose)
            temp = StatName.Song;
        else
            temp = StatName.Draw;

        CalculateBonus(temp, DaySub, DayIncome);
    }


    void CalculateBonus(StatName statname, int DaySub, int DayIncome)
    {
        Bonus tempBonus = Managers.Data.GetProperty(statname);

        Managers.Data._myPlayerData.nowGoldAmount += Mathf.CeilToInt(DayIncome * (tempBonus.IncomeBonus) / 100f);

        Managers.Data._myPlayerData.nowSubCount += Mathf.CeilToInt(DaySub * (tempBonus.IncomeBonus) / 100f);
        Debug.Log($"Ư�� ��� ���ʽ� : {Mathf.CeilToInt(DayIncome * (tempBonus.IncomeBonus) / 100f)} Ư�� ������ ���ʽ� ������ : {Mathf.CeilToInt(DaySub * (tempBonus.IncomeBonus) / 100f)}");
    }

    float StrengthBonus()
    {
        int temp = (int)Math.Floor(Managers.Data._myPlayerData.SixStat[3]);
        float result = (float)(temp / 10);
        result *= 0.05f;
        result = 1 - result;
        return result;
    }

    float MentalBonus()
    {
        int temp = (int)Math.Floor(Managers.Data._myPlayerData.SixStat[4]);
        float result = (float)(temp / 10);
        result *= 0.05f;
        result = 1 - result;
        return result;
    }

    #endregion

    #region SuccessAndFail
    bool isSick = false;        bool SickDayOne = false;
    bool caughtCold = false;    bool caughtDepression = false;

    /// <summary>
    /// ������� ���� �ʴٸ� �ƾ� ��
    /// </summary>
    void CheckPossibilityOfCatching_Aya()
    {
        if(Managers.Data._myPlayerData.NowHeart < 50 || Managers.Data._myPlayerData.NowStar < 50)
        {
            if(Managers.Data._myPlayerData.NowHeart < 25)
            {
                if (UnityEngine.Random.Range(0, 100) < 25)
                {
                    isSick = true;
                    SickDayOne = true;
                    caughtCold = true;
                    return;
                }
            }
            else if(Managers.Data._myPlayerData.NowStar < 25)
            {
                if (UnityEngine.Random.Range(0, 100) < 25)
                {
                    isSick = true;
                    SickDayOne = true;
                    caughtDepression = true;
                    return;
                }
            }
            else if(Managers.Data._myPlayerData.NowHeart < 50)
            {
                if (UnityEngine.Random.Range(0, 100) < 50)
                {
                    isSick = true;
                    SickDayOne = true;
                    caughtCold = true;
                    return;
                }
            }
            else
            {
                if (UnityEngine.Random.Range(0, 100) < 50)
                {
                    isSick = true;
                    SickDayOne = true;
                    caughtDepression = true;
                    return;
                }
            }
        }
    }
    
    
    /// <summary>
    /// ���� �� ����
    /// </summary>
    void CarryOutSickDay()
    {
        if (SickDayOne)
        {
            SickDayOne = false;
        }
        else
        {
            isSick = false;
        }
        int RestHeartStarValue = 10;
        Managers.Data._myPlayerData.NowHeart += RestHeartStarValue;
        Managers.Data._myPlayerData.NowStar  += RestHeartStarValue;
        Debug.Log($"�ǰ� ��ȭ�� : ({Mathf.Clamp((RestHeartStarValue) + Managers.Data._myPlayerData.NowHeart, 0, 100) - Managers.Data._myPlayerData.NowHeart}," +
            $" {Mathf.Clamp((RestHeartStarValue) + Managers.Data._myPlayerData.NowStar, 0, 100) - Managers.Data._myPlayerData.NowStar}), " +
            $"��� ( {Mathf.Clamp((RestHeartStarValue) + Managers.Data._myPlayerData.NowHeart, 0, 100)}, " +
            $"{Mathf.Clamp((RestHeartStarValue) + Managers.Data._myPlayerData.NowStar, 0, 100)})");

        if (caughtCold)
        {
            Debug.Log("���� ���� �ɸ�");
        }
        else
        {
            Debug.Log("���� �����");
        }
    }

    bool BigSuccess = false;

    bool CheckPossibilityOfBigSuccess()
    {
        int LuckGrade = ((int)Managers.Data._myPlayerData.SixStat[5]) / 10;
        if (UnityEngine.Random.Range(0, 100) < (LuckGrade*5))
        {
            return true;
        }

        return false;
    }

    #endregion
}
