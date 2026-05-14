using UnityEngine;
using UnityEngine.UI;

public class ToolButton : MonoBehaviour
{
    [Header("Bu aletin kategorisi")]
    public RecycleCategory toolCategory;

    [Header("Görsel")]
    public Image buttonImage;
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(0.6f, 1f, 0.6f);

    public void OnClicked()
    {
        CleaningSceneManager.Instance.OnToolSelected(toolCategory);
        SetSelected(true);
    }

    public void SetSelected(bool selected)
    {
        if (buttonImage != null)
            buttonImage.color = selected ? selectedColor : normalColor;
    }
}