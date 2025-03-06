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
    [SerializeField] private Transform buttonParent;       // 저장 슬롯 버튼들이 배치될 부모 (예: Content 역할)
    [SerializeField] private GameObject saveFolderButtonPrefab; // 저장 슬롯 버튼 프리팹
    [SerializeField] private RectTransform cursor;         // 선택된 항목 옆에 표시할 커서 이미지

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    [Header("Cursor Settings")]
    [SerializeField] private float cursorOffsetX = 95f; // 커서가 버튼 왼쪽으로 이동할 오프셋

    private List<Button> folderButtons = new List<Button>();
    private int currentIndex = 0;
    private bool inputEnabled = false;

    private void Start()
    {
        menuController = FindObjectOfType<Menu_Controller>();
    }

    private void OnEnable()
    {
        // 패널이 활성화되면 저장 슬롯 버튼들을 동적으로 생성합니다.
        PopulateFolderButtons();
        UpdateCursor();
        currentIndex = 0;
        UpdateSelection();
        inputEnabled = true;
    }

    private void PopulateFolderButtons()
    {
        // 기존 버튼들을 모두 삭제
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

        // 최대 5개만 생성합니다.
        int count = Mathf.Min(5, folders.Length);
        for (int i = 0; i < count; i++)
        {
            GameObject buttonObj = Instantiate(saveFolderButtonPrefab, buttonParent);
            Button btn = buttonObj.GetComponent<Button>();
            Text btnText = buttonObj.GetComponentInChildren<Text>();
            string folderName = Path.GetFileName(folders[i]);
            btnText.text = folderName;

            // SaveFolderButton 스크립트를 추가하여 폴더 경로를 저장합니다.
            SaveFolderButton sfb = buttonObj.GetComponent<SaveFolderButton>();
            if (sfb == null)
                sfb = buttonObj.AddComponent<SaveFolderButton>();
            sfb.folderPath = folders[i];

            folderButtons.Add(btn);
        }
    }

    private void Update()
    {
        if (!inputEnabled || menuController.menuActivated) // 메뉴가 활성화 중이면 입력 차단
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

                // 씬 로드 후 저장 데이터를 불러오도록 이벤트 추가
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene("Main");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ESC키 누르면 이어하기 패널을 닫고 메인 메뉴 패널로 복귀합니다.
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
            // 선택된 메뉴 항목의 RectTransform
            RectTransform itemRect = folderButtons[currentIndex].GetComponent<RectTransform>();
            // 버튼의 중앙 위치 (월드 좌표)
            Vector3 worldCenter = itemRect.TransformPoint(itemRect.rect.center);

            // 커서의 부모가 Canvas라고 가정합니다.
            Canvas canvas = cursor.GetComponentInParent<Canvas>();
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            // 월드 좌표를 스크린 좌표로 변환 (Screen Space - Overlay라면 카메라 인자는 null)
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldCenter);
            // 스크린 좌표를 캔버스의 로컬 좌표로 변환합니다.
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint);
            // 커서가 버튼의 왼쪽에 위치하도록 x축 오프셋 적용
            localPoint.x -= cursorOffsetX;

            cursor.anchoredPosition = localPoint;
        }
    }

    // ESC키를 눌렀을 때 이어하기 메뉴를 닫고 메인 메뉴 패널로 복귀합니다.
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
        SceneManager.sceneLoaded -= OnSceneLoaded; // 중복 실행 방지

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
