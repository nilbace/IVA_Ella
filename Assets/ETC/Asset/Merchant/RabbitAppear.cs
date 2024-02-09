using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class RabbitAppear : MonoBehaviour
{
    public RectTransform targetRectTransform;  // �̵���ų UI ������Ʈ�� RectTransform
    public float offset = 100f;  // �Ʒ��� �̵��� �Ÿ�
    public float moveTime = 1f;  // �̵� �ð�
    public float shakeStrength = 20f;  // ������ ����
    bool startShake;
    bool canMove = true;
    public float cooldown;
    float cooldownTimer;

    public Image ChatBubble;
    public TMPro.TMP_Text ChatTMP;
    public float duration;
    public string[] Dialogues;

    private Vector2 initialPosition;  // �ʱ� ��ġ

    private void Start()
    {
        // �ʱ� ��ġ ����
        initialPosition = targetRectTransform.anchoredPosition;
        ChatBubble.color = Define.alpha0;
        ChatTMP.color = new Color(0, 0, 0, 0);
        ChatTMP.text = Dialogues[Random.Range(0, 5)];

        // �Ʒ��� ��� �̵�
        targetRectTransform.anchoredPosition -= new Vector2(0f, offset);

        // �ö���� Tweener ����
        Tween moveUpTween = targetRectTransform.DOAnchorPosY(initialPosition.y, moveTime);
        startShake = true;
        // �ö���� Tweener�� �Ϸ� ������ ���� ����
        moveUpTween.OnComplete(() =>
        {
            startShake = false;
            targetRectTransform.DOAnchorPos(initialPosition, 0.1f);
            StartCoroutine(ShowChatBubble());
        });


    }
    private void Update()
    {
        if (canMove && startShake)
        {
            // ������ �̵��� ���
            float randomMovement = Random.Range(-shakeStrength, shakeStrength);
            targetRectTransform.anchoredPosition += new Vector2(randomMovement, 0f);

            // ��ٿ� ����
            canMove = false;
            cooldownTimer = cooldown;
        }
        else
        {
            // ��ٿ� ���̸� Ÿ�̸� ����
            cooldownTimer -= Time.deltaTime;

            // ��ٿ��� ������ �̵� ���� ���·� ����
            if (cooldownTimer <= 0f)
            {
                canMove = true;
            }
        }
    }

    IEnumerator ShowChatBubble()
    {
        float elapsedTime = 0f;
        Color startColor = ChatBubble.color;
        Color startColor2 = ChatTMP.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float currentAlpha = Mathf.Lerp(0, 1, t);

            Color newColor = startColor;
            newColor.a = currentAlpha;
            ChatBubble.color = newColor;

            Color newColor2 = startColor2;
            newColor2.a = currentAlpha;
            ChatTMP.color = newColor2;

            yield return null;
        }
    }
}
