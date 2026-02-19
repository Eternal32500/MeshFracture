using UnityEngine;
using UnityEngine.InputSystem;

public class FractureOnMouse : Fracture
{
    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if(hit.collider == null)
            return;

        if (hit.collider.gameObject == this.gameObject)
        {
            localizedFracturePoint = hit.point - (Vector2)transform.position;
            explosionOrigin = hit.point - (Vector2)transform.position;
            FractureNow();
        }
    }
}
