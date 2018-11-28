using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ModelWindow : EditorWindow {

    private GameObject parentObj;
    private CreateObj objCreater;

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
        objCreater = new CreateObj();
    }

    private void OnGUI()
	{
        if (GUILayout.Button("创建扇形", GUILayout.Height(35)))
        {
            GameObject cylinder = objCreater.CreateCylinder(2.5f, 150, 1);
            cylinder.transform.SetParent(parentObj.transform);
        }
    }

}
