using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEffect : MonoBehaviour
{
    public GameObject StarPrefab;
    public int poolSize = 10; // ������Ʈ Ǯ ũ��
    private GameObject[] starPool; // ������Ʈ Ǯ �迭
    private int currentPoolIndex = 0; // ���� ��� ���� ������Ʈ �ε���
    float spawnTime;
    public float defaultTime = 0.05f;

    public GameObject CirclePrefab;
    public int pool2Size = 4;
    GameObject[] circlePool;
    int currentPool2Index = 0;

    void Start()
    {
        // ������Ʈ Ǯ �ʱ�ȭ
        starPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            starPool[i] = Instantiate(StarPrefab, transform);
            starPool[i].SetActive(false);
        }
        circlePool = new GameObject[pool2Size];
        for (int i = 0; i < pool2Size; i++)
        {
            circlePool[i] = Instantiate(CirclePrefab, transform);
            circlePool[i].SetActive(false);
        }
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            TouchCreate();
        }
        if ((Input.GetMouseButton(0) || Input.touchCount > 0) && spawnTime >= defaultTime)
        {
            StarCreate();
            spawnTime = 0;
        }
        spawnTime += Time.deltaTime;
    }

    void StarCreate()
    {
        // ������Ʈ Ǯ���� ��� ������ ������Ʈ ã��
        GameObject star = starPool[currentPoolIndex];
        star.SetActive(true);

        // ���� ��ġ ����
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        star.transform.position = mousePos;
        star.GetComponentInChildren<StarParticle>().Start();

        // ������Ʈ �ε��� ������Ʈ
        currentPoolIndex = (currentPoolIndex + 1) % poolSize;
    }

    void TouchCreate()
    {
        // ������Ʈ Ǯ���� ��� ������ ������Ʈ ã��
        GameObject star = circlePool[currentPool2Index];
        star.SetActive(true);

        // ���� ��ġ ����
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        star.transform.position = mousePos;
        star.GetComponentInChildren<CircleParticle>().Start();

        // ������Ʈ �ε��� ������Ʈ
        currentPool2Index = (currentPool2Index + 1) % pool2Size;
    }
}
