using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }

    [Header("Craft Slotlarý")]
    public Image slotA;        
    public Image slotB;        
    public Image slotResult;  

    [Header("Craft UI")]
    public Button craftButton;
    public Button randomButton;
    public Slider craftProgressSlider;
    public float craftDuration = 3f;

    [Header("Tarifler")]
    public List<CraftRecipe> recipes;

    private InventorySlotData _selectedA;
    private InventorySlotData _selectedB;
    private bool _isCrafting = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        craftButton.interactable = false;
        craftProgressSlider.value = 0f;
        craftProgressSlider.gameObject.SetActive(false);
        ClearSlots();
    }

   
    public void OnInventorySlotClicked(InventorySlotData slot)
    {
        if (_isCrafting) return;

        
        if (_selectedA == slot)
        {
            _selectedA = null;
            slot.isSelected = false;
        }
        else if (_selectedB == slot)
        {
            _selectedB = null;
            slot.isSelected = false;
        }
        else if (_selectedA == null)
        {
            _selectedA = slot;
            slot.isSelected = true;
        }
        else if (_selectedB == null)
        {
            _selectedB = slot;
            slot.isSelected = true;
        }

        UpdateCraftUI();
    }

    void UpdateCraftUI()
    {
       
        slotA.sprite = _selectedA != null ? _selectedA.sprite : null;
        slotB.sprite = _selectedB != null ? _selectedB.sprite : null;
        slotA.color = _selectedA != null ? Color.white : new Color(1, 1, 1, 0.3f);
        slotB.color = _selectedB != null ? Color.white : new Color(1, 1, 1, 0.3f);

        
        if (_selectedA != null && _selectedB != null)
        {
            CraftRecipe recipe = FindRecipe(_selectedA.data, _selectedB.data);
            if (recipe != null)
            {
                slotResult.sprite = recipe.resultSprite;
                slotResult.color = Color.white;
                craftButton.interactable = true;
            }
            else
            {
                slotResult.sprite = null;
                slotResult.color = new Color(1, 1, 1, 0.3f);
                craftButton.interactable = false;
            }
        }
        else
        {
            slotResult.sprite = null;
            slotResult.color = new Color(1, 1, 1, 0.3f);
            craftButton.interactable = false;
        }
    }

    
    public void OnCraftClicked()
    {
        if (_selectedA == null || _selectedB == null || _isCrafting) return;

        CraftRecipe recipe = FindRecipe(_selectedA.data, _selectedB.data);
        if (recipe == null) return;

        StartCoroutine(CraftRoutine(recipe));
    }

   
    public void OnRandomClicked()
    {
        if (_isCrafting) return;

        List<InventorySlotData> recycled = InventoryManager.Instance.GetRecycledItems();
        if (recycled.Count < 2)
        {
            Debug.Log("Yeterli item yok!");
            return;
        }

        
        int indexA = Random.Range(0, recycled.Count);
        int indexB;
        do { indexB = Random.Range(0, recycled.Count); }
        while (indexB == indexA);

        _selectedA = recycled[indexA];
        _selectedB = recycled[indexB];

        UpdateCraftUI();

        
        CraftRecipe recipe = FindRecipe(_selectedA.data, _selectedB.data);
        if (recipe != null)
            StartCoroutine(CraftRoutine(recipe));
        else
            Debug.Log("Bu kombinasyon icin tarif yok!");
    }

    IEnumerator CraftRoutine(CraftRecipe recipe)
    {
        _isCrafting = true;
        craftButton.interactable = false;
        craftProgressSlider.gameObject.SetActive(true);
        craftProgressSlider.value = 0f;

        float elapsed = 0f;
        while (elapsed < craftDuration)
        {
            elapsed += Time.deltaTime;
            craftProgressSlider.value = elapsed / craftDuration;
            yield return null;
        }

        craftProgressSlider.value = 1f;
        yield return new WaitForSeconds(0.2f);

        
        InventoryManager.Instance.RemoveSlot(_selectedA);
        InventoryManager.Instance.RemoveSlot(_selectedB);

       
        InventoryManager.Instance.AddCraftedItem(recipe);

        
        _selectedA = null;
        _selectedB = null;
        _isCrafting = false;
        craftProgressSlider.gameObject.SetActive(false);
        craftProgressSlider.value = 0f;
        UpdateCraftUI();
    }

    CraftRecipe FindRecipe(RecyclableItem a, RecyclableItem b)
    {
        foreach (CraftRecipe recipe in recipes)
        {
            if ((recipe.ingredientA == a && recipe.ingredientB == b) ||
                (recipe.ingredientA == b && recipe.ingredientB == a))
                return recipe;
        }
        return null;
    }

    void ClearSlots()
    {
        slotA.sprite = null;
        slotB.sprite = null;
        slotA.color = new Color(1, 1, 1, 0.3f);
        slotB.color = new Color(1, 1, 1, 0.3f);
        slotResult.color = new Color(1, 1, 1, 0.3f);
    }
}