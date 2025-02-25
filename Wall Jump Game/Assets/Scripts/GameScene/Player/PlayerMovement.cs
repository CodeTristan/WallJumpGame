using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform Line;
    [SerializeField] private Animator animator;
    [SerializeField] private Image slowMotionImage;
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
    private Coroutine fallFromWallCoroutine;
    public void Init()
    {
        Line.gameObject.SetActive(false);
        yPos = transform.position.y;
        cam = Camera.main;

        playerData = PlayerManager.instance.playerData;
        playerSprite = PlayerManager.instance.playerSprite;

        currentJumpCount = playerData.MaxJumpCount;
        currentSlowUsage = playerData.MaxSlowUsage;

        PlayerEventHandler.instance.OnPlayerJump += Jump;

        PlayerEventHandler.instance.OnPlayerDied += OnPlayerDied;

        PlayerEventHandler.instance.OnEnemyKilled += EnemyKilled;

        PlayerEventHandler.instance.OnEnterWall += OnEnterWall;
        PlayerEventHandler.instance.OnLeaveWall += OnExitWall;

        PlayerEventHandler.instance.OnEnterInvisibleWall += OnEnterInvisibleWall;
        PlayerEventHandler.instance.OnLeaveInvisibleWall += OnExitInvisibleWall;

    }

    private void OnDestroy()
    {
        PlayerEventHandler.instance.OnPlayerJump -= Jump;

        PlayerEventHandler.instance.OnPlayerDied -= OnPlayerDied;

        PlayerEventHandler.instance.OnEnemyKilled -= EnemyKilled;

        PlayerEventHandler.instance.OnEnterWall -= OnEnterWall;
        PlayerEventHandler.instance.OnLeaveWall -= OnExitWall;

        PlayerEventHandler.instance.OnEnterInvisibleWall -= OnEnterInvisibleWall;
        PlayerEventHandler.instance.OnLeaveInvisibleWall -= OnExitInvisibleWall;


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
                    
                    StartCoroutine(PlayerScaleEnumerator(0.5f));
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
                PlayerEventHandler.instance.PlayerJump();
            }

        }
    }


    private void EnemyKilled()
    {
        currentJumpCount++;

    }

    private void OnEnterWall()
    {
        rb.gravityScale = 0;
        ResetValues();

        if (fallFromWallCoroutine != null)
            StopCoroutine(fallFromWallCoroutine);
        fallFromWallCoroutine = StartCoroutine(FallFromWall());
    }

    private void OnExitWall()
    {
        animator.SetBool("OnWall", false);
        rb.gravityScale = 1;
    }

    private void OnEnterInvisibleWall()
    {

    }

    private void OnExitInvisibleWall()
    {

    }

    private IEnumerator FallFromWall()
    {
        animator.SetBool("OnWall", true);

        float currentWallTime = wallTime;
        while ( currentWallTime > 0)
        {
            currentWallTime -= Time.deltaTime;
            animator.SetFloat("WallTime", currentWallTime);
            yield return null;
        }

        rb.gravityScale = 1;

    }
    public void ResetValues()
    {
        ResetVelocity();
        StopSlow();
        currentJumpCount = playerData.MaxJumpCount;
        currentSlowUsage = playerData.MaxSlowUsage;
        GameSceneUIManager.instance.UpdateJumpCountText();
    }

    private void OnPlayerDied()
    {
        ResetValues();
        StopAllCoroutines();
        rb.gravityScale = 0;
    }

    private void Jump()
    {
        StartCoroutine(PlayerScaleEnumerator(1));
        currentJumpCount--;
        Move(touchDirection);
        StopSlow();
        Line.gameObject.SetActive(false);
    }

    private IEnumerator PlayerScaleEnumerator(float scale)
    {
        float duration = 0.1f; // Animasyon s�resi (1 saniye)
        float elapsedTime = 0f; // Ge�en zaman

        Vector2 startScale = playerSprite.transform.localScale; // Ba�lang�� boyutu
        Vector2 targetVector = new Vector2(1, scale); // Hedef boyutu

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration); // 0 ile 1 aras�nda de�er

            playerSprite.transform.localScale = Vector2.Lerp(startScale, targetVector, t);

            yield return null; // Bir frame bekle
        }

        // Son de�eri kesinle�tir
        playerSprite.transform.localScale = targetVector;
    }

    private void StopSlow()
    {
        if(slowModeCoroutine != null)
            StopCoroutine(slowModeCoroutine);
        Time.timeScale = 1;
        slowMotionImage.color = new Color32(79, 79, 79, 0);
        inSlowMode = false;
    }

    private IEnumerator slowMode()
    {
        inSlowMode = true;
        slowMotionImage.color = new Color32(79,79,79,140);
        currentSlowUsage--;
        Time.timeScale = 1 - playerData.SlowRate;
        yield return new WaitForSecondsRealtime(playerData.MaxSlowTime);
        Time.timeScale = 1;
        slowMotionImage.color = new Color32(79, 79, 79, 0);
        inSlowMode = false;
    }

    public void ResetVelocity()
    {
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = 0;
        transform.rotation = Quaternion.Euler(0, 0, 0);
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