using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_SelectNickName : UI_Popup
{
    public static UI_SelectNickName instance;
    UI_NickSubContent[] Prefixs;
    UI_NickSubContent[] Suffixs;
    public NickName SelectedPrefix;
    public NickName SelectedSuffix;

    enum Buttons
    {
        StartBTN
    }
    enum Texts
    {
        NameTMP,
        InfoTMP,
        Info2TMP
    }

    enum Transforms
    {
        prefixParentTR,
        suffixParentTR,
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TMPro.TMP_Text>(typeof(Texts));
        Bind<RectTransform>(typeof(Transforms));

        Prefixs = Get<RectTransform>((int)Transforms.prefixParentTR).GetComponentsInChildren<UI_NickSubContent>();
        Suffixs = Get<RectTransform>((int)Transforms.suffixParentTR).GetComponentsInChildren<UI_NickSubContent>();
        CheckOwnedNickName();
        UpdateInfoText();

        GetButton((int)Buttons.StartBTN).onClick.AddListener(StartGame);
    }

    public void SetInfoTexts(NickName nick)
    {
        if(nick.NicknameType == NickNameType.prefix)
        {
            if(nick == SelectedPrefix)
            {
                SelectedPrefix = new NickName();
            }
            else
            {
                SelectedPrefix = nick;
            }
        }
        else
        {
            if (nick == SelectedSuffix)
            {
                SelectedSuffix = new NickName();
            }
            else
            {
                SelectedSuffix = nick;
            }
        }

        foreach(UI_NickSubContent nickName in Prefixs)
        {
            nickName.SetFrameImage();
        }
        foreach (UI_NickSubContent nickName in Suffixs)
        {
            nickName.SetFrameImage();
        }
        UpdateInfoText();
    }

    void UpdateInfoText()
    {
        var NickList = DataParser.Inst.NickNameList;
        if (SelectedPrefix == null) SelectedPrefix = NickList[0];
        if (SelectedSuffix == null) SelectedSuffix = NickList[21];

        //���̴� Īȣ ����
        GetText((int)Texts.NameTMP).text = SelectedPrefix.NicknameString + " " + SelectedSuffix.NicknameString;

        int[] ResultArray = new int[8];
        int[] prefixStats = SelectedPrefix.GetSixStat(); // SelectedPrefix�� 6�� ������ ������
        int[] suffixStats = SelectedSuffix.GetSixStat(); // SelectedSuffix�� 6�� ������ ������

        for (int i = 0; i < 6; i++)
        {
            ResultArray[i] = prefixStats[i] + suffixStats[i]; // �� ���� ������ ���ؼ� ResultArray�� ����
        }
        ResultArray[6] = SelectedPrefix.MoneyValue + SelectedSuffix.MoneyValue;
        ResultArray[7] = SelectedPrefix.SubCount + SelectedSuffix.SubCount;

        GetText((int)Texts.InfoTMP).text = "";
        GetText((int)Texts.Info2TMP).text = "";

        for (int i = 0; i < 3; i++)
        {
            if (prefixStats[i] != 0)
            {
                GetText((int)Texts.InfoTMP).text += $"{(StatNameKor)i} �Ƿ� {GetIconString(i)} +" + prefixStats[i] + "\n";
            }
            if (suffixStats[i] != 0)
            {
                GetText((int)Texts.Info2TMP).text += $"{(StatNameKor)i} �Ƿ� {GetIconString(i)} +" + suffixStats[i] + "\n";
            }
        }


        for (int i = 3; i < 6; i++)
        {
            if (prefixStats[i] != 0)
            {
                GetText((int)Texts.InfoTMP).text += $"{(StatNameKor)i} {GetIconString(i)} +" + prefixStats[i] + "\n";
            }
            if (suffixStats[i] != 0)
            {
                GetText((int)Texts.Info2TMP).text += $"{(StatNameKor)i} {GetIconString(i)} +" + suffixStats[i] + "\n";
            }
        }

        if (SelectedPrefix.MoneyValue != 0)
        {
            GetText((int)Texts.InfoTMP).text += $"�� {GetIconString(StatIcons.Gold)} +" + SelectedPrefix.MoneyValue + "\n";
        }
        if (SelectedSuffix.MoneyValue != 0)
        {
            GetText((int)Texts.Info2TMP).text += $"�� {GetIconString(StatIcons.Gold)} +" + SelectedSuffix.MoneyValue + "\n";
        }

        if (SelectedPrefix.SubCount != 0)
        {
            GetText((int)Texts.InfoTMP).text += $"������ {GetIconString(StatIcons.Sub)}+" + SelectedPrefix.SubCount;
        }
        if (SelectedSuffix.SubCount != 0)
        {
            GetText((int)Texts.Info2TMP).text += $"������ {GetIconString(StatIcons.Sub)}+" + SelectedSuffix.SubCount;
        }
    }

  

    public void CheckOwnedNickName()
    {
        var OwnedCheckList = Managers.Data.PersistentUser.OwnedNickNameBoolList;
        var NickNameList = DataParser.Inst.NickNameList;
        int prefixIndex = 0;
        int suffixIndex = 0;

        //���� �ʱ�ȭ �뵵
        Managers.NickName.OpenBaseNickName();

        //prefix��� suffix�鿡�� �ùٸ� ������ ä����
        for (int i = 0; i < NickNameList.Count; i++)
        {
            if (NickNameList[i].NicknameType == NickNameType.prefix)
            {
                if (prefixIndex < Prefixs.Length)
                {
                    Prefixs[prefixIndex].SetForSelectNickName(NickNameList[i], OwnedCheckList[i]);
                    prefixIndex++;
                }
            }
            else if (NickNameList[i].NicknameType == NickNameType.suffix)
            {
                if (suffixIndex < Suffixs.Length)
                {
                    Suffixs[suffixIndex].SetForSelectNickName(NickNameList[i], OwnedCheckList[i]);
                    suffixIndex++;
                }
            }
        }

        //������ ��Ȱ��ȭ
        for (int j = prefixIndex; j < Prefixs.Length; j++)
        {
            Prefixs[j].gameObject.SetActive(false);
        }

        for (int j = suffixIndex; j < Suffixs.Length; j++)
        {
            Suffixs[j].gameObject.SetActive(false);
        }
    }

    void StartGame()
    {
        //�⺻ ���� �� ������ �ִϸ��̼� ���� ��ġ �� ������Ʈ
        int[] prefixStats = SelectedPrefix.GetSixStat(); 
        int[] suffixStats = SelectedSuffix.GetSixStat(); 

        for (int i = 0; i < 6; i++)
        {
            Managers.Data.PlayerData.SixStat[i] += prefixStats[i] + suffixStats[i];
        }
        Managers.Data.PlayerData.nowGoldAmount += SelectedPrefix.MoneyValue + SelectedSuffix.MoneyValue;
        Managers.Data.PlayerData.nowSubCount += SelectedPrefix.SubCount + SelectedSuffix.SubCount;
        Managers.Data.PlayerData.NowNickName = SelectedPrefix.NicknameString + " " + SelectedSuffix.NicknameString;
        UI_MainBackUI.instance.UpdateUItextsAndCheckNickname();
        Managers.UI_Manager.CloseALlPopupUI();
        Managers.Data.SaveData();
    }
}
