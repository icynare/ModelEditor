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


    public void Start()
    {
        localPos = gameObject.transform.localPosition;
        grivity =  - PlayerPrefs.GetFloat(PrefConstans.GRAVITY, 15);
        upBorder = PlayerPrefs.GetFloat(PrefConstans.UP_DISTANCE, 5);
        upSpeed = PlayerPrefs.GetFloat(PrefConstans.UP_SPEED, 4);
        lowestHeight = localPos.y;
    }

	private void FixedUpdate()
	{
        curHeight = curSpeed * Time.fixedDeltaTime + 0.5f * grivity * Mathf.Pow(Time.fixedDeltaTime, 2);
        curSpeed += grivity * Time.fixedDeltaTime;
        localPos.y += curHeight;

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
    }

}
