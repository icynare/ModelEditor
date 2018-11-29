using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ModelWindow : EditorWindow {

    private GameObject parentObj;
    private CreateObj objCreater;

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

    private GameObject tempObj;
    private PARTS parts = PARTS.ONE;
    private int curLen = 0;

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

        ball = GameObject.Find("Ball");
        ballScale = ball.transform.localScale.x;
        newScale = ballScale.ToString();

        newSpeed = PlayerPrefs.GetFloat(PrefConstans.RORATE_SPEED, 1).ToString();
        newWidth = PlayerPrefs.GetFloat(PrefConstans.GAMEOBJECT_WIDTH, 2.5f).ToString();
        newGravity = PlayerPrefs.GetFloat(PrefConstans.GRAVITY, 15).ToString();
        newUpSpeed = PlayerPrefs.GetFloat(PrefConstans.UP_SPEED, 4).ToString();
        newUpHeight = PlayerPrefs.GetFloat(PrefConstans.UP_DISTANCE, 5).ToString();

        objSaver = new ObjSaver();
        objSaver.Init();
        objData = objSaver.Load();
        SetArrData();

        cubePrefab = Resources.Load<GameObject>("Models/Cube");
        cylinderPrefab = Resources.Load<GameObject>("Models/Cylinder");

        objList = new List<GameObject>();
        for (int i = 0; i < parentObj.transform.childCount; i++)
        {
            objList.Add(parentObj.transform.GetChild(i).gameObject);
        }

        objCreater = new CreateObj();
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

        EditorGUILayout.EndVertical();
        #endregion

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        #region 创建模型按钮
        GUILayout.Label("创建模型：");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("创建扇形", GUILayout.Height(35)))
        {
            GameObject sector = objCreater.CreateCylinder(2.5f, 150, 1);
            CreateNewModel(ModelType.Sector, sector);
        }
        if (GUILayout.Button("创建圆柱", GUILayout.Height(35)))
        {
            GameObject cy = Instantiate(cylinderPrefab);
            float curWidth = PlayerPrefs.GetFloat(PrefConstans.GAMEOBJECT_WIDTH, 2.5f);
            cy.transform.localScale = new Vector3(1, curWidth, 1);
            CreateNewModel(ModelType.Cylinder, cy);
        }
        if (GUILayout.Button("创建条形", GUILayout.Height(35)))
        {
            GameObject cube = Instantiate(cubePrefab);
            float curWidth = PlayerPrefs.GetFloat(PrefConstans.GAMEOBJECT_WIDTH, 2.5f);
            cube.transform.localScale = new Vector3(curWidth * 2, 1, 1);
            CreateNewModel(ModelType.Cube, cube);
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
                objData.modelList[i].SetArg(objPartsArr[i], float.Parse(objThickArr[i]), float.Parse(objAngleArr[i]));
            }
            objSaver.ChangeData(objData);
        }

#endregion

    }

    private void CreateNewModel(ModelType mType, GameObject obj)
    {
        obj.transform.SetParent(parentObj.transform);

        obj.name = string.Format("{0:000}{1}", (objList.Count + 1), objName[(int)mType]);
        obj.tag = objName[(int)mType];

        objList.Add(obj);
        if(objData == null)
        {
            objData = new ObjDataBase();
        }
        objData.modelList.Add(new ModelData(mType, PARTS.ONE, 1, 1));
        SetArrData();
        objSaver.ChangeData(objData);
    }

    private void RemoveAction(int index)
    {
        for (int i = index + 1; i < objList.Count; i++)
        {
            objList[i].name = string.Format("{0:000}{1}", i, objList[i].name.Substring(3));
        }
        DestroyImmediate(objList[index]);
        objList.RemoveAt(index);
        objData.modelList.RemoveAt(index);
        SetArrData();
        objSaver.ChangeData(objData);
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

    private float GetThick(GameObject obj)
    {
        if (obj.tag == "Cylinder")
        {
            return obj.transform.localScale.x;
        }
        else
            return obj.transform.localScale.y;
    }

    private void SetThick(GameObject obj, float thick)
    {
        if (obj.tag == "Cylinder")
            obj.transform.localScale = new Vector3(thick, obj.transform.localScale.y, thick);
        else
            obj.transform.localScale = new Vector3(obj.transform.localScale.x, thick, obj.transform.localScale.z);
    }


    [MenuItem("Tools/清除数据")]
    public static void ClearData()
    {
        PlayerPrefs.DeleteAll();
    }

}
