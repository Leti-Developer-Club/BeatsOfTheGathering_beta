using UnityEngine;
using System.Collections.Generic;

public class EffectPool : MonoBehaviour
{
    public static EffectPool Instance;
    public GameObject effectPrefab; 
    public int poolSize = 20;
    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject effect = Instantiate(effectPrefab);
            effect.SetActive(false);
            pool.Enqueue(effect);
        }
    }

    public GameObject GetEffect(Vector3 position, Quaternion rotation)
    {
        if (pool.Count == 0)
        {
            GameObject effect = Instantiate(effectPrefab);
            pool.Enqueue(effect);
        }
        GameObject pooledEffect = pool.Dequeue();
        pooledEffect.transform.position = position;
        pooledEffect.transform.rotation = rotation;
        pooledEffect.SetActive(true);
        return pooledEffect;
    }

    public void ReturnEffect(GameObject effect)
    {
        effect.SetActive(false);
        pool.Enqueue(effect);
    }
}