using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ModelWindow : EditorWindow {

    [MenuItem("Tools/建造模型")]
    public static void TryCreateModel()
    {
        ModelWindow window = (ModelWindow)EditorWindow.GetWindow(typeof(ModelWindow), true, "模型建造选项", true);
        window.Show();
    }

	private void OnGUI()
	{
		
	}

}
