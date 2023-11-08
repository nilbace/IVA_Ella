using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;
using static Define;
public class ChattingManager : MonoBehaviour
{
    const string Message_NameURL = "https://docs.google.com/spreadsheets/d/1WjIWPgya-w_QcNe6pWE_iug0bsF6uwTFDRY8j2MkO3o/export?format=tsv&gid=0&range=A2:K";
    /// <summary>
    /// �������� �̸����� �������
    /// </summary>
    List<string>[] Message_NameListArray = new List<string>[(int)BroadCastType.MaxCount_Name + 1];

    public static ChattingManager instance;
    private void Awake()
    {
        instance = this;
        for(int i = 0; i<Message_NameListArray.Length; i++)
        {
            Message_NameListArray[i] = new List<string>();
        }
    }

    List<GameObject> ChatGOs = new List<GameObject>();
    public GameObject ClearChatGO;

    [SerializeField] float _chatScale;

    void Start()
    {
        //�迭�� ����ְ� ��Ȱ��ȭ
        int childCount = gameObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform childTransform = gameObject.transform.GetChild(i);
            GameObject childObject = childTransform.gameObject;
            ChatGOs.Add(childObject);
            childObject.SetActive(false);
        }
        StartCoroutine(RequestListDatasFromSheet());
    }



    IEnumerator RequestListDatasFromSheet()
    {
        //�񵿱� ������� �������� �����͸� �о��
        Coroutine chatCoroutine = StartCoroutine(RequestAndSetDatas(Message_NameURL));

        //�ڷ�ƾ�� ��� �Ϸ�� ������ ��ٸ�
        yield return chatCoroutine;

        //���� ���� �ڵ�
        StartGenerateChattingByType(BroadCastType.Healing);

    }

    IEnumerator RequestAndSetDatas(string www)
    {
        UnityWebRequest wwww = UnityWebRequest.Get(www);
        yield return wwww.SendWebRequest();

        string data = wwww.downloadHandler.text;
        string[] lines = data.Substring(0, data.Length).Split('\n');

        foreach (string datas in lines)
        {
            LocateDataToMessageListArray(datas);
        }

        for (int i = 0; i < (int)BroadCastType.MaxCount_Name; i++)
        {
            Message_NameListArray[i] = AutoLineBreak(Message_NameListArray[i]);
        }
    }

    /// <summary>
    /// �����͸� �ùٸ� List�� ����־��
    /// </summary>
    /// <param name="datas"></param>
    void LocateDataToMessageListArray(string datas)
    {
        string[] EachData = datas.Substring(0, datas.Length).Split('\t');
        for (int i = 0; i < (int)BroadCastType.MaxCount_Name + 1; i++)
        {
            if (EachData[i] != "")
            {
                Message_NameListArray[i].Add(EachData[i]);
            }
        }
    }



    /// <summary>
    /// ä�� �޽����� ��� ���� ���ִ� �Լ�
    /// </summary>
    /// <param name="lines"></param>
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


    //��� Ÿ���� �޾Ƽ� ��� ������
    public void StartGenerateChattingByType(BroadCastType broadCastType)
    {
        StartCoroutine(StartGenerateChatting(Message_NameListArray[(int)broadCastType]));
    }

    [Header("ä�� ������ �ð�")]
    [SerializeField] float minChatDelayTime;
    [SerializeField] float maxChatDelayTime;
    [Header("ä�� ���� ����")]
    [SerializeField] float SpaceBetweenChats;


    //ä�� ���� ����
    IEnumerator StartGenerateChatting(List<string> messagelist)
    {
        int index = 0;

        while (true)
        {
            //�� â ���� ����(������ ������)
            string tempMessage = GetRandomStringFromList(messagelist);
            ClearChatGO.GetComponent<TMPro.TMP_Text>().text = tempMessage;
            yield return new WaitForEndOfFrame();
            float newYoffset = ClearChatGO.GetComponent<RectTransform>().sizeDelta.y + SpaceBetweenChats;
            newYoffset *= _chatScale;

            //�� ������ ��
            ChatGOs[(index + ChatGOs.Count + 1) % ChatGOs.Count].SetActive(false);

            //������ ���� ���� �ø�
            foreach (GameObject go in ChatGOs)
            {
                if (go.activeSelf)
                {
                    var rectTransform = go.GetComponent<RectTransform>();
                    var targetPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + newYoffset);
                    rectTransform.DOAnchorPos(targetPosition, 0.3f);
                }
            }
            yield return new WaitForSeconds(0.3f);

            //�� �޽��� �ؿ� ����
            yield return StartCoroutine(MakeRandomChat(index, tempMessage));

            index++;
            if (index == ChatGOs.Count) index = 0;

            float temp = Random.Range(minChatDelayTime, maxChatDelayTime);
            yield return new WaitForSeconds(temp);
        }
    }

    [Header("ä���� Ŀ���� �ð�")]
    [SerializeField] float TimeForChatGetBigger;

    [Header("ä�� ������ǥ")]
    [SerializeField] float ChatBubbleYPos;
    [SerializeField] float ChatBubbleXPos;

    /// <summary>
    /// ���� ä�� �����
    /// </summary>
    /// <param name="index"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// 
    IEnumerator MakeRandomChat(int index, string message)
    {
        Vector3 targetScale = Vector3.one * _chatScale;
        GameObject Go = ChatGOs[index];

        Go.SetActive(true);
        Go.transform.localScale = Vector3.zero;
        Go.GetComponent<RectTransform>().anchoredPosition = new Vector3(ChatBubbleXPos, ClearChatGO.transform.GetComponent<RectTransform>().sizeDelta.y/2f* _chatScale + ChatBubbleYPos, 0);

        Go.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = GetRandomStringFromList(Message_NameListArray[(int)BroadCastType.MaxCount_Name]);
        Go.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = message;
        ClearChatGO.GetComponent<TMPro.TMP_Text>().text = Go.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text;

        var tween = Go.transform.DOScale(targetScale, TimeForChatGetBigger);
        yield return tween.WaitForCompletion();
    }

    string GetRandomStringFromList(List<string> list)
    {
        int rand = Random.Range(0, list.Count);
        return list[rand];
    }
}