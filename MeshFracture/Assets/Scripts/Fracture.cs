using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Fracture : MonoBehaviour
{
    [Header("Fracture Settings")]
    [SerializeField] private bool randomPoints = true;
    [SerializeField] private int fractureCount = 10;
    [SerializeField] private int seed = 0;

    [Header("Physics Settings")]
    [SerializeField] private bool enablePhysics = true;
    [SerializeField] private bool enableCollision = true;
    [SerializeField] private bool enableExplosionForce = true;
    [SerializeField] private float explosionForce = 2f;

    [Header("Localized Fracture Settings")]
    [SerializeField] private bool localizedFracture = false;
    [SerializeField] private Vector2 localizedFracturePoint = Vector2.zero;
    [SerializeField] private int localizedFractureCount = 5;
    [SerializeField] private float localizedMaxDistance = 2f;
    [SerializeField] private float localizedFalloff = 2.5f;
    private Sprite sprite;
    private int meshCreated = 0;

    private GameObject fracturedParent;
    private List<Vector2> spritePolygon = new List<Vector2>();
    private List<Vector2> fracturePoints = new List<Vector2>();
    private List<List<Vector2>> voronoiCells = new List<List<Vector2>>();

    protected void FractureNow()
    {
        meshCreated = 0;
        Destroy(gameObject);
        GetSpritePolygons();
        GenerateFracturePoints();
        ComputeVoronoiCells();
        GenerateFracturesMeshes();
    }

    void GetSpritePolygons()
    {
        sprite = GetComponent<SpriteRenderer>().sprite;
        sprite.GetPhysicsShape(0, spritePolygon);

        Vector3 scale = transform.localScale;
        for (int i = 0; i < spritePolygon.Count; i++)
        {
            spritePolygon[i] = new Vector2(
                spritePolygon[i].x * scale.x,
                spritePolygon[i].y * scale.y
            );
        }
    }

    void GenerateFracturePoints()
    {
        fracturePoints.Clear();

        Bounds localBounds = ComputeLocalBounds(spritePolygon);

        if (randomPoints)
        {
            int seed = Random.Range(int.MinValue, int.MaxValue);
            Random.InitState(seed);
        }
        else
            Random.InitState(seed);

        while (fracturePoints.Count < fractureCount)
        {
            Vector2 p = new Vector2(
                Random.Range(localBounds.min.x, localBounds.max.x),
                Random.Range(localBounds.min.y, localBounds.max.y)
            );

            if (PointInPolygon(p, spritePolygon))
                fracturePoints.Add(p);
        }

        if (localizedFracture)
        {
            if (!PointInPolygon(localizedFracturePoint, spritePolygon))
            {
                Debug.LogWarning(
                    $"[Fracture] Localized fracture point is outside sprite mesh: {localizedFracturePoint}"
                );
                return;
            }

            int added = 0;

            while (added < localizedFractureCount)
            {
                Vector2 dir = Random.insideUnitCircle.normalized;

                float t = Mathf.Pow(Random.value, localizedFalloff);
                float distance = t * localizedMaxDistance;

                Vector2 p = localizedFracturePoint + dir * distance;

                if (PointInPolygon(p, spritePolygon))
                {
                    fracturePoints.Add(p);
                    added++;
                }
            }
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

    void CreateMeshFromCell(List<Vector2> cell)
    {
        Mesh mesh = new Mesh();

        Vector2 meshCenter = ComputeMeshCenter(cell);
        for (int i = 0; i < cell.Count; i++)
        {
            cell[i] -= meshCenter;
        }

        Vector3[] vertices = new Vector3[cell.Count];
        for (int i = 0; i < cell.Count; i++)
        {
            vertices[i] = cell[i];
        }
        mesh.vertices = vertices;

        int[] triangles = new int[(cell.Count - 2) * 3];
        for (int i = 0; i < cell.Count - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GameObject cellObj = new GameObject("FractureCell");

        FractureDebug debug = cellObj.AddComponent<FractureDebug>();
        debug.SetFracturePoint(cellObj.transform.position);
        debug.SetVoronoiCell(voronoiCells[meshCreated]);

        MeshFilter mf = cellObj.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        MeshRenderer mr = cellObj.AddComponent<MeshRenderer>();
        mr.material = GetComponent<SpriteRenderer>().material;
        mr.enabled = true;

        cellObj.transform.position = (Vector3)meshCenter + transform.position;
        cellObj.transform.parent = fracturedParent.transform;

        if(enableCollision)
            AddPolygonCollider(cellObj, cell);

        if (enablePhysics)
            cellObj.AddComponent<Rigidbody2D>();

        if(enableExplosionForce)
            AddExplosionForce(cellObj);

        meshCreated++;
    }

    void GenerateFracturesMeshes()
    {
        fracturedParent = new GameObject("FracturedGameObject");

        for (int i = 0; i < voronoiCells.Count; i++)
        {
            CreateMeshFromCell(voronoiCells[i]);
        }
    }

    Vector2 ComputeMeshCenter(List<Vector2> vertices)
    {
        Vector2 position = Vector2.zero;
        for (int i = 0; i < vertices.Count; i++)
        {
            position += vertices[i];
        }
        return position /= vertices.Count;
    }

    void AddExplosionForce(GameObject cellObj)
    {
        Rigidbody2D rb;

        if (enablePhysics)
            rb = cellObj.GetComponent<Rigidbody2D>();
        else
            rb = cellObj.AddComponent<Rigidbody2D>();


        Vector3 explosionCenter = transform.position;
        Vector2 forceDir = cellObj.transform.position - explosionCenter;

        float distance = Mathf.Max(forceDir.magnitude, 0.01f);
        float force = explosionForce / distance;

        rb.AddForce(forceDir.normalized * force, ForceMode2D.Impulse);
    }

    void AddPolygonCollider(GameObject cellObj, List<Vector2> cell)
    {
        PolygonCollider2D collider = cellObj.AddComponent<PolygonCollider2D>();

        Vector2[] vertices2D = new Vector2[cell.Count];
        Vector3[] vertices = cellObj.GetComponent<MeshFilter>().mesh.vertices;

        for (int i = 0; i < cell.Count; i++)
        {
            vertices2D[i] = vertices[i];
        }

        collider.SetPath(0, vertices2D);
    }
}
