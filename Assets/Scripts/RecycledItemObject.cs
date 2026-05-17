using UnityEngine;

public class RecycledItemObject : MonoBehaviour
{
    public RecyclableItem data { get; private set; }

    private bool _isInitialized = false;

    public void Initialize(RecyclableItem itemData)
    {
        data = itemData;
        _isInitialized = true;

        
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
    }

    void OnMouseDown()
    {
        if (!_isInitialized) return;

        
        InventoryManager.Instance.AddItem(data, GetComponent<SpriteRenderer>().sprite);
        Destroy(gameObject);
    }
}