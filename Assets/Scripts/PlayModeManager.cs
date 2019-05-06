using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayModeManager : MonoBehaviour {

    public static PlayModeManager instance;

    public GameObject guideEffectPrefab;

    MusicInfo musicInfo;
    List<BeatCube> beatCubes;
    List<List<IDBeat>> iDBeats;
    AudioClip cilpToPlay;
    string musicToPlay;

    List<ParticleSystem> guideEffects;
    int countGuideEffectBeats = 0;
    int countGuideEffectSection = 0;
    float elapsedTimeSincePlayStarted;
    float musicStartdelayTime = 5;
    float guideEffectPreTime = 1.15f;

    int currentSection = 0;
    bool createGuideEffects = true;
    bool autoChangeSection = true;


    private void Awake()
    {
        if (PlayModeManager.instance == null)
        {
            PlayModeManager.instance = this;
        }

        beatCubes = new List<BeatCube>();
        iDBeats = new List<List<IDBeat>>();
        guideEffects = new List<ParticleSystem>();
    }


    private void Start()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject go = Instantiate(guideEffectPrefab);
            go.transform.SetParent(this.transform);
            guideEffects.Add(go.GetComponent<ParticleSystem>());
        }
    }

    private void Update()
    {
        if(GameManager.instance.GetGameState() == GameState.PlayGame)
        {
            elapsedTimeSincePlayStarted += Time.deltaTime;

            if (createGuideEffects)
                CreateGuideEffects();

            if (autoChangeSection)
                AutoChangeSection();
        }

    }


    public void StartPlayMode(string _musicToPlay)
    {
        musicToPlay = _musicToPlay;

        GetBeatCubes();

        SoundFile soundFile = FileIOManager.instance.GetSoundFile(musicToPlay);
        cilpToPlay = soundFile.Clip;

        musicInfo = FileIOManager.instance.GetMusicInfo(musicToPlay);

        SectionManager.instance.SetSections(ExtractSectionStartTimesFromMusicInfo(musicInfo));
        SectionManager.instance.CreateSectionButtons();

        InsertBeatsIntoBeatCubes();

        countGuideEffectBeats = 0;
        countGuideEffectSection = 0;
        elapsedTimeSincePlayStarted = -musicStartdelayTime;
        createGuideEffects = true;
        autoChangeSection = true;
    }

    public void ResetPlayMode()
    {
        foreach (var beatCube in beatCubes)
        {
            beatCube.ResetPlayMode();
        }

        countGuideEffectBeats = 0;
        countGuideEffectSection = 0;
        elapsedTimeSincePlayStarted = -musicStartdelayTime;
        createGuideEffects = true;
    }



    void InsertBeatsIntoBeatCubes()
    {
        iDBeats.Clear();

        foreach (var sectionInfo in musicInfo.sectionInfos)
        {
            iDBeats.Add(sectionInfo.iDBeats);
        }

        foreach (var beatCube in beatCubes)
        {
            beatCube.SetAudioClip(cilpToPlay);
            beatCube.MakeSections(iDBeats.Count);
        }

        for (int section = 0; section < iDBeats.Count; section++)
        {
            for (int beatNum = 0; beatNum < iDBeats[section].Count; beatNum++)
            {
                foreach (var beatCube in beatCubes)
                {
                    if (iDBeats[section][beatNum].id == beatCube.id)
                    {
                        Beat beat = new Beat();
                        beat.startTime = iDBeats[section][beatNum].startTime;

                        // the last Beat plays the music till the end
                        if (IsThisIDBeatTheLastOne(section, beatNum))
                            beat.endTime = cilpToPlay.length;
                        else
                            beat.endTime = FindTheNextIDBeat(section, beatNum).startTime;

                        beatCube.AddBeat(section, beat);
                    }
                }
            }
        }
    }



    public void ChangeSection(int section)
    {
        foreach (var cube in beatCubes)
        {
            cube.ChangeSection(section);
        }

        currentSection = section;
        SectionManager.instance.ChangeSection(section);

        if(!autoChangeSection)
        {
            countGuideEffectBeats = 0;
            countGuideEffectSection = section;
        }
    }


    void GetBeatCubes()
    {
        beatCubes.Clear();

        BeatCube[] beatCubeList = BeatCubeManager.instance.transform.GetComponentsInChildren<BeatCube>();
        foreach (var cube in beatCubeList)
        {
            beatCubes.Add(cube);
        }
    }


    void AutoChangeSection()
    {
        if(SectionManager.instance.GetNextSectionStartTime() < elapsedTimeSincePlayStarted)
        {
            int sectionToChange = currentSection + 1;
            if (sectionToChange > iDBeats.Count - 1)
                return;

             ChangeSection(sectionToChange);
        }
    }


    void CreateGuideEffects()
    {
        if (countGuideEffectBeats > iDBeats[countGuideEffectSection].Count - 1)
            return;

        float excuteTime = iDBeats[countGuideEffectSection][countGuideEffectBeats].startTime - guideEffectPreTime;

        if (excuteTime < elapsedTimeSincePlayStarted)
        {
            ParticleSystem guideEffect = GetRestingGuideEffect();
            Vector3 targetPos = GetBeatCubeWithID(iDBeats[countGuideEffectSection][countGuideEffectBeats].id).transform.position;
            targetPos.y += 0.5f;
            guideEffect.transform.position = targetPos;
            guideEffect.Play();

            if (IsThisIDBeatTheLastOne(countGuideEffectSection, countGuideEffectBeats))
            {
                createGuideEffects = false;
            }
            else
            {
                countGuideEffectBeats++;

                if (countGuideEffectBeats > iDBeats[countGuideEffectSection].Count - 1)
                {
                    countGuideEffectSection++;
                    countGuideEffectBeats = 0;
                }
            }
        }
    }


    ParticleSystem GetRestingGuideEffect()
    {
        foreach (var guideEffect in guideEffects)
        {
            if (guideEffect.isStopped)
            {
                return guideEffect;
            }
        }
        return null;
    }


    BeatCube GetBeatCubeWithID(int id)
    {
        foreach (var cube in beatCubes)
        {
            if (cube.id == id)
                return cube;
        }

        return null;
    }


    public void StopAllSounds()
    {
        foreach(var cube in beatCubes)
        {
            cube.ResetBeats();
        }
    }


    bool IsThisIDBeatTheLastOne(int sectionNum, int beatNum)
    {
        // check if more iDBeats left in the same section
        if (beatNum < iDBeats[sectionNum].Count - 1)
            return false;

        // check if all the other sections left are empty sections
        for (int i = sectionNum + 1; i < iDBeats.Count; i++)
        {
            if (iDBeats[i].Count != 0)
                return false;
        }

        return true;
    }

    IDBeat FindTheNextIDBeat(int sectionNum, int beatNum)
    {
        if (beatNum < iDBeats[sectionNum].Count - 1)
            return iDBeats[sectionNum][beatNum + 1];

        for (int i = sectionNum + 1; i < iDBeats.Count; i++)
        {
            if (iDBeats[i].Count != 0)
            {
                return iDBeats[i][0];
            }
        }

        return null;
    }
    
    List<float> ExtractSectionStartTimesFromMusicInfo(MusicInfo musicInfo)
    {
        List<float> timeList = new List<float>();

        foreach(var sectionInfo in musicInfo.sectionInfos)
        {
            timeList.Add(sectionInfo.startTime);
        }

        return timeList;
    }


    public void SetAutoChangeSection(bool on)
    {
        autoChangeSection = on;
    }

    public float GetElapsedTime()
    {
        return elapsedTimeSincePlayStarted;
    }
}
