using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SellingSceneManager : MonoBehaviour
{
    public static SellingSceneManager Instance { get; private set; }

    [Header("Spawn")]
    public Transform cleanItemSpawnPoint;

    [Header("Test Modu")]
    public bool testMode = false;
    public RecyclableItem[] testItems;

    private Queue<CleanedItemData> _spawnQueue = new Queue<CleanedItemData>();
    private GameObject _currentItem = null;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (GameManager.Instance == null)
        {
            GameObject gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
        }

        if (testMode) LoadTestData();

        foreach (CleanedItemData cleaned in GameManager.Instance.itemsToSell)
            _spawnQueue.Enqueue(cleaned);

        SpawnNext();
    }

    public void SpawnNext()
    {
        if (_spawnQueue.Count == 0) return;

        CleanedItemData cleaned = _spawnQueue.Dequeue();

        GameObject go = new GameObject("CleanedItem_" + cleaned.item.itemName);
        go.transform.position = cleanItemSpawnPoint.position;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = cleaned.item.cleanSprite;
        sr.sortingOrder = 10;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;

        CleanedItemDraggable drag = go.AddComponent<CleanedItemDraggable>();
        drag.Initialize(cleaned.item);

        _currentItem = go;
    }

    void LoadTestData()
    {
        GameManager.Instance.itemsToSell.Clear();
        foreach (RecyclableItem item in testItems)
        {
            GameManager.Instance.itemsToSell.Add(new CleanedItemData
            {
                item = item,
                quality = 100
            });
        }
    }
}