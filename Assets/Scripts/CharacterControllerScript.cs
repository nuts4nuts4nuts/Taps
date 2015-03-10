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

    [HideInInspector]
    public int health = 100;
    public Players me = Players.Invalid;
    [HideInInspector]
    public int physLayer = 0;

    //Audio
    private AudioSource characterSFX;
    public List<AudioClip> clipList;
    private DamageHelper damageHelper;

    [HideInInspector]
    public InputDevice controller;
    private InputControl controlScheme;
    public Color color;

    //Team stuff
    int[] friends;

    ShakyCam cam;

    void Start()
    {
        anim = (Animator)GetComponentInChildren(typeof(Animator));
        gameObject.layer = physLayer;
        characterSFX = GetComponent<AudioSource>();
        grabber = GetComponentInChildren<GrabberScript>();
        damageHelper = GameObject.Find("DamageEffect").GetComponent<DamageHelper>();
        controlScheme = controller.Action1;

        cam = Camera.main.GetComponent<ShakyCam>();

        wallJumpVelocity = new Vector2(maxSpeed, 15);
    }

    public void SetMe(Players i)
    {
        me = i;
        physLayer = (int)me + 8;
    }

    public void SetFriends(int[] newFriends)
    {
        friends = newFriends;
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
                    //Release
                    ball.whoHolds = Players.Invalid;

                    float x = controller.LeftStickX;
                    float y = controller.LeftStickY;
                    ball.transform.parent = null;
                    ball.rigidbody2D.velocity = new Vector2(x * throwStrength * ball.chargeFactor, y * throwStrength * ball.chargeFactor);
                    ball.collider2D.enabled = true;
                    ball.chargeFactor = 1;
                    ball.UpdateSpeed();
                    characterSFX.clip = clipList[1];
                    characterSFX.Play();
                }
                else if (controlScheme.IsPressed)
                {
                    //Charge
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
                //Catch
                ball.whoHolds = me;

                ball.transform.parent = transform;
                ball.rigidbody2D.velocity = Vector2.zero;
                ball.collider2D.enabled = false;

                ball.IgnorePhysics(friends[0]);
                ball.SetParticleColor(color);

                if(friends.Length > 1)
                {
                    for (int i = 1; i < friends.Length; ++i)
                    {
                        ball.IgnorePhysics(friends[i], true, false);
                    }
                }

                characterSFX.clip = clipList[0];
                characterSFX.Play();

                cam.Sleep(0.1f, 0.1f);
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
        ball.ExplodeParticles();
        ball.Reset();
        ball = null;

        Vector2 particlePos = transform.position;
        particlePos.y -= 0.5f;

        if (health <= 0)
        {
            damageHelper.PlaySound();
            damageHelper.BurstParticles((int)DamageHelper.ParticleAmount.death, color, particlePos);
            cam.Shake(0.001f * 100, 0.00001f * 100);
            cam.Sleep(0.1f, 0.5f);
            Destroy(gameObject);
        }
        else
        {
            damageHelper.BurstParticles(damage * 5, color, particlePos);
            cam.Shake(0.001f * damage, 0.00003f * damage);
            cam.Sleep(0.01f, 0.002f * damage);
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

    private void ResetCooldown()
    {
        onCooldown = false;
    }

    private void ResetCanWalk()
    {
        canWalk = true;
    }
}