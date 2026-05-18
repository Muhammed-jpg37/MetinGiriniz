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

        List<InventorySlotData> crafted = InventoryManager.Instance
            .GetAllSlots().FindAll(s => s.isCrafted);

        Debug.Log("Market'te satilacak: " + crafted.Count);

        foreach (InventorySlotData slot in crafted)
        {
            GameObject row = Instantiate(marketItemPrefab, marketItemGrid);

            Image[] images = row.GetComponentsInChildren<Image>();
            if (images.Length >= 2)
            {
                images[1].sprite = slot.sprite;
                images[1].preserveAspect = true;
            }

            
            float multiplier = slot.quality / 100f;
            int price = Mathf.RoundToInt(slot.recipe.marketValue * multiplier);

            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0)
                texts[0].text = slot.recipe.resultName;
            if (texts.Length > 1)
                texts[1].text = price + " TL (%" + slot.quality + ")";

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

        float multiplier = slot.quality / 100f;
        int price = Mathf.RoundToInt(slot.recipe.marketValue * multiplier);

        if (GameManager.Instance != null)
            GameManager.Instance.AddMoney(price);

        Debug.Log(slot.recipe.resultName + " satildi: " + price + " TL");

        InventoryManager.Instance.RemoveSlot(slot);
        RefreshMarket();
        UpdateMoneyUI();
    }

    void UpdateMoneyUI()
    {
        if (totalMoneyText != null && GameManager.Instance != null)
            totalMoneyText.text = "Para: " + GameManager.Instance.totalMoney + " TL";
    }
}