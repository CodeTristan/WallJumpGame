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
    public Vector2 sameWallJumpSpeed;
    public int MaxSpeedLimitBreaker = 5;
    public float wallTime;

    public Vector2 velocity;
    public int currentJumpCount;
    private int currentSlowUsage;
    private int SpeedLimitBreaker = 0;

    private bool inSlowMode;
    private Vector2 touchDirection;
    private Vector2 touchStartPos;

    private PlayerData playerData;
    private PlayerSprite playerSprite;
    private Coroutine slowModeCoroutine;
    private Coroutine fallFromWallCoroutine;

    public void Init()
    {
        Line.gameObject.SetActive(false);
        cam = Camera.main;

        playerData = PlayerManager.instance.playerData;
        playerSprite = PlayerManager.instance.playerSprite;

        currentJumpCount = playerData.MaxJumpCount;
        currentSlowUsage = playerData.MaxSlowUsage;

        rb.gravityScale = 1;
        rb.velocity = Vector2.zero;
    }



    private void LateUpdate()
    {
        //camera movement
        //transform = player
        Vector3 target = new Vector3(cam.transform.position.x, transform.position.y, transform.position.z) + cameraOffset;
        Vector3 smoothPos = Vector3.Lerp(cam.transform.position, target, cameraSpeed);

        if (transform.position.y > 0 && !PlayerManager.instance.isDead)
            cam.transform.position = smoothPos;
    }
    private void Update()
    {
        if (AdManager.instance.InAdMenu) //NO MOVEMENT IN AD
            return;

        if(PlayerManager.instance.OnInvisWall)
            currentJumpCount = playerData.MaxJumpCount;

        //Touch movement Drag and jump
        if (Input.touchCount > 0 &&  currentJumpCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                Line.gameObject.SetActive(true);
                Line.localScale = Vector3.zero;

            }
            else if (Vector2.Distance(touchStartPos,touch.position) > 50 &&(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary))
            {
                Line.gameObject.SetActive(true);
                touchDirection = touchStartPos - touch.position;

                float angle = Mathf.Atan2(touchDirection.y, touchDirection.x) * Mathf.Rad2Deg;
                if (!PlayerManager.instance.OnInvisWall) //If not on invis wall. Added this because rotating triangles cause disconnection between wall
                    playerSprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

                Line.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); //Rotation

                Line.transform.localScale = new Vector3(1f, touchDirection.magnitude / 50, 1);
                if (Line.transform.localScale.y > 4f)
                    Line.transform.localScale = new Vector3(1f, 4f, 1);
                if (Line.transform.localScale.y < 1f)
                    Line.transform.localScale = new Vector3(1f, 1, 1);

                if (!PlayerManager.instance.OnWall && Vector2.Distance(touchStartPos, touch.position) > 30)
                {
                    if (!inSlowMode && currentSlowUsage > 0)
                    {
                        slowModeCoroutine = StartCoroutine(slowMode());
                        StartCoroutine(PlayerScaleEnumerator(0.5f));
                    }
                }

            }
            else if (!PlayerManager.instance.isDead && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                if (Vector2.Distance(touchStartPos, touch.position) > 50)
                {
                    GameSceneEventHandler.instance.PlayerJump();
                }
                else if(PlayerManager.instance.OnWall)
                    StartCoroutine(JumpOnSameWall());
            }

        }
    }

    private IEnumerator JumpOnSameWall()
    {
        int dir = transform.position.x > 0 ? -1 : 1;
        rb.velocity = new Vector2(dir * sameWallJumpSpeed.x, 1 * sameWallJumpSpeed.y);
        yield return new WaitForSeconds(0.085f);
        rb.velocity = new Vector2(-dir * sameWallJumpSpeed.x, 1 * sameWallJumpSpeed.y);

    }


    public void EnemyKilled()
    {
        currentJumpCount++;
        GameSceneUIManager.instance.UpdateJumpCountText();
        rb.velocity = Vector2.up * 5;
    }

    public void BarePass()
    {
        currentJumpCount+=2;
        currentSlowUsage+=2;
        if (currentSlowUsage > playerData.MaxSlowUsage)
            currentSlowUsage = playerData.MaxSlowUsage;
        if (currentJumpCount > playerData.MaxJumpCount)
            currentJumpCount = playerData.MaxJumpCount;
        GameSceneUIManager.instance.UpdateJumpCountText();
    }

    public void OnEnterWall()
    {
        rb.gravityScale = 0;
        ResetValues();
        ResetVelocity();

        if (fallFromWallCoroutine != null)
            StopCoroutine(fallFromWallCoroutine);
        fallFromWallCoroutine = StartCoroutine(FallFromWall());
    }

    public void OnExitWall()
    {
        animator.SetBool("OnWall", false);
        rb.gravityScale = 1;
    }

    public void OnEnterInvisibleWall()
    {
        velocity = rb.velocity;
    }

    public void OnExitInvisibleWall()
    {

    }

    private IEnumerator FallFromWall()
    {
        if (transform.position.y < 5) //If on the start
            yield break;

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
        StopSlow();
        currentJumpCount = playerData.MaxJumpCount;
        currentSlowUsage = playerData.MaxSlowUsage;
        touchDirection = Vector2.zero;
        SpeedLimitBreaker = 0;
        GameSceneUIManager.instance.UpdateJumpCountText();
    }

    public void OnPlayerDie()
    {
        ResetVelocity();
        ResetValues();
        StopAllCoroutines();
        rb.gravityScale = 0;
    }

    public void Jump()
    {
        StartCoroutine(PlayerScaleEnumerator(1));
        currentJumpCount--;
        Move(touchDirection);
        StopSlow();
        Line.gameObject.SetActive(false);
        SpeedLimitBreaker++;
    }

    private IEnumerator PlayerScaleEnumerator(float scale)
    {
        float duration = 0.1f; // Animasyon süresi (1 saniye)
        float elapsedTime = 0f; // Geçen zaman

        Vector2 startScale = playerSprite.transform.localScale; // Baþlangýç boyutu
        Vector2 targetVector = new Vector2(1, scale); // Hedef boyutu

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration); // 0 ile 1 arasýnda deðer

            playerSprite.transform.localScale = Vector2.Lerp(startScale, targetVector, t);

            yield return null; // Bir frame bekle
        }

        // Son deðeri kesinleþtir
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
        Debug.Log("Resetting Velocity");
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = 0;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void Move(Vector2 dir)
    {
        ResetVelocity();
        int multDivider = 10;
        int mult = System.Convert.ToInt32(dir.magnitude / multDivider);

        if (mult < 6)
            mult = 6;
        if (mult > playerData.MaxSpeed)
            mult = playerData.MaxSpeed;

        mult += SpeedLimitBreaker;
        if (mult > playerData.MaxSpeed + MaxSpeedLimitBreaker)
            mult = playerData.MaxSpeed + MaxSpeedLimitBreaker;

        if(mult > PlayerManager.MAX_SPEED)
            mult = PlayerManager.MAX_SPEED;


        rb.AddForce(dir.normalized * mult, ForceMode2D.Impulse);
    }
}