using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WaveType
{
    Square,
    Diamond,
    Cross,
}


public enum NeighborType
{
    UpLeft,
    Up,
    UpRight,
    Left,
    Right,
    DownLeft,
    Down,
    DownRight
}


public class BackgroundCube : MonoBehaviour {

    Renderer renderer;
    Color originColor;
    BackgroundCube[] neighborCubes;

    List<int> waveNums;
    float waveSpeed = 0.02f;

	// Use this for initialization
	void Awake () {
        neighborCubes = new BackgroundCube[System.Enum.GetNames(typeof(NeighborType)).Length];
        renderer = transform.GetComponent<Renderer>();
        originColor = Color.black;
        waveNums = new List<int>();
    }

    private void Start()
    {
        renderer.material.SetColor("_EmissionColor", Color.black);
    }


    public void Twinkle()
    {
        renderer.material.SetColor("_EmissionColor", GetRandomColor());

        Invoke("RestoreColor", 0.1f);
    }


    public void Twinkle(int waveNum, WaveType type)
    {
        if (waveNums.Contains(waveNum))
            return;

        CancelInvoke("RestoreColor");
        waveNums.Add(waveNum);
        renderer.material.SetColor("_EmissionColor", GetRandomColor());

        switch(type)
        {
            case WaveType.Square:
                StartCoroutine(SquareWave(waveNum));
                break;
            case WaveType.Diamond:
                StartCoroutine(DiamondWave(waveNum));
                break;
            case WaveType.Cross:

                break;
        }
        Invoke("RestoreColor", 0.1f);
    }
    
    IEnumerator SquareWave(int waveNum)
    {
        yield return new WaitForSeconds(waveSpeed);

        foreach(var cube in neighborCubes)
        {
            if(cube)
                cube.Twinkle(waveNum, WaveType.Square);
        }

        if (waveNums.Count >= 5)
            waveNums.RemoveAt(0);
    }

    IEnumerator DiamondWave(int waveNum)
    {
        yield return new WaitForSeconds(waveSpeed);

        if (neighborCubes[(int)NeighborType.Up])
            neighborCubes[(int)NeighborType.Up].Twinkle(waveNum, WaveType.Diamond);
        if (neighborCubes[(int)NeighborType.Down])
            neighborCubes[(int)NeighborType.Down].Twinkle(waveNum, WaveType.Diamond);
        if (neighborCubes[(int)NeighborType.Left])
            neighborCubes[(int)NeighborType.Left].Twinkle(waveNum, WaveType.Diamond);
        if (neighborCubes[(int)NeighborType.Right])
            neighborCubes[(int)NeighborType.Right].Twinkle(waveNum, WaveType.Diamond);

        if (waveNums.Count >= 5)
            waveNums.RemoveAt(0);
    }

    

    void RestoreColor()
    {
        renderer.material.SetColor("_EmissionColor", originColor);
    }

    Color GetRandomColor()
    {
        int num = Random.Range(0, 6);

        switch(num)
        {
            case 0:
                return Color.blue;
            case 1:
                return Color.magenta;
            case 2:
                return Color.white;
            case 3:
                return Color.red;
            case 4:
                return Color.cyan;
            case 5:
                return Color.green;
            default:
                return Color.gray;
        }

    }


    public void SetNeighborCube(NeighborType type, BackgroundCube cube)
    {
        neighborCubes[(int)type] = cube;
    }

}
