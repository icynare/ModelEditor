using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class MapEditor : EditorWindow {

    private GameObject content;
    private GameObject mapModel;
    private GameObject newIcon;

    private LinkedList<GameObject> mapList;
    private bool[] mapArr = new bool[100];
    private string PATH = "";

    private LinkedListNode<GameObject> tempObj;
    private GameObject curMap;
    private string tempStr;
    private GUIStyle style;
    private Texture2D tex;

    private float sliderMap;
    private int chilCount;

    [MenuItem("Tools/MapEditor", false, 19)]
    public static void ShowWindow()
    {
        if (SceneManager.GetActiveScene().name != "Map")
        {
            EditorUtility.DisplayDialog("提示", "当前场景不为Map，请切换后打开！", "好的");
            return; 
        }
        //Rect rect = new Rect(100, 100, 500, 600);
        //MapEditor editor = (MapEditor)GetWindowWithRect(typeof(MapEditor), rect, false, "地图编辑器");
        MapEditor editor = GetWindow<MapEditor>(false, "地图编辑器");
        editor.Init();
        editor.Show();
    }

    private void Init()
    {
        PATH = Application.dataPath + "/Resources/MapPrefab/";
        content = GameObject.Find("Content");
        mapModel = Resources.Load("MapPrefab/Model/MapModel") as GameObject;
        mapList = new LinkedList<GameObject>();
        newIcon = Resources.Load("MapPrefab/Model/LevelIcon") as GameObject;
        ClearScene();
        GetMapPrefab();
    }

	private void OnGUI()
	{
        GUILayout.Label("当前地图：");
        if(GUILayout.Button("刷新", GUILayout.Height(35)))
        {
            if (curMap != null)
            {
                if (EditorUtility.DisplayDialog("提示：" + curMap.name, "是否保存当前编辑场景？", "保存", "取消"))
                {
                    SaveMap(curMap);
                }
            }
            Init();
        }
        if (mapList == null)
            return;
        tempObj = mapList.First;
        while (tempObj != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(tempObj.Value.name, tempObj.Value, typeof(GameObject), false);
            if(GUILayout.Button("编辑"))
            {
                if(curMap != null)
                {
                    if(EditorUtility.DisplayDialog("提示：" + curMap.name, "是否保存当前编辑场景？", "保存", "取消"))
                    {
                        SaveMap(curMap);
                    }
                }
                //mapPrefab.Remove(tempObj);
                ClearScene();
                curMap = PrefabUtility.InstantiatePrefab(tempObj.Value) as GameObject;
                if (curMap != null)
                {
                    curMap.transform.parent = content.transform;
                    curMap.transform.localScale = Vector3.one;
                    curMap.transform.localRotation = Quaternion.Euler(-180, 0, 0);
                }
            }
            tempObj = tempObj.Next;
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("添加地图块", GUILayout.Height(50)))
        {
            ClearScene();
            curMap = PrefabUtility.InstantiatePrefab(mapModel) as GameObject;
            if (curMap != null)
            {
                curMap.transform.parent = content.transform;
                curMap.transform.localScale = Vector3.one;
                curMap.transform.localRotation = Quaternion.Euler(-180, 0, 0);
            }
            //mapCount++;
            //mapPrefab.AddLast(curMap);
            curMap.name = GetNewMapName();
       
           
        }

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Separator();
        if(curMap != null)
        {
            EditorGUILayout.BeginVertical();
            curMap = EditorGUILayout.ObjectField(curMap.name, curMap, typeof(GameObject), true) as GameObject;
            tex = EditorGUILayout.ObjectField("背景图片", curMap.GetComponent<RawImage>().texture, typeof(Texture2D), true) as Texture2D;
            curMap.GetComponent<RawImage>().texture = tex;
            if(GUILayout.Button("添加新关卡", GUILayout.Height(30)))
            {
                Instantiate(newIcon, curMap.transform);
            }
            chilCount = curMap.transform.childCount;
            for (int i = 0; i < chilCount; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField("关卡" + (i+1), curMap.transform.GetChild(i).gameObject, typeof(GameObject), false);
                if(GUILayout.Button("选中"))
                {
                    Selection.activeGameObject = curMap.transform.GetChild(i).gameObject;
                }
                if(GUILayout.Button("删除"))
                {
                    DestroyImmediate(curMap.transform.GetChild(i).gameObject);
                    i--;
                    chilCount--;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("保存", GUILayout.Height(40), GUILayout.Width(200)))
            {
                //CreatePrefabObj(curMap);
                SaveMap(curMap);
            }
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.red;
            if(GUILayout.Button("删除", GUILayout.Height(40), GUILayout.Width(200)))
            {
                if (EditorUtility.DisplayDialog("删除：" + curMap.name, "是否删除当前编辑场景？", "删除", "取消"))
                {
                    DeleteMap(curMap);
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

	}

    private void SaveMap(GameObject map)
    {
        LinkedListNode<GameObject> node = mapList.First;
        while(node != null)
        {
            if(node.Value.name == map.name)
            {
                //mapList.AddBefore(node, map);
                node.Value = map;
                PrefabUtility.ReplacePrefab(map, PrefabUtility.GetPrefabParent(map), ReplacePrefabOptions.ConnectToPrefab);
                GetMapPrefab();
                return;
            }
            node = node.Next;
        }
        mapList.AddLast(map);
        CreatePrefabObj(map);
        GetMapPrefab();
    }

    private void DeleteMap(GameObject map)
    {
        LinkedListNode<GameObject> node = mapList.First;
        while (node != null)
        {
            if (node.Value.name == map.name)
            {
                mapList.Remove(map);
                if (IsFileExists(map.name))
                {
                    File.Delete(GetFullPath(map.name));
                    curMap = null;
                    AssetDatabase.Refresh();
                }
                GetMapPrefab(); 
                return;
            }
            node = node.Next;
        }
        curMap = null;
    }

    private void ClearScene()
    {
        int count = content.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            DestroyImmediate(content.transform.GetChild(0).gameObject);
        }
    }

    private void GetMapPrefab()
    {
        mapList.Clear();
        int index = 0;
        for (int i = 0; i < mapArr.Length; i++)
        {
            mapArr[i] = false;
        }
        mapArr[index] = true;
        if (Directory.Exists(PATH))
        {
            DirectoryInfo direction = new DirectoryInfo(PATH);
            FileInfo[] files = direction.GetFiles("*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".prefab", System.StringComparison.Ordinal))
                {
                    string filename = Path.GetFileNameWithoutExtension(files[i].ToString());
                    mapList.AddLast((GameObject)Resources.Load("MapPrefab/" + filename, typeof(GameObject)));
                    index = int.Parse(filename.Substring(filename.Length - 2, 2));
                    mapArr[index] = true;
                }
            }

        }
    }

    string GetNewMapName()
    {
        for (int i = 1; i < mapArr.Length; i++)
        {
            if(!mapArr[i])
            {
                if (i > 9)
                    return "Map0" + i;
                else
                    return "Map00" + i;
            }
        }
        return "";
    }

    void CreatePrefabObj(GameObject obj)
    {
        string newPath = "Assets/Resources/MapPrefab/" + obj.name + ".prefab";
        PrefabUtility.CreatePrefab(newPath, obj);
        AssetDatabase.Refresh();
    }

    bool IsFileExists(string fileName)
    {
        return File.Exists(GetFullPath(fileName));
    }

    string GetFullPath(string fileName)
    {
        return PATH + fileName + ".prefab";
    }

    //在修改了子物体”LevelIcon“后运行该脚本更新所有关卡子模块
    [MenuItem("Tools/MapUpdate", false, 20)]
    public static void UpdateItems()
    {
        
        MapEditor editor = new MapEditor();
        editor.Init();
        editor.UpdateAllMap();
    }

    //[ExecuteInEditMode()]
    void UpdateAllMap()
    {
        if (mapList == null)
            return;
        tempObj = mapList.First;
        List<Vector2> posList = new List<Vector2>();//用于保存子物体的位置
        List<GameObject> iconList = new List<GameObject>();
        while(tempObj != null)
        {
            GameObject mapObj = PrefabUtility.InstantiatePrefab(tempObj.Value) as GameObject;

            for (int i = 0; i < mapObj.transform.childCount; i++)
            {
                GameObject temp = mapObj.transform.GetChild(i).gameObject;
                Vector2 pos = temp.GetComponent<RectTransform>().anchoredPosition;
                posList.Add(pos);
                iconList.Add(temp);
            }
            for (int i = 0; i < posList.Count; i++)
            {
                DestroyImmediate(iconList[i]);
                GameObject obj = Instantiate(newIcon, mapObj.transform);
                obj.GetComponent<RectTransform>().anchoredPosition = posList[i];
            }
            if (PrefabUtility.GetPrefabParent(mapObj) != null)
            {
                //Apply 当前map的prefab
                PrefabUtility.ReplacePrefab(mapObj, PrefabUtility.GetPrefabParent(mapObj), ReplacePrefabOptions.ConnectToPrefab);
                DestroyImmediate(mapObj);
            }
            tempObj = tempObj.Next;
            posList.Clear();
            iconList.Clear();
        }

        EditorUtility.DisplayDialog("完成", "更新关卡ITEM完成", "好的");

    }

}
