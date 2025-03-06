using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("모든 층 배열")]
    [SerializeField] private GameObject[] floors;

    [Header("플레이어 위치")]
    [SerializeField] private Transform player;

    [Header("점수 텍스트")]
    [SerializeField] private Text text;

    [Header("순간이동 층")]
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
