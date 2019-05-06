using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


public class EditorManager : MonoBehaviour
{

    public static EditorManager instance;

    List<List<IDBeat>> iDBeats;


    private void Awake()
    {
        if (EditorManager.instance == null)
        {
            EditorManager.instance = this;
        }

        iDBeats = new List<List<IDBeat>>();
    }


    public void OnBeatCubeHit(BeatCube cube)
    {
        StartMusicIfItsTheFirstSection();

        if (AudioManager.instance.IsPlaying() && CheckAudioPlaybackTimeIsWithinSection())
        {
            AddIDBeats(cube.id);

            cube.SubmitReaction();
            BackgroundManager.instance.SetTwinkle(cube.sector);
        }
    }


    // if its the first hit of section 0
    // the music needs to start, and has to set a timer to stop the music by next section
    void StartMusicIfItsTheFirstSection()
    {
        int section = SectionManager.instance.GetCurrentSection();

        if (section == 0 && AudioManager.instance.IsPlaying() == false)
        {
            AudioManager.instance.PlayAudio();

            // if it was the last section, -1 will be returned
            float invokeTime = SectionManager.instance.GetDurationOfCurrentSection();

            if (invokeTime > 0)
                AudioManager.instance.SetStopTimer(invokeTime);
        }
    }

    void AddIDBeats(int id)
    {
        int section = SectionManager.instance.GetCurrentSection();

        iDBeats[section].Add(new IDBeat(id, AudioManager.instance.GetCurrentPlayTime()));
    }


    public void ChangeSection(int section)
    {
        iDBeats[section].Clear();

        if (section != 0)
        {
            // give it a little bit of time to listen to the music before going into the section
            // so I give 3 seconds
            int delayTime = 3;
            float time = SectionManager.instance.GetCurrentSectionStartTime() - delayTime;
            AudioManager.instance.PlayAudioAtPoint(time);

            // if it was the last section, -1 will be returned
            float invokeTime = SectionManager.instance.GetDurationOfCurrentSection();

            if (invokeTime > 0)
                AudioManager.instance.SetStopTimer(invokeTime + delayTime);
        }
        else
        {
            AudioManager.instance.StopAudio();
        }
    }





    public void StartEditingMusic()
    {
        AudioManager.instance.StopAudio();

        iDBeats.Clear();
        List<float> sections = SectionManager.instance.GetSections();
        foreach (var section in sections)
        {
            List<IDBeat> idBeatList = new List<IDBeat>();
            iDBeats.Add(idBeatList);

            if(Mathf.Approximately(section, 0) == false)
                UIManager.instance.AddSectionPointToTimeLine(section);
        }
    }


    public void SaveMusicInfo()
    {
        FileIOManager.instance.SaveMusicInfo(AudioManager.instance.GetAudioClip().name,
                                                SectionManager.instance.GetSections(),
                                                iDBeats);
    }




    bool CheckAudioPlaybackTimeIsWithinSection()
    {
        List<float> sections = SectionManager.instance.GetSections();
        int currentSection = SectionManager.instance.GetCurrentSection();

        float startTime = sections[currentSection];
        float endTime;
        if (sections.Count > currentSection + 1)
            endTime = sections[currentSection + 1];
        else
            endTime = AudioManager.instance.GetAudioClip().length;

        float currentPlayBackTime = AudioManager.instance.GetCurrentPlayTime();
        if (currentPlayBackTime >= startTime && currentPlayBackTime <= endTime)
            return true;
        else
            return false;
    }




}
