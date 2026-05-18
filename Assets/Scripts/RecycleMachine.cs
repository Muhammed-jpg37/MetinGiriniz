using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RecycleMachine : MonoBehaviour
{
    public static RecycleMachine Instance { get; private set; }

    [Header("Pozisyonlar")]
    public Transform inputSlot;
    public Transform outputSlot;
    public Transform finalSlot;

    [Header("Ayarlar")]
    public float slideSpeed = 3f;
    public float processTime = 2f;

    [Header("UI")]
    public Slider processSlider;    
    public TextMeshProUGUI countdownText;

    private bool _isBusy = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (processSlider != null)
        {
            processSlider.value = 0f;
            processSlider.gameObject.SetActive(false);
        }
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
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

        
        if (processSlider != null)
        {
            processSlider.value = 0f;
            processSlider.gameObject.SetActive(true);
        }
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        
        float elapsed = 0f;
        while (elapsed < processTime)
        {
            elapsed += Time.deltaTime;

            if (processSlider != null)
                processSlider.value = elapsed / processTime;

            if (countdownText != null)
            {
                float remaining = processTime - elapsed;
                countdownText.text = remaining.ToString("F1") + "s";
            }

            yield return null;
        }

        
        if (processSlider != null)
        {
            processSlider.value = 1f;
            processSlider.gameObject.SetActive(false);
        }
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        
        if (data.recycledSprite != null)
            sr.sprite = data.recycledSprite;
        else
            sr.color = new Color(0.3f, 1f, 0.3f);

        
        itemGo.transform.position = outputSlot.position;
        itemGo.SetActive(true);

        
        float duration = Vector3.Distance(outputSlot.position, finalSlot.position) / slideSpeed;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            itemGo.transform.position = Vector3.Lerp(
                outputSlot.position,
                finalSlot.position,
                t / duration
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