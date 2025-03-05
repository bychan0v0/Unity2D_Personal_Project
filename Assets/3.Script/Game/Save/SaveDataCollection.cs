using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // 플레이어의 위치
    public Vector3 playerPosition;
}

[System.Serializable]
public class SaveDataCollection
{
    // 여러 저장 데이터를 저장할 리스트
    public List<GameData> saves = new List<GameData>();
}
