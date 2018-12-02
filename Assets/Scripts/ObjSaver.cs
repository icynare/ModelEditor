using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using SimpleJSON;

public enum ModelType
{
    Sector,
    Cylinder,
    Cube
}

public enum PARTS
{
    ONE,
    TWO,
    THREE,
    FOUR,
    SIX
}

[System.Serializable]
public class ModelData
{
    public ModelType modelType;
    public PARTS parts;
    public float thick;
    public float angle;

    public ModelData(ModelType mType, PARTS pt, float thi, float ang)
    {
        modelType = mType;
        parts = pt;
        thick = thi;
        angle = ang;
    }

    public void SetArg(PARTS pt, float thi, float ang)
    {
        parts = pt;
        thick = thi;
        angle = ang;
    }

    public void Print()
    {
        Debug.Log(string.Format("类型：{0},等分：{1},厚度：{2},角度：{3}", modelType, parts, thick, angle));
    }
}


[System.Serializable]
public class ObjDataBase
{
    public List<ModelData> modelList;

    public ObjDataBase()
    {
        modelList = new List<ModelData>();   
    }

    public void Print()
    {
        if (modelList == null)
            Debug.Log(">>>List is Null");
        else
        {
            for (int i = 0; i < modelList.Count; i++)
            {
                modelList[i].Print();
            }
        }
    }

}

public class ObjSaver {

    public static string PLATFORMPATH = "";
    public static string FILEPATH = "/Obj";
    public static string FILENAME = "/model.txt";
    private string path;

    public ObjDataBase objDataBase;

    public void Init()
    {
        objDataBase = new ObjDataBase();
        PLATFORMPATH = Application.streamingAssetsPath;
    }

    public ObjDataBase Load()
    {
        path = PLATFORMPATH + FILEPATH + FILENAME;
        if (!Directory.Exists(PLATFORMPATH + FILEPATH))
        {
            Debug.Log(">>>不存在：" + path);
            Directory.CreateDirectory(PLATFORMPATH + FILEPATH);
        }
        if (File.Exists(path))
        {
            //二进制格式读取
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(path, FileMode.Open);
            //objDataBase = (ObjDataBase)bf.Deserialize(file);
            //objDataBase.Print();
            //file.Close();

            //Json格式读取
            string str = File.ReadAllText(path);
            Debug.Log(">>>TXT文本：" + str);
            JSONNode json = JSON.Parse(str.ToString());
            Debug.Log(">>>Json:" + json["id"]);
            return objDataBase;
        }

        return null;
    }

    public void Save()
    {
        path = PLATFORMPATH + FILEPATH + FILENAME;

        //二进制格式保存
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Open(path, FileMode.OpenOrCreate);
        //bf.Serialize(file, objDataBase);
        //file.Close();

        //Json保存
        JSONArray array = new JSONArray();
        ModelData tempData;
        StringBuilder sb = new StringBuilder();
        string tempStr = "";
        for (int i = 0; i < objDataBase.modelList.Count; i++)
        {
            tempData = objDataBase.modelList[i];
            tempStr = string.Format("\"type\":{0},\"parts\":{1},\"thick\":{2},\"angle\":{3}",
                (int)tempData.modelType,
                (int)tempData.parts,
                tempData.thick,
                tempData.angle);
            tempStr = "{" + tempStr + "}";
            JSONNode node = JSON.Parse(tempStr);
            array.Add(node);
        }
        string str = array.ToString();
        str = "{\"list\":" + str + "}";
        Debug.Log(">>>保存的JSonSTR：" + str);
        File.WriteAllText(path, str);
      
    }

    public static void DeleteData()
    {
        PLATFORMPATH = Application.streamingAssetsPath;
        string path = PLATFORMPATH + FILEPATH + FILENAME;
        if (File.Exists(path))
        {
            Debug.Log(">>>删除：" + path);
            File.Delete(path);
        }
    }

    public void ChangeData(ObjDataBase data)
    {
        objDataBase = data;
        Save();
    }

}
