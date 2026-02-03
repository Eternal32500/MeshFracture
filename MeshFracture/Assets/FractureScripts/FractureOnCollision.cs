using UnityEngine;

public class FractureOnCollision : Fracture
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.GetContact(0);

        localizedFracturePoint = contact.point;
        localizedFracturePoint -= (Vector2)transform.position;
        checkIsInside = false;
        FractureNow();
    }
}
