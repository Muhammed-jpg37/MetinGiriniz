using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Referanslar")]
    public Transform content;
    public GameObject inventorySlotPrefab;

    private List<InventorySlotData> _slots = new List<InventorySlotData>();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddItem(RecyclableItem data, Sprite sprite)
    {
        CreateSlot(data, sprite, false, null, false, 0);
    }

    public void AddRecycledItem(RecyclableItem data, Sprite sprite)
    {
        CreateSlot(data, sprite, false, null, true, 0);
        Debug.Log("Recycled eklendi: " + data.itemName);
    }

    public void AddCraftedItem(CraftRecipe recipe, int quality)
    {
        CreateSlot(null, recipe.resultSprite, true, recipe, false, quality);
        Debug.Log("Crafted eklendi: " + recipe.resultName + " Quality: " + quality);
    }

    void CreateSlot(RecyclableItem data, Sprite sprite, bool isCrafted,
                 CraftRecipe recipe, bool isRecycled, int quality)
    {
        string itemName = isCrafted
            ? recipe.resultName + " (%" + quality + ")"
            : (data != null ? data.itemName : "?");

        GameObject slot = Instantiate(inventorySlotPrefab, content);

        Image img = slot.transform.Find("ItemImage/Icon")?.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = sprite;
            img.preserveAspect = true;
        }

        TextMeshProUGUI txt = slot.transform.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
        if (txt != null)
            txt.text = itemName;

        InventorySlotData slotData = new InventorySlotData
        {
            data = data,
            sprite = sprite,
            slotGo = slot,
            isCrafted = isCrafted,
            isRecycled = isRecycled,
            recipe = recipe,
            isSelected = false,
            quality = quality
        };
        _slots.Add(slotData);

        
        Button btn = slot.GetComponent<Button>();
        if (btn == null) btn = slot.AddComponent<Button>();
        btn.onClick.AddListener(() => OnSlotClicked(slotData));
    }

    void OnSlotClicked(InventorySlotData slotData)
    {
        if (slotData == null) return;
        CraftingManager.Instance.OnInventorySlotClicked(slotData);
    }

    public void RemoveSlot(InventorySlotData slot)
    {
        if (_slots.Contains(slot))
        {
            Destroy(slot.slotGo);
            _slots.Remove(slot);
        }
    }

    public List<InventorySlotData> GetRecycledItems()
        => _slots.FindAll(s => s.isRecycled);

    public List<InventorySlotData> GetAllSlots() => _slots;
}

[System.Serializable]
public class InventorySlotData
{
    public RecyclableItem data;
    public Sprite sprite;
    public GameObject slotGo;
    public bool isSelected;
    public bool isCrafted;
    public bool isRecycled;
    public CraftRecipe recipe;
    public int quality;
}