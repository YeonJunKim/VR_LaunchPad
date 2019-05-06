using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class BackgroundManager : MonoBehaviour
{ 
    public static BackgroundManager instance;

    List<List<BackgroundCube>> backgroundCubes;
    BackgroundCube[] sectorCubes;

    const float xOffset_3set = 0.2f;
    const float xOffset_6set = 0.1f;
    const float yOffset_6set = 0.1f;

    int waveNum = 0;

    private void Awake()
    {
        if (BackgroundManager.instance == null)
        {
            BackgroundManager.instance = this;
        }

        backgroundCubes = new List<List<BackgroundCube>>();
        sectorCubes = new BackgroundCube[System.Enum.GetNames(typeof(Sector)).Length];
    }

    // Use this for initialization
    void Start()
    {
        InitBackgroundCubes();
        InitSectorCubes();
    }


    public void SetTwinkle(Sector sector)
    {
        sectorCubes[(int)sector].Twinkle(waveNum++, WaveType.Diamond);
    }


    BackgroundCube FindBackgroundCubeBySector(Sector sector)
    {
        BackgroundCube cube = backgroundCubes[0][0];

        // the most center BackgroundCube
        float y = 0.5f;
        float x = 0.5f;

        switch (sector)
        {
            case Sector.Left:
                x -= xOffset_3set;
                break;
            case Sector.Center:
                break;
            case Sector.Right:
                x += xOffset_3set;
                break;
            case Sector.Left_Left:
                x -= xOffset_6set * 2;
                y += yOffset_6set;
                break;
            case Sector.Right_Right:
                x += xOffset_6set * 2;
                y += yOffset_6set;
                break;
            case Sector.Center_Left:
                x -= xOffset_6set;
                y += yOffset_6set;
                break;
            case Sector.Center_Right:
                x += xOffset_6set;
                y += yOffset_6set;
                break;
            case Sector.Top_Left:
                x -= xOffset_6set * 1.5f;
                y -= yOffset_6set;
                break;
            case Sector.Top_Right:
                x += xOffset_6set * 1.5f;
                y -= yOffset_6set;
                break;
        }

        int i = (int)(backgroundCubes.Count * y);
        int j = (int)(backgroundCubes[0].Count * x);

        if (CheckValidIndexOfBackgroundCubes(i, j))
            cube = backgroundCubes[i][j];

        return cube;
    }


    void InitSectorCubes()
    {
        int count = System.Enum.GetNames(typeof(Sector)).Length;
        for (int i = 0; i < count; i++)
        {
            sectorCubes[i] = FindBackgroundCubeBySector((Sector)i);
        }
    }


    void InitBackgroundCubes()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("BackgroundCube");
        SortCubes(gos);
        SetCubeNeighbors();
    }


    void SortCubes(GameObject[] gos)
    {
        List<float> columnsY = new List<float>();
        foreach (var go in gos)
        {
            if (!columnsY.Contains(go.transform.position.y))
            {
                columnsY.Add(go.transform.position.y);
            }
        }

        columnsY.Sort();
        columnsY.Reverse();

        foreach (var col in columnsY)
        {
            List<BackgroundCube> cubeColumns = new List<BackgroundCube>();
            foreach (var go in gos)
            {
                if (Mathf.Approximately(col, go.transform.position.y))
                {
                    cubeColumns.Add(go.GetComponent<BackgroundCube>());
                }
            }

            cubeColumns.Sort(delegate (BackgroundCube a, BackgroundCube b)
            {
                if (a.transform.position.x > b.transform.position.x)
                {
                    return 1;
                }
                else if (a.transform.position.x < b.transform.position.x)
                {
                    return -1;
                }

                return 0;
            });

            backgroundCubes.Add(cubeColumns);
        }
    }


    void SetCubeNeighbors()
    {
        List<BackgroundCube> cubeColumns;
        for (int i = 0; i < backgroundCubes.Count; i++)
        {
            cubeColumns = backgroundCubes[i];

            for (int j = 0; j < cubeColumns.Count; j++)
            {
                if (CheckValidIndexOfBackgroundCubes(i - 1, j - 1))
                {
                    cubeColumns[j].SetNeighborCube(NeighborType.UpLeft, backgroundCubes[i - 1][j - 1]);
                }

                if (CheckValidIndexOfBackgroundCubes(i - 1, j))
                {
                    cubeColumns[j].SetNeighborCube(NeighborType.Up, backgroundCubes[i - 1][j]);
                }

                if (CheckValidIndexOfBackgroundCubes(i - 1, j + 1))
                {
                    cubeColumns[j].SetNeighborCube(NeighborType.UpRight, backgroundCubes[i - 1][j + 1]);
                }

                if (CheckValidIndexOfBackgroundCubes(i, j - 1))
                {
                    cubeColumns[j].SetNeighborCube(NeighborType.Left, backgroundCubes[i][j - 1]);
                }

                if (CheckValidIndexOfBackgroundCubes(i, j + 1))
                {
                    cubeColumns[j].SetNeighborCube(NeighborType.Right, backgroundCubes[i][j + 1]);
                }

                if (CheckValidIndexOfBackgroundCubes(i + 1, j - 1))
                {
                    cubeColumns[j].SetNeighborCube(NeighborType.DownLeft, backgroundCubes[i + 1][j - 1]);
                }

                if (CheckValidIndexOfBackgroundCubes(i + 1, j))
                {
                    cubeColumns[j].SetNeighborCube(NeighborType.Down, backgroundCubes[i + 1][j]);
                }

                if (CheckValidIndexOfBackgroundCubes(i + 1, j + 1))
                {
                    cubeColumns[j].SetNeighborCube(NeighborType.DownRight, backgroundCubes[i + 1][j + 1]);
                }
            }
        }
    }

    bool CheckValidIndexOfBackgroundCubes(int i, int j)
    {
        if (i < 0 || j < 0)
            return false;

        if (backgroundCubes.Count <= i)
            return false;

        if(backgroundCubes[i].Count <= j)
            return false;

        return true;
    }


    public void StartTwinkleBackground()
    {
        StopAllCoroutines();
        StartCoroutine(TwinkleBackground());
    }

    public void StopTwinkleBackground()
    {
        StopAllCoroutines();
    }

    IEnumerator TwinkleBackground()
    {
        foreach (var columns in backgroundCubes)
        {
            foreach (var cube in columns)
            {
                cube.Twinkle();
                yield return new WaitForSeconds(0.02f);
            }
        }

        StartCoroutine(TwinkleBackground());
    }
}
