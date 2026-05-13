using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Sahne Ýsimleri")]
    public string sortingSceneName = "SortingScene";
    public string cleaningSceneName = "CleaningScene";
    public string sellingSceneName = "SellingScene";

    [Header("Oyun Verisi")]
    public int totalMoney = 0;

    
    public List<RecyclableItem> itemsToClean = new();
    
    public List<CleanedItemData> itemsToSell = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    

    public void GoToCleaning()
    {
        SceneManager.LoadScene(cleaningSceneName);
    }

    public void GoToSelling()
    {
        SceneManager.LoadScene(sellingSceneName);
    }

    public void GoToSorting() 
    {
        itemsToClean.Clear();
        itemsToSell.Clear();
        totalMoney = 0;
        SceneManager.LoadScene(sortingSceneName);
    }

  

    public void AddMoney(int amount)
    {
        totalMoney += amount;
    }
}


[System.Serializable]
public class CleanedItemData
{
    public RecyclableItem item;
    public int quality; 
}