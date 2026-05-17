using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory UI")]
    public Transform inventoryGrid;  
    public GameObject inventorySlotPrefab; 
    public int maxSlots = 20;

    
    private List<InventorySlotData> _slots = new List<InventorySlotData>();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddItem(RecyclableItem data, Sprite sprite)
    {
        if (_slots.Count >= maxSlots) return;

        
        GameObject slotGo = Instantiate(inventorySlotPrefab, inventoryGrid);
        Image slotImage = slotGo.GetComponentInChildren<Image>();
        if (slotImage != null)
            slotImage.sprite = sprite;

        InventorySlotData slotData = new InventorySlotData
        {
            data = data,
            sprite = sprite,
            slotGo = slotGo,
            isSelected = false
        };
        _slots.Add(slotData);

        Button btn = slotGo.GetComponent<Button>();
        if (btn != null)
        {
            int index = _slots.Count - 1;
            btn.onClick.AddListener(() => OnSlotClicked(index));
        }

        Debug.Log("Inventory'e eklendi: " + data.itemName);
    }

    public void AddCraftedItem(CraftRecipe recipe)
    {
        if (_slots.Count >= maxSlots) return;

        GameObject slotGo = Instantiate(inventorySlotPrefab, inventoryGrid);
        Image slotImage = slotGo.GetComponentInChildren<Image>();
        if (slotImage != null)
            slotImage.sprite = recipe.resultSprite;

        InventorySlotData slotData = new InventorySlotData
        {
            data = recipe.result,
            sprite = recipe.resultSprite,
            slotGo = slotGo,
            isCrafted = true,
            recipe = recipe,
            isSelected = false
        };
        _slots.Add(slotData);

        int index = _slots.Count - 1;
        Button btn = slotGo.GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(() => OnSlotClicked(index));
    }

    void OnSlotClicked(int index)
    {
        if (index >= _slots.Count) return;
        CraftingManager.Instance.OnInventorySlotClicked(_slots[index]);
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
    {
        return _slots.FindAll(s => !s.isCrafted);
    }
}

[System.Serializable]
public class InventorySlotData
{
    public RecyclableItem data;
    public Sprite sprite;
    public GameObject slotGo;
    public bool isSelected;
    public bool isCrafted;
    public CraftRecipe recipe;
}