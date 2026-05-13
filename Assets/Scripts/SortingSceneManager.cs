using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SortingSceneManager : MonoBehaviour
{
    [Header("UI")]
    public Button nextPhaseButton;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI itemCountText;

    [Header("Ayar")]
    public int itemsNeededToProgress = 10;

    private int _sortedCount = 0;

    void Start()
    {
        nextPhaseButton.interactable = false;
        UpdateUI();
    }

    
    public void OnItemSortedCorrectly(RecyclableItem item)
    {
        _sortedCount++;
        GameManager.Instance.itemsToClean.Add(item);

        UpdateUI();

        if (_sortedCount >= itemsNeededToProgress)
            nextPhaseButton.interactable = true;
    }

    
    public void OnNextButtonClicked()
    {
        FindObjectOfType<ConveyorBelt>()?.StopBelt();
        GameManager.Instance.GoToCleaning();
    }

    void UpdateUI()
    {
        itemCountText.text = "Siralanan: " + _sortedCount + "/" + itemsNeededToProgress;
    }
}