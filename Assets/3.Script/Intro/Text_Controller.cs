using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Text_Controller : MonoBehaviour
{
    [SerializeField] private Text promptText;
    [SerializeField] private float blinkDuration = 0.8f;

    void Start()
    {
        promptText.canvasRenderer.SetAlpha(0f);
    }

    public void StartBlinking()
    {
        promptText.canvasRenderer.SetAlpha(1f);
        promptText.DOFade(0f, blinkDuration).SetLoops(-1, LoopType.Yoyo);
    }
}
