using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UIContent
{
    public GameObject go;
    public TextMesh textMesh;
    public Renderer outLine;
}



public class UIManager : MonoBehaviour {

    public static UIManager instance;

    public GameObject screen;
    public TextMesh titleText;
    public GameObject centerTextGO;
    public TextMesh centerText;
    public GameObject centerTextArrows;
    public VerticalContentManager verticalContentManager;
    public TimeLine timeLine;


    private void Awake()
    {
        if (UIManager.instance == null)
        {
            UIManager.instance = this;
        }
    }

    public void UpdateUI()
    {
        StopAllCoroutines();
        SetUIScreenVisible(true);
        centerText.fontSize = 80;

        GameState gameState = GameManager.instance.GetGameState();

        if (gameState != GameState.New_DivideSection && gameState != GameState.New_EditMusic_Edit)
            SetTimeLineVisible(false);

        switch (gameState)
        {
            case GameState.Loading:
                IsSlideMode(true, false);
                titleText.text = "Loading Files";
                StartCoroutine(RepeatText("Loading"));
                break;
            case GameState.MainMenu:
                IsSlideMode(false);
                titleText.text = "Main Menu";
                break;
            case GameState.New_MusicSelection:
                IsSlideMode(true);
                titleText.text = "Select Music";
                UpdateCenterTextIfContentExist();
                break;
            case GameState.New_Menu:
                IsSlideMode(false);
                titleText.text = "Edit Menu";
                break;
            case GameState.Option:
                IsSlideMode(false);
                titleText.text = "Option";
                break;
            case GameState.Load_MusicSelection:
                IsSlideMode(true);
                titleText.text = "Select Music";
                UpdateCenterTextIfContentExist();
                break;
            case GameState.New_DivideSection:
                IsSlideMode(true, false);
                titleText.text = "Divide Sections";
                StartCoroutine(RepeatText("Recording"));
                SetTimeLineVisible(true);
                break;
            case GameState.New_EditMusic_KeySelection:
                IsSlideMode(false);
                titleText.text = "Select Keys";
                break;
            case GameState.New_EditMusic_Edit:
                IsSlideMode(true, false);
                titleText.text = "Edit Music";
                StartCoroutine(RepeatText("Recording"));
                SetTimeLineVisible(true);
                break;
            case GameState.PlayGame:
                // changed it to count 3 seconds then disappear
                //SetUIScreenVisible(false);
                ScreenCountTimeThenDisappear(3);
                break;
        }

        verticalContentManager.UpdateUI(gameState);
    }


    public void OnBeatCubeHitForMenu(BeatCube cube)
    {
        switch (cube.sector)
        {
            case Sector.Left:
                verticalContentManager.ChangeSelectedContent(true);
                break;
            case Sector.Center:
                verticalContentManager.ConfirmSelectedMenu();
                cube.SubmitReaction();
                BackgroundManager.instance.SetTwinkle(cube.sector);
                break;
            case Sector.Right:
                verticalContentManager.ChangeSelectedContent(false);
                break;
        }
    }


    IEnumerator RepeatText(string text)
    {
        int count = 0;

        while (true)
        {
            if (count == 0)
                centerText.text = text + ".";
            else if (count == 1)
                centerText.text = text + "..";
            else
                centerText.text = text + "...";

            count++;

            count = count % 3;

            yield return new WaitForSeconds(0.5f);
        }
    }
    

    void UpdateCenterTextIfContentExist()
    {
        if (AudioManager.instance.GetAudioClip() != null)
        {
            SetCenterText(AudioManager.instance.GetAudioClip().name);
        }
        else
        {
            SetCenterText("No Music Found");
        }
    }


    void SetCenterText(string text)
    {
        if(text.Length > 20)
        {
            text = text.Substring(0, 20);
            text += "..";
        }

        centerText.text = text;
    }


    void SetUIScreenVisible(bool visible)
    {
        screen.SetActive(visible);
    }

    void SetTimeLineVisible(bool visible)
    {
        if(visible)
        {
            timeLine.gameObject.SetActive(true);
            timeLine.SetTimeLine(true);
        }
        else
        {
            timeLine.gameObject.SetActive(false);
            timeLine.SetTimeLine(false);
            timeLine.RemoveAllSectionPoints();
        }
    }


    void IsSlideMode(bool slideMode, bool arrowsOn = true)
    {
        if(slideMode)
        {

            verticalContentManager.RemoveAllContent();

            centerTextGO.SetActive(true);
        }
        else
        {
            centerTextGO.SetActive(false);
        }

        centerTextArrows.SetActive(arrowsOn);
    }

    

    void ScreenCountTimeThenDisappear(int timeInSeconds)
    {
        SetUIScreenVisible(true);
        IsSlideMode(true);
        centerTextArrows.SetActive(false);
        titleText.text = "";
        centerText.fontSize = 240;
        StartCoroutine(ScreenCountTime(timeInSeconds));
    }

    IEnumerator ScreenCountTime(int timeInSeconds)
    {
        int count = timeInSeconds;

        while (true)
        {
            centerText.text = count.ToString();

            if (count <= 0)
                break;

            count--;

            yield return new WaitForSeconds(1f);
        }

        SetUIScreenVisible(false);
        centerText.fontSize = 80;
    }


    public void UpdateTimeLineCursor()
    {
        timeLine.UpdateCursor();
    }

    public void AddSectionPointToTimeLine(float time)
    {
        timeLine.AddSectionPoint(time);
    }
}
