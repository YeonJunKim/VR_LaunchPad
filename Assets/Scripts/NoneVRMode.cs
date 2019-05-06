using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoneVRMode : MonoBehaviour {

    public BeatCube[] beatCubes;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            beatCubes[0].TriggerEnter();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            beatCubes[1].TriggerEnter();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            beatCubes[2].TriggerEnter();
        }


        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameManager.instance.OnSectionButtonClick(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameManager.instance.OnSectionButtonClick(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameManager.instance.OnSectionButtonClick(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GameManager.instance.OnSectionButtonClick(3);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            GameManager.instance.ResetDoings();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            GameManager.instance.SaveEditInfos();
        }

        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameManager.instance.GetGameState() == GameState.New_EditMusic_Edit)
            {
                GameManager.instance.SaveEditInfos();
            }

            GameManager.instance.ChangeToPreviousState();
        }
    }
}
