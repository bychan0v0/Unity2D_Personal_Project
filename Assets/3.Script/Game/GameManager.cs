using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("��� �� �迭")]
    [SerializeField] private GameObject[] floors;

    [Header("�÷��̾� ��ġ")]
    [SerializeField] private Transform player;

    [Header("���� �ؽ�Ʈ")]
    [SerializeField] private Text text;

    [Header("�����̵� ��")]
    [SerializeField] private int floor;

    private float floorHeight = 10f;
    private float score = 0f;

    private void Update()
    {
        int currentFloor = Mathf.FloorToInt(player.position.y / floorHeight);

        for (int i = 0; i < floors.Length; i++)
        {
            if (i == currentFloor || i == currentFloor - 1 || i == currentFloor + 1)
            {
                floors[i].SetActive(true);
            }
            else
            {
                floors[i].SetActive(false);
            }
        }

        CheckScore();
    }

    private void CheckScore()
    {
        float maxY = 426.4f;

        score = player.position.y / maxY * 100f;
        text.text = score.ToString("F1") + " %";
    }
}
