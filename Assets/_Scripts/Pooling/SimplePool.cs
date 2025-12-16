using UnityEngine;
using System.Collections.Generic;

public class SimplePool : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] int warmup = 20;
    readonly Queue<GameObject> q = new();

    void Awake()
    {
        for (int i = 0; i < warmup; i++)
        {
            var go = Instantiate(prefab, transform);
            go.SetActive(false);
            q.Enqueue(go);
        }
    }

    public GameObject Get(Vector3 pos, Quaternion rot)
    {
        GameObject go = null;

        while (q.Count > 0 && go == null)
        {
            go = q.Dequeue();
        }

        if (go == null)
        {
            go = Instantiate(prefab, transform);
        }

        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);
        return go;
    }

    public void Return(GameObject go)
    {
        if (go == null) return;

        go.SetActive(false);
        q.Enqueue(go);
    }
}
