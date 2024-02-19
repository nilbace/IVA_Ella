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
        OpenNicknameWithoutAlarm(0);
        OpenNicknameWithoutAlarm(21);
    }

    /// <summary>
    /// � Īȣ�� �ر��ߴ��� �˶��� ������
    /// </summary>
    /// <param name="n"></param>
    public void OpenNickname(int n)
    {
        if(Managers.Data.PersistentUser.OwnedNickNameBoolList[n] == false)
        {
            Managers.Data.PersistentUser.OwnedNickNameBoolList[n] = true;
            NickName temp = DataParser.Inst.NickNameList[n];
            Alarm.ShowAlarm($"Īȣ '{temp.NicknameString}'�� ȹ���Ͽ����ϴ�.");
        }
    }

    public void CheckPerfectNickName()
    {
        if (Managers.Data.PersistentUser.WatchedEndingName.Count == (int)EndingName.MaxCount &&
            Managers.Data.PersistentUser.WatchedRandEvent.Count == 18&&
            Managers.Data.PersistentUser.WatchedBroadCast.Count == (int)BroadCastType.MaxCount_Name)
            OpenNickname(NickNameKor.�Ϻ�������);
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
}
