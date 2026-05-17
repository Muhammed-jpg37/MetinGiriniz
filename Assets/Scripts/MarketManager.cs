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

        
        List<InventorySlotData> allSlots = InventoryManager.Instance.GetRecycledItems();

        foreach (InventorySlotData slot in allSlots)
        {
            GameObject row = Instantiate(marketItemPrefab, marketItemGrid);

            
            Image img = row.GetComponentInChildren<Image>();
            if (img != null) img.sprite = slot.sprite;

            
            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0) texts[0].text = slot.data.itemName;
            if (texts.Length > 1) texts[1].text = slot.data.baseValue + " TL";

            
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
        GameManager.Instance.AddMoney(slot.data.baseValue);
        InventoryManager.Instance.RemoveSlot(slot);
        RefreshMarket();
        UpdateMoneyUI();
    }

    void UpdateMoneyUI()
    {
        totalMoneyText.text = "Para: " + GameManager.Instance.totalMoney + " TL";
    }
}