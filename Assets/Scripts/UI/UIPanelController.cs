using UnityEngine;
using UnityEngine.UI;

public class UIPanelController : MonoBehaviour
{
    public GameObject panel;
    public float showAnimationDuration = 0.5f;
    public float hideAnimationDuration = 0.5f;

    private bool isVisible = false;

    public void TogglePanel()
    {
        if (isVisible)
        {
            HidePanel();
        }
        else
        {
            ShowPanel();
        }
    }

    [ContextMenu("Show")]
    public void ShowPanel()
    {
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;
        LeanTween.scale(panel, Vector3.one, showAnimationDuration).setEase(LeanTweenType.easeInBack);
        isVisible = true;
    }

    [ContextMenu("Hide")]
    public void HidePanel()
    {
        LeanTween.scale(panel, Vector3.zero, hideAnimationDuration).setEase(LeanTweenType.easeInBack)
            .setOnComplete(() => panel.SetActive(false));
        isVisible = false;
    }
}