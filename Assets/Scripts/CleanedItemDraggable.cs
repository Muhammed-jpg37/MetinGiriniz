using UnityEngine;

public class CleanedItemDraggable : MonoBehaviour
{
    public RecyclableItem data { get; private set; }

    private Camera _cam;
    private SpriteRenderer _sr;
    private bool _isDragging = false;
    private Vector3 _startPos;
    private Vector3 _offset;

    public void Initialize(RecyclableItem itemData)
    {
        data = itemData;
        _cam = Camera.main;
        _sr = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown()
    {
        _isDragging = true;
        _startPos = transform.position;
        _offset = transform.position - GetMouseWorld();
        _sr.sortingOrder = 20;
    }

    void OnMouseDrag()
    {
        if (!_isDragging) return;
        transform.position = GetMouseWorld() + _offset;
    }

    void OnMouseUp()
    {
        if (!_isDragging) return;
        _isDragging = false;
        _sr.sortingOrder = 5;

        
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        bool dropped = false;
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("RecycleMachine"))
            {
                RecycleMachine.Instance.ProcessItem(gameObject, data);
                dropped = true;
                break;
            }
        }

        if (!dropped)
            transform.position = _startPos;
    }

    Vector3 GetMouseWorld()
    {
        Vector3 mp = Input.mousePosition;
        mp.z = Mathf.Abs(_cam.transform.position.z);
        return _cam.ScreenToWorldPoint(mp);
    }
}