
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UIMenu
{
    Default,

    Main,
    New,
    Load,
    Option,
    Exit,

    DivideSection,
    InsertMusicIntoDrum,
    LightEffect,
    TestPlay,

    Key3,
    Key6,
    Key9,

}


public class VerticalContentManager : MonoBehaviour {

    public GameObject contentPrefab;
    List<VerticalContent> contentActive;
    List<VerticalContent> contentPool;

    int contentIDCount = 0;
    int maxNumOfContents = 6;
    int currentSelectedContent = 0;

    private void Awake()
    {
        contentActive = new List<VerticalContent>();
        contentPool = new List<VerticalContent>();

        for (int i = 0; i < maxNumOfContents; i++)
        {
            GameObject inst = Instantiate<GameObject>(contentPrefab);
            VerticalContent content = inst.GetComponent<VerticalContent>();
            content.transform.SetParent(this.transform);
            content.SetHighLight(false);
            contentPool.Add(content);
            content.gameObject.SetActive(false);
        }
    }

	
    void CreateContent(UIMenu menu, string menuName)
    {
        foreach(var content in contentPool)
        {
            if(content.gameObject.activeSelf == false)
            {
                content.gameObject.SetActive(true);
                content.menu = menu;
                content.contentID = contentIDCount++;
                content.SetText(menuName);
                contentActive.Add(content);
                break;
            }
        }
    }

    public void RemoveAllContent()
    {
        foreach (var content in contentActive)
        {
            content.gameObject.SetActive(false);
        }

        contentActive.Clear();
        contentIDCount = 0;
    }


    void AlignContents()
    {
        float verticalWidth = 3;
        float scale = 1;

        if(contentIDCount > 3)
        {
            scale = 1 - ((contentIDCount - 3) * 0.1f);  // if over 3 content, reduce size by 10%
        }

        this.transform.localScale = new Vector3(scale, scale, this.transform.localScale.z);

        for(int i = 0; i < contentActive.Count; i++)
        {
            float yPos = ((contentActive.Count - 1) * 1.5f) - (i * verticalWidth);
            contentActive[i].transform.localPosition = new Vector3(0, yPos, 0);
        }
    }


    public void ConfirmSelectedMenu()
    {
        UIMenu menu = contentActive[currentSelectedContent].menu;

        switch (menu)
        {
            case UIMenu.New:
                GameManager.instance.ChangeState(GameState.New_MusicSelection, true);
                break;
            case UIMenu.Load:
                GameManager.instance.ChangeState(GameState.Load_MusicSelection, true);
                break;
            case UIMenu.Option:
                GameManager.instance.ChangeState(GameState.Option, true);
                break;
            case UIMenu.Exit:
                Application.Quit();
                break;
            case UIMenu.DivideSection:
                GameManager.instance.ChangeState(GameState.New_DivideSection, true);
                break;
            case UIMenu.InsertMusicIntoDrum:
                if (SectionManager.instance.GetSections().Count <= 0)
                    SectionManager.instance.Init();
                GameManager.instance.ChangeState(GameState.New_EditMusic_KeySelection, true);
                break;
            case UIMenu.LightEffect:
                //GameManager.instance.ChangeState(GameState.New_LightEffect, true);
                break;
            case UIMenu.TestPlay:
                GameManager.instance.ChangeState(GameState.PlayGame, true);
                break;
            case UIMenu.Key3:
                GameManager.instance.ChangeBeatCubeSet(BeatCubeSetType.set_3);
                GameManager.instance.ChangeState(GameState.New_EditMusic_Edit, true);
                break;
            case UIMenu.Key6:
                GameManager.instance.ChangeBeatCubeSet(BeatCubeSetType.set_6);
                GameManager.instance.ChangeState(GameState.New_EditMusic_Edit, true);
                break;
            case UIMenu.Key9:
                //GameManager.instance.ChangeBeatCubeSet(BeatCubeSetType.set_9);
                GameManager.instance.ChangeBeatCubeSet(BeatCubeSetType.set_6);
                GameManager.instance.ChangeState(GameState.New_EditMusic_Edit, true);
                break;
        }
    }


    public void UpdateUI(GameState _gameState)
    {
        List<UIMenu> menus;
        List<string> menuNames;

        switch (_gameState)
        {
            case GameState.MainMenu:
                menus = new List<UIMenu> { UIMenu.New, UIMenu.Load, UIMenu.Option, UIMenu.Exit };
                menuNames = new List<string> { "New", "Load", "Option", "Exit" };
                ChangeContents(menus, menuNames);
                break;
            case GameState.Option:
                menus = new List<UIMenu> { UIMenu.Default, UIMenu.Default, UIMenu.Default, UIMenu.Default };
                menuNames = new List<string> { "Default", "Default", "Default", "Default" };
                ChangeContents(menus, menuNames);
                break;
            case GameState.New_Menu:
                menus = new List<UIMenu> { UIMenu.DivideSection, UIMenu.InsertMusicIntoDrum,
                    UIMenu.LightEffect, UIMenu.TestPlay };
                menuNames = new List<string> { "Divide Section", "Edit Section", "Light Effect", "Test Play" };
                ChangeContents(menus, menuNames);
                break;
            case GameState.New_EditMusic_KeySelection:
                menus = new List<UIMenu> { UIMenu.Key3, UIMenu.Key6, UIMenu.Key9 };
                menuNames = new List<string> { "3 Keys", "6 Keys", "9 Keys" };
                ChangeContents(menus, menuNames);
                break;
        }

   
    }


    void ChangeContents(List<UIMenu> menus, List<string> menuNames)
    {
        if (menus.Count != menuNames.Count)
        {
            Debug.Log("the length doesn't match");
            return;
        }

        RemoveAllContent();

        for (int i = 0; i < menus.Count; i++)
        {
            CreateContent(menus[i], menuNames[i]);
        }

        AlignContents();
        currentSelectedContent = 0;
        HighLightSelectedContent();
    }


    public void ChangeSelectedContent(bool up)
    {
        if (up)
            currentSelectedContent--;
        else
            currentSelectedContent++;

        currentSelectedContent = Mathf.Clamp(currentSelectedContent, 0, contentActive.Count - 1);

        HighLightSelectedContent();
    }

    void HighLightSelectedContent()
    {
        foreach (var content in contentActive)
        {
            content.SetHighLight(false);
        }
        contentActive[currentSelectedContent].SetHighLight(true);
    }

    public VerticalContent GetSelectedContent()
    {
        return contentActive[currentSelectedContent];
    }
}
