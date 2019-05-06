using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum Sector
{
    // for 3 set
    Left,
    Center,
    Right,

    // for 6 set
    Left_Left,
    Center_Left,
    Center_Right,
    Right_Right,
    Top_Left,
    Top_Right,
}

public enum GameState
{
    Loading,
    MainMenu,
    Option,

    New_MusicSelection,
    New_Menu,
    New_DivideSection,
    New_EditMusic_KeySelection,
    New_EditMusic_Edit,
    New_LightEffect,

    Load_MusicSelection,

    PlayGame,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    GameState gameState;
    List<ControlButton> controlButtons;

    List<SoundFile> soundFiles;
    int countSoundNum = 0;

    List<GameState> stateStack;

    private void Awake()
    {
        if (GameManager.instance == null)
        {
            GameManager.instance = this;
        }

        controlButtons = new List<ControlButton>();
        soundFiles = new List<SoundFile>();
        stateStack = new List<GameState>();
    }

    private void Start()
    {
        GameObject[] buttonGOs = GameObject.FindGameObjectsWithTag("ControlButton");

        foreach (var go in buttonGOs)
        {
            controlButtons.Add(go.GetComponent<ControlButton>());
        }

        ChangeState(GameState.Loading, false);
    }


    public void OnButtonClick(ButtonType type)
    {
        switch (type)
        {
            case ButtonType.Save:
                SaveEditInfos();
                break;
            case ButtonType.Refresh:
                ResetDoings();
                break;
            case ButtonType.Back:
                if (gameState == GameState.New_EditMusic_Edit)
                    SaveEditInfos();

                ChangeToPreviousState();
                break;
        }
    }


    public void OnBeatCubeHit(BeatCube cube)
    {
        //AudioManager.instance.PlayDrumSound();

        if (gameState == GameState.PlayGame)
        {
            bool react = cube.PlaySound();
            if (react)
            {
                cube.SubmitReaction();
                BackgroundManager.instance.SetTwinkle(cube.sector);
            }
            return;
        }

        if (gameState == GameState.New_EditMusic_Edit)
        {
            EditorManager.instance.OnBeatCubeHit(cube);
            return;
        }


        if (IsMenuSelectingState(gameState))
        {
            UIManager.instance.OnBeatCubeHitForMenu(cube);
            return;
        }


        switch (cube.sector)
        {
            case Sector.Left:
                if (gameState == GameState.New_MusicSelection || gameState == GameState.Load_MusicSelection)
                {
                    MoveToNextMusic(true);
                    UIManager.instance.UpdateUI();
                }
                else if (gameState == GameState.New_DivideSection)
                {
                    AudioManager.instance.AudioFastForward(false);
                    UIManager.instance.UpdateTimeLineCursor();
                }
                break;
            case Sector.Center:
                if (gameState == GameState.New_MusicSelection)
                {
                    if (AudioManager.instance.GetAudioClip() != null)
                    {
                        ChangeState(GameState.New_Menu, true);
                        cube.SubmitReaction();
                        BackgroundManager.instance.SetTwinkle(cube.sector);
                    }
                }
                else if (gameState == GameState.Load_MusicSelection)
                {
                    if (AudioManager.instance.GetAudioClip() != null)
                    {
                        ChangeState(GameState.PlayGame, true);
                        cube.SubmitReaction();
                        BackgroundManager.instance.SetTwinkle(cube.sector);
                    }
                }
                else if (gameState == GameState.New_DivideSection)
                {
                    bool react = SectionManager.instance.AddSection();
                    if (react)
                    {
                        cube.SubmitReaction();
                        BackgroundManager.instance.SetTwinkle(cube.sector);
                        UIManager.instance.AddSectionPointToTimeLine(AudioManager.instance.GetCurrentPlayTime());
                    }
                }
                break;
            case Sector.Right:
                if (gameState == GameState.New_MusicSelection || gameState == GameState.Load_MusicSelection)
                {
                    MoveToNextMusic(false);
                    UIManager.instance.UpdateUI();
                }
                else if (gameState == GameState.New_DivideSection)
                {
                    AudioManager.instance.AudioFastForward(true);
                    UIManager.instance.UpdateTimeLineCursor();
                }
                break;
        }
    }



    public void ChangeState(GameState _gameState, bool stackTheState)
    {
        AudioManager.instance.StopAudio();
        PlayModeManager.instance.StopAllSounds();

        if(gameState == GameState.New_Menu && _gameState == GameState.PlayGame)
        {
            if (FileIOManager.instance.GetMusicInfo(AudioManager.instance.GetAudioClip().name) == null)
            {
                Debug.Log("unable to get music info");
                return;
            }
        }

        gameState = _gameState;

        switch (_gameState)
        {
            case GameState.Loading:
                FileIOManager.instance.LoadMusicInfos();
                FileIOManager.instance.LoadSoundFiles();
                break;
            case GameState.MainMenu:
                BackgroundManager.instance.StartTwinkleBackground();
                break;
            case GameState.New_MusicSelection:
                soundFiles = FileIOManager.instance.GetSoundFiles();
                StartMusicSeletion();
                break;
            case GameState.New_Menu:
                //
                break;
            case GameState.Option:
                //
                break;
            case GameState.Load_MusicSelection:
                soundFiles = FileIOManager.instance.GetSoundFilesOfPremade();
                StartMusicSeletion();
                break;
            case GameState.New_DivideSection:
                SectionManager.instance.StartDividingSections();
                break;
            case GameState.New_EditMusic_Edit:
                InitBeatCubes();
                EditorManager.instance.StartEditingMusic();
                OnSectionButtonClick(0);
                break;
            case GameState.PlayGame:
                BackgroundManager.instance.StopTwinkleBackground();
                AudioManager.instance.StopAudio();
                string musicToPlay = AudioManager.instance.GetAudioClip().name;
                BeatCubeSetType setType = FileIOManager.instance.GetMusicInfo(musicToPlay).setType;
                BeatCubeManager.instance.ChangeBeatCubeSet(setType);
                InitBeatCubes();
                PlayModeManager.instance.StartPlayMode(musicToPlay);
                OnSectionButtonClick(0);
                break;
        }

        UIManager.instance.UpdateUI();
        BeatCubeManager.instance.UpdateBeatCubeForm();

        if (stackTheState)
            StackState();
    }

    public void ChangeToPreviousState()
    {
        if (stateStack.Count <= 1)
            return;

        int lastIndex = stateStack.Count - 1;
        GameState temp = stateStack[lastIndex - 1];
        stateStack.RemoveAt(lastIndex);
        ChangeState(temp, false);
    }


    public void OnSectionButtonClick(int section)
    {
        if (gameState != GameState.New_EditMusic_Edit && gameState != GameState.PlayGame)
            return;

        SectionManager.instance.ChangeSection(section);

        if (gameState == GameState.New_EditMusic_Edit)
            EditorManager.instance.ChangeSection(section);
        else if (gameState == GameState.PlayGame)
        {
            //PlayModeManager.instance.SetAutoChangeSection(false);
            PlayModeManager.instance.ChangeSection(section);
        }
    }



    void StartMusicSeletion()
    {
        if (soundFiles.Count <= 0)
            return;

        countSoundNum = 0;
        AudioManager.instance.SetAudioClip(soundFiles[countSoundNum].Clip);
        AudioManager.instance.PlayAudio();
    }


    void MoveToNextMusic(bool left)
    {
        if (soundFiles.Count <= 0)
            return;

        //audioSource.Stop();

        if (left)
            countSoundNum--;
        else
            countSoundNum++;

        if (countSoundNum >= soundFiles.Count)
            countSoundNum = 0;
        else if (countSoundNum < 0)
            countSoundNum = soundFiles.Count - 1;

        AudioManager.instance.SetAudioClip(soundFiles[countSoundNum].Clip);
        AudioManager.instance.PlayAudio();
    }



    public GameState GetGameState()
    {
        return gameState;
    }


    public void ChangeBeatCubeSet(BeatCubeSetType type)
    {
        if (type == BeatCubeSetType.set_3)
        {
            BeatCubeManager.instance.ChangeBeatCubeSet(BeatCubeSetType.set_3);
        }
        else if (type == BeatCubeSetType.set_6)
        {
            BeatCubeManager.instance.ChangeBeatCubeSet(BeatCubeSetType.set_6);
        }
        else
        {
            BeatCubeManager.instance.ChangeBeatCubeSet(BeatCubeSetType.set_9);
        }
    }


    void InitBeatCubes()
    {
        BeatCube[] beatCubes = BeatCubeManager.instance.transform.GetComponentsInChildren<BeatCube>();

        int count = 0;
        foreach (var cube in beatCubes)
        {
            cube.id = count;
            cube.ResetBeats();

            count++;
        }
    }


    bool IsMenuSelectingState(GameState _gameState)
    {
        if (_gameState == GameState.MainMenu || _gameState == GameState.New_Menu ||
            _gameState == GameState.Option || _gameState == GameState.New_EditMusic_KeySelection)
            return true;

        return false;
    }

    void StackState()
    {
        if (gameState == GameState.New_EditMusic_KeySelection)
            return;

        if (stateStack.Count > 0)
        {
            if (gameState != stateStack[stateStack.Count - 1])
                stateStack.Add(gameState);
        }
        else
        {
            stateStack.Add(gameState);
        }
    }


    public void SaveEditInfos()
    {
        switch (gameState)
        {
            case GameState.New_DivideSection:
                // save Section Infos to file
                break;
            case GameState.New_EditMusic_Edit:
                // load Section Infos from file
                EditorManager.instance.SaveMusicInfo();
                FileIOManager.instance.LoadMusicInfos();
                break;
        }
    }


    public void ResetDoings()
    {
        switch(gameState)
        {
            case GameState.PlayGame:
                PlayModeManager.instance.ResetPlayMode();
                break;
            case GameState.New_DivideSection:
                SectionManager.instance.StartDividingSections();
                break;
            case GameState.New_EditMusic_Edit:
                EditorManager.instance.StartEditingMusic();
                break;
        }
    }
}
