using UnityEngine;
using System.Collections.Generic;

public class SellingSceneManager : MonoBehaviour
{
    public static SellingSceneManager Instance { get; private set; }

    [Header("Temiz Item Spawn Noktasý")]
    public Transform cleanItemSpawnPoint;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        SpawnCleanedItems();
    }

    void SpawnCleanedItems()
    {
        float offsetX = 0f;

        foreach (CleanedItemData cleaned in GameManager.Instance.itemsToSell)
        {
            Vector3 pos = cleanItemSpawnPoint.position + new Vector3(offsetX, 0, 0);

            GameObject go = new GameObject("CleanedItem_" + cleaned.item.itemName);
            go.transform.position = pos;

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = cleaned.item.cleanSprite;
            sr.sortingOrder = 10;

            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;

            CleanedItemDraggable drag = go.AddComponent<CleanedItemDraggable>();
            drag.Initialize(cleaned.item);

            offsetX += 1.2f;
        }
    }
}