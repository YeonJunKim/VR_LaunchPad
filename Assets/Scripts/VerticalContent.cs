using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VerticalContent : MonoBehaviour {

    public TextMesh textMesh;
    public Renderer outLine;

    Color selectedColor;
    Color defaultColor = Color.gray;

    public int contentID;
    public UIMenu menu;

    private void Awake()
    {
        selectedColor = outLine.material.GetColor("_EmissionColor");
        outLine.material.SetColor("_EmissionColor", defaultColor);
    }


    public void SetHighLight(bool on)
    {
        if(on)
        {
            outLine.material.SetColor("_EmissionColor", selectedColor);
        }
        else
        {
            outLine.material.SetColor("_EmissionColor", defaultColor);
        }
    }


    public void SetText(string text)
    {
        textMesh.text = text;
    }
}
