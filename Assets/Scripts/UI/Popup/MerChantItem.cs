using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class MerChantItem : MonoBehaviour
{
    public TMPro.TMP_Text NameTmp;
    public TMPro.TMP_Text InfoTmp;
    public Item _thisItem;

    public void Setting(Item item)
    {
        _thisItem = item;

        if(_thisItem.Cost <= Managers.Data._myPlayerData.nowGoldAmount && !IsBought(_thisItem))
        {
            NameTmp.text = _thisItem.ItemName + "     /" + _thisItem.Cost;
            InfoTmp.text = "���� : " + _thisItem.SixStats[0].ToString() + " /";
            InfoTmp.text += "�뷡 : " + _thisItem.SixStats[1].ToString() + " /";
            InfoTmp.text += "��ê : " + _thisItem.SixStats[2].ToString() + "\n";
            InfoTmp.text += "�ٷ� : " + _thisItem.SixStats[3].ToString() + " /";
            InfoTmp.text += "��Ż : " + _thisItem.SixStats[4].ToString() + " /";
            InfoTmp.text += "��� : " + _thisItem.SixStats[5].ToString();
            GetComponent<Button>().interactable = true;
        }
        else if(IsBought(_thisItem))
        {
            NameTmp.text = _thisItem.ItemName + "���� �Ϸ�";
            InfoTmp.text = "";
            GetComponent<Button>().interactable = false;
        }
        else
        {
            NameTmp.text = _thisItem.ItemName + "���� �Ұ�";
            InfoTmp.text = "";
            GetComponent<Button>().interactable = false;
        }

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(Buy);
    }

    bool IsBought(Item item)
    {
        foreach(string temp in Managers.Data._myPlayerData.BoughtItems)
        {
            if (temp == item.ItemName) return true;
        }
        return false;
    }

    void Buy()
    {
        Managers.Data._myPlayerData.nowGoldAmount -= _thisItem.Cost;

        for(int i = 0;i<6;i++)
        {
            Managers.Data._myPlayerData.SixStat[i] += _thisItem.SixStats[i];
        }

        UI_MainBackUI.instance.UpdateUItexts();
        Managers.Data._myPlayerData.BoughtItems.Add(_thisItem.ItemName);
        Managers.Data.SaveData();
        UI_Merchant.instance.UpdateTexts();
    }
    
}
