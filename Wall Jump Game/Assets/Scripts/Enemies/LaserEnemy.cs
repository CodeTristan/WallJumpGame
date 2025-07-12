using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy : EnemyBase
{
    public float ShootTimer;
    public float LaserTimer;
    public float colorInc = 0.1f;
    [SerializeField] private float burstImageRotationSpeed = 100f; // Rotation speed for the burst image


    private float currentShootTimer;
    private float currentLaserTimer;

    [SerializeField] private EnemyBase LaserBeam;
    [SerializeField] private SpriteRenderer LaserLine;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform BurstImage;
    private Color laserLineColor;

    [SerializeField] private Sprite[] sprites;
    private float animTimer;
    private int animIndex = 0;
    private int animDivider = 6;

    private int currentAnimDivider;
    private int currentAnimIndex;
    private float colorStart = 0;

    private Quaternion BurstTargetRotation = Quaternion.identity;
    public override void Init()
    {
        base.Init();
        currentShootTimer = ShootTimer;
        currentLaserTimer = LaserTimer;
        currentAnimDivider = animDivider;
        currentAnimIndex = animIndex;

        animTimer = currentShootTimer / currentAnimDivider;
        laserLineColor = LaserLine.color;

        LaserBeam.Init();
        LaserBeam.gameObject.SetActive(false);
        BurstImage.gameObject.SetActive(false);
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
            if (colorStart + colorInc <= 1)
                colorStart += colorInc;
            LaserLine.color = new Color(laserLineColor.r, laserLineColor.g, laserLineColor.b,colorStart);
        }


        if (currentShootTimer <= 0)
        {
            LaserBeam.gameObject.SetActive(true);
            BurstImage.gameObject.SetActive(true);
            currentLaserTimer -= Time.deltaTime;
            RotateBurstImage();
            spriteRenderer.sprite = sprites[4];
            
            if (currentLaserTimer < 0)
            {
                LaserBeam.gameObject.SetActive(false);
                BurstImage.gameObject.SetActive(false);
                currentShootTimer = ShootTimer;
                currentLaserTimer = LaserTimer;
                currentAnimDivider = animDivider;
                currentAnimIndex = animIndex;
                LaserLine.color = laserLineColor;
                colorStart = 0;

            }

        }
    }


    private void RotateBurstImage()
    {
        if(BurstTargetRotation != BurstImage.rotation)
        {
            BurstImage.rotation = Quaternion.RotateTowards(BurstImage.rotation, BurstTargetRotation, burstImageRotationSpeed * Time.deltaTime);
        }
        else
        {
            BurstTargetRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }
    }
}
