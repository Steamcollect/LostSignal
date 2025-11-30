using FischlWorks_FogWar;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FieldOfView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_ViewRadius;
    [Range(0, 360)]
    [SerializeField] private float m_ViewAngle; 
    [SerializeField] private float m_ProximityRadius = 3f;

    [SerializeField] private LayerMask m_TargetMask;
    [SerializeField] private LayerMask m_ObstacleMask;

    [SerializeField] private float meshResolution;
    [SerializeField] private MeshFilter viewMeshFilter;

    [SerializeField] private Transform m_Canon;
    
    private List<GameObject> m_VisibleTargets = new List<GameObject>();

    private int edgeResolveIterations;
    private float edgeDstThreshold;

    private Mesh viewMesh;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    private void Update()
    {
        FindVisibleTargets();

        if (m_Canon != null)
        {
            Quaternion newRotation = Quaternion.Euler(0, m_Canon.rotation.eulerAngles.y, 0);
            transform.rotation = newRotation;
        }
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    private void FindVisibleTargets()
    {
        List<GameObject> targetsPreviouslyVisible = new List<GameObject>(m_VisibleTargets);
        m_VisibleTargets.Clear();

        float searchRadius = Mathf.Max(m_ViewRadius, m_ProximityRadius);
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, searchRadius, m_TargetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            GameObject target = targetsInViewRadius[i].gameObject;
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

            bool inProximityRange = dstToTarget < m_ProximityRadius;

            bool inViewCone = (dstToTarget < m_ViewRadius) && (Vector3.Angle(transform.forward, dirToTarget) < m_ViewAngle / 2);

            if (inProximityRange || inViewCone)
            {
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, m_ObstacleMask))
                {
                    m_VisibleTargets.Add(target);
                }
            }
        }

        foreach (GameObject oldTarget in targetsPreviouslyVisible)
        {
            if (oldTarget != null && !m_VisibleTargets.Contains(oldTarget))
            {
                FogVisibilityObject fogScript = oldTarget.GetComponentInChildren<FogVisibilityObject>();

                if(fogScript != null)
                {
                    fogScript.m_Visibility = false;
                }
            }
        }

        foreach (GameObject newVisibleTarget in m_VisibleTargets)
        {
            if (newVisibleTarget != null)
            {
                FogVisibilityObject fogScript = newVisibleTarget.GetComponentInChildren<FogVisibilityObject>();

                if (fogScript != null && !fogScript.m_Visibility) fogScript.m_Visibility = true;
            }
        }
    }

    private void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(m_ViewAngle * meshResolution);
        float stepAngleSize = m_ViewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - m_ViewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.Distance - newViewCast.Distance) > edgeDstThreshold;
                if (oldViewCast.Hit != newViewCast.Hit || (oldViewCast.Hit && newViewCast.Hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.PointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointA);
                    }
                    if (edge.PointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.Point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }


    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.Angle;
        float maxAngle = maxViewCast.Angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.Distance - newViewCast.Distance) > edgeDstThreshold;
            if (newViewCast.Hit == minViewCast.Hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.Point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.Point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }


    private ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, m_ViewRadius, m_ObstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * m_ViewRadius, m_ViewRadius, globalAngle);
        }
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private struct ViewCastInfo
    {
        public bool Hit;
        public Vector3 Point;
        public float Distance;
        public float Angle;

        public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
        {
            Hit = hit;
            Point = point;
            Distance = distance;
            Angle = angle;
        }
    }

    private struct EdgeInfo
    {
        public Vector3 PointA;
        public Vector3 PointB;

        public EdgeInfo(Vector3 pointA, Vector3 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, m_ViewRadius);
        Vector3 viewAngleA = DirFromAngle(-m_ViewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(m_ViewAngle / 2, false);

        Handles.DrawLine(transform.position, transform.position + viewAngleA * (m_ViewRadius + 0.5f));
        Handles.DrawLine(transform.position, transform.position + viewAngleB * (m_ViewRadius + 0.5f));

        Handles.color = Color.red;
        foreach (GameObject visibleTarget in m_VisibleTargets)
        {
            Handles.DrawLine(transform.position, visibleTarget.transform.position);
        }
    }
#endif
}