using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public enum ModelType
{
    Sector,
    Cylinder,
    Cube
}

[System.Serializable]
public class ModelData
{
    public ModelType modelType;
    public int parts;
    public float thick;
    public float angle;

    public ModelData(ModelType mType, int pt, int thi, int ang)
    {
        modelType = mType;
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

    private string PLATFORMPATH = "";
    public string FILEPATH = "/Obj";
    public string FILENAME = "/obj.gd";
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
            //读取数据
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            objDataBase = (ObjDataBase)bf.Deserialize(file);
            objDataBase.Print();
            file.Close();
            return objDataBase;
        }

        return null;
    }

    public void Save()
    {
        path = PLATFORMPATH + FILEPATH + FILENAME;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.OpenOrCreate);
        bf.Serialize(file, objDataBase);
        file.Close();
    }

    public void DeleteData()
    {
        path = PLATFORMPATH + FILEPATH + FILENAME;
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public void ChangeData(ObjDataBase data)
    {
        objDataBase = data;
        Save();
    }

}
