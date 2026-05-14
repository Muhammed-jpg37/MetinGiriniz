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
        nextPhaseButton.GetComponent<Image>().color = Color.clear; // Butonu gri yaparak devre dışı bırak
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
        progressSlider.value++;
        if(_sortedCount >= itemsNeededToProgress)
        {
            nextPhaseButton.GetComponent<Image>().color = Color.green; // Butonu yeşil yaparak aktif olduğunu göster
            Destroy(progressSlider.gameObject); // İlerleme çubuğunu kaldır
        }
    }
}