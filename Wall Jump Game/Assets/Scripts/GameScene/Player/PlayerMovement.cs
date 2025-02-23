using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform Line;
    [SerializeField] public Rigidbody2D rb;


    [Header("Camera")]
    public float cameraSpeed = 0.2f;
    private Camera cam;
    private Vector3 cameraOffset = new Vector3(0, 0, -10);


    [Header("Movement")]
    public float wallTime;
    [SerializeField] private float SlowTimePlayerShrinkSpeed = 5f;

    public int currentJumpCount;
    private int currentSlowUsage;
    private bool inSlowMode;
    private Vector2 touchDirection;
    private Vector2 touchStartPos;
    private float yPos;

    private PlayerData playerData;
    private PlayerSprite playerSprite;
    private Coroutine slowModeCoroutine;
    public void Init()
    {
        Line.gameObject.SetActive(false);
        yPos = transform.position.y;
        cam = Camera.main;

        playerData = PlayerManager.instance.playerData;
        playerSprite = PlayerManager.instance.playerSprite;

        currentJumpCount = playerData.MaxJumpCount;
        currentSlowUsage = playerData.MaxSlowUsage;

        PlayerEventHandler.OnPlayerJump += Jump;
        PlayerEventHandler.OnTouchWall += OnEnterWall;
        PlayerEventHandler.OnLeaveWall += OnExitWall;
    }

    private void OnDestroy()
    {
        PlayerEventHandler.OnPlayerJump -= Jump;
        PlayerEventHandler.OnTouchWall -= OnEnterWall;
        PlayerEventHandler.OnLeaveWall -= OnExitWall;
    }


    private void LateUpdate()
    {
        //camera movement
        //transform = player
        Vector3 target = new Vector3(cam.transform.position.x, transform.position.y, transform.position.z) + cameraOffset;
        Vector3 smoothPos = Vector3.Lerp(cam.transform.position, target, cameraSpeed);

        if (transform.position.y > 0)
            cam.transform.position = smoothPos;
    }
    private void Update()
    {
        //Point Adjustment
        if (transform.position.y > yPos + 1)
        {
            //Point = Point + (1 * doublePointExponent);
            //yPos = transform.position.y;
        }


        //Touch movement Drag and jump
        if (Input.touchCount > 0 &&  currentJumpCount > 0 && !PlayerManager.instance.isDead)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                Line.gameObject.SetActive(true);
                if (!PlayerManager.instance.OnWall)
                { 
                    if (!inSlowMode && currentSlowUsage > 0)
                        slowModeCoroutine = StartCoroutine(slowMode());

                        playerSprite.transform.localScale = Vector2.Lerp(playerSprite.transform.localScale, new Vector2(1,0.5f), SlowTimePlayerShrinkSpeed * Time.deltaTime);
                }
            }
            if (touch.phase == TouchPhase.Moved)
            {
                touchDirection = touchStartPos - touch.position;

                float angle = Mathf.Atan2(touchDirection.y, touchDirection.x) * Mathf.Rad2Deg;
                if (!PlayerManager.instance.OnInvisWall) //If not on invis wall. Added this because rotating triangles cause disconnection between wall
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

                Line.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); //Rotation

                Line.transform.localScale = new Vector3(1f, touchDirection.magnitude / 70, 1);
                if (Line.transform.localScale.y > 4f)
                    Line.transform.localScale = new Vector3(1f, 4f, 1);
                if (Line.transform.localScale.y < 1f)
                    Line.transform.localScale = new Vector3(1f, 1, 1);

            }
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                PlayerEventHandler.PlayerJump();
            }

        }
    }
    private void EnemyKilled()
    {
        currentJumpCount++;

    }

    private void OnEnterWall()
    {
        ResetValues();
        rb.gravityScale = 0;
    }

    private void OnExitWall()
    {
        rb.gravityScale = 1;
    }
    private void ResetValues()
    {
        ResetVelocity();
        StopSlow();
        currentJumpCount = playerData.MaxJumpCount;
        currentSlowUsage = playerData.MaxSlowUsage;
    }

    private void Jump()
    {
        currentJumpCount--;
        Move(touchDirection);
        StopSlow();
        Line.gameObject.SetActive(false);
    }

    private void StopSlow()
    {
        StopCoroutine(slowModeCoroutine);
        Time.timeScale = 1;
        inSlowMode = false;
    }

    private IEnumerator slowMode()
    {
        inSlowMode = true;
        currentSlowUsage--;
        Time.timeScale = playerData.SlowRate;
        yield return new WaitForSecondsRealtime(playerData.MaxSlowTime);
        Time.timeScale = 1;
        inSlowMode = false;
    }

    public void ResetVelocity()
    {
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = 0;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        currentJumpCount = playerData.MaxJumpCount;
        currentSlowUsage = playerData.MaxSlowUsage;
    }

    private void Move(Vector2 dir)
    {
        ResetVelocity();
        int multDivider = 15;
        int mult = System.Convert.ToInt32(dir.magnitude / multDivider);

        if (mult < 6)
            mult = 6;
        if (mult > playerData.MaxSpeed)
            mult = playerData.MaxSpeed;

        rb.AddForce(dir.normalized * mult, ForceMode2D.Impulse);
    }
}