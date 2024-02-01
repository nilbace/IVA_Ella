using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Define;


public class UI_Tutorial : UI_Popup, IPointerClickHandler
{
    public static UI_Tutorial instance;
    public Sprite[] BubbleIMGs;
    public Sprite[] CharIMGs;
    TMPro.TMP_Text dialogueText;
    public List<Dialogue> dialogues;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    public float TypingDelay;
    private WaitForSeconds typingDelay;
    private int currentDialogueIndex = 0;
    Button NowSelctedBTN;


    bool isEnd;

    enum Texts { sentenceTMP, Option1TMP, Option2TMP }
    enum Images
    {
        LeftIMG,
        RightIMG,
        FocusIMG,
        BlackIMG,
        ChatBubbleIMG
    }

    enum Buttons
    {
        Option1BTN,
        Option2BTN,
    }
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Init();
    }


    public override void Init()
    {
        base.Init();
        Bind<TMPro.TMP_Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        typingDelay = new WaitForSeconds(TypingDelay);


        GetImage((int)Images.LeftIMG).gameObject.SetActive(false);
        GetImage((int)Images.RightIMG).gameObject.SetActive(false);
        dialogueText = GetText((int)Texts.sentenceTMP);
    }

    void Update()
    {
        //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
        //{
        //    // ȭ���� ��ġ�ϸ� ���� ���� �Ѿ��
        //    if (isTyping)
        //    {
        //        // ���� ��� ���̶�� ��� ��� �� ���� �����ֱ�
        //        StopCoroutine(typingCoroutine);
        //        dialogueText.text = dialogues[currentDialogueIndex].sentence;
        //        isTyping = false;
        //    }
        //    //��簡 ���� ���¿���
        //    else
        //    {
        //        //Focus����Ʈ�� �ִٸ�
        //        if (dialogues[currentDialogueIndex].tutorialFocus != TutorialFocusPoint.MaxCount)
        //        {
        //            //�Ѿ�°� OnPointerClick�� ����
        //            return;
        //        }
        //        //Focus����Ʈ�� ���ٸ� ���� ���� �Ѿ
        //        else
        //        {
        //            NextDialogue();
        //        }
        //    }
        //}
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isTyping)
        {
            // ���� ��� ���̶�� ��� ��� �� ���� �����ֱ�
            StopCoroutine(typingCoroutine);
            dialogueText.text = dialogues[currentDialogueIndex].sentence;
            isTyping = false;
        }
        else
        {
            if (dialogues[currentDialogueIndex].tutorialFocus != TutorialFocusPoint.MaxCount)
            {
                // ��ġ�� GameObject�� img���� Ȯ��
                if (eventData.pointerCurrentRaycast.gameObject == GetImage((int)Images.BlackIMG).gameObject)
                {
                    if (NowSelctedBTN != null)
                    {
                        Debug.Log("��ư��");
                        NowSelctedBTN.onClick.Invoke();
                    }
                    NextDialogue();
                }
            }
            //Focus����Ʈ�� ���ٸ� ���� ���� �Ѿ
            else
            {
                NextDialogue();
            }

            
        }

        //����

        //// ��ġ�� GameObject�� img���� Ȯ��
        //if (eventData.pointerCurrentRaycast.gameObject == GetImage((int)Images.BlackIMG).gameObject)
        //{
        //    if (NowSelctedBTN != null)
        //    {
        //        NowSelctedBTN.onClick.Invoke();
        //    }
        //    NextDialogue();
        //}
    }

    public void StartDiagloue(List<Dialogue> DiaList)
    {
        dialogues = DiaList;
        ShowDialogue(dialogues[0]);
    }

    void ShowDialogue(Dialogue dialogue)
    {
        ChooseBubbleIMG(dialogue);
        ShowImage(dialogue);
        SetFocusImg(dialogue);
        // ��� ����� ���� �ڷ�ƾ ����
        typingCoroutine = StartCoroutine(TypeSentence(dialogue));
    }

    void NextDialogue()
    {
        if (!isEnd)
        {
            // ���� ��� �ε����� �̵�
            currentDialogueIndex++;

            // ��簡 ��� ������ ��� ��ȭ ����
            if (currentDialogueIndex >= dialogues.Count)
            {
                EndDialogue();
                return;
            }

            // ���� ��� ���
            ShowDialogue(dialogues[currentDialogueIndex]);
        }
    }

    void EndDialogue()
    {
        Managers.UI_Manager.ClosePopupUI();
    }

    IEnumerator TypeSentence(Dialogue dialogue)
    {
        

        // ���� ��� ������ ǥ��
        isTyping = true;

        // ��� �ʱ�ȭ
        dialogueText.text = "";

        int chatIndex = 0;

        foreach (char letter in dialogue.sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return typingDelay;

            Managers.Sound.Play(Define.Sound.Chat1);
            chatIndex++;
        }

        // ���� ��� �Ϸ� �� ��� ���� ������Ʈ
        isTyping = false;
    }


    void ChooseBubbleIMG(Dialogue dialogue)
    {
        if (dialogue.name == "����")
        {
            if (dialogue.isLeft) GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[0];
            else GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[1];
        }
        else if (dialogue.name == "����")
        {
            if (dialogue.isLeft) GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[2];
            else GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[3];
        }
        else if (dialogue.name == "����")
        {
            if (dialogue.isLeft) GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[4];
            else GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[5];
        }
        else
        {
            GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[6];
        }
    }

    void ShowImage(Dialogue dialogue)
    {
        if (dialogue.name == "����")
        {
            TurnOnImage(dialogue.isLeft, CharIMGs[0]);
        }
        else if (dialogue.name == "����")
        {
            TurnOnImage(dialogue.isLeft, CharIMGs[1]);
        }
        else
        {
            TurnOnImage(dialogue.isLeft, CharIMGs[2]);
        }
    }

    void SetFocusImg(Dialogue dialogue)
    {
        
        switch (dialogue.tutorialFocus)
        {
            case TutorialFocusPoint.StartSchedule:
                SetFocusImg("CreateScheduleBTN");
                break;
            case TutorialFocusPoint.Screen:
                SetFocusImg("ScreenIMG");
                break;
            case TutorialFocusPoint.BroadcastBTN:
                break;
            case TutorialFocusPoint.RestBTN:
                break;
            case TutorialFocusPoint.GoOutBTN:
                break;
            case TutorialFocusPoint.Healing:
                break;
            case TutorialFocusPoint.LOL:
                break;
            case TutorialFocusPoint.Sketch:
                break;
            case TutorialFocusPoint.BC_Draw:
                break;
            case TutorialFocusPoint.MaxCount:
                FocusImgDisappear();
                break;
        }
    }

    GameObject SetFocusImg(string Objectname)
    {
        var FocusImg = GetImage((int)Images.FocusIMG);
        GameObject go = GameObject.Find(Objectname);
        FocusImg.sprite = go.GetComponent<Image>().sprite;
        FocusImg.GetComponent<RectTransform>().sizeDelta = go.GetComponent<RectTransform>().sizeDelta;
        FocusImg.GetComponent<RectTransform>().anchoredPosition = go.GetComponent<RectTransform>().anchoredPosition;

        Button button = go.GetComponent<Button>();

        // Button ������Ʈ�� �ִ��� Ȯ��
        if (button != null)
        {
            NowSelctedBTN = button;
            Debug.Log("��ư �ִ�");
        }
        else
        {
            NowSelctedBTN = button;
            Debug.Log("��ư ����");
        }

        return go;
    }

    void FocusImgDisappear()
    {
        var FocusImg = GetImage((int)Images.FocusIMG);
        FocusImg.GetComponent<RectTransform>().sizeDelta = new Vector3(0, 0, 0);
    }

    void TurnOnImage(bool isLeft, Sprite sprite)
    {
        GetImage((int)Images.LeftIMG).gameObject.SetActive(isLeft);
        GetImage((int)Images.LeftIMG).sprite = isLeft ? sprite : null;
        GetImage((int)Images.LeftIMG).color = isLeft ? Color.white : Color.gray;

        GetImage((int)Images.RightIMG).gameObject.SetActive(!isLeft);
        GetImage((int)Images.RightIMG).sprite = !isLeft ? sprite : null;
        GetImage((int)Images.RightIMG).color = !isLeft ? Color.white : Color.gray;
    }

    
}