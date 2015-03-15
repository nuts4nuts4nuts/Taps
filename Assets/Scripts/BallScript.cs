using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BallScript : MonoBehaviour
{
    public float startSpeed = 9f;
    public float maxSpeed = 40.0f;
    private float currentSpeed = 7.5f;
    public float speedCoefficient = 1.2f;
    private Vector2 startPos;

    [HideInInspector]
    public Players whoHolds = Players.Invalid;
    [HideInInspector]
    public int numBounces = 0;
    [HideInInspector]
    public float chargeFactor = 1;
    [HideInInspector]
    public int physicsIgnoreLayer = 31;

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
    public Color trailParticleColor;
    public Color orbitParticleColor;
    public AnimationCurve colorCurve;

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
            rigidbody2D.velocity = Vector2.ClampMagnitude(rigidbody2D.velocity, currentSpeed);

    }

    void Update()
    {
        int trailParticleNum = (int)rigidbody2D.velocity.magnitude * trailParticleCoefficient;
        int orbitParticleNum = (int)rigidbody2D.velocity.magnitude * orbitParticleCoefficient;
        pSystems[0].emissionRate = trailParticleNum;
        pSystems[1].emissionRate = orbitParticleNum;

        //Color
        ParticleSystem.Particle[] pList = new ParticleSystem.Particle[pSystems[0].particleCount];
        pSystems[0].GetParticles(pList);
        for (int i = 0; i < pList.Length; ++i)
        {
            float lifePercentage = (pList[i].lifetime / pList[i].startLifetime);
            float curvedPercentage = colorCurve.Evaluate(lifePercentage);
            Color lerpedColor = Color.Lerp(trailParticleColor, orbitParticleColor, curvedPercentage);
            pList[i].color = lerpedColor * 2; //CRANK IT
        }

        pSystems[0].SetParticles(pList, pSystems[0].particleCount);

        ParticleSystem.Particle[] pListOrbit = new ParticleSystem.Particle[pSystems[1].particleCount];
        pSystems[1].GetParticles(pListOrbit);
        for (int i = 0; i < pListOrbit.Length; ++i)
        {
            float lifePercentage = (pListOrbit[i].lifetime / pListOrbit[i].startLifetime);
            float curvedPercentage = colorCurve.Evaluate(lifePercentage);
            Color lerpedColor = Color.Lerp(trailParticleColor, orbitParticleColor, curvedPercentage);
            pListOrbit[i].color = lerpedColor * 4; //CRANK IT
        }

        pSystems[1].SetParticles(pListOrbit, pSystems[1].particleCount);
    }

    public void ExplodeParticles()
    {
        float oldSpeed = pSystems[0].startSpeed;
        float oldLifetime = pSystems[0].startLifetime;
        int particleAmount = 10 * numBounces;
        float lifetimeBonus = particleAmount * 0.001f;

        pSystems[0].startSpeed = 2;
        pSystems[0].startLifetime = oldLifetime + lifetimeBonus;
        pSystems[0].Emit(particleAmount); //TODO? Make less janky

        pSystems[0].startSpeed = oldSpeed;
        pSystems[0].startLifetime = oldLifetime;
        pSystems[0].Play();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "wall")
        {
            numBounces++;
            ballSFX.clip = clipList[0];
            ballSFX.Play();
            GameManager.instance.UpdateBallText(numBounces.ToString());

            if(numBounces % 5 == 0)
            {
                UpdateSpeed();
            }

            ExplodeParticles();
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

        //Hard reset ball references
        foreach(CharacterControllerScript c in GameManager.instance.GetLivingChars())
        {
            c.ball = null;
        }

        rigidbody2D.gravityScale = 0.0f;
        collider2D.enabled = false;
        currentSpeed = startSpeed;
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
        trailParticleColor = pSystems[0].startColor;
        trailParticleColor.r = color.r;
        trailParticleColor.g = color.g;
        trailParticleColor.b = color.b;
        pSystems[0].startColor = trailParticleColor;

        orbitParticleColor = pSystems[1].startColor;
        orbitParticleColor.r = 1 - color.r;
        orbitParticleColor.g = 1 - color.g;
        orbitParticleColor.b = 1 - color.b;
        pSystems[1].startColor = orbitParticleColor;
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
            SetParticleColor(renderer.material.color);

            ballSFX.clip = clipList[2];
            ballSFX.Play();
        }
    }

    public void UpdateSpeed()
    {
        currentSpeed *= speedCoefficient;

        if(currentSpeed >= maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
    }
}