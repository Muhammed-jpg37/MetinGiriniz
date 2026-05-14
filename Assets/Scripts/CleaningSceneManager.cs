using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CleaningSceneManager : MonoBehaviour
{
    public static CleaningSceneManager Instance { get; private set; }

    [Header("Sol Üst — Kirli Item Kutusu")]
    public Transform dirtyItemSlot;

    [Header("Orta — Çalýþma Alaný")]
    public Transform workAreaSlot;

    [Header("Sol Alt — Temiz Item Kutusu")]
    public Transform cleanItemSlot;
    public Collider2D cleanBoxCollider;

    [Header("Sað — Araç Paneli")]
    public ToolButton[] toolButtons;

    [Header("UI")]
    public Button nextPhaseButton;
    public TextMeshProUGUI progressText;

    private Queue<RecyclableItem> dirtyQueue = new Queue<RecyclableItem>();
    private CleaningItemObject currentDirtyItem;
    private CleaningItemObject workItem;
    private List<GameObject> cleanedItems = new List<GameObject>();
    private GameObject lastCleanedItem = null;


    private RecycleCategory? selectedToolCategory = null;
    private int totalItems = 0;
    private int cleanedCount = 0;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        nextPhaseButton.interactable = false;
        cleanBoxCollider.enabled = false;

        foreach (RecyclableItem item in GameManager.Instance.itemsToClean)
            dirtyQueue.Enqueue(item);

        totalItems = dirtyQueue.Count;
        UpdateProgressUI();
        ShowNextDirtyItem();
    }

    
    void ShowNextDirtyItem()
    {
        if (dirtyQueue.Count == 0) return;

        RecyclableItem data = dirtyQueue.Dequeue();

        
        GameObject go = new GameObject("DirtyItem" + data.itemName);
        go.transform.position = dirtyItemSlot.position;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = data.dirtySprite;
        sr.sortingOrder = 5;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;

        CleaningItemObject itemObj = go.AddComponent<CleaningItemObject>();
        itemObj.Initialize(data, CleaningItemObject.ItemState.Dirty);

        currentDirtyItem = itemObj;
    }

    
    public void OnToolSelected(RecycleCategory toolCategory)
    {
        selectedToolCategory = toolCategory;

        
        foreach (ToolButton tb in toolButtons)
            tb.SetSelected(false);
    }

    
    public void OnWorkItemClicked()
    {
        if (workItem == null) return;
        if (selectedToolCategory == null)
        {
            Debug.Log("Once bir alet sec!");
            return;
        }

        
        if (selectedToolCategory == workItem.data.category)
        {
            StartCoroutine(CleanItem());
        }
        else
        {
            StartCoroutine(WrongToolFeedback());
        }
    }

    IEnumerator CleanItem()
    {
        
        workItem.SetClean();

        yield return new WaitForSeconds(0.3f);

      
        cleanBoxCollider.enabled = true;
        workItem.SetState(CleaningItemObject.ItemState.Clean);

        Debug.Log("Item temizlendi, temiz kutuya surukle!");
    }

   
    public void OnItemDroppedToCleanBox(CleaningItemObject item)
    {
        cleanedCount++;
        if (lastCleanedItem != null)
        {
            Destroy(lastCleanedItem);
        }
        lastCleanedItem = item.gameObject;
        cleanedItems.Add(item.gameObject);

      
        CleanedItemData cleaned = new CleanedItemData
        {
            item = item.data,
            quality = 100
        };
        GameManager.Instance.itemsToSell.Add(cleaned);

        
        item.transform.position = cleanItemSlot.position;
        item.GetComponent<SpriteRenderer>().sortingOrder = cleanedCount;

        workItem = null;
        selectedToolCategory = null;
        cleanBoxCollider.enabled = false;

        UpdateProgressUI();

        
        if (dirtyQueue.Count > 0)
        {
            ShowNextDirtyItem();
        }
        else if (cleanedCount >= totalItems)
        {
           
            nextPhaseButton.interactable = true;
            Debug.Log("Tum itemler temizlendi!");
        }
    }

   
    public void OnItemMovedToWorkArea(CleaningItemObject item)
    {
        workItem = item;
        workItem.transform.position = workAreaSlot.position;
        currentDirtyItem = null;
    }

    public void OnNextPhaseClicked()
    {
        GameManager.Instance.GoToSelling();
    }

    IEnumerator WrongToolFeedback()
    {
        SpriteRenderer sr = workItem.GetComponent<SpriteRenderer>();
        sr.color = new Color(1f, 0.3f, 0.3f);
        yield return new WaitForSeconds(0.2f);
        sr.color = Color.white;
        Debug.Log("Yanlis alet!");
    }

    void UpdateProgressUI()
    {
        progressText.text = "Temizlenen: " + cleanedCount + "/" + totalItems;
    }
}