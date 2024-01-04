using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Communication : UI_Popup
{
    public Sprite[] BubbleIMGs;
    public Sprite[] CharIMGs;
    TMPro.TMP_Text dialogueText;
    public Dialogue[] dialogues;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    public float TypingDelay;
    private WaitForSeconds typingDelay;
    private int currentDialogueIndex = 0;
    
    enum Texts { sentenceTMP }
    enum Images 
    { LeftIMG, RightIMG,
        ChatBubbleIMG }

    private void Start()
    {
        Init();
        typingDelay = new WaitForSeconds(TypingDelay);
        ShowDialogue(dialogues[currentDialogueIndex]);

    }

    public override void Init()
    {
        base.Init();
        Bind<TMPro.TMP_Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

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

    void ShowDialogue(Dialogue dialogue)
    {
        // ��� ����� ���� �ڷ�ƾ ����
        typingCoroutine = StartCoroutine(TypeSentence(dialogue));
    }

    void NextDialogue()
    {
        // ���� ��� �ε����� �̵�
        currentDialogueIndex++;

        // ��簡 ��� ������ ��� ��ȭ ����
        if (currentDialogueIndex >= dialogues.Length)
        {
            EndDialogue();
            return;
        }

        // ���� ��� ���
        ShowDialogue(dialogues[currentDialogueIndex]);
    }

    void EndDialogue()
    {
        Debug.Log("HI");
        Managers.UI_Manager.CloseALlPopupUI();
    }

    IEnumerator TypeSentence(Dialogue dialogue)
    {
        ChooseBubbleIMG(dialogue);
        ShowImage(dialogue);

        // ���� ��� ������ ǥ��
        isTyping = true;

        // ��� �ʱ�ȭ
        dialogueText.text = "";

        // �� ���ھ� ���
        foreach (char letter in dialogue.sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return typingDelay; // �ۿ��� ������ WaitForSeconds ���� ����
        }

        // ���� ��� �Ϸ� �� ��� ���� ������Ʈ
        isTyping = false;
    }

    void ChooseBubbleIMG(Dialogue dialogue)
    {
        if(dialogue.name == "����")
        {
            if (dialogue.isLeft) GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[0];
            else GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[1];
        }
        else if(dialogue.name == "����")
        {
            if (dialogue.isLeft) GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[2];
            else GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[3];
        }
        else
        {
            if (dialogue.isLeft) GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[4];
            else GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[5];
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
        if (isLeft)
        {
            GetImage((int)Images.LeftIMG).gameObject.SetActive(true);
            GetImage((int)Images.LeftIMG).sprite = sprite;
            GetImage((int)Images.LeftIMG).color = Color.white;
            GetImage((int)Images.RightIMG).color = Color.gray;
        }
        else
        {
            GetImage((int)Images.RightIMG).gameObject.SetActive(true);
            GetImage((int)Images.RightIMG).sprite = sprite;
            GetImage((int)Images.LeftIMG).color = Color.gray;
            GetImage((int)Images.RightIMG).color = Color.white;
        }
    }
}