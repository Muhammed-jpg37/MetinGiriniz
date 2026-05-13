using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConveyorBelt : MonoBehaviour
{
    [Header("Ayarlar")]
    public List<RecyclableItem> itemPool;
    public GameObject itemPrefab;
    public Transform spawnPoint;
    public Transform despawnPoint;
    public float spawnInterval = 2.5f;
    public float beltSpeed = 1.5f;

    private Coroutine _spawnRoutine;
    private bool _running = false;

    void Start()
    {
       
        StartBelt();
    }

    public void StartBelt()
    {
       
        _running = true;
        _spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopBelt()
    {
        _running = false;
        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);
    }

    IEnumerator SpawnLoop()
    {
       
        while (_running)
        {
            SpawnItem();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnItem()
    {

        if (itemPool == null || itemPool.Count == 0) return;

        RecyclableItem data = itemPool[Random.Range(0, itemPool.Count)];
        GameObject go = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);

        go.GetComponent<DraggableItem>().Initialize(data);

        ConveyorMover mover = go.GetComponent<ConveyorMover>();
        mover.speed = beltSpeed;
        mover.despawnX = despawnPoint.position.x;
    }
}