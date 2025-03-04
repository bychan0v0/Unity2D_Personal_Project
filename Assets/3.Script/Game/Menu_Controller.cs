using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Menu_Controller : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text promptText;      // "Press Space" 안내 문구 (Canvas에 있는 UI Text)
    [SerializeField] private GameObject menuPanel; // 메뉴 패널 (이어하기, 새로하기, 조작법, 종료하기 버튼들이 포함된 GameObject)

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f; // 안내문 FadeOut 시간

    private bool menuActivated = false;

    private void Start()
    {
        // 메뉴 패널은 처음에 비활성화
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // 메뉴가 아직 활성화되지 않았고, 스페이스키가 눌렸을 때 처리
        if (!menuActivated && Input.GetKeyDown(KeyCode.Space))
        {
            ActivateMenu();
        }
    }

    private void ActivateMenu()
    {
        menuActivated = true;

        // 안내 문구를 페이드 아웃시키고, 완료되면 메뉴 패널 활성화
        if (promptText != null)
        {
            promptText.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                // FadeOut 후 안내 문구 GameObject 비활성화 (필요시)
                promptText.gameObject.SetActive(false);

                // 메뉴 패널 활성화
                if (menuPanel != null)
                {
                    menuPanel.SetActive(true);
                }
            });
        }
    }
}
