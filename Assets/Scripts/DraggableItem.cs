using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(ConveyorMover))]
public class DraggableItem : MonoBehaviour
{
    public RecyclableItem data { get; private set; }

    private SpriteRenderer _sr;
    private Camera _cam;
    private ConveyorMover _mover;
    private bool _isDragging = false;
    private Vector3 _offset;
    private float _beltY;

    public void Initialize(RecyclableItem itemData, float beltY)
    {
        data = itemData;
        _sr = GetComponent<SpriteRenderer>();
        _cam = Camera.main;
        _mover = GetComponent<ConveyorMover>();
        _beltY = beltY;

        _sr.sprite = data.dirtySprite;
    }

    void OnMouseDown()
    {
        _isDragging = true;
        _mover.Pause();
        _offset = transform.position - GetMouseWorldPos();
        _sr.sortingOrder = 10;
    }

    void OnMouseDrag()
    {
        if (!_isDragging) return;
        transform.position = GetMouseWorldPos() + _offset;
    }

    void OnMouseUp()
    {
        _isDragging = false;
        _sr.sortingOrder = 1;

        RecycleBin bin = FindBinUnder();
        if (bin != null)
        {
            bin.TryReceive(this);
        }
        else
        {
            transform.position = new Vector3(
                transform.position.x,
                _beltY,
                transform.position.z
            );
            _mover.Resume();
        }
    }

    RecycleBin FindBinUnder()
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        foreach (Collider2D hit in hits)
        {
            RecycleBin bin = hit.GetComponent<RecycleBin>();
            if (bin != null) return bin;
        }
        return null;
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mp = Input.mousePosition;
        mp.z = Mathf.Abs(_cam.transform.position.z);
        return _cam.ScreenToWorldPoint(mp);
    }
}