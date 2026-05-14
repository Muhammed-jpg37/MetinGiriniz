using UnityEngine;

public class CleaningItemObject : MonoBehaviour
{
    public enum ItemState { Dirty, InWork, Clean }

    public RecyclableItem data { get; private set; }
    public ItemState state { get; private set; }

    private SpriteRenderer sr;
    private Camera cam;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 startPos;
    private bool isCleaned = false;

    public void Initialize(RecyclableItem itemData, ItemState initialState)
    {
        data = itemData;
        state = initialState;
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;
        sr.sprite = data.dirtySprite;
    }

    public void SetState(ItemState newState) => state = newState;

    public void SetClean()
    {
        isCleaned = true;
        sr.sprite = data.cleanSprite != null ? data.cleanSprite : data.dirtySprite;
    }

    void OnMouseDown()
    {
        
        if (state == ItemState.Dirty)
        {
            isDragging = true;
            startPos = transform.position;
            offset = transform.position - GetMouseWorld();
            sr.sortingOrder = 20;
            return;
        }

        
        if (state == ItemState.InWork && !isCleaned)
        {
            CleaningSceneManager.Instance.OnWorkItemClicked();
            return;
        }

        
        if (state == ItemState.Clean)
        {
            isDragging = true;
            startPos = transform.position;
            offset = transform.position - GetMouseWorld();
            sr.sortingOrder = 20;
        }
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;
        transform.position = GetMouseWorld() + offset;
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;
        sr.sortingOrder = 5;

        if (state == ItemState.Dirty)
        {
            
            CleaningSceneManager mgr = CleaningSceneManager.Instance;
            Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
            bool droppedToWork = false;

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("WorkArea"))
                {
                    mgr.OnItemMovedToWorkArea(this);
                    state = ItemState.InWork;
                    droppedToWork = true;
                    break;
                }
            }

            if (!droppedToWork)
                transform.position = startPos; 
        }
        else if (state == ItemState.Clean)
        {
            
            Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
            bool droppedToClean = false;

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("CleanBox"))
                {
                    CleaningSceneManager.Instance.OnItemDroppedToCleanBox(this);
                    droppedToClean = true;
                    break;
                }
            }

            if (!droppedToClean)
                transform.position = startPos; 
        }
    }

    Vector3 GetMouseWorld()
    {
        Vector3 mp = Input.mousePosition;
        mp.z = Mathf.Abs(cam.transform.position.z);
        return cam.ScreenToWorldPoint(mp);
    }
}