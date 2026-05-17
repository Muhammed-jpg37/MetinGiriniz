using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Referanslar")]
    public Transform content;

    private List<InventorySlotData> _slots = new List<InventorySlotData>();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddItem(RecyclableItem data, Sprite sprite)
    {
        CreateSlot(data, sprite, false, null, false);
        Debug.Log("Hammadde eklendi: " + data.itemName);
    }

    public void AddRecycledItem(RecyclableItem data, Sprite sprite)
    {
        CreateSlot(data, sprite, false, null, true);
        Debug.Log("Recycled eklendi: " + data.itemName);
    }

    public void AddCraftedItem(CraftRecipe recipe)
    {
        CreateSlot(null, recipe.resultSprite, true, recipe, false);
        Debug.Log("Crafted eklendi: " + recipe.resultName);
    }

    void CreateSlot(RecyclableItem data, Sprite sprite, bool isCrafted, CraftRecipe recipe, bool isRecycled)
    {
        string itemName = isCrafted ? recipe.resultName : (data != null ? data.itemName : "?");

        GameObject row = new GameObject("Slot_" + itemName);
        row.transform.SetParent(content, false);

        Image bg = row.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10;
        hlg.padding = new RectOffset(8, 8, 8, 8);
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;
        hlg.childAlignment = TextAnchor.MiddleLeft;

        LayoutElement le = row.AddComponent<LayoutElement>();
        le.preferredHeight = 60;
        le.flexibleWidth = 1;

        // İkon
        GameObject imgGo = new GameObject("Icon");
        imgGo.transform.SetParent(row.transform, false);
        Image img = imgGo.AddComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;
        LayoutElement imgLe = imgGo.AddComponent<LayoutElement>();
        imgLe.preferredWidth = 44;
        imgLe.preferredHeight = 44;

        // İsim
        GameObject textGo = new GameObject("Name");
        textGo.transform.SetParent(row.transform, false);
        TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = itemName;
        tmp.fontSize = 18;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        LayoutElement textLe = textGo.AddComponent<LayoutElement>();
        textLe.flexibleWidth = 1;

        Button btn = row.AddComponent<Button>();
        int index = _slots.Count;
        btn.onClick.AddListener(() => OnSlotClicked(index));

        InventorySlotData slotData = new InventorySlotData
        {
            data = data,
            sprite = sprite,
            slotGo = row,
            isCrafted = isCrafted,
            isRecycled = isRecycled,
            recipe = recipe,
            isSelected = false
        };
        _slots.Add(slotData);
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
        return _slots.FindAll(s => s.isRecycled);
    }

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
}