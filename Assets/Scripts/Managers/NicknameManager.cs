using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using System;

//�߰� ����
public class NicknameManager
{
    /// <summary>
    /// Īȣ�� ó�� ������ �� ���Ǵ� �Լ�
    /// </summary>
    public void OpenBaseNickName()
    {
        var OwnedCheckList = Managers.Data.PersistentUser.OwnedNickNameBoolList;
        var NickNameList = DataParser.Inst.NickNameList;
        while (OwnedCheckList.Count < NickNameList.Count)
        {
            OwnedCheckList.Add(false);
        }
        OpenNicknameWithoutAlarm(0);
        OpenNicknameWithoutAlarm(21);
    }
    public void OpenNickname(int n)
    {
        if(Managers.Data.PersistentUser.OwnedNickNameBoolList[n] == false)
        {
            Managers.Data.PersistentUser.OwnedNickNameBoolList[n] = true;
            NickName temp = DataParser.Inst.NickNameList[n];
            Alarm.ShowAlarm($"Īȣ {temp.NicknameString}�� ȹ���Ͽ����ϴ�.");
        }
    }

    public void OpenNicknameWithoutAlarm(int n)
    {
        if (Managers.Data.PersistentUser.OwnedNickNameBoolList[n] == false)
        {
            Managers.Data.PersistentUser.OwnedNickNameBoolList[n] = true;
            NickName temp = DataParser.Inst.NickNameList[n];
        }
    }

    public void OpenNickname(NickNameKor kor)
    {
        OpenNickname((int)kor);
    }


    public Action UnlockNicknameIfConditionsMetAction;

    public void UnlockNicknameIfConditionsMet()
    {
        UnlockNicknameIfConditionsMetAction?.Invoke();
    }
}
