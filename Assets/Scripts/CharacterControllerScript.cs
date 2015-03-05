using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class CharacterControllerScript : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float jumpForce = 1000f;
    private bool facingRight = true;

    private bool grounded = false;
    public Transform groundCheck;
    private float groundRadius = 0.2f;
    public LayerMask whatIsGround;

    private Animator anim;

    [HideInInspector]
    public BallScript ball;
    public float throwStrength = 10.0f;

    private int health = 100;
    public Players me = Players.Invalid;
    private int physLayer = 0;

    //Audio
    private AudioSource characterSFX;
    public List<AudioClip> clipList;

    [HideInInspector]
    public InputDevice controller;

    void Start()
    {
        anim = (Animator)GetComponentInChildren(typeof(Animator));
        physLayer = (int)me + 8;
        gameObject.layer = physLayer;
        characterSFX = GetComponent<AudioSource>();
    }

	void Update()
    {
        if (grounded && controller.Action1.WasPressed)
        {
            anim.SetBool("Ground", false);
            rigidbody2D.AddForce(new Vector2(0, jumpForce));
        }

        if(ball)
        {
            ball.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
            if(ball.whoHolds == me)
            {
                Vector2 ballPos = transform.position;
                ballPos.y += 0.75f;
                ball.rigidbody2D.transform.position = ballPos;

                if(controller.Action3.WasReleased)
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
                else if(controller.Action3.IsPressed)
                {
                    ball.chargeFactor += Time.deltaTime;
                }

                if(grounded)
                {
                    TakeDamage(ball.numBounces);
                }
            }
            else if(controller.Action3.WasPressed && ball.whoHolds == Players.Invalid)
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
        }
    }

	void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        anim.SetBool("Ground", grounded);
        anim.SetFloat("vSpeed", rigidbody2D.velocity.y);

        float movement = controller.LeftStickX;

        anim.SetFloat("Speed", Mathf.Abs(movement));

        rigidbody2D.velocity = new Vector2(movement * maxSpeed, rigidbody2D.velocity.y);

        if(movement > 0 && !facingRight)
        {
            Flip();
        }
        else if(movement < 0 && facingRight)
        {
            Flip();
        }
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
            Destroy(gameObject);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 playerScale = transform.localScale;
        playerScale.x *= -1;
        transform.localScale = playerScale;
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
}
