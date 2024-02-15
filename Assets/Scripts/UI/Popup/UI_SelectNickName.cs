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
    readonly NickName EmptyNickName = new NickName();

    enum Buttons
    {
        StartBTN
    }
    enum Texts
    {
        NameTMP,
        InfoTMP,
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
            SelectedPrefix = nick;
        }
        else
        {
            SelectedSuffix = nick;
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


        GetText((int)Texts.InfoTMP).text = "ȸ�� ���� ��\n";
        for (int i = 0; i < 3; i++)
        {
            if (ResultArray[i] != 0)
            {
                GetText((int)Texts.InfoTMP).text += $"{(StatNameKor)i} �Ƿ� {GetIconString(i)} +" + ResultArray[i] + "\n";
            }
        }
        for (int i = 3; i < 6; i++)
        {
            if (ResultArray[i] != 0)
            {
                GetText((int)Texts.InfoTMP).text += $"{(StatNameKor)i} {GetIconString(i)} +" + ResultArray[i] + "\n";
            }
        }
        if (ResultArray[6] != 0)
        {
            GetText((int)Texts.InfoTMP).text += $"��� {GetIconString(StatIcons.Gold)}+" + ResultArray[6] + "\n";
        }
        if (ResultArray[7] != 0)
        {
            GetText((int)Texts.InfoTMP).text += $"��� {GetIconString(StatIcons.Sub)}+" + ResultArray[7];
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
        UI_MainBackUI.instance.UpdateUItexts();
        Managers.UI_Manager.CloseALlPopupUI();
        Managers.Data.SaveData();
    }
}
