using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

[System.Serializable]
public class FocusPoint {
    public Vector2 Poz;
    public Vector2 WidthHeight;
}


public class UI_Tutorial : UI_Popup
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


    bool isEnd;

    enum Texts { sentenceTMP, Option1TMP, Option2TMP }
    enum Images
    {
        LeftIMG,
        RightIMG,
        ChatBubbleIMG
    }

    enum Buttons
    {
        Option1BTN,
        Option2BTN,
    }

    private void Start()
    {
        Init();
    }


    public override void Init()
    {
        instance = this;
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
        // ȭ���� ��ġ�ϸ� ���� ���� �Ѿ��
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began
            || Input.GetMouseButtonDown(0))
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
                // ���� ���� �Ѿ��
                NextDialogue();
            }
        }
    }

    public void StartDiagloue(List<Dialogue> DiaList)
    {
        dialogues = DiaList;
        ShowDialogue(dialogues[0]);
    }

    void ShowDialogue(Dialogue dialogue)
    {
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
        ChooseBubbleIMG(dialogue);
        ShowImage(dialogue);

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