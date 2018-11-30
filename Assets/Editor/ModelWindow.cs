using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ModelWindow : EditorWindow {

    private GameObject parentObj;
    private CreateObj objCreater;
    private Transform camera;
    private Transform pillar;

    private float labelWidth = 100;
    private float labelHeight = 20;

    private GameObject ball;
    private float ballScale = 0;
    private string newScale;

    private string newSpeed;
    private string newWidth;
    private string newGravity;
    private string newUpSpeed;
    private string newUpHeight;
    private string newGap;

    private GameObject cubePrefab;
    private GameObject cylinderPrefab;

    private List<GameObject> objList;
    private string[] objName = { "Sector", "Cylinder", "Cube" };
    private const int MAX_COUNT = 200;
    private string[] objThickArr = new string[MAX_COUNT];
    private string[] objAngleArr = new string[MAX_COUNT];
    private PARTS[] objPartsArr = new PARTS[MAX_COUNT];

    private ObjSaver objSaver;
    private ObjDataBase objData;

    private GameObject curObj;

    private float curThickness = 0;

    [MenuItem("Tools/建造模型")]
    public static void TryCreateModel()
    {
        ModelWindow window = (ModelWindow)EditorWindow.GetWindow(typeof(ModelWindow), true, "模型建造选项", true);
        window.Show();
        window.Init();
    }

    private void Init()
    {
        parentObj = GameObject.Find("Parent");
        camera = GameObject.Find("Main Camera").transform;
        pillar = GameObject.Find("Pillar").transform;

        ball = GameObject.Find("Ball");
        ballScale = ball.transform.localScale.x;
        newScale = ballScale.ToString();

        newSpeed = PlayerPrefs.GetFloat(PrefConstans.RORATE_SPEED, 1).ToString();
        newWidth = PlayerPrefs.GetFloat(PrefConstans.GAMEOBJECT_WIDTH, 2.5f).ToString();
        newGravity = PlayerPrefs.GetFloat(PrefConstans.GRAVITY, 15).ToString();
        newUpSpeed = PlayerPrefs.GetFloat(PrefConstans.UP_SPEED, 4).ToString();
        newUpHeight = PlayerPrefs.GetFloat(PrefConstans.UP_DISTANCE, 5).ToString();
        newGap = PlayerPrefs.GetFloat(PrefConstans.GAMEOBJECT_GAP, 5).ToString();

        objSaver = new ObjSaver();
        objSaver.Init();
        objData = objSaver.Load();
        SetArrData();

        cubePrefab = Resources.Load<GameObject>("Models/Cube");
        cylinderPrefab = Resources.Load<GameObject>("Models/Cylinder");

        objList = new List<GameObject>();
        InitSavedObj();
        //for (int i = 0; i < parentObj.transform.childCount; i++)
        //{
        //    objList.Add(parentObj.transform.GetChild(i).gameObject);
        //}

        objCreater = new CreateObj();
    }

    private void InitSavedObj()
    {
        if (objData == null)
            return;
        for (int i = 0; i < objData.modelList.Count; i ++)
        {
            GenerateObj(objData.modelList[i]);
        }
    }

    private void GenerateObj(ModelData data)
    {
        switch (data.modelType)
        {
            case ModelType.Sector:
                {
                    GameObject sector = objCreater.CreateCylinder(2.5f, 120, 1);
                    CreateNewModel(ModelType.Sector, sector, false);
                    SetNewParts(sector, data.parts);
                    break;
                }
            case ModelType.Cylinder:
                {
                    GameObject cy = Instantiate(cylinderPrefab);
                    float curWidth = PlayerPrefs.GetFloat(PrefConstans.GAMEOBJECT_WIDTH, 2.5f);
                    cy.transform.localScale = new Vector3(1, curWidth / 2, 1);
                    cy.transform.localPosition += new Vector3(curWidth / 2, 0, 0);
                    CreateNewModel(ModelType.Cylinder, cy, true);
                    break;
                }
            case ModelType.Cube:
                {
                    break;
                }
        }
    }

    private void OnGUI()
    {
        #region 基本参数
        GUILayout.Label("基本参数：");
        EditorGUILayout.BeginVertical();

        //小球大小
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("小球大小:", GUILayout.Width(labelWidth));
        newScale = GUILayout.TextField(newScale, GUILayout.Width(labelWidth));
        if (GUILayout.Button("确认", GUILayout.Width(labelWidth)))
        {
            float temp = float.Parse(newScale);
            ball.transform.localScale = new Vector3(temp, temp, temp);
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();


        //旋转速度
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("旋转速度:", GUILayout.Width(labelWidth));
        newSpeed = GUILayout.TextField(newSpeed, GUILayout.Width(labelWidth));
        if (GUILayout.Button("确认", GUILayout.Width(labelWidth)))
        {
            float temp = float.Parse(newSpeed);
            PlayerPrefs.SetFloat(PrefConstans.RORATE_SPEED, temp);
            PlayerPrefs.Save();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();


        //模型宽度
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("模型宽度:", GUILayout.Width(labelWidth));
        newWidth = GUILayout.TextField(newWidth, GUILayout.Width(labelWidth));
        if (GUILayout.Button("确认", GUILayout.Width(labelWidth)))
        {
            float temp = float.Parse(newWidth);
            PlayerPrefs.SetFloat(PrefConstans.GAMEOBJECT_WIDTH, temp);
            PlayerPrefs.Save();
            //TODO 重设宽度
            ResetWidth(temp);
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();


        //下落加速度
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("下落加速度:", GUILayout.Width(labelWidth));
        newGravity = GUILayout.TextField(newGravity, GUILayout.Width(labelWidth));
        if (GUILayout.Button("确认", GUILayout.Width(labelWidth)))
        {
            float temp = float.Parse(newGravity);
            PlayerPrefs.SetFloat(PrefConstans.GRAVITY, temp);
            PlayerPrefs.Save();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();


        //跳跃速度
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("跳跃速度:", GUILayout.Width(labelWidth));
        newUpSpeed = GUILayout.TextField(newUpSpeed, GUILayout.Width(labelWidth));
        if (GUILayout.Button("确认", GUILayout.Width(labelWidth)))
        {
            float temp = float.Parse(newUpSpeed);
            PlayerPrefs.SetFloat(PrefConstans.UP_SPEED, temp);
            PlayerPrefs.Save();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        //高度上限
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("跳跃高度上限:", GUILayout.Width(labelWidth));
        newUpHeight = GUILayout.TextField(newUpHeight, GUILayout.Width(labelWidth));
        if (GUILayout.Button("确认", GUILayout.Width(labelWidth)))
        {
            float temp = float.Parse(newUpHeight);
            PlayerPrefs.SetFloat(PrefConstans.UP_DISTANCE, temp);
            PlayerPrefs.Save();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        //间隔
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("模型间隔距离:", GUILayout.Width(labelWidth));
        newGap = GUILayout.TextField(newGap, GUILayout.Width(labelWidth));
        if (GUILayout.Button("确认", GUILayout.Width(labelWidth)))
        {
            float temp = float.Parse(newGap);
            PlayerPrefs.SetFloat(PrefConstans.GAMEOBJECT_GAP, temp);
            PlayerPrefs.Save();
            ResetGap(temp);
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        #endregion

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        #region 创建模型按钮
        GUILayout.Label("创建模型：");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("创建扇形", GUILayout.Height(35)))
        {
            GameObject sector = objCreater.CreateCylinder(2.5f, 120, 1);
            CreateNewModel(ModelType.Sector, sector, true);
        }
        if (GUILayout.Button("创建圆柱", GUILayout.Height(35)))
        {
            GameObject cy = Instantiate(cylinderPrefab);
            float curWidth = PlayerPrefs.GetFloat(PrefConstans.GAMEOBJECT_WIDTH, 2.5f);
            cy.transform.localScale = new Vector3(1, curWidth / 2, 1);
            cy.transform.localPosition += new Vector3(curWidth / 2, 0, 0);
            CreateNewModel(ModelType.Cylinder, cy, true);
        }
        if (GUILayout.Button("创建条形", GUILayout.Height(35)))
        {
            GameObject cube = Instantiate(cubePrefab);
            float curWidth = PlayerPrefs.GetFloat(PrefConstans.GAMEOBJECT_WIDTH, 2.5f);
            cube.transform.localScale = new Vector3(curWidth, 1, 1);
            cube.transform.localPosition += new Vector3(curWidth / 2, 0, 0);
            CreateNewModel(ModelType.Cube, cube, true);
        }
        EditorGUILayout.EndHorizontal();
#endregion

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

#region 物体列表
        GUILayout.Label("当前模型列表：");
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("物体：", GUILayout.Width(300));
        GUILayout.Label("等分：", GUILayout.Width(70));
        GUILayout.Label("厚度：", GUILayout.Width(70));
        GUILayout.Label("角度：", GUILayout.Width(70));

        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < objList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.ObjectField(objList[i].name + ":", objList[i], typeof(GameObject), false, GUILayout.Width(300));
            objPartsArr[i] = (PARTS)EditorGUILayout.EnumPopup(objPartsArr[i], GUILayout.Width(70));
            objThickArr[i] = GUILayout.TextField(objThickArr[i], GUILayout.Width(70));

            if (objList[i].tag == "Sector")
            {
                objAngleArr[i] = GUILayout.TextField(objAngleArr[i], GUILayout.Width(70));
            }
            else
            {
                GUILayout.Label("无", GUILayout.Width(70));
            }

            if (GUILayout.Button("删除"))
            {
                RemoveAction(i);
                i--;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("保存更改", GUILayout.Height(35)))
        {
            for(int i = 0; i < objData.modelList.Count; i++)
            {
                //修改等分
                if(objData.modelList[i].parts != objPartsArr[i])
                {
                    Debug.Log(">>>parts变化" + i + "::" + objData.modelList[i].parts + "->" + objPartsArr[i]);
                    //GameObject newpart = Instantiate(objList[i]);
                    //newpart.transform.SetParent(objList[i].transform);
                    //newpart.transform.RotateAround(Vector3.zero, Vector3.up, 180);
                    SetNewParts(objList[i], objPartsArr[i]);
                }

                //修改厚度
                if(Mathf.Abs(objData.modelList[i].thick - float.Parse(objThickArr[i])) > float.Epsilon)
                {
                    Debug.Log(">>>thick变化" + i + "::" + objData.modelList[i].thick + "->" + objThickArr[i]);
                    curThickness = float.Parse(objThickArr[i]);
                    if(objList[i].tag == "Cylinder")
                    {
                        objList[i].transform.localScale = new Vector3(curThickness, objList[i].transform.localScale.y, curThickness);
                    }
                    else
                    {
                        objList[i].transform.localScale = new Vector3(objList[i].transform.localScale.x, curThickness, objList[i].transform.localScale.z);
                    }
                }

                //修改角度
                if(Mathf.Abs(objData.modelList[i].angle - float.Parse(objAngleArr[i])) > float.Epsilon)
                {
                    Debug.Log(">>>angle变化" + i + "::" + objData.modelList[i].angle + "->" + objAngleArr[i]);
                    if(objList[i].tag == "Sector")
                    {
                        float radius = PlayerPrefs.GetFloat(PrefConstans.GAMEOBJECT_WIDTH, 2.5f);
                        objCreater.CreateCylinder(radius, int.Parse(objAngleArr[i]), float.Parse(objThickArr[i]), objList[i]);
                    }
                }
                objData.modelList[i].SetArg(objPartsArr[i], float.Parse(objThickArr[i]), float.Parse(objAngleArr[i]));
            }
            objSaver.ChangeData(objData);
        }

#endregion

    }

    //创建新模型
    private void CreateNewModel(ModelType mType, GameObject obj, bool needSave)
    {
        obj.transform.SetParent(parentObj.transform);

        obj.name = string.Format("{0:000}{1}", (objList.Count + 1), objName[(int)mType]);
        obj.tag = objName[(int)mType];

        objList.Add(obj);

        if (needSave)
        {
            if (objData == null)
            {
                objData = new ObjDataBase();
            }
            objData.modelList.Add(new ModelData(mType, PARTS.ONE, 1, 120));
            SetArrData();
            objSaver.ChangeData(objData);
        }

        float curGap = PlayerPrefs.GetFloat(PrefConstans.GAMEOBJECT_GAP, 5);
        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, -(curGap * (objList.Count - 1)), 0);
        camera.localPosition -= new Vector3(0, curGap, 0);
        pillar.localPosition -= new Vector3(0, curGap, 0);
    }

    //删除模型
    private void RemoveAction(int index)
    {
        float curGap = PlayerPrefs.GetFloat(PrefConstans.GAMEOBJECT_GAP, 5);
        for (int i = index + 1; i < objList.Count; i++)
        {
            objList[i].name = string.Format("{0:000}{1}", i, objList[i].name.Substring(3));
            objList[i].transform.localPosition += new Vector3(0, curGap, 0);
        }
        DestroyImmediate(objList[index]);
        objList.RemoveAt(index);
        objData.modelList.RemoveAt(index);
        SetArrData();
        objSaver.ChangeData(objData);

        camera.localPosition += new Vector3(0, curGap, 0);
        pillar.localPosition += new Vector3(0, curGap, 0);
    }

    //重设间隔
    private void ResetGap(float tGap)
    {
        if (objList == null || objList.Count <= 0)
            return;
        for (int i = 0; i < objList.Count; i++)
        {
            objList[i].transform.localPosition = new Vector3(objList[i].transform.localPosition.x, -i * tGap, objList[i].transform.localPosition.z);
        }
        camera.localPosition = new Vector3(0, 11 - tGap * objList.Count , camera.localPosition.z);
        pillar.localPosition = new Vector3(0, 6 - tGap * objList.Count, 0);
    }

    //重设宽度
    private void ResetWidth(float tWidth)
    {
        if (objList == null || objList.Count <= 0)
            return;
        float curWidth = tWidth;
        for (int i = 0; i < objList.Count; i++)
        {
            if(objList[i].tag == "Sector")
            {
                curWidth = tWidth / CreateObj.Radius;
                objList[i].transform.localScale = new Vector3(curWidth, 1, curWidth);
            }
            else if(objList[i].tag == "Cylinder")
            {
                curWidth = tWidth / 2;
                objList[i].transform.localScale = new Vector3(1, curWidth, 1);
                objList[i].transform.localPosition = new Vector3(curWidth, objList[i].transform.localPosition.y, objList[i].transform.localPosition.z);
            }
            else
            {
                curWidth = tWidth;
                objList[i].transform.localScale = new Vector3(curWidth, 1, 1);
                objList[i].transform.localPosition = new Vector3(curWidth / 2, objList[i].transform.localPosition.y, objList[i].transform.localPosition.z);
            }
        }
    }

    private void SetNewParts(GameObject obj, PARTS part)
    {
        for (int i = obj.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(obj.transform.GetChild(i).gameObject);
        }
        GameObject template = Instantiate(obj);
        GameObject emptyTemp = Resources.Load<GameObject>("Models/Empty");

        int[] arrs = { 0, 1, 2, 3, 5 };
        int tempP;
        tempP = arrs[(int)part];
        for (int i = 0; i < tempP; i++)
        {
            GameObject newpart = Instantiate(template);
            GameObject empty = Instantiate(emptyTemp);
            empty.transform.SetParent(parentObj.transform);
            newpart.transform.SetParent(empty.transform);
            if (obj.tag == "Sector")
            {
                newpart.transform.localRotation = Quaternion.Euler(0, (360 / (tempP + 1)) * (i + 1), 0);
            }
            else
            {
                newpart.transform.RotateAround(pillar.position, Vector3.up, (360 / (tempP + 1)) * (i + 1));
            }
            empty.transform.SetParent(obj.transform);
        }

        DestroyImmediate(template);
    }

    private void SetArrData()
    {
        if (objData != null)
        {
            for (int i = 0; i < objData.modelList.Count; i++)
            {
                objThickArr[i] = objData.modelList[i].thick.ToString();
                objAngleArr[i] = objData.modelList[i].angle.ToString();
                objPartsArr[i] = objData.modelList[i].parts;
            }
        }
    }


    [MenuItem("Tools/清空模型")]
    public static void ClearData()
    {
        if (EditorUtility.DisplayDialog("清空", "是否清空当前所有模型", "清空", "取消"))
        {
            //DeleteMap(curMap);
            PlayerPrefs.DeleteAll();
            Transform parent = GameObject.Find("Parent").transform;
            Transform camera = GameObject.Find("Main Camera").transform;
            Transform pillar = GameObject.Find("Pillar").transform;
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(parent.GetChild(i).gameObject);
            }
            ObjSaver.DeleteData();
            camera.transform.localPosition = new Vector3(0, 11, camera.transform.localPosition.z);
            pillar.transform.localPosition = new Vector3(0, 6, 0);
        }
    }

}
