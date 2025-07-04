using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : MonoBehaviour
{
    public enum PlayerShape
    {
        Sphere,
        Cube,
        Triangle
    }

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite sphereSprite;
    [SerializeField] private Sprite cubeSprite;
    [SerializeField] private Sprite triangleSprite;

    public PlayerShape currentShape = PlayerShape.Sphere;

    private Dictionary<PlayerShape, Sprite> shapeSpriteDict;
    public void Init()
    {
        spriteRenderer.size = new Vector2(0.5f, 0.5f);
        shapeSpriteDict = new Dictionary<PlayerShape, Sprite>();
        shapeSpriteDict.Add(PlayerShape.Sphere, sphereSprite);
        shapeSpriteDict.Add(PlayerShape.Cube, cubeSprite);
        shapeSpriteDict.Add(PlayerShape.Triangle, triangleSprite);
        ChangeShape(currentShape);
    }

    public void Reset()
    {
        ChangeShape(PlayerShape.Sphere);
    }


    public void AdjustSphereStart() //This script is made for using in Restart function. To make player sphere in the beginning.
    {
        spriteRenderer.sprite = sphereSprite;
        currentShape = PlayerShape.Sphere;
    }

    public void ChangeShape(PlayerShape shape)
    {
        currentShape = shape;
        spriteRenderer.sprite = shapeSpriteDict[shape];

        PlayerManager.instance.playerCollisionHandler.ChangeCollider(shape);
    }

}
