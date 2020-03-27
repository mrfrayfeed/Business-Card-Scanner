using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIBase : MonoBehaviour {

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image bgOnWindow;
    [SerializeField] private bool isShowBg;

    public virtual void Show () {
        if (isShowBg) ShowWindowBg ();
        canvasGroup.DOFade (1, 0.25f).OnComplete (delegate {
            canvasGroup.blocksRaycasts = true;
        });

    }

    public virtual void Hide () {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade (0, 0.25f);
        if (isShowBg) HideBg ();
    }

    private void ShowWindowBg () {
        bgOnWindow?.DOFade (1f, 0.5f);
        isShowBg = true;
    }
    private void HideBg () {
        bgOnWindow?.DOFade (0f, 0.5f);
        isShowBg = false;
    }
}