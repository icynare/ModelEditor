using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    public float grivity
    {
        get;
        set;
    }

    public Camera m_Camera;
    public GameObject m_Pillar;

    private Vector3 localPos;

    private float curSpeed = 0;
    private float curHeight = 0;

    private float lowestHeight = 0;
    private float upBorder = 0;
    private float upSpeed = 0;

    private float upBorderSingle = 2;
    private float curUp = 0;

    public static float CAMERA_INIT_HEIGHT = 6;
    public static float PILLAR_INIT_HEIGHT = 1;
    public static float BALL_INTI_HEIGHT = 5;

    private float downGravity;
    private float upGravity;

    public void Start()
    {
        localPos = gameObject.transform.localPosition;
        downGravity =  - PlayerPrefs.GetFloat(PrefConstans.GRAVITY, PrefConstans.DEFAULT_GRAVITY);
        upGravity =  - PlayerPrefs.GetFloat(PrefConstans.DOWN_GRAVITY, PrefConstans.DEFAULT_DOWN_GRAVITY);
        upBorder = PlayerPrefs.GetFloat(PrefConstans.UP_DISTANCE, PrefConstans.DEFAULT_UP_DISTANCE);
        upSpeed = PlayerPrefs.GetFloat(PrefConstans.UP_SPEED, PrefConstans.DEFAULT_UP_SPEED);
        upBorderSingle = PlayerPrefs.GetFloat(PrefConstans.UP_SINGLE, PrefConstans.DEFAULT_UP_SINGLE);
        lowestHeight = localPos.y;

        m_Camera.transform.localPosition = new Vector3(0, CAMERA_INIT_HEIGHT, m_Camera.transform.localPosition.z);
        m_Pillar.transform.localPosition = new Vector3(0, PILLAR_INIT_HEIGHT, 0);
    }

	private void Update()
	{
        //Debug.Log(">>>curSpeed" + curSpeed);
        transform.Rotate(-curSpeed, 0, 0);
	}

	private void FixedUpdate()
	{

        curHeight = curSpeed * Time.fixedDeltaTime + 0.5f * grivity * Mathf.Pow(Time.fixedDeltaTime, 2);
        curSpeed += grivity * Time.fixedDeltaTime;
        localPos.y += curHeight;

        if (curSpeed > 0)
        {
            curUp += curHeight;
            if(curUp > upBorderSingle)
            {
                curUp = 0;
                curSpeed = 0;
            }
        }
        else
        {
            curUp = 0;
            grivity = downGravity;
        }

        //判断上升边界
        if (localPos.y < lowestHeight)
        {
            lowestHeight = localPos.y;
        }
        else if (localPos.y > lowestHeight + upBorder)
        {
            curHeight = lowestHeight + upBorder - (localPos.y - curHeight);
            localPos.y = lowestHeight + upBorder;
            curSpeed = 0;
        }


        gameObject.transform.localPosition = localPos;

        m_Camera.gameObject.transform.localPosition = new Vector3(m_Camera.transform.localPosition.x, m_Camera.transform.localPosition.y + curHeight, m_Camera.transform.localPosition.z);
        m_Pillar.transform.localPosition = new Vector3(m_Pillar.transform.localPosition.x, m_Pillar.transform.localPosition.y + curHeight, m_Pillar.transform.localPosition.z);

	}

    public void OnClickJump()
    {
        curSpeed = upSpeed;
        curUp = 0;
        grivity = upGravity;
    }

}
