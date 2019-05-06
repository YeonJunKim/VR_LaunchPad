using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLine : MonoBehaviour {

    public GameObject line;
    public GameObject cursor;
    public TextMesh startTimeText;
    public TextMesh endTimeText;
    public TextMesh currentTimeText;

    public GameObject sectionPointPrefab;

    float lineLength;
    List<GameObject> sectionPoints;
    float previousTime;

    Vector3 cursorStartPos;

    private void Awake()
    {
        sectionPoints = new List<GameObject>();
        lineLength = line.transform.localScale.x;
        startTimeText.text = "0";
        previousTime = 0;
        cursor.transform.localPosition = Vector3.zero;
        cursorStartPos = cursor.transform.position;
    }

	
    public void SetTimeLine(bool on)
    {
        if (on)
        {
            previousTime = 0;

            float endTime = AudioManager.instance.GetAudioClip().length;
            int endTimeInt = (((int)endTime) % 60);
            if (endTimeInt < 10)
                endTimeText.text = ((int)endTime / 60).ToString() + ":0" + endTimeInt.ToString();
            else
                endTimeText.text = ((int)endTime / 60).ToString() + ":" + endTimeInt.ToString();

            StartCoroutine(SimulateTimeLine());
        }
        else
        {
            StopAllCoroutines();
        }
    }

    IEnumerator SimulateTimeLine()
    {
        while(true)
        {
            UpdateCursor();

            yield return new WaitForSeconds(1);
        }
    }

    public void UpdateCursor()
    {
        float endTime = AudioManager.instance.GetAudioClip().length;
        float currentTime = AudioManager.instance.GetCurrentPlayTime();
        float timeRatio = currentTime / endTime;

        if (currentTime == 0 && previousTime > (endTime / 2))
        {
            timeRatio = 1;
            currentTime = endTime;
        }

        float xPos = lineLength * timeRatio - (lineLength / 2);
        cursor.transform.localPosition = new Vector3(xPos, 0, 0);

        int currentTimeInt = (((int)currentTime) % 60);
        if (currentTimeInt < 10)
            currentTimeText.text = ((int)currentTime / 60).ToString() + ":0" + currentTimeInt.ToString();
        else
            currentTimeText.text = ((int)currentTime / 60).ToString() + ":" + currentTimeInt.ToString();

        if (currentTime > 0)
            previousTime = currentTime;
    }

   
    public void AddSectionPoint(float time)
    {
        float endTime = AudioManager.instance.GetAudioClip().length;
        float timeRatio = time / endTime;

        float xPos = lineLength * timeRatio - (lineLength / 2);
        Vector3 pos = cursorStartPos;
        pos.x += xPos;

        GameObject sectionPoint = Instantiate<GameObject>(sectionPointPrefab);
        sectionPoint.transform.position = pos;

        sectionPoints.Add(sectionPoint);
    }


    public void RemoveAllSectionPoints()
    {
        foreach(var point in sectionPoints)
        {
            Destroy(point);
        }

        sectionPoints.Clear();
    }
}
