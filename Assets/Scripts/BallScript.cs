using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BallScript : MonoBehaviour
{
    public float maxSpeed = 40.0f;
    private Vector2 startPos;

    [HideInInspector]
    public Players whoHolds = Players.Invalid;
    [HideInInspector]
    public int numBounces = 0;
    [HideInInspector]
    public float chargeFactor = 1;
    [HideInInspector]
    public int physicsIgnoreLayer = 31;
    public float throwStrength = 10.0f;

    private AudioSource ballSFX;
    public List<AudioClip> clipList;

    public Image ballTimerBG;
    public Text ballTimerText;

    private const float START_BALL_TIME = 5;
    private const float RESET_BALL_TIME = 1;
    private float ballTime = START_BALL_TIME;
    private float currentBallTime = START_BALL_TIME;
    private float ballTimeDelta = 1;

    private int orbitParticleCoefficient = 50;
    private int trailParticleCoefficient = 15;

    private ParticleSystem[] pSystems;

    void Awake()
    {
        pSystems = GetComponentsInChildren<ParticleSystem>();

        foreach(ParticleSystem system in pSystems)
        {
            system.renderer.sortingLayerName = "Foreground";
        }

        ballSFX = GetComponent<AudioSource>();
    }

    void Start()
    {
        startPos = transform.position;
        Reset();
    }

    void FixedUpdate()
    {
        if(whoHolds == Players.Invalid)
            rigidbody2D.velocity = Vector2.ClampMagnitude(rigidbody2D.velocity, maxSpeed);

    }

    void Update()
    {
        int trailParticleNum = (int)rigidbody2D.velocity.magnitude * trailParticleCoefficient;
        int orbitParticleNum = (int)rigidbody2D.velocity.magnitude * orbitParticleCoefficient;
        pSystems[0].emissionRate = trailParticleNum;
        pSystems[1].emissionRate = orbitParticleNum;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "wall")
        {
            numBounces++;
            ballSFX.clip = clipList[0];
            ballSFX.Play();
            GameManager.instance.UpdateBallText(numBounces.ToString());

            float oldSpeed = pSystems[0].startSpeed;
            pSystems[0].startSpeed = 2;
            int particleAmount = 30 * (int)rigidbody2D.velocity.magnitude;
            pSystems[0].Emit(particleAmount); //TODO? Make less janky
            pSystems[0].startSpeed = oldSpeed;
            pSystems[0].Play();
        }
    }

    public void Reset()
    {
        transform.parent = null;
        transform.position = startPos;
        rigidbody2D.velocity = Vector2.zero;
        whoHolds = Players.Invalid;
        chargeFactor = 1;
        numBounces = 0;

        GameManager.instance.UpdateBallText(numBounces.ToString());
        EnableAllPhysics();

        rigidbody2D.gravityScale = 0.0f;
        collider2D.enabled = false;
        CountdownBallTimer();
    }

    public void IgnorePhysics(int layer, bool yOrN = true, bool resetOld = true)
    {
        if(resetOld)
        {
            Physics2D.IgnoreLayerCollision(12, physicsIgnoreLayer, false);
        }

        physicsIgnoreLayer = layer;
        int layerToPlayer = layer - 8;
        renderer.material.color = GameManager.instance.charPanels[layerToPlayer].color;
        SetParticleColor(renderer.material.color);

        Physics2D.IgnoreLayerCollision(12, layer, yOrN);
    }

    public void EnableAllPhysics()
    {
        for(int i = 8; i < 12; ++i)
        {
            IgnorePhysics(i, false);
        }

        renderer.material.color = Color.white;
    }

    public void SetParticleColor(Color color)
    {
        Color newColor = pSystems[0].startColor;
        newColor.r = color.r;
        newColor.g = color.g;
        newColor.b = color.b;
        foreach(ParticleSystem system in pSystems)
        {
            system.startColor = newColor;
        }
    }

    private void CountdownBallTimer()
    {
        if(currentBallTime > 0)
        {
            ballTimerText.text = currentBallTime.ToString();
            Color32 color = ballTimerBG.color;
            color.a = (byte)(25 * currentBallTime);
            ballTimerBG.color = color;

            if(currentBallTime != ballTime)
            {
                ballSFX.clip = clipList[1];
                ballSFX.Play();
            }
            currentBallTime -= ballTimeDelta;

            Invoke("CountdownBallTimer", ballTimeDelta);
        }
        else
        {
            ballTimerText.text = "";
            Color32 color = ballTimerBG.color;
            color.a = (byte)(25 * currentBallTime);
            ballTimerBG.color = color;
            rigidbody2D.gravityScale = 0.2f;
            collider2D.enabled = true;
            ballTime = RESET_BALL_TIME;
            currentBallTime = ballTime;

            ballSFX.clip = clipList[2];
            ballSFX.Play();
        }
    }
}