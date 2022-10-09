using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerMovement : MonoBehaviour
{


    [Header("GameObjects")]
    public GameObject Line;
    public GameObject barePassImage;
    public GameObject killCountImage;
    public Image slowPanel;
    public SpriteRenderer sprite;

    public GameObject restartMenu;

    [Header("Texts")]
    public TextMeshProUGUI pointText;
    public TextMeshProUGUI jumpCountText;
    public TextMeshProUGUI BarePassXCount;
    public TextMeshProUGUI BarePassXCountTimer;
    public TextMeshProUGUI KillCountX;
    public TextMeshProUGUI KillCountXTimer;
    public TextMeshProUGUI DeathMaxPointText;
    public TextMeshProUGUI DeathPointText;
    public TextMeshProUGUI DeathCoinText;
    public TextMeshProUGUI GainedCoinText;
    public TextMeshProUGUI doublePointTimerText;
    public TextMeshProUGUI bomberTimerText;

    [Header("Sprite type")]
    public bool isSphere = true;
    public bool isCube = false;
    public bool isTriangle = false;

    [Header("Values")]
    public int Point = 0;
    public int Coin;
    public int Health;
    public float cameraSpeed = 0.2f;
    public int MaxCombo;
    public float BarePassTimer;
    public float KillCountTimer;

    [Header("Movement")]
    public int maxSpeed;
    public int minSpeed;
    public int maxJumpCount;
    public float slowRate;
    public float slowTime;
    public int maxSlowUsage;

    [Header("PowerUpValues")]
    public int slingShotYPower;
    public float doublePointTimer;
    public float bomberTimer;
    public bool isBomber;

    private float currentBomberTimer;
    private float currentDoublePointTimer;
    private int doublePointExponent = 1;

    [Header("Just for check")]
    public int mult;
    public int multDivider;
    public int completedLevels = 0;
    public int currentHealth;
    public bool died;
    public bool onWall;
    public bool onInvisWall;
    public bool waiting;
    public bool slowUsed;
    public float gravityScale;
    public int jumpCount;
    public int slowUsage;
    public float yPos;

    private int BarePassExponent = 0;
    private int KillCountExponent = 0;
    private float startFixedDeltaTime;
    private float currentBarePassTimer;
    private float currentKillCountTimer;
    private int currentCoin;
    private float currentCoinExponent = 1;

    private Vector3 cameraOffset = new Vector3(0, 0, -10);
    private bool resetted = false;

    private Transform LineTransform;
    private LevelGenerator levelGenerator;
    private PlayerSprite playerSprite;
    private Rigidbody2D rb;
    private Camera cam;
    private Vector2 dir;
    private Vector2 startPos;
    private Vector3 camStartPos;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        levelGenerator = FindObjectOfType<LevelGenerator>();
        playerSprite = FindObjectOfType<PlayerSprite>();

        if (PlayerPrefs.GetInt("MaxCombo") == 0)
            PlayerPrefs.SetInt("MaxCombo", 2);
        if (PlayerPrefs.GetInt("MaxJump") == 0)
            PlayerPrefs.SetInt("MaxJump", 2);
        if (PlayerPrefs.GetInt("MaxSpeed") == 0)
            PlayerPrefs.SetInt("MaxSpeed", 11);

        if (PlayerPrefs.GetInt("slingShotPower") == 0)
            PlayerPrefs.SetInt("slingShotPower", 30);
        if (PlayerPrefs.GetFloat("doublePointTimer") == 0)
            PlayerPrefs.SetFloat("doublePointTimer", 10);
        if (PlayerPrefs.GetFloat("bomberTimer") == 0)
            PlayerPrefs.SetFloat("bomberTimer", 10);



        FindObjectOfType<SaveSystem>().LoadGame();
        currentCoin = Coin;

        cam = Camera.main;
        camStartPos = cam.transform.position;

        currentHealth = Health;
        jumpCount = maxJumpCount;
        slowUsage = maxSlowUsage;
        startFixedDeltaTime = Time.fixedDeltaTime;
        currentBarePassTimer = BarePassTimer;

        Line.SetActive(false);
        yPos = transform.position.y;

        LineTransform = Line.GetComponent<Transform>();

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Wall")
        {
            onWall = true;
            slowUsed = false;
            jumpCount = maxJumpCount;
            slowUsage = maxSlowUsage;
            resetVelocity();
            rb.gravityScale = 0;
        }
        if(collision.collider.tag == "BouncyWall")
        {
            slowUsed = false;
            jumpCount = maxJumpCount;
            slowUsage = maxSlowUsage;
        }
        if (collision.gameObject.tag == "Enemy")
        {
            if(isBomber)
            {
                //Add anim and voice
                if(collision.gameObject.name == "LaserBeam")
                {
                    Destroy(collision.gameObject.transform.parent.gameObject);
                }
                else
                {
                    Destroy(collision.gameObject);
                }
                jumpCount++;
                rb.velocity = new Vector3(1f, 5f, 0);

            }
            else
            {
                currentHealth--;
                resetVelocity();
            }
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Wall")
        {
            onWall = false;
            rb.gravityScale = gravityScale;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            onInvisWall = true;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            rb.gravityScale = 0;
        }
        if (collision.tag == "EndLine")
        {
            completedLevels++;
            Destroy(collision.gameObject);
        }
        if(collision.gameObject.tag == "Enemy")
        {
            if(BarePassExponent < MaxCombo)
                BarePassExponent++;
            currentBarePassTimer = BarePassTimer;
            BarePassXCount.gameObject.SetActive(true);
            BarePassXCountTimer.gameObject.SetActive(true);
            barePassImage.SetActive(true);
            Point += (15 * BarePassExponent * doublePointExponent);
            Coin += (15 * BarePassExponent * doublePointExponent);
            barePassImage.GetComponentInChildren<TextMeshProUGUI>().text = "+" + (15 * BarePassExponent * doublePointExponent).ToString();
            BarePassXCount.text = BarePassExponent.ToString() + "X";
            collision.enabled = false;
        }
        if (collision.gameObject.tag == "SphereEnemy")
        {
            if(!isSphere && !isBomber)
            {
                currentHealth--;
                resetVelocity();
                return;
            }
            EnemyKilled();
            Destroy(collision.gameObject);
            resetVelocity();
            rb.velocity = new Vector3(1f, 7.5f, 0);
            jumpCount++;
        }
        if (collision.gameObject.tag == "CubeEnemy")
        {
            if (!isCube && !isBomber)
            {
                currentHealth--;
                resetVelocity();
                return;
            }
            EnemyKilled();
            Destroy(collision.gameObject);
            resetVelocity();
            rb.velocity = new Vector3(1f, 7.5f, 0);
            jumpCount++;
        }
        if (collision.gameObject.tag == "TriangleEnemy")
        {
            if (!isTriangle && !isBomber)
            {
                currentHealth--;
                resetVelocity();
                return;
            }
            EnemyKilled();
            Destroy(collision.gameObject);
            resetVelocity();
            rb.velocity = new Vector3(1f, 7.5f, 0);
            jumpCount++;
        }

        if(collision.gameObject.tag == "Slingshot")
        {
            StopCoroutine(playerSprite.SlingShotImmunity());
            resetVelocity();
            StartCoroutine(playerSprite.SlingShotImmunity());
            rb.AddForce(Vector2.up * slingShotYPower, ForceMode2D.Impulse);
            slowUsed = false;
            jumpCount = maxJumpCount;
            slowUsage = maxSlowUsage;
            //Add Anim and voice
        }
        if (collision.gameObject.tag == "DoublePoint")
        {
            currentDoublePointTimer = doublePointTimer;
            doublePointExponent = 2;
            doublePointTimerText.gameObject.SetActive(true);
            //Add Anim and voice
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "Bomber")
        {
            isBomber = true;
            currentBomberTimer = bomberTimer;
            bomberTimerText.gameObject.SetActive(true);
            //Add anim and voice
            Destroy(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            onInvisWall = false;
            rb.gravityScale = gravityScale;
        }
    }
    private void LateUpdate()
    {
        //camera movement
        //transform = player
        Vector3 target = new Vector3(cam.transform.position.x,transform.position.y,transform.position.z) + cameraOffset;
        Vector3 smoothPos = Vector3.Lerp(cam.transform.position, target ,cameraSpeed);

        if(transform.position.y > 0)
        cam.transform.position = smoothPos;
    }
    private void Update()
    {
        currentBarePassTimer -= Time.deltaTime;
        BarePassXCountTimer.text = currentBarePassTimer.ToString();
        currentKillCountTimer -= Time.deltaTime;
        KillCountXTimer.text = currentKillCountTimer.ToString();
        currentDoublePointTimer -= Time.deltaTime;
        doublePointTimerText.text = currentDoublePointTimer.ToString();
        currentBomberTimer -= Time.deltaTime;
        bomberTimerText.text = currentBomberTimer.ToString();
        if (currentBarePassTimer <= 0)
        {
            BarePassExponent = 0;
            BarePassXCount.gameObject.SetActive(false);
            BarePassXCountTimer.gameObject.SetActive(false);
            barePassImage.SetActive(false);
        }
        if (currentKillCountTimer <= 0)
        {
            KillCountExponent = 0;
            KillCountX.gameObject.SetActive(false);
            KillCountXTimer.gameObject.SetActive(false);
            killCountImage.SetActive(false);
        }
        if(currentDoublePointTimer <= 0)
        {
            doublePointExponent = 1;
            doublePointTimerText.gameObject.SetActive(false);
        }
        if (currentBomberTimer <= 0)
        {
            isBomber = false;
            bomberTimerText.gameObject.SetActive(false);
        }
        //Health check
        if (currentHealth <= 0 && !died)
        {
            Die();
        }
        if (transform.position.y < yPos - 45 && !died)
            Die();

        //Point Adjustment
        if (transform.position.y > yPos + 1)
        {
            Point = Point + (1 * doublePointExponent);
            yPos = transform.position.y;
        }
        //Text Adjustment
        pointText.text = Point.ToString();
        jumpCountText.text = jumpCount.ToString();


        //Touch movement Drag and jump
        if (Input.touchCount > 0 && jumpCount > 0 && !died)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startPos = touch.position;
                Line.SetActive(true);
            }
            if (!onWall)
            {
                if(!waiting && slowUsage > 0 && !slowUsed)
                    StartCoroutine(slowMode());
                if(sprite.transform.localScale.y > 0.5)
                    sprite.transform.localScale = new Vector3(1, sprite.transform.localScale.y - 0.02f, 1);
            }
            if (touch.phase == TouchPhase.Moved)
            {
                dir = startPos - touch.position;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                if(!onInvisWall) //If not on invis wall. Added this because rotating triangles cause disconnection between wall
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                LineTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); //Rotation

                Line.transform.localScale = new Vector3(1f,dir.magnitude / 70, 1);
                if(Line.transform.localScale.y > 4f)
                    Line.transform.localScale = new Vector3(1f, 4f, 1);
                if(Line.transform.localScale.y < 1f)
                    Line.transform.localScale = new Vector3(1f, 1, 1);
                
            }
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                Move(dir);
                if(resetted)
                {
                    resetVelocity();
                    transform.position = new Vector3(0, -12, 0);
                    resetted = false;
                }
                StopCoroutine(slowMode());
                Time.timeScale = 1;
                Time.fixedDeltaTime = startFixedDeltaTime;
                waiting = false;
                slowUsed = false;
                Line.SetActive(false);
                slowPanel.color = new Color32(79, 79, 79, 0);
                sprite.transform.localScale = new Vector3(1, 1, 1);


            }

        }

        if(restartMenu.activeSelf && currentCoin < Coin)
        {
            currentCoin += 3;
            DeathCoinText.text = currentCoin.ToString() + "$";
        }

    }

    public void Restart()
    {
        //Level Generator
        completedLevels = 0;
        levelGenerator.resetGame();
        levelGenerator.MakeGame();

        restartMenu.SetActive(false);
        currentHealth = Health;
        transform.position = new Vector3(0, -12, 0);
        cam.transform.position = camStartPos;
        yPos = transform.position.y;

        Point = 0;
        rb.gravityScale = gravityScale;
        slowUsed = false;

        jumpCount = maxJumpCount+1;
        slowUsage = maxSlowUsage+1;

        resetVelocity();
        resetted = true;
        died = false;

        currentBarePassTimer = BarePassTimer;
        BarePassExponent = 0;
        BarePassXCount.gameObject.SetActive(false);
        BarePassXCountTimer.gameObject.SetActive(false);
        barePassImage.SetActive(false);
        pointText.gameObject.SetActive(true);

        currentKillCountTimer = KillCountTimer;
        KillCountExponent = 0;
        KillCountX.gameObject.SetActive(false);
        KillCountXTimer.gameObject.SetActive(false);
        killCountImage.SetActive(false);

        GainedCoinText.gameObject.GetComponent<Animator>().SetBool("Dead", false);
        Time.timeScale = 1;
        FindObjectOfType<PlayerSprite>().AdjustSphereStart();
        currentCoin = Coin;
        rb.constraints = RigidbodyConstraints2D.None;

    }
    private void EnemyKilled()
    {
        if(KillCountExponent < MaxCombo)
            KillCountExponent++;
        currentKillCountTimer = KillCountTimer;
        KillCountX.gameObject.SetActive(true);
        KillCountXTimer.gameObject.SetActive(true);
        killCountImage.SetActive(true);
        Point += (15 * KillCountExponent * doublePointExponent);
        Coin += (15 * KillCountExponent * doublePointExponent);
        killCountImage.GetComponentInChildren<TextMeshProUGUI>().text = "+" + (15 * KillCountExponent * doublePointExponent).ToString();
        KillCountX.text = KillCountExponent.ToString() + "X";
    }
    private void Die()
    {
        //Add Animation
        died = true;
        GainedCoinText.gameObject.GetComponent<Animator>().SetBool("Dead", true);
        Coin += Point * 3 / 10;

        FindObjectOfType<SaveSystem>().SaveGame();
        if (PlayerPrefs.GetInt("MaxPoint") < Point)
            PlayerPrefs.SetInt("MaxPoint", Point);

        DeathMaxPointText.text = PlayerPrefs.GetInt("MaxPoint").ToString();
        DeathPointText.text = Point.ToString();
        GainedCoinText.text = "+ " + (Coin - currentCoin).ToString() + "$";

        currentHealth = 0;
        barePassImage.SetActive(false);
        restartMenu.SetActive(true);
        pointText.gameObject.SetActive(false);
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    private IEnumerator slowMode()
    {
        waiting = true;
        slowUsed = true;
        slowUsage--;
        slowPanel.color = new Color32(79, 79, 79, 130);
        Time.timeScale = slowRate;
        Time.fixedDeltaTime = startFixedDeltaTime * slowRate;
        yield return new WaitForSecondsRealtime(slowTime);
        Time.timeScale = 1;
        Time.fixedDeltaTime = startFixedDeltaTime;
        slowPanel.color = new Color32(79, 79, 79, 0);
        waiting = false;
    }

    private void resetVelocity()
    {
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = 0;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void Move(Vector2 dir)
    {
        resetVelocity();
        
        mult = System.Convert.ToInt32(dir.magnitude / multDivider);
        if (mult < minSpeed)
            mult = minSpeed;
        if (mult > maxSpeed)
            mult = maxSpeed;
        
        rb.AddForce(dir.normalized * mult, ForceMode2D.Impulse);
        jumpCount--;
    }
}
