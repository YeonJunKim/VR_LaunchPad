using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour
{
    public GameObject stickPick;
    Vector3 previousPos;

    private SteamVR_TrackedObject trackedObj;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }


    private void Awake()
    {
        trackedObj = GetComponentInParent<SteamVR_TrackedObject>();
    }

    private void Update()
    {
        previousPos = stickPick.transform.position;
    }


    public bool IsPressingTouchPad()
    {
        if (Controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
            return true;

        return false;
    }

    public bool IsPressingTrigger()
    {
        if (Controller.GetHairTrigger())
            return true;

        return false;
    }


    public bool IsGoingUp()
    {
        if (previousPos.y - stickPick.transform.position.y < 0)
            return true;

        return false;
    }

    public bool IsMovingFastEnough()
    {
        return (Mathf.Abs(previousPos.y - stickPick.transform.position.y) > 0.01f);
        
    }

    public void Vibrate()
    {
        StartCoroutine(LongVibration(0.03f, 65000));
    }

    
    IEnumerator LongVibration(float length, ushort strength)
    {
        for(float i = 0; i<length; i += Time.deltaTime)
        {
            Controller.TriggerHapticPulse(strength);

           yield return null; //every single frame for the duration of "length" you will vibrate at "strength" amount
        }
    }
}
