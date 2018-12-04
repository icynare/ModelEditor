using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentObj : MonoBehaviour {

    private List<GameObject> objList;
    private List<int> rotateDir;
    private int[] dirArr = { -1, 1 };

    private float _rotateSpeed = 0;
    private float _curRotate = 0;
    private Transform _pillar;

	// Use this for initialization
	void Start () {
        Init();
	}

    private void Init()
    {
        objList = new List<GameObject>();
        rotateDir = new List<int>();
        _pillar = GameObject.Find("Pillar").transform;

        _rotateSpeed = PlayerPrefs.GetFloat(PrefConstans.RORATE_SPEED, PrefConstans.DEFAULT_SPEED);

        GameObject curObj = null;
        float curRotate = 0;
        int curDir = 1;
        for (int i = 0; i < transform.childCount; i++)
        {
            curObj = transform.GetChild(i).gameObject;
            curRotate = Random.Range(0, 360);
            if (curObj.tag == "Sector")
            {//curObj.transform.localRotation = Quaternion.Euler(90, curRotate, 0);
                curObj.transform.localRotation = Quaternion.Euler(0, curRotate, 0);
                //curObj.transform.Rotate(Vector3.zero, Vector3.up, curRotate);
            }
            else
            {
                curObj.transform.RotateAround(Vector3.zero, Vector3.up, curRotate);
            }

            objList.Add(transform.GetChild(i).gameObject);

            curDir = Random.Range(0, 2);
            Debug.Log(curDir);
            rotateDir.Add(dirArr[curDir]);
        }
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < objList.Count; i++)
        {
            _curRotate = _rotateSpeed * rotateDir[i];
            if (objList[i].tag == "Sector")
            {
                objList[i].transform.Rotate(0, _curRotate, 0, Space.World);
            }
            else
            {
                objList[i].transform.RotateAround(Vector3.zero, Vector3.up, _curRotate);
            }
        }
	}
}
