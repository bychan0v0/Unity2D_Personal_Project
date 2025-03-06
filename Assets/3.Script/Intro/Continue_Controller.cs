using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Continue_Controller : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Menu_Controller menuController;
    [SerializeField] private GameObject continuePanel;
    [SerializeField] private Transform buttonParent;       // ���� ���� ��ư���� ��ġ�� �θ� (��: Content ����)
    [SerializeField] private GameObject saveFolderButtonPrefab; // ���� ���� ��ư ������
    [SerializeField] private RectTransform cursor;         // ���õ� �׸� ���� ǥ���� Ŀ�� �̹���

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    [Header("Cursor Settings")]
    [SerializeField] private float cursorOffsetX = 95f; // Ŀ���� ��ư �������� �̵��� ������

    private List<Button> folderButtons = new List<Button>();
    private int currentIndex = 0;
    private bool inputEnabled = false;

    private void Start()
    {
        menuController = FindObjectOfType<Menu_Controller>();
    }

    private void OnEnable()
    {
        // �г��� Ȱ��ȭ�Ǹ� ���� ���� ��ư���� �������� �����մϴ�.
        PopulateFolderButtons();
        UpdateCursor();
        currentIndex = 0;
        UpdateSelection();
        inputEnabled = true;
    }

    private void PopulateFolderButtons()
    {
        // ���� ��ư���� ��� ����
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }
        folderButtons.Clear();

        string[] folders = SaveSlotManager.Instance.GetSaveFolders();
        if (folders.Length == 0)
        {
            continuePanel.SetActive(false);
            return;
        }

        // �ִ� 5���� �����մϴ�.
        int count = Mathf.Min(5, folders.Length);
        for (int i = 0; i < count; i++)
        {
            GameObject buttonObj = Instantiate(saveFolderButtonPrefab, buttonParent);
            Button btn = buttonObj.GetComponent<Button>();
            Text btnText = buttonObj.GetComponentInChildren<Text>();
            string folderName = Path.GetFileName(folders[i]);
            btnText.text = folderName;

            // SaveFolderButton ��ũ��Ʈ�� �߰��Ͽ� ���� ��θ� �����մϴ�.
            SaveFolderButton sfb = buttonObj.GetComponent<SaveFolderButton>();
            if (sfb == null)
                sfb = buttonObj.AddComponent<SaveFolderButton>();
            sfb.folderPath = folders[i];

            folderButtons.Add(btn);
        }
    }

    private void Update()
    {
        if (!inputEnabled || menuController.menuActivated) // �޴��� Ȱ��ȭ ���̸� �Է� ����
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = (currentIndex - 1 + folderButtons.Count) % folderButtons.Count;
            UpdateSelection();
            UpdateCursor();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = (currentIndex + 1) % folderButtons.Count;
            UpdateSelection();
            UpdateCursor();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (folderButtons.Count > 0)
            {
                string selectedFolder = folderButtons[currentIndex].GetComponent<SaveFolderButton>().folderPath;
                SaveSlotManager.Instance.currentSaveFolder = selectedFolder;

                // �� �ε� �� ���� �����͸� �ҷ������� �̺�Ʈ �߰�
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene("Main");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ESCŰ ������ �̾��ϱ� �г��� �ݰ� ���� �޴� �гη� �����մϴ�.
            HideContinuePanel();
        }
    }

    private void UpdateSelection()
    {
        for (int i = 0; i < folderButtons.Count; i++)
        {
            Text t = folderButtons[i].GetComponentInChildren<Text>();
            t.color = (i == currentIndex) ? selectedColor : normalColor;
        }
    }

    public void UpdateCursor()
    {
        if (cursor != null && folderButtons.Count > 0)
        {
            // ���õ� �޴� �׸��� RectTransform
            RectTransform itemRect = folderButtons[currentIndex].GetComponent<RectTransform>();
            // ��ư�� �߾� ��ġ (���� ��ǥ)
            Vector3 worldCenter = itemRect.TransformPoint(itemRect.rect.center);

            // Ŀ���� �θ� Canvas��� �����մϴ�.
            Canvas canvas = cursor.GetComponentInParent<Canvas>();
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            // ���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ (Screen Space - Overlay��� ī�޶� ���ڴ� null)
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldCenter);
            // ��ũ�� ��ǥ�� ĵ������ ���� ��ǥ�� ��ȯ�մϴ�.
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint);
            // Ŀ���� ��ư�� ���ʿ� ��ġ�ϵ��� x�� ������ ����
            localPoint.x -= cursorOffsetX;

            cursor.anchoredPosition = localPoint;
        }
    }

    // ESCŰ�� ������ �� �̾��ϱ� �޴��� �ݰ� ���� �޴� �гη� �����մϴ�.
    private void HideContinuePanel()
    {
        continuePanel.SetActive(false);
        gameObject.SetActive(false);
        menuPanel.SetActive(true);
        inputEnabled = false;
        menuController.menuActivated = true;
        menuController.UpdateCursor();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // �ߺ� ���� ����

        AutoSaveManager autoSave = FindObjectOfType<AutoSaveManager>();
        if (autoSave != null)
        {
            Debug.Log("AutoSaveManager found in Main Scene. Loading game...");
            autoSave.LoadGame();
        }
        else
        {
            Debug.LogError("AutoSaveManager not found in Main Scene!");
        }
    }
}
