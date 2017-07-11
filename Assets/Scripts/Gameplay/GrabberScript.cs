using UnityEngine;
using System.Collections;

public class GrabberScript : MonoBehaviour
{
    CharacterControllerScript cs;
    ParticleSystem pSystem;

    void Awake()
    {
        cs = GetComponentInParent<CharacterControllerScript>();
        pSystem = GetComponentInChildren<ParticleSystem>();
        pSystem.GetComponent<Renderer>().sortingLayerName = "Foreground";
    }

    void Start()
    {
        pSystem.startColor = cs.color;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        BallScript bs = collider.gameObject.GetComponent<BallScript>();

        if(bs)
        {
            cs.ball = bs;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        BallScript bs = collider.gameObject.GetComponent<BallScript>();

        if(bs && cs.ball && cs.ball.whoHolds != cs.me)
        {
            cs.ball = null;
        }
    }

    public void SetAngle(float angle)
    {
        Quaternion newRotation = gameObject.transform.rotation;
        Vector3 eulerRotation = newRotation.eulerAngles;

        if(Mathf.Abs(angle) > 90)
        {
            //Account for y rotation of character
            angle = 180 - angle;
        }

        eulerRotation.z = angle;

        newRotation = Quaternion.Euler(eulerRotation);
        gameObject.transform.rotation = newRotation;
    }

    public void PlayParticles()
    {
        pSystem.Emit(500);
    }
}
