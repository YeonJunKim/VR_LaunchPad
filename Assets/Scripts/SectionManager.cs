using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionManager : MonoBehaviour {

    public static SectionManager instance;


    public GameObject sectionButtonPrefab;

    List<SectionButton> sectionButtons;
    int currentSection = 0;

    const int maxSections = 4;
    List<float> sections;

    private void Awake()
    {
        if (SectionManager.instance == null)
        {
            SectionManager.instance = this;
        }

        sectionButtons = new List<SectionButton>();
        sections = new List<float>();
    }


    public void StartDividingSections()
    {
        Init();
        CreateSectionButton();
        AudioManager.instance.PlayAudio();
    }

    public void Init()
    {
        InitSectionButtons();
        sections.Clear();
        currentSection = 0;

        sections.Add(0);
    }

    void InitSectionButtons()
    {
        foreach (var button in sectionButtons)
        {
            Destroy(button.gameObject);
        }
        sectionButtons.Clear();
    }


    public void ChangeSection(int section)
    {
        currentSection = section;

        foreach (var sb in sectionButtons)
        {
            if (sb.GetSection() == section)
                sb.SetLight(true);
            else
                sb.SetLight(false);
        }
    }


    public int GetCurrentSection()
    {
        return currentSection;
    }



    public bool AddSection()
    {
        if (AudioManager.instance.IsPlaying() == false)
            return false;

        if (sections.Count >= maxSections)
        {
            Debug.Log("Cannot add more than " + maxSections + " sections");
            return false;
        }

        float currentPlayBackTime = AudioManager.instance.GetCurrentPlayTime();
        if (sections.Count != 0)
        {
            if (sections[sections.Count - 1] > currentPlayBackTime)
            {
                Debug.Log("new section has to be 'later' than this point");
                return false;
            }
        }

        sections.Add(currentPlayBackTime);
        CreateSectionButton();

        return true;
    }



    void CreateSectionButton()
    {
        GameObject inst = Instantiate(sectionButtonPrefab);
        inst.transform.parent = this.transform;

        SectionButton sb = inst.GetComponent<SectionButton>();
        sb.SetSection(sections.Count - 1);
        sectionButtons.Add(sb);

        SetButtonPositions();
    }

    void SetButtonPositions()
    {
        float xPadding = 0;
        if (sectionButtons.Count % 2 == 1)
            xPadding += 0.5f;

        for (int i = 0; i < sectionButtons.Count; i++)
        {
            float xPos = (maxSections - sections.Count) / 2 + i + xPadding;
            sectionButtons[i].transform.localPosition = new Vector3(xPos, 0, 0);
        }
    }

    public void CreateSectionButtons()
    {
        for (int i = 0; i < sections.Count; i++)
        {
            GameObject inst = Instantiate(sectionButtonPrefab);
            inst.transform.parent = this.transform;
            float xPos = (maxSections - sections.Count) / 2 + i;
            inst.transform.localPosition = new Vector3(xPos, 0, 0);

            SectionButton sb = inst.GetComponent<SectionButton>();
            sb.SetSection(i);
            sectionButtons.Add(sb);
        }
    }


    public List<float> GetSections()
    {
        return sections;
    }

    public void SetSections(List<float> _sections)
    {
        Init();
        sections = _sections;
    }

    public float GetCurrentSectionStartTime()
    {
        return sections[currentSection];
    }

    public float GetNextSectionStartTime()
    {
        if(currentSection < sections.Count - 1)
            return sections[currentSection + 1];

        return float.MaxValue;
    }


    public float GetDurationOfCurrentSection()
    {
        int i = currentSection;

        if(i + 1 >= sections.Count)
        {
            return -1;
        }
           
        return sections[i + 1] - sections[i];
    }

}
