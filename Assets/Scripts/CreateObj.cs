using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObj : MonoBehaviour {

    public Material m_FirstMat;

    private float _radius = 2.5f;
    private int _angle = 120;
    private int _parts = 40;
    private float _height = 1;

    private float _rotateSpeed = 3;
    private int _rotateDir = -1;

    private GameObject _cylinder;

	// Use this for initialization
	void Start () {
        _cylinder = CreateCylinder(_radius, _angle, _height);
        _cylinder.transform.localPosition = new Vector3(0, 5, 0);
        GameObject obj = CreateCylinder(2, 90, 5f);
        obj.transform.localPosition = new Vector3(0, -3, 0);
	}
	
	// Update is called once per frame
	void Update () {
        if (_cylinder != null)
        {
            _cylinder.transform.Rotate(0, _rotateSpeed * _rotateDir, 0, Space.Self);
        }
	}

    private GameObject CreateCylinder(float radius, int angle, float height)
    {
        GameObject go = new GameObject("Cylinder");
        go.transform.SetParent(transform);
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        int verLen = 6 * (_parts + 2);
        Vector3[] vertices = new Vector3[verLen];
        Vector3[] normals = new Vector3[verLen];
        int triLen = 12 * (_parts + 1);
        int[] triangles = new int[triLen];

        float deltaAngle = Mathf.Deg2Rad * angle / _parts;
        float curAngle = deltaAngle;

        float upH = height / 2;
        float downH = -height / 2;
        vertices[0].Set(0, upH, 0);
        vertices[1].Set(0, downH, 0);
        vertices[2].Set(radius, upH, 0);
        vertices[3].Set(radius, downH, 0);
        normals[0] = Vector3.up;
        normals[1] = Vector3.down;
        normals[2] = Vector3.up;
        normals[3] = Vector3.down;

        float curX = 0;
        float curZ = 0;
        int i = 4;
        int j = 0;
        int upDownCount = 2 * (_parts + 2);
        //设置上下表面 顶点，法线，三角形
        for (; i < upDownCount; i += 2, j += 6)
        {
            curX = radius * Mathf.Cos(curAngle);
            curZ = radius * Mathf.Sin(curAngle);

            vertices[i].Set(curX, upH, curZ);
            vertices[i + 1].Set(curX, downH, curZ);
            normals[i] = Vector3.up;
            normals[i + 1] = Vector3.down;
            triangles[j] = 0;
            triangles[j + 1] = i;
            triangles[j + 2] = i - 2;
            triangles[j + 3] = 1;
            triangles[j + 4] = i - 1;
            triangles[j + 5] = i + 1;

            curAngle += deltaAngle;
        }

        //设置侧面
        curAngle = 0;
        for (int k = 0; i < vertices.Length; i += 4, j += 6, k += 2)
        {
            vertices[i] = vertices[k];
            vertices[i + 1] = vertices[k + 1];
            vertices[i + 2] = vertices[k + 2];
            vertices[i + 3] = vertices[k + 3];

            curX = Mathf.Cos(curAngle);
            curZ = Mathf.Sin(curAngle);
            normals[i].Set(curX, 0, curZ);
            normals[i + 1].Set(curX, 0, curZ);
            normals[i + 2].Set(curX, 0, curZ);
            normals[i + 3].Set(curX, 0, curZ);

            triangles[j] = i;
            triangles[j + 1] = i + 2;
            triangles[j + 2] = i + 1;
            triangles[j + 3] = i + 1;
            triangles[j + 4] = i + 2;
            triangles[j + 5] = i + 3;

            curAngle += deltaAngle;
        }

        //两片横截面特殊处理
        normals[upDownCount] = Vector3.back;
        normals[upDownCount + 1] = Vector3.back;
        normals[upDownCount + 2] = Vector3.back;
        normals[upDownCount + 3] = Vector3.back;

        curX = - Mathf.Sin(curAngle);
        curZ = Mathf.Cos(curAngle);
        normals[verLen - 4].Set(curX, 0, curZ);
        normals[verLen - 3].Set(curX, 0, curZ);
        normals[verLen - 2].Set(curX, 0, curZ);
        normals[verLen - 1].Set(curX, 0, curZ);


        mf.mesh.vertices = vertices;
        mf.mesh.triangles = triangles;
        mf.mesh.normals = normals;
        mr.material = m_FirstMat;
        //mf.mesh.RecalculateNormals();

        MeshCollider meshCollider = go.AddComponent<MeshCollider>();
        meshCollider.convex = true;


        return go;
    }
}
