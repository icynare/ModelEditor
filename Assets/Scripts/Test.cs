using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    public Material m_FirstMat;

    private float _radius = 2.5f;
    private int _angle = 360;
    private int _parts = 4;
    private int _height = 1;

    private float _rotateSpeed = 3;
    private int _rotateDir = -1;

    private GameObject _cylinder;

    // Use this for initialization
    private void Start()
    {
        //CreateCube();
        //CreateCircle(_radius, _angle);
        _cylinder = CreateCylinder(_radius, _angle, _height);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_cylinder != null)
        {
            _cylinder.transform.Rotate(0, _rotateSpeed * _rotateDir, 0, Space.Self);
        }
    }

    private GameObject CreateCylinder(float radius, int angle, int height)
    {
        GameObject go = new GameObject("Cylinder");
        go.layer = 9;
        go.transform.SetParent(transform);
        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();

        int verLen = _parts + 2;
        Vector3[] vertices1 = new Vector3[verLen];
        Vector3[] vertices2 = new Vector3[verLen];
        Vector3[] vertices3 = new Vector3[4 * verLen];
        Vector3[] normals1 = new Vector3[verLen];
        Vector3[] normals2 = new Vector3[verLen];
        Vector3[] normals3 = new Vector3[4 * verLen];

        int triLen = 3 * _parts;
        int[] triangles1 = new int[triLen];
        int[] triangles2 = new int[triLen];
        int[] triangles3 = new int[2 * triLen + 12];

        float deltaAngle = Mathf.Deg2Rad * angle / _parts;
        float curAngle = deltaAngle;

        //添加所有顶点
        vertices1[0].Set(0, 0, 0);
        vertices2[0].Set(0, -height, 0);
        normals1[0] = Vector3.up;
        normals2[0] = Vector3.down;
        //上下两个平面的顶点
        curAngle = 0;
        for (int i = 1; i < verLen; i++)
        {
            vertices1[i].Set(radius * Mathf.Cos(curAngle), 0, radius * Mathf.Sin(curAngle));
            vertices2[i].Set(radius * Mathf.Cos(curAngle), -height, radius * Mathf.Sin(curAngle));
            //normals1[i] = Vector3.up;
            //normals2[i] = Vector3.down;
            curAngle += deltaAngle;
        }
        //侧面顶点
        curAngle = - deltaAngle / 2;
        for (int i = 4, j = 1; j < verLen - 1; i+=4, j++)
        {
            vertices3[i] = vertices1[j];
            vertices3[i + 1] = vertices2[j];
            vertices3[i + 2] = vertices1[j + 1];
            vertices3[i + 3] = vertices2[j + 1];
            normals3[i].Set(- Mathf.Cos(curAngle), 0, Mathf.Sin(curAngle));
            normals3[i + 1].Set( -Mathf.Cos(curAngle), 0, Mathf.Sin(curAngle));
            normals3[i + 2].Set(-Mathf.Cos(curAngle), 0, Mathf.Sin(curAngle));
            normals3[i + 3].Set(-Mathf.Cos(curAngle), 0, Mathf.Sin(curAngle));
            curAngle += deltaAngle;
        }
        vertices3[vertices3.Length - 2] = vertices1[0];
        vertices3[vertices3.Length - 1] = vertices2[0];

        normals3[0] = Vector3.back;
        normals3[1] = Vector3.back;
        normals3[2] = Vector3.back;
        normals3[3] = Vector3.back;




        if (angle == 360)
        {
            vertices1[verLen - 1] = vertices1[1];
            vertices2[verLen - 1] = vertices2[1];
        }


        //添加所有三角形
        int index = 0;
        for (int i = 0; i < triangles1.Length; i += 3, index++)
        {
            triangles1[i] = 0;
            triangles2[i] = verLen;
            triangles1[i + 1] = index + 2;
            triangles2[i + 1] = verLen + index + 1;
            triangles1[i + 2] = index + 1;
            triangles2[i + 2] = verLen + index + 2;
        }

        index = 0;
        for (int i = 6; i < triangles3.Length - 6; i += 6, index += 3)
        {
            triangles3[i] = triangles2[index + 1];
            triangles3[i + 1] = triangles1[index + 2];
            triangles3[i + 2] = triangles1[index + 1];

            triangles3[i + 3] = triangles2[index + 1];
            triangles3[i + 4] = triangles1[index + 1];
            triangles3[i + 5] = triangles2[index + 2];
        }

        if (angle != 360)
        {
            triangles3[0] = triangles2[0];
            triangles3[1] = triangles1[0];
            triangles3[2] = triangles1[2];
            triangles3[3] = triangles2[0];
            triangles3[4] = triangles1[2];
            triangles3[5] = triangles2[1];

            triangles3[triangles3.Length - 6] = triangles2[triangles2.Length - 1];
            triangles3[triangles3.Length - 5] = triangles1[triangles1.Length - 2];
            triangles3[triangles3.Length - 4] = triangles1[0];
            triangles3[triangles3.Length - 3] = triangles2[triangles2.Length - 1]; ;
            triangles3[triangles3.Length - 2] = triangles1[0];
            triangles3[triangles3.Length - 1] = triangles2[0];
        }

        //将三个面的顶点数组，三角形数组合并
        int[] triangles = new int[triangles1.Length + triangles2.Length + triangles3.Length];
        for (int i = 0; i < triangles1.Length; i++)
        {
            triangles[i] = triangles1[i];
        }

        for (int i = triangles1.Length, j = 0; j < triangles2.Length; i++, j++)
        {
            triangles[i] = triangles2[j];
        }

        for (int i = triangles1.Length + triangles2.Length, j = 0; j < triangles3.Length; i++, j++)
        {
            triangles[i] = triangles3[j];
        }

        //合并顶点
        Vector3[] vertices = new Vector3[6 * verLen];
        for (int i = 0; i < verLen; i++)
        {
            vertices[i] = vertices1[i];
        }
        for (int i = verLen, j = 0; i < 2 * verLen; i++, j++)
        {
            vertices[i] = vertices2[j];
        }
        for (int i = 2 * verLen, j = 0; i < 6 * verLen; i ++, j++)
        {
            vertices[i] = vertices3[j];
        }
        //将所有法向量合并
        Vector3[] normals = new Vector3[6 * verLen];
        for (int i = 0; i < verLen; i++)
        {
            normals[i] = normals1[i];
        }
        for (int i = verLen, j = 0; i < 2 * verLen; i++, j++)
        {
            normals[i] = normals2[j];
        }
        for (int i = 2 * verLen, j = 0; i < 6 * verLen; i++, j++)
        {
            normals[i] = normals3[j];
        }

        for (int i = 0; i < normals.Length; i++)
        {
            Debug.Log(">>>Normal:" + i + ":" + normals[i]);
        }



        mf.mesh.vertices = vertices;
        mf.mesh.triangles = triangles;
        mf.mesh.normals = normals;
        mr.material = m_FirstMat;
        //mf.mesh.RecalculateNormals();

        MeshCollider meshCollider = go.AddComponent<MeshCollider>();
        meshCollider.convex = true;


        return go;
    }

    #region 创建圆形
    void CreateCircle(float radius, int angle)
    {
        GameObject go = new GameObject("Circle");
        go.transform.SetParent(transform);
        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();

        Vector3[] vertices = new Vector3[_parts + 2];
        int[] triangles = new int[3 * _parts];

        float deltaAngle = Mathf.Deg2Rad * angle / _parts;
        float curAngle = deltaAngle;

        vertices[0].Set(0, 0, 0);
        for (int i = 1; i < vertices.Length; i++)
        {
            vertices[i].Set(radius * Mathf.Cos(curAngle), 0, radius * Mathf.Sin(curAngle));
            curAngle += deltaAngle;
        }

        if (angle == 360)
        {
            vertices[vertices.Length - 1] = vertices[1];
        }

        int index = 0;
        for (int i = 0; i < triangles.Length; i += 3, index++)
        {
            triangles[i] = 0;
            triangles[i + 1] = index + 2;
            triangles[i + 2] = index + 1;
            Debug.Log(">>>i:" + i + ">>>i+2:" + (i + 2) + ">>>index:" + index);
        }

        mf.mesh.vertices = vertices;
        mf.mesh.triangles = triangles;
        mr.material = m_FirstMat;

        MeshCollider meshCollider = go.AddComponent<MeshCollider>();
        meshCollider.convex = true;

    }
    #endregion

    #region 创建立方体
    void CreateCube()
    {
        GameObject go = new GameObject("Cube");
        go.transform.SetParent(transform);
        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();

        Vector3[] vertices = new Vector3[24];
        Vector3[] normals = new Vector3[24];
        int[] triangles = new int[36];

        //forward
        vertices[0].Set(0, 0, 0);
        vertices[1].Set(1, 0, 0);
        vertices[2].Set(0, 1, 0);
        vertices[3].Set(1, 1, 0);
        normals[0] = Vector3.back;
        normals[1] = Vector3.back;
        normals[2] = Vector3.back;
        normals[3] = Vector3.back;

        //back
        vertices[4].Set(1, 0, 1);
        vertices[5].Set(0, 0, 1);
        vertices[6].Set(1, 1, 1);
        vertices[7].Set(0, 1, 1);
        normals[4] = Vector3.forward;
        normals[5] = Vector3.forward;
        normals[6] = Vector3.forward;
        normals[7] = Vector3.forward;

        //up
        vertices[8] = vertices[1];
        vertices[9] = vertices[4];
        vertices[10] = vertices[3];
        vertices[11] = vertices[6];
        normals[8] = Vector3.up;
        normals[9] = Vector3.up;
        normals[10] = Vector3.up;
        normals[11] = Vector3.up;

        //down
        vertices[12] = vertices[5];
        vertices[13] = vertices[0];
        vertices[14] = vertices[7];
        vertices[15] = vertices[2];
        normals[12] = Vector3.down;
        normals[13] = Vector3.down;
        normals[14] = Vector3.down;
        normals[15] = Vector3.down;

        //left
        vertices[16] = vertices[2];
        vertices[17] = vertices[3];
        vertices[18] = vertices[7];
        vertices[19] = vertices[6];
        normals[16] = Vector3.left;
        normals[17] = Vector3.left;
        normals[18] = Vector3.left;
        normals[19] = Vector3.left;

        //right
        vertices[20] = vertices[5];
        vertices[21] = vertices[4];
        vertices[22] = vertices[0];
        vertices[23] = vertices[1];
        normals[20] = Vector3.right;
        normals[21] = Vector3.right;
        normals[22] = Vector3.right;
        normals[23] = Vector3.right;

        int curCount = 0;
        for (int i = 0; i < 24; i = i + 4)
        {
            triangles[curCount++] = i;
            triangles[curCount++] = i + 3;
            triangles[curCount++] = i + 1;

            triangles[curCount++] = i;
            triangles[curCount++] = i + 2;
            triangles[curCount++] = i + 3;
        }

        mf.mesh.vertices = vertices;
        mf.mesh.triangles = triangles;
        mf.mesh.normals = normals;
        mr.material = m_FirstMat;

        MeshCollider meshCollider = go.AddComponent<MeshCollider>();
        meshCollider.convex = true;
    }
}
#endregion
