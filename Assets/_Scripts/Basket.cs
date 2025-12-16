using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    void OnCollisionEnter(Collision coll)
    {
        // Find out what hit this basket
        GameObject collidedWith = coll.gameObject;

        if (collidedWith.CompareTag("Apple"))
        {
            Destroy(collidedWith);
            // Normal apple gives 100 points
            ScoreManager.Instance.AddPoints(100);

            // Play Normal Apple SFX
            AudioManager.Instance.PlayAppleSfx(AppleType.Normal);
        }
        else if (collidedWith.CompareTag("GoldApple"))
        {
            Destroy(collidedWith);
            // Gold apple gives 300 points
            ScoreManager.Instance.AddPoints(300);

            // Play Gold Apple SFX
            AudioManager.Instance.PlayAppleSfx(AppleType.Gold);
        }
        else if (collidedWith.CompareTag("PoisonApple"))
        {
            Destroy(collidedWith);
            // Poison apple deducts 200 points
            ScoreManager.Instance.AddPoints(-200);

            // Play Poison Apple SFX
            AudioManager.Instance.PlayAppleSfx(AppleType.Poison);
        }
    }

    void Update()
    {
        // Get the current screen position of the mouse from Input
        Vector3 mousePos2D = Input.mousePosition;

        // The Camera’s z position sets how far to push the mouse into 3D
        // If this line causes a NullReferenceException, select the Main Camera
        // in the Hierarchy and set its tag to MainCamera in the Inspector.
        mousePos2D.z = -Camera.main.transform.position.z;

        // Convert the point from 2D screen space into 3D game world space
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        // Move the x position of this Basket to the x position of the Mouse
        Vector3 pos = this.transform.position;
        pos.x = mousePos3D.x;
        this.transform.position = pos;
    }
}
