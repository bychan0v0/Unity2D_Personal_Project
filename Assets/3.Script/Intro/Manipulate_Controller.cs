using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manipulate_Controller : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Menu_Controller menuController;
    [SerializeField] private GameObject manipulatePanel;
    [SerializeField] private RectTransform cursor;

    private void Start()
    {
        menuController = FindObjectOfType<Menu_Controller>();
    }

    private void OnEnable()
    {
        if (cursor != null)
            cursor.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideManipulatePanel();
        }
    }

    private void HideManipulatePanel()
    {
        manipulatePanel.SetActive(false);
        gameObject.SetActive(false);
        menuPanel.SetActive(true);
        menuController.menuActivated = true;
        menuController.UpdateCursor();

        if (cursor != null)
            cursor.gameObject.SetActive(true);
    }
}
