using System.Collections.Generic;
using UnityEngine;

public class FractureDebug : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    private float sphereSize = 0.02f;

    private Vector3 fracturePoint;
    private List<Vector2> voronoiCell = new List<Vector2>();

    public void SetFracturePoint(Vector3 point)
    {
        fracturePoint = point;
    }

    public void SetVoronoiCell(List<Vector2> cell)
    {
        voronoiCell = cell;
    }

    private void OnDrawGizmos()
    {
        // Points Voronoï
        Gizmos.color = Color.red;
        Vector3 p = fracturePoint;

        p.x += transform.position.x;
        p.y += transform.position.y;
        p.z += transform.position.z;

        Gizmos.DrawSphere(p, sphereSize);

        // Cellules
        Gizmos.color = Color.green;
        for (int i = 0; i < voronoiCell.Count; i++)
        {
            Vector3 a = voronoiCell[i];
            Vector3 b = voronoiCell[(i + 1) % voronoiCell.Count];

            a.x += transform.position.x;
            a.y += transform.position.y;
            a.z += transform.position.z;

            b.x += transform.position.x;
            b.y += transform.position.y;
            b.z += transform.position.z;

            Gizmos.DrawLine(a, b);
        }
    }
}
