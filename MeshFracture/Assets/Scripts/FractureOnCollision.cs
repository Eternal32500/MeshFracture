using UnityEngine;

public class FractureOnCollision : Fracture
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        FractureNow();
    }
}
