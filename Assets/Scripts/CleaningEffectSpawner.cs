using UnityEngine;
using System.Collections;

public class CleaningEffectSpawner : MonoBehaviour
{
    public static CleaningEffectSpawner Instance { get; private set; }

    [Header("Alet Baþýna Efekt Sprite'larý")]
    public Sprite[] plastikEffects; 
    public Sprite[] kagitEffects;   
    public Sprite[] camEffects;    
    public Sprite[] metalEffects;

    [Header("Ayarlar")]
    public float effectDuration = 0.4f;
    public float effectMoveSpeed = 1.5f;
    public float spawnRadius = 0.4f;

    private RecycleCategory currentCategory = RecycleCategory.Plastik;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    
    public void SetCategory(RecycleCategory category)
    {
        currentCategory = category;
    }

    public void SpawnEffects(Vector3 position, int count = 3)
    {
        Sprite[] sprites = GetSpritesForCategory(currentCategory);
        if (sprites == null || sprites.Length == 0) return;

        for (int i = 0; i < count; i++)
        {
            Sprite randomSprite = sprites[Random.Range(0, sprites.Length)];
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                Random.Range(-spawnRadius, spawnRadius),
                0f
            );
            StartCoroutine(SpawnSingleEffect(position + randomOffset, randomSprite));
        }
    }

    Sprite[] GetSpritesForCategory(RecycleCategory category)
    {
        
        return category switch
        {
            RecycleCategory.Plastik => plastikEffects,
            RecycleCategory.Kagit => kagitEffects,
            RecycleCategory.Cam => camEffects,
            RecycleCategory.Metal => metalEffects,
            _ => plastikEffects
        };
    }

    IEnumerator SpawnSingleEffect(Vector3 pos, Sprite sprite)
    {
        GameObject go = new GameObject("CleanEffect");
        go.transform.position = pos;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 30;

        float scale = Random.Range(0.2f, 0.5f);
        go.transform.localScale = Vector3.one * scale;

        float elapsed = 0f;
        while (elapsed < effectDuration)
        {
            elapsed += Time.deltaTime;
            float ratio = elapsed / effectDuration;

            go.transform.position += Vector3.up * effectMoveSpeed * Time.deltaTime;
            sr.color = new Color(1f, 1f, 1f, 1f - ratio);

            yield return null;
        }

        Destroy(go);
    }
}