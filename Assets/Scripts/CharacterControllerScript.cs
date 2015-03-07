using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class CharacterControllerScript : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float jumpForce = 1000f;
    private bool facingRight = true;

    public Transform groundCheck;
    private float groundRadius = 0.2f;
    public LayerMask whatIsGround;
    private bool grounded = false;

    public Transform wallCheck;
    private float wallRadius = 0.1f;
    public LayerMask whatIsWall;
    private bool canWallJump = false;
    private Vector2 wallJumpVelocity;
    private bool canWalk = true;

    private Animator anim;
    private GrabberScript grabber;
    public float grabCooldown = 0.1f;
    private bool onCooldown = false;

    [HideInInspector]
    public BallScript ball;
    public float throwStrength = 10.0f;

    private int health = 100;
    public Players me = Players.Invalid;
    private int physLayer = 0;

    //Audio
    private AudioSource characterSFX;
    public List<AudioClip> clipList;
    private DamageHelper damageHelper;

    [HideInInspector]
    public InputDevice controller;
    private InputControl controlScheme;
    public Color color;

    void Start()
    {
        anim = (Animator)GetComponentInChildren(typeof(Animator));
        physLayer = (int)me + 8;
        gameObject.layer = physLayer;
        characterSFX = GetComponent<AudioSource>();
        grabber = GetComponentInChildren<GrabberScript>();
        damageHelper = GameObject.Find("DamageEffect").GetComponent<DamageHelper>();
        controlScheme = controller.Action1;

        wallJumpVelocity = new Vector2(maxSpeed, 15);
    }

	void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        canWallJump = Physics2D.OverlapCircle(wallCheck.position, wallRadius, whatIsWall);
        anim.SetBool("Ground", grounded);
        anim.SetFloat("vSpeed", rigidbody2D.velocity.y);

        float movement = controller.LeftStickX;

        anim.SetFloat("Speed", Mathf.Abs(movement));

        if(canWalk)
        {
            rigidbody2D.velocity = new Vector2(movement * maxSpeed, rigidbody2D.velocity.y);
        }

        if(rigidbody2D.velocity.x > 0 && !facingRight)
        {
            Flip();
        }
        else if(rigidbody2D.velocity.x < 0 && facingRight)
        {
            Flip();
        }
	}

	void Update()
    {
        if (ball)
        {
            ball.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
            if (ball.whoHolds == me)
            {
                Vector2 ballPos = transform.position;
                ballPos.y += 0.75f;
                ball.rigidbody2D.transform.position = ballPos;

                if (controlScheme.WasReleased)
                {
                    ball.whoHolds = Players.Invalid;

                    float x = controller.LeftStickX;
                    float y = controller.LeftStickY;
                    ball.transform.parent = null;
                    ball.rigidbody2D.velocity = new Vector2(x * throwStrength * ball.chargeFactor, y * throwStrength * ball.chargeFactor);
                    ball.collider2D.enabled = true;
                    ball.chargeFactor = 1;
                    characterSFX.clip = clipList[1];
                    characterSFX.Play();
                }
                else if (controlScheme.IsPressed)
                {
                    ball.chargeFactor += Time.deltaTime;
                }

                if (grounded)
                {
                    TakeDamage(ball.numBounces);
                }
            }
        }

        if (grounded && controller.Action1.WasPressed)
        {
            anim.SetBool("Ground", false);
            rigidbody2D.AddForce(new Vector2(0, jumpForce));
        }
        else if (canWallJump && controller.Action1.WasPressed)
        {
            anim.SetBool("Ground", false);
            Vector3 jumpVelocity = wallJumpVelocity;
            canWalk = false;

            if(facingRight)
            {
                jumpVelocity.x *= -1;
            }

            rigidbody2D.velocity = jumpVelocity;

            Invoke("ResetCanWalk", 0.12f);
        }
        else if(controlScheme.WasPressed && !onCooldown)
        {
            grabber.PlayParticles();

            if (ball && ball.whoHolds == Players.Invalid)
            {
                ball.whoHolds = me;

                ball.transform.parent = transform;
                ball.rigidbody2D.velocity = Vector2.zero;
                ball.collider2D.enabled = false;
                ball.IgnorePhysics(physLayer);
                characterSFX.clip = clipList[0];
                characterSFX.Play();

                Time.timeScale = 0.1f;
                Invoke("ResetTimeScale", 0.01f);
            }

            onCooldown = true;
            Invoke("ResetCooldown", grabCooldown);
        }
    }

    void LateUpdate()
    {
        float x = controller.LeftStickX;
        float y = controller.LeftStickY;
        grabber.SetAngle(Mathf.Rad2Deg * Mathf.Atan2(y, x));
    }

    void UpdateHealth(int newHealth)
    {
        health = newHealth;
        GameManager.instance.UpdateCharText(this, health.ToString());
    }

    void TakeDamage(int damage)
    {
        characterSFX.clip = clipList[2];
        characterSFX.Play();
        UpdateHealth(health - damage);
        ball.Reset();
        ball = null;
        if (health <= 0)
        {
            damageHelper.PlaySound();
            damageHelper.BurstParticles((int)DamageHelper.ParticleAmount.death, color, transform.position);
            Destroy(gameObject);
        }
        else
        {
            damageHelper.BurstParticles(damage * 5, color, transform.position);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        if(eulerRotation.y == 180)
        {
            eulerRotation.y = 0;
        }
        else
        {
            eulerRotation.y = 180;
        }
        transform.Rotate(eulerRotation);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        BallScript bs = collision.gameObject.GetComponent<BallScript>();

        if(bs && ball)
        {
            TakeDamage(ball.numBounces);
        }
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
    }

    private void ResetCooldown()
    {
        onCooldown = false;
    }

    private void ResetCanWalk()
    {
        canWalk = true;
    }
}