using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Properties")]
    public float speed = 14f;
    public float accel = 6f;
    public float airAccel = 3f;
    public float jumpForce = 14f;
    public float rayLength = 0.25f;

    [Header("References")]
    public Animator animator;
    public Rigidbody2D rb2D;
    public Collider2D collider;
    public LayerMask levelMask;
    public SpriteRenderer spriteRenderer;

    Vector2 bounds;
    Vector2 input;


    void Awake()
    {
        bounds = new Vector2(collider.bounds.extents.x + 0.1f, collider.bounds.extents.y + 0.2f);
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            input.y = 1;
    }

    void FixedUpdate()
    {
        Vector2 force = new Vector2(0.0f, 0.0f);
        Vector2 velocity = new Vector2(0.0f, 0.0f);

        if (input.x == 0 && isGround())
            velocity.x = 0f;
        else
            velocity.x = rb2D.velocity.x;

        if (input.y == 1 && (isGround() || isWall()))
            velocity.y = jumpForce * (isWall() ? 0.8f : 1f);
        else
            velocity.y = rb2D.velocity.y;

        if (input.x < 0) spriteRenderer.flipX = true; 
        else  if (input.x > 0) spriteRenderer.flipX = false;

        float acceleration = (isGround() ? accel : airAccel);
        force.x = ((input.x * speed) - rb2D.velocity.x) * acceleration;

        if (isWall() && !isGround() && input.y == 1)
            velocity.x = -WallDirection() * speed * 0.75f;

        rb2D.AddForce(force);
        rb2D.velocity = velocity;

        input.y = 0;

        animator.SetFloat("vel_x", Mathf.Abs(input.x));
        animator.SetFloat("vel_y", rb2D.velocity.normalized.y);
        animator.SetBool("ground", isGround());
        animator.SetBool("wall", isWall());
    }

    //Retorna se o jogador ta ou nao encostando na parede
    public bool isWall()
    {
        Vector2 point = (Vector2)transform.position + collider.offset;

        bool left = Physics2D.Raycast(new Vector2(point.x - bounds.x, point.y), -Vector2.right, rayLength, levelMask);
        bool right = Physics2D.Raycast(new Vector2(point.x + bounds.x, point.y), Vector2.right, rayLength, levelMask);

        if (left || right)
            return true;
        else
            return false;
    }

    //Retorna se o jogador ta ou nao encostando no chão
    public bool isGround()
    {
        Vector2 point = (Vector2)transform.position + collider.offset;

        bool bottom1 = Physics2D.Raycast(new Vector2(point.x, point.y - bounds.y), -Vector2.up, rayLength, levelMask);

        if (bottom1)
            return true;
        else
            return false;
    }


    //Retorna a direção da parede sendo encostada
    public int WallDirection()
    {
        Vector2 point = (Vector2)transform.position + collider.offset;

        bool left = Physics2D.Raycast(new Vector2(point.x - bounds.x, point.y), -Vector2.right, rayLength, levelMask);
        bool right = Physics2D.Raycast(new Vector2(point.x + bounds.x, point.y), Vector2.right, rayLength, levelMask);

        if (left)
            return -1;
        else if (right)
            return 1;
        else
            return 0;
    }
}
