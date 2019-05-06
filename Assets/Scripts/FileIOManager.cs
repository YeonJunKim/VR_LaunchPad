using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public struct Beat
{
    public float startTime;
    public float endTime;
}


[System.Serializable]
public class IDBeat
{
    public int id;
    public float startTime;

    public IDBeat(int _id, float _startTime)
    {
        id = _id;
        startTime = _startTime;
    }
}

[System.Serializable]
public class SectionInfo
{
    public int section;
    public float startTime;
    public List<IDBeat> iDBeats;
}

[System.Serializable]
public class MusicInfo
{
    public string musicName;
    public BeatCubeSetType setType;
    public List<SectionInfo> sectionInfos;
}


public class SoundFile
{
    public string FolderName;
    public string ClipName;
    public AudioClip Clip;
}


public class FileIOManager : MonoBehaviour {

    public static FileIOManager instance;

    string dataPath;
    List<string> folderNames;
    List<SoundFile> soundFiles;
    List<MusicInfo> musicInfos;



    private void Awake()
    {
        if (FileIOManager.instance == null)
        {
            FileIOManager.instance = this;
        }

        dataPath = Application.streamingAssetsPath;
        folderNames = new List<string>();
        soundFiles = new List<SoundFile>();
        musicInfos = new List<MusicInfo>();

        GetFoldersInDataPath();
    }


    void GetFoldersInDataPath()
    {
        folderNames.Clear();

        folderNames.Add("");   // default path (witch is the /StreamingAssets/)

        string[] directories = Directory.GetDirectories(dataPath);

        foreach (var directory in directories)
        {
            folderNames.Add(directory.Substring(dataPath.Length));

        }
    }


    public void LoadSoundFiles()
    {
        GetFoldersInDataPath();
        StartCoroutine(LoadSounds());
    }

    IEnumerator LoadSounds()
    {
        soundFiles.Clear();

        foreach (var folderName in folderNames)
        {
            string path = dataPath + folderName;

            string[] files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                string extension = file.Substring(file.Length - 4);
                if (!extension.Equals(".ogg") && !extension.Equals(".wav"))
                {
                    continue;
                }

                WWW www = new WWW(file);
                yield return www;

                AudioClip clip = www.GetAudioClip();
                if (clip)
                {
                    SoundFile soundFile = new SoundFile();
                    soundFile.Clip = www.GetAudioClip();

                    soundFile.ClipName = file.Substring(path.Length + 1, file.Length - (path.Length + 1) - 4);
                    soundFile.Clip.name = soundFile.ClipName;
                    soundFile.FolderName = folderName;

                    soundFiles.Add(soundFile);

                    Debug.Log(soundFile.ClipName);
                }
            }
        }

        GameManager.instance.ChangeState(GameState.MainMenu, true);
    }



    public void SaveMusicInfo(string musicName, List<float> sections, List<List<IDBeat>> iDBeats)
    {
        MusicInfo musicInfo = new MusicInfo();

        List<SectionInfo> sectionInfos = new List<SectionInfo>();

        for (int section = 0; section < sections.Count; section++)
        {
            SectionInfo sectionInfo = new SectionInfo();
            sectionInfo.section = section;
            sectionInfo.iDBeats = iDBeats[section];
            sectionInfo.startTime = sections[section];
            sectionInfos.Add(sectionInfo);
        }

        musicInfo.musicName = musicName;
        musicInfo.sectionInfos = sectionInfos;
        musicInfo.setType = BeatCubeManager.instance.GetSetType();

        string toJson = JsonUtility.ToJson(musicInfo, prettyPrint: true);
        File.WriteAllText(dataPath + "/" + musicName + ".json", toJson);
    }


    public void LoadMusicInfos()
    {
        GetFoldersInDataPath();
        musicInfos.Clear();

        foreach (var folderName in folderNames)
        {
            string path = dataPath + folderName;

            string[] files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                string extension = file.Substring(file.Length - 5);
                if (!extension.Equals(".json"))
                {
                    continue;
                }

                string jsonString = File.ReadAllText(file);
                var data = JsonUtility.FromJson<MusicInfo>(jsonString);

                musicInfos.Add(data);
            }
        }
    }



    public List<SoundFile> GetSoundFiles()
    {
        return soundFiles;
    }

    public SoundFile GetSoundFile(string musicName)
    {
        foreach(var file in soundFiles)
        {
            if (file.ClipName.Equals(musicName))
            {
                return file;
            }
        }

        return null;
    }

    public List<SoundFile> GetSoundFilesOfPremade()
    {
        List<SoundFile> tempList = new List<SoundFile>();

        foreach (var soundFile in soundFiles)
        {
            foreach(var musicInfo in musicInfos)
            {
                if (soundFile.ClipName.Equals(musicInfo.musicName))
                {
                    tempList.Add(soundFile);
                }
            }
        }

        return tempList;
    }


    public MusicInfo GetMusicInfo(string musicName)
    {
        foreach (var musicInfo in musicInfos)
        {
            if (musicInfo.musicName.Equals(musicName))
            {
                return musicInfo;
            }
        }

        return null;
    }


}

