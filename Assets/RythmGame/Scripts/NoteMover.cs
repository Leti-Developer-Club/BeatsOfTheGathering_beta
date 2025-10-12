// NoteMover.cs
using UnityEngine;

public class NoteMover : MonoBehaviour
{
    public float speed = 8f;      // Units per second
    public float despawnY = -10f; // Where notes get turned off

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.Self);

        if (transform.position.y < despawnY)
            gameObject.SetActive(false);
    }
}
