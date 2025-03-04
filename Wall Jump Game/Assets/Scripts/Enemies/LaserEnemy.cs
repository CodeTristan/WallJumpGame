using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy : EnemyBase
{
    public float ShootTimer;
    public float LaserTimer;


    private float currentShootTimer;
    private float currentLaserTimer;

    [SerializeField] private EnemyBase LaserBeam;
    [SerializeField] private SpriteRenderer LaserLine;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color32 laserLineColor;

    [SerializeField] private Sprite[] sprites;
    private float animTimer;
    private int animIndex = 0;
    private int animDivider = 6;
    private byte colorInc;

    private int currentAnimDivider;
    private int currentAnimIndex;
    private byte colorStart = 0;

    public override void Init()
    {
        currentShootTimer = ShootTimer;
        currentLaserTimer = LaserTimer;
        currentAnimDivider = animDivider;
        currentAnimIndex = animIndex;

        animTimer = currentShootTimer / currentAnimDivider;
        laserLineColor = LaserLine.color;

        LaserBeam.Init();
        LaserBeam.gameObject.SetActive(false);
        Inited = true;
    }
    private void Update()
    {
        if (Inited == false)
        {
            return;
        }

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
            LaserBeam.gameObject.SetActive(true);
            currentLaserTimer -= Time.deltaTime;
            spriteRenderer.sprite = sprites[4];
            if (currentLaserTimer < 0)
            {
                LaserBeam.gameObject.SetActive(false);
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
