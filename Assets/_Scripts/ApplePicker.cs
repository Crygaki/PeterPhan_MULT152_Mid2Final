using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplePicker : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject basketPrefab;
    public int numBaskets = 3;
    public float basketBottomY = -14f;
    public float basketSpacingY = 2f;
    public List<GameObject> basketList;
    public int score;

    void Start()
    {
        basketList = new List<GameObject>();

        for (int i = 0; i < numBaskets; i++)
        {
            GameObject tBasketGO = Instantiate<GameObject>(basketPrefab);
            Vector3 pos = Vector3.zero;
            pos.y = basketBottomY + (basketSpacingY * i);
            tBasketGO.transform.position = pos;
            basketList.Add(tBasketGO);
        }
    }

    public void AppleMissed(GameObject missedApple)
    {
        // Destroy the missed apple
        Destroy(missedApple);

        if (missedApple.CompareTag("Apple"))
        {
            RemoveBasket();
        }
        else if (missedApple.CompareTag("GoldApple"))
        {
            RemoveBasket();
        }
        else if (missedApple.CompareTag("PoisonApple"))
        {
            return;
        }
    }

    private void RemoveBasket()
    {
        if (basketList.Count == 0) return;

        int basketIndex = basketList.Count - 1;
        GameObject basketGO = basketList[basketIndex];

        basketList.RemoveAt(basketIndex);

        // --- Play Basket Removed SFX ---
        AudioManager.Instance.PlayBasketRemovedSfx();

        Destroy(basketGO);

        if (basketList.Count == 0)
        {
            ScoreManager.Instance.ReloadScene();
        }
    }
}
