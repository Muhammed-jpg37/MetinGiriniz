using UnityEngine;
using System.Collections;

public class RecycleMachine : MonoBehaviour
{
    public static RecycleMachine Instance { get; private set; }

    [Header("Pozisyonlar")]
    public Transform inputSlot;    
    public Transform outputSlot;  
    public Transform insideSlot;   

    [Header("Ayarlar")]
    public float slideSpeed = 2f;  
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

        
        yield return StartCoroutine(SlideTo(itemGo, inputSlot.position, slideSpeed));

        
        yield return StartCoroutine(SlideTo(itemGo, insideSlot.position, slideSpeed));
        itemGo.SetActive(false);

        
        
        yield return new WaitForSeconds(processTime);
        

        
        if (data.recycledSprite != null)
            sr.sprite = data.recycledSprite;

       
        itemGo.transform.position = outputSlot.position;
        itemGo.SetActive(true);

        
        Vector3 finalPos = outputSlot.position + Vector3.right * 1.5f;
        yield return StartCoroutine(SlideTo(itemGo, finalPos, slideSpeed));

        
        RecycledItemObject recycledObj = itemGo.GetComponent<RecycledItemObject>();
        if (recycledObj == null)
            recycledObj = itemGo.AddComponent<RecycledItemObject>();
        recycledObj.Initialize(data);

        _isBusy = false;
    }

    IEnumerator SlideTo(GameObject go, Vector3 target, float speed)
    {
        while (Vector3.Distance(go.transform.position, target) > 0.01f)
        {
            go.transform.position = Vector3.MoveTowards(
                go.transform.position,
                target,
                speed * Time.deltaTime
            );
            yield return null;
        }
        go.transform.position = target;
    }
}