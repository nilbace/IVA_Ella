using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static Define;
public class ChattingManager : MonoSingleton<ChattingManager>
{
    const string Message_NameURL = "https://docs.google.com/spreadsheets/d/1WjIWPgya-w_QcNe6pWE_iug0bsF6uwTFDRY8j2MkO3o/export?format=tsv&gid=0&range=A2:J";

    Dictionary<BroadCastType, List<string>> ChatMessage_NameDic = new Dictionary<BroadCastType, List<string>>();
    List<GameObject> ChatGOs = new List<GameObject>();

    //���� �ؽ�Ʈ�� ���� ������ �˱� ����� width, height�� �̸� üũ�� GameObject
    public GameObject TransparentChatBox;

    [HideInInspector] public float ChatBubbleRiseDuration;
    [HideInInspector] public float MaxChatDelayTime;
    [HideInInspector] public float SpaceBetweenChats;
    [HideInInspector] public float TimeForChatGetBigger;
    const float ChatBoxYPos = -88f;
    const float ChatBoxXPos = -121f;
        
    void Start()
    {
        for (int i = 0; i < (int)BroadCastType.MaxCount_Name + 1; i++)
        {
            ChatMessage_NameDic[(BroadCastType)i] = new List<string>();
        }

        foreach (Transform childTransform in transform)
        {
            GameObject childObject = childTransform.gameObject;
            ChatGOs.Add(childObject);
        }
        StartCoroutine(RequestListDatasFromSheet());

        ScheduleExecuter.Inst.SetAniSpeedAction -= SetChatboxAniSpeed;
        ScheduleExecuter.Inst.SetAniSpeedAction += SetChatboxAniSpeed;
    }

    void SetChatboxAniSpeed(int speed)
    {
        MaxChatDelayTime = 0.1f / (float)speed;
        TimeForChatGetBigger = 0.08f / (float)speed;
        ChatBubbleRiseDuration = 0.3f / (float)speed;
    }

    //Ȱ��ȭ, ��Ȱ��ȭ�� �����Ǹ� Ȱ��ȭ �� ���� ä��â�� ���� �Ⱥ��̰� ó���ϱ� ����
    public void OnEnable()
    {
        foreach(GameObject ChatGO in ChatGOs)
        {
            ChatGO.SetActive(false);
        }
    }

    #region Temp
    IEnumerator RequestListDatasFromSheet()
    {
        Coroutine chatCoroutine = StartCoroutine(RequestAndSetDatas(Message_NameURL));

        yield return chatCoroutine;
        gameObject.SetActive(false);
    }

    IEnumerator RequestAndSetDatas(string www)
    {
        UnityWebRequest wwww = UnityWebRequest.Get(www);
        yield return wwww.SendWebRequest();

        string data = wwww.downloadHandler.text;
        string[] lines = data.Substring(0, data.Length).Split('\n');

        foreach (string datas in lines)
        {
            SetDataToDictionary(datas);
        }

        for (int i = 0; i < (int)BroadCastType.MaxCount_Name; i++)
        {
            ChatMessage_NameDic[(BroadCastType)i] = AutoLineBreak(ChatMessage_NameDic[(BroadCastType)i]);
        }
    }
    #endregion

    void SetDataToDictionary(string datas)
    {
        string[] EachData = datas.Substring(0, datas.Length).Split('\t');
        for (int i = 0; i < (int)BroadCastType.MaxCount_Name + 1; i++)
        {
            if (EachData[i] != "" && EachData[i] != "\r")
            {
                ChatMessage_NameDic[(BroadCastType)i].Add(EachData[i]);
            }
        }
    }

    //�ڵ� �ٹٲ�
    public List<string> AutoLineBreak(List<string> lines)
    {
        List<string> result = new List<string>();

        foreach (string line in lines)
        {
            string templine = line;
            int count = 0;
            string modifiedLine = "";

            foreach (char c in templine)
            {
                modifiedLine += c;
                count++;

                if (count >= 9 && c == ' ')
                {
                    modifiedLine = modifiedLine.TrimEnd();
                    modifiedLine += "\n";
                    count = 0;
                }
                else if (count > 11)
                {
                    modifiedLine += "\n";
                    count = 0;
                }
            }

            if (modifiedLine.EndsWith("\n"))
            {
                modifiedLine = modifiedLine.Substring(0, modifiedLine.Length - 1);
            }
            result.Add(modifiedLine);
        }
        return result;
    }

    public void StartGenerateChattingByType(BroadCastType broadCastType)
    {
        StartCoroutine(StartGenerateChatting(ChatMessage_NameDic[broadCastType]));
    }

    IEnumerator StartGenerateChatting(List<string> messageList)
    {
        int index = 0;

        while (true)
        {
            // ���� â ���� ���� (������ ������)
            string tempMessage = GetRandomStringFromList(messageList);
            TransparentChatBox.GetComponent<TMPro.TMP_Text>().text = tempMessage;
            yield return new WaitForEndOfFrame();
            float newYOffset = TransparentChatBox.GetComponent<RectTransform>().sizeDelta.y + SpaceBetweenChats;

            // �� ���� ä�� ��Ȱ��ȭ
            ChatGOs[(index + ChatGOs.Count + 1) % ChatGOs.Count].SetActive(false);

            // ������ ä���� ���� �̵�
            foreach (GameObject go in ChatGOs)
            {
                if (go.activeSelf)
                {
                    var rectTransform = go.GetComponent<RectTransform>();
                    var targetPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + newYOffset);
                    rectTransform.DOAnchorPos(targetPosition, ChatBubbleRiseDuration);
                }
            }
            yield return new WaitForSeconds(ChatBubbleRiseDuration);

            // ���ο� �޽��� ����
            yield return StartCoroutine(MakeRandomChat(index, tempMessage));

            index++;
            if (index == ChatGOs.Count)
                index = 0;

            float temp = Random.Range(0, MaxChatDelayTime);
            yield return new WaitForSeconds(temp);
        }
    }


    IEnumerator MakeRandomChat(int nowBoxIndex, string message)
    {
        GameObject Go = ChatGOs[nowBoxIndex];
        TMPro.TMP_Text nameText = Go.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>();
        TMPro.TMP_Text chatText = Go.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>();
        float yPoz = TransparentChatBox.transform.GetComponent<RectTransform>().sizeDelta.y / 2f + ChatBoxYPos;

        //Ȱ��ȭ �� ũ��� ������ �� ����
        Go.SetActive(true);
        Go.transform.localScale = Vector3.zero;
        Go.GetComponent<RectTransform>().anchoredPosition = new Vector3(ChatBoxXPos, yPoz, 0);

        // �̸��� ä�� ���� ����
        nameText.text = GetRandomStringFromList(ChatMessage_NameDic[BroadCastType.MaxCount_Name]);
        chatText.text = message;

        // ClearChatGO�� �ؽ�Ʈ ����
        TransparentChatBox.GetComponent<TMPro.TMP_Text>().text = chatText.text;

        //�ٽ� Ŀ���� �ִϸ��̼�
        var tween = Go.transform.DOScale(Vector3.one, TimeForChatGetBigger);
        yield return tween.WaitForCompletion();
    }

    string GetRandomStringFromList(List<string> list)
    {
        int rand = Random.Range(0, list.Count);
        return list[rand];
        
    }
}
