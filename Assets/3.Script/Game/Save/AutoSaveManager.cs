using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AutoSaveManager : MonoBehaviour
{
    // 저장 파일 경로 (Application.persistentDataPath는 플랫폼별로 적절한 경로를 제공합니다)
    private string saveFilePath;
    // 저장 데이터 최대 개수
    private int maxSaveCount = 5;

    // 플레이어 참조 (인스펙터에서 할당하거나, FindWithTag 등을 통해 찾기)
    [SerializeField] private GameObject player;

    // IsGrounded() 전환을 감지하기 위한 변수  
    private bool prevGrounded = false;

    private void Awake()
    {
        saveFilePath = Path.Combine(SaveSlotManager.Instance.currentSaveFolder, "autosave.json");
    }

    private void Update()
    {
        bool grounded = player.GetComponent<Player_Controller>().IsGrounded();
        // 또는 플레이어 컨트롤러 내부에서 이벤트를 발생시켜 저장하도록 구현할 수도 있습니다.

        // 전환 감지: 이전 프레임에 땅에 있지 않았는데 현재 땅에 닿은 경우
        if (!prevGrounded && grounded)
        {
            SaveGame();
        }
        prevGrounded = grounded;
    }

    public void SaveGame()
    {
        // 기존 저장 데이터를 로드
        SaveDataCollection collection = new SaveDataCollection();
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            collection = JsonUtility.FromJson<SaveDataCollection>(json);
            if (collection == null)
            {
                collection = new SaveDataCollection();
            }
        }

        // 새 게임 데이터를 생성 (여기서는 플레이어 위치와 점수 예시)
        GameData newData = new GameData();
        newData.playerPosition = player.transform.position;
        newData.isWind = player.GetComponent<Player_Controller>().isWind;

        // 새 데이터를 추가
        collection.saves.Add(newData);

        // 최대 개수를 넘으면 가장 오래된 데이터를 삭제
        if (collection.saves.Count > maxSaveCount)
        {
            collection.saves.RemoveAt(0);
        }

        // 다시 JSON 문자열로 변환하여 파일에 저장
        string outputJson = JsonUtility.ToJson(collection, true);
        File.WriteAllText(saveFilePath, outputJson);
        // Debug.Log("Game Saved. Total saves: " + collection.saves.Count);
    }

    public void LoadGame()
    {
        string saveFilePath = Path.Combine(SaveSlotManager.Instance.currentSaveFolder, "autosave.json");

        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveDataCollection collection = JsonUtility.FromJson<SaveDataCollection>(json);

            if (collection != null && collection.saves.Count > 0)
            {
                GameData latest = collection.saves[collection.saves.Count - 1];

                // 플레이어 위치 적용
                if (player != null)
                {
                    player.transform.position = latest.playerPosition;
                    player.GetComponent<Player_Controller>().isWind = latest.isWind;
                }
            }
        }
    }
}
