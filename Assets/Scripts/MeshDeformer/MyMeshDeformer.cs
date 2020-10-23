using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MyMeshDeformer : MonoBehaviour
{
    Mesh deformingMesh;//获得对应物体的网格
    Vector3[] originalVertices, displacedVertices;//用于记录物体网格的原始顶点位置和形变过程中的顶点位置
    Vector3[] vertexVelocities;//用于存储每个顶点的速度
    public float springForce = 20f;//弹簧力
    public float damping = 5f;//阻尼
    float uniformScale = 1f;

    void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }
        vertexVelocities = new Vector3[originalVertices.Length];
    }

    public void AddDeformingForce(Vector3 point, float force)
    {
        point = transform.InverseTransformPoint(point);//把世界坐标转换成物体的坐标，受缩放影响
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            AddForceToVertex(i, point, force);
        }
    }

    //针对point受到的力,然后根据距离获得每个点受到的力
    void AddForceToVertex(int i, Vector3 point, float force)
    {
        Vector3 pointToVertex = displacedVertices[i] - point;
        pointToVertex *= uniformScale;
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        float velocity = attenuatedForce * Time.deltaTime;
        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }

    void Update()
    {
        uniformScale = transform.localScale.x;
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            UpdateVertex(i);
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
    }

    //更新每个时间步长受到的力，根据弹簧力和阻尼缩小（弹簧质点系统）
    void UpdateVertex(int i)
    {
        Vector3 velocity = vertexVelocities[i];
        Vector3 displacement = displacedVertices[i] - originalVertices[i];
        displacement *= uniformScale;
        velocity -= displacement * springForce * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime;
        vertexVelocities[i] = velocity;
        displacedVertices[i] += velocity * (Time.deltaTime / uniformScale);
    }
}
