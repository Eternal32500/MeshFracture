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
        Gizmos.DrawSphere(transform.TransformPoint(fracturePoint), sphereSize);

        // Cellules
        Gizmos.color = Color.green;
        for (int i = 0; i < voronoiCell.Count; i++)
        {
            Vector3 a = transform.TransformPoint(voronoiCell[i]);
            Vector3 b = transform.TransformPoint(voronoiCell[(i + 1) % voronoiCell.Count]);

            Gizmos.DrawLine(a, b);
        }
    }
}
