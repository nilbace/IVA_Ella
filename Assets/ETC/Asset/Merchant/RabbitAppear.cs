using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    private Vector2 initialPosition;  // �ʱ� ��ġ

    private void Start()
    {
        // �ʱ� ��ġ ����
        initialPosition = targetRectTransform.anchoredPosition;

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
}
