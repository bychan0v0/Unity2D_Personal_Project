using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("모든 층 배열")]
    [SerializeField] private GameObject[] floors;

    [Header("플레이어 위치")]
    [SerializeField] private Transform player;

    private float floorHeight = 10f;

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
    }
}
