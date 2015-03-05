using UnityEngine;
using System.Collections;

public class GrabberScript : MonoBehaviour
{
    CharacterControllerScript cs;

    void Awake()
    {
        cs = GetComponentInParent<CharacterControllerScript>();
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


}
