using System;
using UnityEngine;

public class BaseMenu : MonoBehaviour
{
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected bool isVisible = false;

    private void OnValidate()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
        }
    }

    public void Toggle(Action _callback)
    {
        if (isVisible)
        {
            Hide(_callback);
        }
        else
        {
            Show(_callback);
        }
    }

    public virtual void Show(Action _callback)
    {
        gameObject.SetActive(true);
        // Show only if not visible but callback should be called anyway
        if (isVisible)
        {
            _callback?.Invoke();
            return;
        }

        canvasGroup.blocksRaycasts = true;
        isVisible = true;

        LeanTween.value(canvasGroup.alpha, to: 1, time: AnimationTimers.TABCONTENT_SHOW)
        .setEaseInOutSine()
        .setOnUpdate((float val) =>
        {
            canvasGroup.alpha = val;
        }).setOnComplete(() => {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            _callback?.Invoke();
        });
    }

    public virtual void Hide(Action _callback)
    {
        // Hide only if is visible but callback should be called anyway
        if (!isVisible)
        {
            _callback?.Invoke();
            return;
        }

        canvasGroup.interactable = false;

        LeanTween.value(canvasGroup.alpha, to: 0, time: AnimationTimers.TABCONTENT_HIDE)
        .setEaseInOutSine()
        .setOnUpdate((float val) =>
        {
            canvasGroup.alpha = val;
        }).setOnComplete(() => {
            isVisible = false;
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            _callback?.Invoke();
            gameObject.SetActive(false);
        });
    }

    public virtual void InstantHide()
    {
        isVisible = false;
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    protected virtual void UpdateContent(Action _callback)
    {
        _callback?.Invoke();
    }

}
