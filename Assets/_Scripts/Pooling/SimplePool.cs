using UnityEngine;
using System.Collections.Generic;

public class SimplePool : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] int warmup = 20;
    readonly Queue<GameObject> q = new();

    void Awake(){
        for (int i=0;i<warmup;i++){
            var go = Instantiate(prefab, transform);
            //go.SetActive(false);
            q.Enqueue(go);
        }
    }

    public GameObject Get(Vector3 pos, Quaternion rot){
        var go = q.Count>0 ? q.Dequeue() : Instantiate(prefab, transform);
        go.transform.SetPositionAndRotation(pos, rot);
        //go.SetActive(true);
        return go;
    }

    public void Return(GameObject go){
        //go.SetActive(false);
        q.Enqueue(go);
    }
}