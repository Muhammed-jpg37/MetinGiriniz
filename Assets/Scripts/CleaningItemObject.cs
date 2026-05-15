using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CleaningItemObject : MonoBehaviour
{
    public enum ItemState { Dirty, InWork, Clean }

    public RecyclableItem data { get; private set; }
    public ItemState state { get; private set; }

    [Header("Temizleme Ayarlarý")]
    public float cleaningRequired = 60f;
    public float cleaningPerPixel = 2f;

    private SpriteRenderer sr;
    private Camera cam;
    private bool isDragging = false;
    private bool isCleaned = false;
    private Vector3 startPos;
    private Vector3 lastMousePos;
    private float cleanProgress = 0f;
    private bool isFlashing = false;
    private UnityEngine.UI.Slider progressSlider;


    public void Initialize(RecyclableItem itemData, ItemState initialState, UnityEngine.UI.Slider slider)
    {
        data = itemData;
        state = initialState;
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;

        sr.sprite = data.dirtySprite;
        sr.color = Color.white;
        progressSlider = slider;
        progressSlider.value = 0f;
        progressSlider.gameObject.SetActive(false);
    }

    

    public void SetState(ItemState newState) => state = newState;

    public void SetEffectColor(Color color) { } // ileride kullanýlabilir

    void OnMouseDown()
    {
        if (state == ItemState.Dirty)
        {
            isDragging = true;
            startPos = transform.position;
            sr.sortingOrder = 20;
            return;
        }

        if (state == ItemState.InWork && !isCleaned)
        {
            if (CleaningSceneManager.Instance.IsToolSelected())
            {
                isDragging = true;
                lastMousePos = GetMouseWorld();
                sr.sortingOrder = 20;
            }
            else
            {
                Debug.Log("Once bir alet sec!");
            }
            return;
        }

        if (state == ItemState.Clean)
        {
            isDragging = true;
            startPos = transform.position;
            sr.sortingOrder = 20;
        }
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 currentMouse = GetMouseWorld();

        if (state == ItemState.Dirty)
        {
            transform.position = currentMouse;
        }
        else if (state == ItemState.InWork && !isCleaned)
        {
            float delta = Vector3.Distance(currentMouse, lastMousePos);

            if (delta > 0.01f)
            {
                if (CleaningSceneManager.Instance.IsCorrectTool(data.category))
                {
                    cleanProgress += delta * cleaningPerPixel;
                    cleanProgress = Mathf.Clamp(cleanProgress, 0f, cleaningRequired);

                    UpdateSpriteTransition();
                    SpawnCleaningEffect(currentMouse);

                    if (cleanProgress >= cleaningRequired && !isCleaned)
                        FinishCleaning();
                }
                else
                {
                    
                    StartCoroutine(WrongToolFlash());
                }
            }

            lastMousePos = currentMouse;

            if (cleanProgress >= cleaningRequired && !isCleaned)
                FinishCleaning();
        }
        else if (state == ItemState.Clean)
        {
            transform.position = currentMouse;
        }
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;
        sr.sortingOrder = 5;

        if (state == ItemState.Dirty)
        {
            Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
            bool dropped = false;
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("WorkArea"))
                {
                    CleaningSceneManager.Instance.OnItemMovedToWorkArea(this);
                    state = ItemState.InWork;
                    dropped = true;
                    break;
                }
            }
            if (!dropped)
                transform.position = startPos;
        }
        else if (state == ItemState.Clean)
        {
            Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
            bool dropped = false;
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("CleanBox"))
                {
                    CleaningSceneManager.Instance.OnItemDroppedToCleanBox(this);
                    dropped = true;
                    break;
                }
            }
            if (!dropped)
                transform.position = startPos;
        }
    }

    void UpdateSpriteTransition()
    {
        if (data.cleanSprite == null) return;

        float ratio = cleanProgress / cleaningRequired;

        if (ratio < 0.5f)
        {
            sr.sprite = data.dirtySprite;
        }
        else
        {
            sr.sprite = data.cleanSprite;
        }
        if(progressSlider != null)
        {
            progressSlider.gameObject.SetActive(true);
            progressSlider.value = ratio;
        }
            
    }

    void SpawnCleaningEffect(Vector3 mousePos)
    {
        if (Random.value > 0.3f) return;
        CleaningEffectSpawner.Instance?.SpawnEffects(mousePos, 3);
    }

    void FinishCleaning()
    {
        isCleaned = true;
        sr.sprite = data.cleanSprite != null ? data.cleanSprite : data.dirtySprite;
        sr.color = Color.white;
        cleanProgress = cleaningRequired;
        state = ItemState.Clean;

        
        Debug.Log(data.itemName + " temizlendi!");

        if (progressSlider != null)
        { 
            progressSlider.value = 0f;
            progressSlider.gameObject.SetActive(false);

        }

        CleaningSceneManager.Instance.OnItemCleaned();
    }

    Vector3 GetMouseWorld()
    {
        Vector3 mp = Input.mousePosition;
        mp.z = Mathf.Abs(cam.transform.position.z);
        return cam.ScreenToWorldPoint(mp);
    }

    System.Collections.IEnumerator WrongToolFlash()
    {
        if (isFlashing) yield break;
        isFlashing = true;
        sr.color = new Color(1f, 0.4f, 0.4f);
        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white;
        isFlashing = false;
    }
}