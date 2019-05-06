using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BeatCubeSetType
{
    set_3,
    set_6,
    set_9,
}

public enum IconType
{
    Forward,
    Backward,
    Divide,
    Select,
    Down,
    Up,
}


public class BeatCubeManager : MonoBehaviour {

    public static BeatCubeManager instance;

    public GameObject beatCube_3set;
    public GameObject beatCube_6set;
    public GameObject beatCube_9set;

    public BeatCube leftCube;
    public BeatCube centerCube;
    public BeatCube rightCube;

    public Material icon_Up;
    public Material icon_Down;
    public Material icon_Select;
    public Material icon_Forward;
    public Material icon_Backward;
    public Material icon_Divide;
    public Material icon_Right;
    public Material icon_Left;

    BeatCubeSetType curSetType;

    private void Awake()
    {
        if (BeatCubeManager.instance == null)
        {
            BeatCubeManager.instance = this;
        }
    }

    private void Start()
    {
        beatCube_3set.SetActive(true);
        beatCube_6set.SetActive(false);

        curSetType = BeatCubeSetType.set_3;
    }

    public void ChangeBeatCubeSet(BeatCubeSetType type)
    {
        GameState gameState = GameManager.instance.GetGameState();

        switch (type)
        {
            case BeatCubeSetType.set_3:
                if (curSetType != BeatCubeSetType.set_3)
                {
                    beatCube_6set.SetActive(false);
                    beatCube_3set.SetActive(true);
                }
                break;
            case BeatCubeSetType.set_6:
                if (curSetType != BeatCubeSetType.set_6)
                {
                    beatCube_3set.SetActive(false);
                    beatCube_6set.SetActive(true);
                }
                break;
            case BeatCubeSetType.set_9:
                break;
        }

        if (gameState == GameState.New_EditMusic_KeySelection || gameState == GameState.PlayGame)
        {
            StartCoroutine(DelayChangeBeatCubeSet(type));
        }

        curSetType = type;
    }

    public BeatCubeSetType GetSetType()
    {
        return curSetType;
    }


    public void UpdateBeatCubeForm()
    {
        SetCubeIconsVisible(true);


        GameState gameState = GameManager.instance.GetGameState();

        if (gameState != GameState.New_EditMusic_KeySelection && gameState != GameState.PlayGame 
            && gameState != GameState.New_EditMusic_Edit)
            ChangeBeatCubeSet(BeatCubeSetType.set_3);

        switch (gameState)
        {
            case GameState.New_DivideSection:
                SetCubeIcons(icon_Backward, icon_Divide, icon_Forward);
                break;
            case GameState.New_MusicSelection:
                SetCubeIcons(icon_Left, icon_Select, icon_Right);
                break;
            case GameState.Load_MusicSelection:
                SetCubeIcons(icon_Left, icon_Select, icon_Right);
                break;
            case GameState.PlayGame:
                SetCubeIconsVisible(false);
                break;
            case GameState.New_EditMusic_Edit:
                SetCubeIconsVisible(false);
                break;
            case GameState.Loading:
                SetCubeIconsVisible(false);
                break;
            default:
                SetCubeIcons(icon_Up, icon_Select, icon_Down);
                break;
        }
    }


    void SetCubeIcons(Material left, Material center, Material right)
    {
        leftCube.SetIcon(left);
        centerCube.SetIcon(center);
        rightCube.SetIcon(right);
    }

    void SetCubeIconsVisible(bool on)
    {
        leftCube.SetIconVisible(on);
        centerCube.SetIconVisible(on);
        rightCube.SetIconVisible(on);
    }

    IEnumerator DelayChangeBeatCubeSet(BeatCubeSetType type)
    {
        BeatCube[] beatCubes = GetComponentsInChildren<BeatCube>();
        foreach(var cube in beatCubes)
        {
            cube.GetComponent<Collider>().enabled = false;
        }

        yield return new WaitForSeconds(1);

        foreach (var cube in beatCubes)
        {
            cube.GetComponent<Collider>().enabled = true;
        }
    }
}
