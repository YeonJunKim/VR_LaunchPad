using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionButton : MonoBehaviour {

    int section;
    TextMesh textMesh;

    Renderer m_renderer;
    Color originColor;


    private void Awake()
    {
        m_renderer = transform.GetComponent<Renderer>();
        originColor = m_renderer.material.GetColor("_EmissionColor");
        textMesh = transform.GetComponentInChildren<TextMesh>();

        SetLight(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Stick")
        {
            if (other.GetComponent<Stick>().IsPressingTrigger())
                GameManager.instance.OnSectionButtonClick(section);
        }
    }


    public void SetSection(int _section)
    {
        section = _section;
        textMesh.text = (section + 1).ToString();   // in UI, section starts with 1, 2, 3... not 0, 1, 2...
    }

    public int GetSection()
    {
        return section;
    }


    public void SetLight(bool lightOn)
    {
        if (lightOn)
        {
            m_renderer.material.SetColor("_EmissionColor", originColor);
        }
        else
        {
            m_renderer.material.SetColor("_EmissionColor", Color.black);
        }
    }
}
