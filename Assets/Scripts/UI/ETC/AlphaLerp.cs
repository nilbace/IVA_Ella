using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaLerp : MonoBehaviour
{
    public float minAlpha = 1/255f; // �ּ� ���İ�
    public float maxAlpha = 50f/255f; // �ִ� ���İ�
    public float lerpSpeed = 1f; // lerp �ӵ�

    private float currentAlpha; // ���� ���İ�
    private bool isIncreasing = true; // ���İ��� ���� ������ ����
    Image _image;

    private void Start()
    {
        // �ʱ� ���İ� ����
        currentAlpha = minAlpha;
        _image = GetComponent<Image>();

        // ���İ��� lerp�ϴ� �޼��� ȣ��
        StartAlphaLerp();
    }

    private void StartAlphaLerp()
    {
        // ���İ��� lerp�Ͽ� �����ϴ� �޼���
        currentAlpha = minAlpha;
        isIncreasing = true;
        LerpAlpha();
    }

    private void LerpAlpha()
    {
        // ���İ��� lerp�Ͽ� ����
        float t = Mathf.PingPong(Time.time * lerpSpeed, 1f);
        currentAlpha = Mathf.Lerp(minAlpha, maxAlpha, t);

        // ���İ��� �ִ밪�� ������ ���
        if (currentAlpha >= maxAlpha)
        {
            // ���İ��� �ִ밪���� �����ϰ� �����ϴ� �������� ����
            currentAlpha = maxAlpha;
            isIncreasing = false;
        }
        // ���İ��� �ּҰ��� ������ ���
        else if (currentAlpha <= minAlpha)
        {
            // ���İ��� �ּҰ����� �����ϰ� �����ϴ� �������� ����
            currentAlpha = minAlpha;
            isIncreasing = true;
        }

        // �̹��� ������Ʈ�� ���İ� ����
        _image.color = new Color(1f, 1f, 1f, currentAlpha);
    }

    private void Update()
    {
        // ���İ��� lerp�ϴ� �޼��� ȣ��
        LerpAlpha();
    }
}
