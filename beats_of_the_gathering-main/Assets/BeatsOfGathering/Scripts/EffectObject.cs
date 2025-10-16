using UnityEngine;

public class EffectObject : MonoBehaviour
{
    public float lifeTime = 1;
    
    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, lifeTime);
    }
}