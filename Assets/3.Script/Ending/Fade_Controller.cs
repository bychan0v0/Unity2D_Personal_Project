using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Fade_Controller : MonoBehaviour
{
    public static Fade_Controller Instance; // 싱글턴 패턴 적용

    private Image fadeImage; // 페이드 효과를 위한 이미지
    [SerializeField] private float fadeDuration = 1f; // 페이드 인/아웃 지속 시간

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 유지
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 변경될 때 자동 실행
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindFadeImage();
        FadeIn();
    }

    private void FindFadeImage()
    {
        GameObject fadeObj = GameObject.Find("EndPanel"); // 새 씬에서 페이드 패널 찾기
        if (fadeObj != null)
        {
            fadeImage = fadeObj.GetComponent<Image>();
        }
    }

    // 씬 시작 시 페이드 인 (어두운 화면 → 밝아짐)
    public void FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0, 0, 0, 1);
        fadeImage.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            fadeImage.gameObject.SetActive(false);
        });
    }

    // 씬 전환 시 페이드 아웃 (밝은 화면 → 어두워짐) + 씬 변경
    public void FadeOutAndLoadScene(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }
}
