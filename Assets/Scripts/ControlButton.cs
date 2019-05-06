using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ButtonType
{
    Save,
    Refresh,
    Back,
}


public class ControlButton : MonoBehaviour {

    public Transform meshTransfrom;
    public Renderer drumCenterRenderer;
    public Renderer drumSideRenderer;

    public ButtonType buttonType;

    Vector3 meshOriginScale;
    Color originDrumCenterColor;
    Color originDrumSideColor;
    Color drumCenterColorOnHit = Color.white;
    Color drumSideColorOnHit = Color.red;

    SpringJoint springJoint;
    Vector3 originPos;

    Renderer m_renderer;
    Color originColor;


    private void Awake()
    {
        springJoint = transform.GetComponent<SpringJoint>();
        originPos = transform.position;
    }

    private void Start()
    {
        meshOriginScale = meshTransfrom.localScale;
        originDrumCenterColor = drumCenterRenderer.material.GetColor("_EmissionColor");
        originDrumSideColor = drumSideRenderer.material.GetColor("_EmissionColor");
    }


    private void Update()
    {
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
            Stick stick = col.gameObject.gameObject.GetComponent<Stick>();
            if (stick.IsGoingUp() || !(stick.IsMovingFastEnough())) // only trigger when the stick is going downwords, and moving fastEnough
                return;

            HitReaction();
            stick.Vibrate();
            GameManager.instance.OnButtonClick(buttonType);
        }

    }


    void HitReaction()
    {
        meshTransfrom.localScale *= 0.95f;
        drumSideRenderer.material.SetColor("_EmissionColor", drumSideColorOnHit);
        drumCenterRenderer.material.SetColor("_EmissionColor", drumCenterColorOnHit);
        Invoke("EndHitReaction", 0.1f);
    }

    void EndHitReaction()
    {
        meshTransfrom.localScale = meshOriginScale;
        drumSideRenderer.material.SetColor("_EmissionColor", originDrumSideColor);
        drumCenterRenderer.material.SetColor("_EmissionColor", originDrumCenterColor);
    }
   

}
