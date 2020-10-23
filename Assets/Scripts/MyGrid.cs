using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MyGrid : MonoBehaviour
{
    public int xSize, ySize;
    private Vector3[] vertices;
    private Mesh mesh;

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];//顶点
        Vector2[] uv = new Vector2[vertices.Length];//uv映射
        Vector4[] tangents = new Vector4[vertices.Length];//法线方向
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);//默认法线方向
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                //设置顶点
                vertices[i] = new Vector3(x, y);
                //设置UV映射坐标  只有在0到1之间的坐标值才可以正确的跨越整个Grid，所以我们必须确保除法是使用浮点数来计算的。
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                //设置法线方向
                tangents[i] = tangent;
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[xSize * ySize * 6];//三角形
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
        //让Mesh自己依据它的三角形去寻找自己的法线
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        //显示每个顶点
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
