using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy : MonoBehaviour
{
    public float ShootTimer;
    public float LaserTimer;


    public float currentShootTimer;
    public float currentLaserTimer;

    public GameObject Laser;
    public SpriteRenderer LaserLine;
    private Color32 laserLineColor;

    public Sprite[] sprites;
    public float animTimer;
    public int animIndex = 0;
    public int animDivider = 6;
    public byte colorInc;

    private SpriteRenderer spriteRenderer;
    private int currentAnimDivider;
    private int currentAnimIndex;
    private byte colorStart = 0;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        currentShootTimer = ShootTimer;
        currentLaserTimer = LaserTimer;
        currentAnimDivider = animDivider;
        currentAnimIndex = animIndex;

        animTimer = currentShootTimer / currentAnimDivider;
        laserLineColor = LaserLine.color;
    }
    private void Update()
    {
        currentShootTimer -= Time.deltaTime;
        animTimer -= Time.deltaTime;
        if(animTimer <= 0 && currentShootTimer > 0)
        {
            if (currentAnimIndex >= 5)
                currentAnimIndex = 4;
            spriteRenderer.sprite = sprites[currentAnimIndex];
            currentAnimIndex++;
            currentAnimDivider--;
            animTimer = currentShootTimer / currentAnimDivider;
            if (colorStart + colorInc <= 255)
                colorStart += colorInc;
            LaserLine.color = new Color32(laserLineColor.r, laserLineColor.g, laserLineColor.b,colorStart);
        }


        if (currentShootTimer <= 0)
        {
            Laser.SetActive(true);
            currentLaserTimer -= Time.deltaTime;
            spriteRenderer.sprite = sprites[4];
            if (currentLaserTimer < 0)
            {
                Laser.SetActive(false);
                currentShootTimer = ShootTimer;
                currentLaserTimer = LaserTimer;
                currentAnimDivider = animDivider;
                currentAnimIndex = animIndex;
                LaserLine.color = laserLineColor;
                colorStart = 0;

            }

        }
    }
}
