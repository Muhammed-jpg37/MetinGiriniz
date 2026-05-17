using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CleaningSceneManager : MonoBehaviour
{
    public static CleaningSceneManager Instance { get; private set; }

    [Header("Sol �st � Kirli Item Kutusu")]
    public Transform dirtyItemSlot;

    [Header("Orta � �al��ma Alan�")]
    public Transform workAreaSlot;

    [Header("Sol Alt � Temiz Item Kutusu")]
    public Transform cleanItemSlot;
    public Collider2D cleanBoxCollider;

    [Header("Sa� � Ara� Paneli")]
    public ToolButton[] toolButtons;

    [Header("UI")]
    public Button nextPhaseButton;
    public TextMeshProUGUI progressText;
    public UnityEngine.UI.Slider cleaningSlider;

    private Queue<RecyclableItem> dirtyQueue = new Queue<RecyclableItem>();
    private CleaningItemObject currentDirtyItem;
    private CleaningItemObject workItem;
    private GameObject lastCleanedItem;

    private RecycleCategory? selectedToolCategory = null;
    private int totalItems = 0;
    private int cleanedCount = 0;
    [Header("Cursor")]
    CursorArrow currentCursor = CursorArrow.DEFAULT;
    public Texture2D cursorArrowPlastic;
    public Texture2D cursorArrowMetal;
    public Texture2D cursorArrowGlass;
    public Texture2D cursorArrowPaper;
     enum CursorArrow
    {
        Plastic,
        Metal,
        Glass,
        Paper,
        DEFAULT
    }

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

        GameObject go = new GameObject("DirtyItem_" + data.itemName);
        go.transform.position = dirtyItemSlot.position;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = data.dirtySprite;
        sr.sortingOrder = 5;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;

        CleaningItemObject itemObj = go.AddComponent<CleaningItemObject>();
        itemObj.Initialize(data, CleaningItemObject.ItemState.Dirty, cleaningSlider);

        currentDirtyItem = itemObj;
    }

    
    public void OnToolSelected(RecycleCategory toolCategory)
    {
        selectedToolCategory = toolCategory;
        ChangeCursor(toolCategory switch
        {
            RecycleCategory.Plastik => CursorArrow.Plastic,
            RecycleCategory.Kagit => CursorArrow.Paper,
            RecycleCategory.Cam => CursorArrow.Glass,
            RecycleCategory.Metal => CursorArrow.Metal,
            _ => CursorArrow.DEFAULT
        });
        
        CleaningEffectSpawner.Instance?.SetCategory(toolCategory);

        foreach (ToolButton tb in toolButtons)
            tb.SetSelected(false);

        
        Color effectColor = toolCategory switch
        {
            RecycleCategory.Plastik => new Color(0.4f, 0.6f, 1f),
            RecycleCategory.Kagit => new Color(0.9f, 0.8f, 0.3f),
            RecycleCategory.Cam => new Color(0.3f, 0.8f, 0.4f),
            RecycleCategory.Metal => new Color(0.9f, 0.3f, 0.3f),
            _ => Color.white
        };

        workItem?.SetEffectColor(effectColor);
    }

    public bool IsToolSelected() => selectedToolCategory != null;

    public bool IsCorrectTool(RecycleCategory itemCategory)
    {
        if (selectedToolCategory == null) return false;
        return selectedToolCategory == itemCategory;
    }


    public void OnItemCleaned()
    {
        cleanBoxCollider.enabled = true;
        selectedToolCategory = null;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        foreach (ToolButton tb in toolButtons)
            tb.SetSelected(false);

        Debug.Log("Item temizlendi, temiz kutuya surukle!");
        ChangeCursor(CursorArrow.DEFAULT);
    }

   
    public void OnItemMovedToWorkArea(CleaningItemObject item)
    {
        workItem = item;
        workItem.transform.position = workAreaSlot.position;
        currentDirtyItem = null;
    }

  
    public void OnItemDroppedToCleanBox(CleaningItemObject item)
    {
        cleanedCount++;

        
        if (lastCleanedItem != null)
            Destroy(lastCleanedItem);
        lastCleanedItem = item.gameObject;

        
        CleanedItemData cleaned = new CleanedItemData
        {
            item = item.data,
            quality = 100
        };
        GameManager.Instance.itemsToSell.Add(cleaned);

        item.transform.position = cleanItemSlot.position;
        item.GetComponent<SpriteRenderer>().sortingOrder = 5;

        workItem = null;
        selectedToolCategory = null;
        cleanBoxCollider.enabled = false;

        UpdateProgressUI();
        if (cleanedCount >= totalItems)
        {
            nextPhaseButton.interactable = true;
            Debug.Log("Tum itemler temizlendi!");
        }
        else
        {
            ShowNextDirtyItem();
        }
    }

    
    public void OnNextPhaseClicked()
    {
        Debug.Log("OnNextPhaseClicked calisti");
        GameManager.Instance.GoToSelling();
    }

    void UpdateProgressUI()
    {
        progressText.text = "Temizlenen: " + cleanedCount + "/" + totalItems;
    }
     private void ChangeCursor(CursorArrow newCursor)
    {

        if (currentCursor != newCursor)
        {
            switch (newCursor)
            {
                case CursorArrow.Plastic:
                    Cursor.SetCursor(cursorArrowPlastic, Vector2.zero, CursorMode.Auto);
                    break;
                case CursorArrow.Metal:
                    Cursor.SetCursor(cursorArrowMetal, Vector2.zero, CursorMode.Auto);
                    break;
                case CursorArrow.Glass:
                    Cursor.SetCursor(cursorArrowGlass, Vector2.zero, CursorMode.Auto);
                    break;
                case CursorArrow.Paper:
                    Cursor.SetCursor(cursorArrowPaper, Vector2.zero, CursorMode.Auto);
                    break;
                case CursorArrow.DEFAULT:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
            }

            currentCursor = newCursor;
        }
    }
}