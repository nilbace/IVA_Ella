using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using System;

//�߰� ����
public class NicknameManager
{
    void OpenNickname(int n)
    {
        Managers.Data.PersistentUser.OwnedNickNameBoolList[n] = true;
        Managers.Data.SaveData();
    }

    public Action UnlockNicknameIfConditionsMetAction;

    public void UnlockNicknameIfConditionsMet()
    {
        UnlockNicknameIfConditionsMetAction?.Invoke();
    }
}
