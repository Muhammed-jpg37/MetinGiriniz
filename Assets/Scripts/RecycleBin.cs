using UnityEngine;
using System.Collections;

public class RecycleBin : MonoBehaviour
{
    [Header("Kategori")]
    public RecycleCategory acceptedCategory;

    [Header("Görsel")]
    public SpriteRenderer binRenderer;
    public Sprite normalSprite;
    public Sprite correctSprite;
    public Sprite wrongSprite;

    [Header("Efekt")]
    public ParticleSystem successParticle;

    public void TryReceive(DraggableItem item)
    {
        if (item.data.category == acceptedCategory)
            OnCorrect(item);
        else
            StartCoroutine(OnWrong(item));
    }

    void OnCorrect(DraggableItem item)
    {
        binRenderer.sprite = correctSprite;
        successParticle?.Play();

        
        SortingSceneManager mgr = FindObjectOfType<SortingSceneManager>();
        if (mgr != null) mgr.OnItemSortedCorrectly(item.data);

        Destroy(item.gameObject);
        Invoke(nameof(ResetSprite), 0.8f);
    }

    IEnumerator OnWrong(DraggableItem item)
    {
        binRenderer.sprite = wrongSprite;

        
        Vector3 originalPos = transform.position;
        for (int i = 0; i < 6; i++)
        {
            transform.position = originalPos + (Vector3)Random.insideUnitCircle * 0.08f;
            yield return new WaitForSeconds(0.05f);
        }
        transform.position = originalPos;

        
        item.GetComponent<ConveyorMover>().Resume();
        ResetSprite();
    }

    void ResetSprite() => binRenderer.sprite = normalSprite;
}