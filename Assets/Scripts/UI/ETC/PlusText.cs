using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using static Define;

public class PlusText : MonoSingleton<PlusText>
{
    TMP_Text text;
    RectTransform rect;
    public float MoveTime;
    public float Offset;
    Vector2 InitPoz;
    
    void Start()
    {
        text = GetComponent<TMP_Text>();
        rect = GetComponent<RectTransform>();
        InitPoz = rect.anchoredPosition;
    }

    public void PlayAnimation(StatName statName, int value)
    {
        text.text = "+" + value.ToString();
        int intValue = (int)statName;
        Vector2 offset = Vector2.zero;

        if (intValue >= 0 && intValue <= 2)
        {
            offset.y = -39f * intValue ;
        }
        else if (intValue >= 3 && intValue <= 5)
        {
            offset.x = 152f;
            offset.y = -39f * (intValue - 3);
        }
        rect.anchoredPosition = InitPoz;
        rect.anchoredPosition += offset;
       
        StartCoroutine(AnimationCoroutine());
    }

    private IEnumerator AnimationCoroutine()
    {
        // �ʱ� ��ġ �� ���İ� ����
        Vector2 initialPosition = rect.anchoredPosition;
        float initialAlpha = 1f;

        // ��ǥ ��ġ �� ���İ� ����
        Vector2 targetPosition = initialPosition + new Vector2(0f, Offset);
        float targetAlpha = 0f;

        // �ð� ���� �ʱ�ȭ
        float elapsedTime = 0f;

        // �ִϸ��̼� ����
        while (elapsedTime < MoveTime)
        {
            // �ð� ����� ���� ������ ���
            float t = elapsedTime / MoveTime;

            // ��ġ �̵�
            rect.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, t);

            // ���İ� ����
            text.alpha = Mathf.Lerp(initialAlpha, targetAlpha, t);

            // �ð� ������Ʈ
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // �ִϸ��̼� ���� �� �ʱ� ���·� �ǵ�����
        rect.anchoredPosition = initialPosition;
    }
}