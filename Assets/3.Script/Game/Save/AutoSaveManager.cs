using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AutoSaveManager : MonoBehaviour
{
    // ���� ���� ��� (Application.persistentDataPath�� �÷������� ������ ��θ� �����մϴ�)
    private string saveFilePath;
    // ���� ������ �ִ� ����
    private int maxSaveCount = 5;

    // �÷��̾� ���� (�ν����Ϳ��� �Ҵ��ϰų�, FindWithTag ���� ���� ã��)
    [SerializeField] private GameObject player;

    // IsGrounded() ��ȯ�� �����ϱ� ���� ����  
    private bool prevGrounded = false;

    private void Awake()
    {
        saveFilePath = Path.Combine(SaveSlotManager.Instance.currentSaveFolder, "autosave.json");
    }

    private void Update()
    {
        bool grounded = player.GetComponent<Player_Controller>().IsGrounded();
        // �Ǵ� �÷��̾� ��Ʈ�ѷ� ���ο��� �̺�Ʈ�� �߻����� �����ϵ��� ������ ���� �ֽ��ϴ�.

        // ��ȯ ����: ���� �����ӿ� ���� ���� �ʾҴµ� ���� ���� ���� ���
        if (!prevGrounded && grounded)
        {
            SaveGame();
        }
        prevGrounded = grounded;
    }

    public void SaveGame()
    {
        // ���� ���� �����͸� �ε�
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

        // �� ���� �����͸� ���� (���⼭�� �÷��̾� ��ġ�� ���� ����)
        GameData newData = new GameData();
        newData.playerPosition = player.transform.position;
        newData.isWind = player.GetComponent<Player_Controller>().isWind;

        // �� �����͸� �߰�
        collection.saves.Add(newData);

        // �ִ� ������ ������ ���� ������ �����͸� ����
        if (collection.saves.Count > maxSaveCount)
        {
            collection.saves.RemoveAt(0);
        }

        // �ٽ� JSON ���ڿ��� ��ȯ�Ͽ� ���Ͽ� ����
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

                // �÷��̾� ��ġ ����
                if (player != null)
                {
                    player.transform.position = latest.playerPosition;
                    player.GetComponent<Player_Controller>().isWind = latest.isWind;
                }
            }
        }
    }
}
