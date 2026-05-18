using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SortingSceneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button nextPhaseButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI itemCountText;
    [SerializeField] private Slider progressSlider;

    [Header("Ayar")]
    public int itemsNeededToProgress = 10;

    private int _sortedCount = 0;

    void Start()
    {
        nextPhaseButton.interactable = false;
        UpdateUI();
        progressSlider.maxValue = itemsNeededToProgress;
        progressSlider.value = 0;
        nextPhaseButton.GetComponent<Image>().color = Color.clear;
    }

    
    public void OnItemSortedCorrectly(RecyclableItem item)
    {
        _sortedCount++;
        GameManager.Instance.itemsToClean.Add(item);

        UpdateUI();

        if (_sortedCount >= itemsNeededToProgress)
        {
            nextPhaseButton.interactable = true;
            FindObjectOfType<ConveyorBelt>()?.StopBelt();
        }
            
    }

    
    public void OnNextButtonClicked()
    {
        FindObjectOfType<ConveyorBelt>()?.StopBelt();
        GameManager.Instance.GoToCleaning();
        
    }

    void UpdateUI()
    {
        itemCountText.text = "Item Count: " + _sortedCount + "/" + itemsNeededToProgress;
        progressSlider.value++;
        if(_sortedCount >= itemsNeededToProgress)
        {
            nextPhaseButton.GetComponent<Image>().color = Color.green;
            Destroy(progressSlider.gameObject);
        }
    }
}