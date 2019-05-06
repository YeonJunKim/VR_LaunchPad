using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BeatCube : MonoBehaviour {

    public Transform meshTransfrom;
    public Renderer drumCenterRenderer;
    public Renderer drumSideRenderer;
    public Renderer iconRenderer;
    public ParticleSystem[] particleSystems;

    public int id;
    public Sector sector;

    AudioSource audioSource;
    List<List<Beat>> beats;

    Vector3 meshOriginScale;
    Color originDrumCenterColor;
    Color originDrumSideColor;
    Color drumCenterColorOnHit = Color.white;
    Color drumSideColorOnHit = Color.red;

    bool isRepeatOn = false;
    bool isSetting = false;
    float setStartTime = 0;
    float repeatInterval = 0;
    float countRepeatTime = 0;

    int countBeatNum = 0;
    float fadeTime = 0.0f;

    SpringJoint springJoint;
    Vector3 originPos;

    int currentPlayingSection = 0;


    private void Awake()
    {
        beats = new List<List<Beat>>();
        audioSource = transform.GetComponent<AudioSource>();
        springJoint = transform.GetComponent<SpringJoint>();
        originPos = transform.position;
        iconRenderer.gameObject.SetActive(false);
    }

    private void Start()
    {
        meshOriginScale = meshTransfrom.localScale;
        originDrumCenterColor = drumCenterRenderer.material.GetColor("_EmissionColor");
        originDrumSideColor = drumSideRenderer.material.GetColor("_EmissionColor");
    }


    private void Update()
    {
        RepeatSound();
        ResetRepeatSoundAfterWaiting();

        LimitYPos();
    }


    void LimitYPos()
    {
        if (originPos.y < transform.position.y)
        {
            transform.position = originPos;
        }

        if (originPos.y - transform.position.y > 1)
        {
            transform.position = new Vector3(originPos.x, originPos.y - 1, originPos.z);
        }
    }


    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Stick")
        {
           // HitReaction();
            Stick stick = col.gameObject.gameObject.GetComponent<Stick>();
            if (stick.IsGoingUp() || !(stick.IsMovingFastEnough())) // only trigger when the stick is going downwords, and moving fastEnough
                return;

            HitReaction();
            stick.Vibrate();
            GameManager.instance.OnBeatCubeHit(this);
        }

    }


    // for NoneVRMode
    public void TriggerEnter()
    {
        HitReaction();
        PressDown();

        GameManager.instance.OnBeatCubeHit(this);
    }


    public void ChangeSection(int section)
    {
        currentPlayingSection = section;
        countBeatNum = 0;
    }


    public bool PlaySound()
    {
        if (beats.Count == 0)
            return false;

        if (beats[currentPlayingSection].Count == 0)
            return false;

        if (beats[currentPlayingSection][countBeatNum].startTime < PlayModeManager.instance.GetElapsedTime() - 2)
            return false;
    
        if (countBeatNum >= beats[currentPlayingSection].Count)
            countBeatNum = 0;


        CancelInvoke("StopSound");
        StopAllCoroutines();
        audioSource.volume = 1;
        Invoke("StopSound", beats[currentPlayingSection][countBeatNum].endTime
            - beats[currentPlayingSection][countBeatNum].startTime - fadeTime);

        audioSource.time = beats[currentPlayingSection][countBeatNum].startTime;
        audioSource.Play();

        countBeatNum++;

        return true;
    }

    void StopSound()
    {
        StartCoroutine(VolumeFade(audioSource, 0, fadeTime));
    }

    public void ResetPlayMode()
    {
        countBeatNum = 0;
        StopSound();
    }


    void HitReaction()
    {
        meshTransfrom.localScale *= 0.95f;
        drumSideRenderer.material.SetColor("_EmissionColor", drumSideColorOnHit);
        Invoke("EndHitReaction", 0.1f);
    }

    void EndHitReaction()
    {
        meshTransfrom.localScale = meshOriginScale;
        drumSideRenderer.material.SetColor("_EmissionColor", originDrumSideColor);
    }

    public void SubmitReaction()
    {
        foreach(var particle in particleSystems)
        {
            particle.Play();
        }

        drumCenterRenderer.material.SetColor("_EmissionColor", drumCenterColorOnHit);
        Invoke("EndSubmitReaction", 0.1f);
    }

    void EndSubmitReaction()
    {
        drumCenterRenderer.material.SetColor("_EmissionColor", originDrumCenterColor);
    }


    public void MakeSections(int num)
    {
        for(int i = 0; i < num; i++)
        {
            List<Beat> temp = new List<Beat>();
            beats.Add(temp);
        }
    }

    public void AddBeat(int section , Beat _beat)
    {
        beats[section].Add(_beat);
    }


    public void ResetBeats()
    {
        beats.Clear();
        countBeatNum = 0;
        audioSource.Stop();
    }

    public void SetAudioClip(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
    }


    public void StartSettingRepeatSound()
    {
        if(isSetting == false)
        {
            isSetting = true;
            drumCenterRenderer.material.SetColor("_EmissionColor", Color.red);
            setStartTime = Time.time;
        }
        else
        {
            repeatInterval = Time.time - setStartTime;
            if (repeatInterval < 0.03f) // 버그로 두번 타닥 쳐졌을때 시끄럽게 되버리는 것을 방지
                return;

            isSetting = false;
            isRepeatOn = true;
            drumCenterRenderer.material.SetColor("_EmissionColor", originDrumCenterColor);
            countRepeatTime = 0;
        }
        
    }

    void RepeatSound()
    {
        if (!isRepeatOn)
            return;

        countRepeatTime += Time.deltaTime;

        if(countRepeatTime > repeatInterval)
        {
            countRepeatTime -= repeatInterval;
            PlaySound();
        }
    }

    void ResetRepeatSoundAfterWaiting()
    {
        if (!isSetting)
            return;

        float collapsedTime = Time.time - setStartTime;
        if(collapsedTime > 10)
        {
            drumCenterRenderer.material.SetColor("_EmissionColor", originDrumCenterColor);
            isSetting = false;
            setStartTime = 0;
        }
    }


    IEnumerator VolumeFade(AudioSource _AudioSource, float _EndVolume, float _FadeLength)
    {

        float _StartVolume = _AudioSource.volume;

        float _StartTime = Time.time;

        while (Time.time < _StartTime + _FadeLength)
        {

            _AudioSource.volume = _StartVolume + ((_EndVolume - _StartVolume) * ((Time.time - _StartTime) / _FadeLength));

            yield return null;

        }

        if (_EndVolume == 0)
        {
            _AudioSource.Stop();
        }
    }


    void PressDown()
    {
        Vector3 pos = transform.localPosition;
        pos.y -= 0.5f;

        transform.localPosition = pos;
    }

   
    public void SetIconVisible(bool on)
    {
        if(on)
        { 
            iconRenderer.gameObject.SetActive(true);
        }
        else
        {
            iconRenderer.gameObject.SetActive(false);
        }
    }

    public void SetIcon(Material material)
    {
        iconRenderer.material = material;
    }
}
