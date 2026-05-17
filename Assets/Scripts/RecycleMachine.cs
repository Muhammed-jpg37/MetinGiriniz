using UnityEngine;
using System.Collections;

public class RecycleMachine : MonoBehaviour
{
    public static RecycleMachine Instance { get; private set; }

    [Header("Pozisyonlar")]
    public Transform inputSlot;   // sol kenar
    public Transform outputSlot;  // sað kenar — buradan çýkar
    public Transform finalSlot;   // durma noktasý

    [Header("Ayarlar")]
    public float slideSpeed = 3f;
    public float processTime = 2f;

    private bool _isBusy = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public bool IsBusy() => _isBusy;

    public void ProcessItem(GameObject itemGo, RecyclableItem data)
    {
        if (_isBusy) return;
        StartCoroutine(ProcessRoutine(itemGo, data));
    }

    IEnumerator ProcessRoutine(GameObject itemGo, RecyclableItem data)
    {
        _isBusy = true;

        SpriteRenderer sr = itemGo.GetComponent<SpriteRenderer>();
        Collider2D col = itemGo.GetComponent<Collider2D>();

        CleanedItemDraggable drag = itemGo.GetComponent<CleanedItemDraggable>();
        if (drag != null) drag.enabled = false;
        if (col != null) col.enabled = false;

        
        yield return StartCoroutine(SlideTo(itemGo, inputSlot.position));

        
        itemGo.SetActive(false);

       
        yield return new WaitForSeconds(processTime);

       
        if (data.recycledSprite != null)
            sr.sprite = data.recycledSprite;

       
        itemGo.transform.position = outputSlot.position;
        itemGo.SetActive(true);

        
        float elapsed = 0f;
        float duration = Vector3.Distance(outputSlot.position, finalSlot.position) / slideSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            itemGo.transform.position = Vector3.Lerp(
                outputSlot.position,
                finalSlot.position,
                elapsed / duration
            );
            yield return null;
        }

        itemGo.transform.position = finalSlot.position;

        
        if (col != null) col.enabled = true;

        RecycledItemObject recycledObj = itemGo.GetComponent<RecycledItemObject>();
        if (recycledObj == null)
            recycledObj = itemGo.AddComponent<RecycledItemObject>();
        recycledObj.Initialize(data);

        SellingSceneManager.Instance.SpawnNext();
        _isBusy = false;
    }

    IEnumerator SlideTo(GameObject go, Vector3 target)
    {
        while (go != null && Vector3.Distance(go.transform.position, target) > 0.02f)
        {
            go.transform.position = Vector3.MoveTowards(
                go.transform.position,
                target,
                slideSpeed * Time.deltaTime
            );
            yield return null;
        }

        if (go != null)
            go.transform.position = target;
    }
}