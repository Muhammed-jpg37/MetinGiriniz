using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MarketManager : MonoBehaviour
{
    public static MarketManager Instance { get; private set; }

    [Header("Market Panel")]
    public GameObject marketPanel;
    public Transform marketItemGrid;
    public GameObject marketItemPrefab; 
    public TextMeshProUGUI totalMoneyText;
    public Button closeButton;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        marketPanel.SetActive(false);
    }

   
    public void OpenMarket()
    {
        if (GameManager.Instance == null)
        {
            GameObject gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
        }

        marketPanel.SetActive(true);
        RefreshMarket();
    }

    public void CloseMarket()
    {
        marketPanel.SetActive(false);
    }

    void RefreshMarket()
    {
        foreach (Transform child in marketItemGrid)
            Destroy(child.gameObject);

        List<InventorySlotData> crafted = InventoryManager.Instance.GetAllSlots()
            .FindAll(s => s.isCrafted);

        Debug.Log("Market'te satilacak item sayisi: " + crafted.Count);

        if (crafted.Count == 0)
        {
            Debug.Log("Hic crafted item yok!");
            return;
        }

        foreach (InventorySlotData slot in crafted)
        {
            GameObject row = Instantiate(marketItemPrefab, marketItemGrid);

            Image[] images = row.GetComponentsInChildren<Image>();
            if (images.Length >= 2)
                images[1].sprite = slot.sprite;
            else if (images.Length == 1)
                images[0].sprite = slot.sprite;

            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0) texts[0].text = slot.recipe != null ? slot.recipe.resultName : "?";
            if (texts.Length > 1) texts[1].text = slot.recipe != null ? slot.recipe.marketValue + " TL" : "0 TL";

            Button sellBtn = row.GetComponentInChildren<Button>();
            if (sellBtn != null)
            {
                InventorySlotData captured = slot;
                sellBtn.onClick.AddListener(() => SellItem(captured));
            }
        }

        UpdateMoneyUI();
    }

    void SellItem(InventorySlotData slot)
    {
        if (slot == null || slot.recipe == null) return;

        int value = slot.recipe.marketValue;

        if (GameManager.Instance != null)
            GameManager.Instance.AddMoney(value);

        InventoryManager.Instance.RemoveSlot(slot);
        RefreshMarket();
        UpdateMoneyUI();

        Debug.Log(slot.recipe.resultName + " satildi: " + value + " TL");
    }

    void UpdateMoneyUI()
    {
        totalMoneyText.text = "Para: " + GameManager.Instance.totalMoney + " TL";
    }
}