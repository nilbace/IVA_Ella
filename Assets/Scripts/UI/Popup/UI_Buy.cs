using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Buy : UI_Popup
{
    Item item;
    enum Buttons
    {
        YesBTN, NoBTN
    }
    enum Texts
    {
        EventText,
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
        item = MerChantItem.BuyUIItem;

        GetButton(0).onClick.AddListener(BuyItem);
        GetButton(1).onClick.AddListener(CloseBTN);
        GetText(0).text = item.ItemInfoText;

        void BuyItem()
        {
            Managers.Data.PlayerData.nowGoldAmount -= item.Cost;

            for (int i = 0; i < 6; i++)
            {
                Managers.Data.PlayerData.ChangeStat((StatName)i, item.SixStats[i]);
            }
            Managers.Data.PlayerData.RubiaKarma += item.Karma;

            if (item.ItemName == "ȣ����罺 ��ȯ��") Managers.NickName.OpenNickname(NickNameKor.�渶����);
            if (item.ItemName == "������ ����") Managers.NickName.OpenNickname(NickNameKor.����ȣ);

            UI_MainBackUI.instance.UpdateUItextsAndCheckNickname();
            Managers.Sound.Play(Sound.Buy);
            Managers.UI_Manager.ClosePopupUI();
            Managers.Data.PlayerData.BoughtItems.Add(item.ItemName);
            UI_Merchant.instance.UpdateTexts();
        }
    }

}
