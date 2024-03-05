using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class TitleIMG : UI_Popup, IPointerClickHandler
{
    public Image Title;
    public Image TouchToStart;

    public float duration = 1f; // Lerp�� �ɸ��� �ð�
    private float startAlpha = 0.3f; // ���� ���� ��
    private float targetAlpha = 1f; // ��ǥ ���� ��.

    private void Start()
    {
        base.Init();
        StartCoroutine(LerpAlphaRepeat());
        StartCoroutine(LerpAlpha());
        Title.rectTransform.DOAnchorPosY(Title.rectTransform.anchoredPosition.y + 30f, duration);
    }

    private IEnumerator LerpAlphaRepeat()
    {
        float elapsedTime = 0f;
        Color startColor = TouchToStart.color;
        float startAlpha = 0.3f; // ���� ���� ��
        float targetAlpha = 1f; // ��ǥ ���� ��
        bool increasing = true; // ���� ���� ���� ������ ���θ� ��Ÿ���� �÷���

        while (true)
        {
            if (increasing)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= duration)
                {
                    elapsedTime = duration;
                    increasing = false;
                }
            }
            else
            {
                elapsedTime -= Time.deltaTime;
                if (elapsedTime <= 0f)
                {
                    elapsedTime = 0f;
                    increasing = true;
                }
            }

            float t = elapsedTime / duration;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            Color newColor = startColor;
            newColor.a = currentAlpha;

            TouchToStart.color = newColor;

            yield return null;
        }
    }

    private IEnumerator LerpAlpha()
    {
        float elapsedTime = 0f;
        Color startColor = Title.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            Color newColor = startColor;
            newColor.a = currentAlpha;

            Title.color = newColor;

            yield return null;
        }

        // ���� ���� ���� ��ǥġ�� ��Ȯ�� �����ϵ��� ����
        Color finalColor = startColor;
        finalColor.a = targetAlpha;
        Title.color = finalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UI_MainBackUI.instance.SetScreenAniSpeed(1);
        UI_MainBackUI.instance.StartScreenAnimation("WaitingArea");
        Managers.instance.CloseTitle();
    }
}