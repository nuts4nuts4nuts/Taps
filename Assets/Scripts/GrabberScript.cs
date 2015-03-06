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
        pSystem.renderer.sortingLayerName = "Foreground";
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

        if(bs)
        {
            cs.ball = null;
        }
    }

    public void SetAngle(float angle)
    {
        print(angle);
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
