using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float jumpDuration = 0.5f;

    private SpriteRenderer spriteRenderer;
    private bool isJumping = false;
    private float jumpTimer = 0f;
    private Vector3 startPosition;
    private bool isMovingRight = false;

    [Header("Animation Controller")]
    public RuntimeAnimatorController idleController;
    public RuntimeAnimatorController jumpController;
    public RuntimeAnimatorController runController;

    private Animator animator;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = idleController;
    }

    void Update()
    {
        Vector2 moveDirection = Vector2.zero;

        // if (Input.GetKey(KeyCode.LeftArrow))
        // {
        //     moveDirection.x -= 1f;
        //     isMovingRight = false; // 왼쪽 이동
        // }
        // if (Input.GetKey(KeyCode.RightArrow))
        // {
        //     moveDirection.x += 1f;
        //     isMovingRight = true; // 오른쪽 이동
        // }

        // // 이동 방향이 바뀌면 스프라이트 뒤집기
        // if (moveDirection.x > 0f)
        // {
        //     spriteRenderer.flipX = false; // 오른쪽 바라봄
        // }
        // else if (moveDirection.x < 0f)
        // {
        //     spriteRenderer.flipX = true; // 왼쪽 바라봄
        // }

        if (!isJumping)
        {
            if (moveDirection.x != 0f)
            {
                animator.runtimeAnimatorController = runController;
            }
            else
            {
                animator.runtimeAnimatorController = idleController;
            }
        }   

        moveDirection = moveDirection.normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            StartJump();
        }

        if (isJumping)
        {
            UpdateJump();
        }
    }

    void StartJump()
    {
        isJumping = true;
        jumpTimer = 0f;
        startPosition = transform.position;

        animator.runtimeAnimatorController = jumpController;
    }

    void UpdateJump()
    {
        jumpTimer += Time.deltaTime;
        float progress = jumpTimer / jumpDuration;

        if (progress >= 1f)
        {
            transform.position = new Vector3(transform.position.x, startPosition.y, transform.position.z);
            isJumping = false;
            animator.runtimeAnimatorController = idleController;
        }
        else
        {
            float height = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
            transform.position = new Vector3(transform.position.x, startPosition.y + height, transform.position.z);            
        }
    }
}


