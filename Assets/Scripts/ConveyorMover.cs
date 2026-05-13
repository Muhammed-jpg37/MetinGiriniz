using UnityEngine;

public class ConveyorMover : MonoBehaviour
{
    public float speed = 1.5f;
    public float despawnX = -6f;

    private bool _paused = false;

    public void Pause() => _paused = true;
    public void Resume() => _paused = false;

    void Update()
    {
        if (_paused) return;

        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < despawnX)
            Destroy(gameObject);
    }
}