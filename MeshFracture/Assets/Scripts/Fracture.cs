using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Fracture : MonoBehaviour
{
    [Header("Fracture Settings")]
    public bool randomPoints = true;
    public int fractureCount = 8;
    public int randomSeed = 0;

    private Sprite sprite;

    private List<Vector2> spritePolygon = new List<Vector2>();
    private List<Vector2> fracturePoints = new List<Vector2>();
    private List<List<Vector2>> voronoiCells = new List<List<Vector2>>();

    void Start()
    {
        if(randomPoints)
        {
            int seed = Random.Range(int.MinValue, int.MaxValue);
            Random.InitState(seed);
        }
        else
            Random.InitState(randomSeed);

        sprite = GetComponent<SpriteRenderer>().sprite;
        sprite.GetPhysicsShape(0, spritePolygon);

        GenerateFracturePoints();
        ComputeVoronoiCells();

    }

    void GenerateFracturePoints()
    {
        fracturePoints.Clear();

        Bounds localBounds = ComputeLocalBounds(spritePolygon);

        while (fracturePoints.Count < fractureCount)
        {
            Vector2 p = new Vector2(
                Random.Range(localBounds.min.x, localBounds.max.x),
                Random.Range(localBounds.min.y, localBounds.max.y)
            );

            if (PointInPolygon(p, spritePolygon))
                fracturePoints.Add(p);
        }
    }

    Bounds ComputeLocalBounds(List<Vector2> poly)
    {
        Vector2 min = poly[0];
        Vector2 max = poly[0];

        foreach (Vector2 p in poly)
        {
            min = Vector2.Min(min, p);
            max = Vector2.Max(max, p);
        }

        Bounds b = new Bounds();
        b.SetMinMax(min, max);
        return b;
    }

    bool PointInPolygon(Vector2 p, List<Vector2> poly)
    {
        bool inside = false;

        for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
        {
            if ((poly[i].y > p.y) != (poly[j].y > p.y) &&
                p.x < (poly[j].x - poly[i].x) * (p.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x)
            {
                inside = !inside;
            }
        }

        return inside;
    }

    void ComputeVoronoiCells()
    {
        voronoiCells.Clear();

        foreach (Vector2 site in fracturePoints)
        {
            List<Vector2> cell = new List<Vector2>(spritePolygon);

            foreach (Vector2 other in fracturePoints)
            {
                if (other == site) continue;

                cell = ClipPolygonByVoronoiPlane(cell, site, other);
                if (cell.Count < 3)
                    break;
            }

            if (cell.Count >= 3)
                voronoiCells.Add(cell);
        }
    }

    bool IsInsideVoronoiHalfPlane(Vector2 p, Vector2 A, Vector2 B)
    {
        return Vector2.Distance(p, A) <= Vector2.Distance(p, B);
    }

    List<Vector2> ClipPolygonByVoronoiPlane(
        List<Vector2> polygon,
        Vector2 A,
        Vector2 B)
    {
        List<Vector2> result = new List<Vector2>();

        for (int i = 0; i < polygon.Count; i++)
        {
            Vector2 current = polygon[i];
            Vector2 prev = polygon[(i - 1 + polygon.Count) % polygon.Count];

            bool currInside = IsInsideVoronoiHalfPlane(current, A, B);
            bool prevInside = IsInsideVoronoiHalfPlane(prev, A, B);

            if (currInside)
            {
                if (!prevInside)
                    result.Add(Intersect(prev, current, A, B));

                result.Add(current);
            }
            else if (prevInside)
            {
                result.Add(Intersect(prev, current, A, B));
            }
        }

        return result;
    }

    Vector2 Intersect(Vector2 p1, Vector2 p2, Vector2 A, Vector2 B)
    {
        Vector2 mid = (A + B) * 0.5f;
        Vector2 normal = (B - A).normalized;
        Vector2 dir = p2 - p1;

        float denom = Vector2.Dot(dir, normal);
        if (Mathf.Abs(denom) < 0.00001f)
            return p1;

        float t = Vector2.Dot(mid - p1, normal) / denom;
        return p1 + dir * t;
    }


    void OnDrawGizmos()
    {
        // Points Voronoï
        Gizmos.color = Color.red;
        foreach (Vector2 p in fracturePoints)
            Gizmos.DrawSphere(p, 0.01f);

        // Cellules
        Gizmos.color = Color.green;
        foreach (var cell in voronoiCells)
        {
            for (int i = 0; i < cell.Count; i++)
            {
                Vector2 a = cell[i];
                Vector2 b = cell[(i + 1) % cell.Count];
                Gizmos.DrawLine(a, b);
            }
        }
    }
}
