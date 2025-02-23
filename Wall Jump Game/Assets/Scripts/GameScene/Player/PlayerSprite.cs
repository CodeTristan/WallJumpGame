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

    public PlayerShape currentShape;

    private Dictionary<PlayerShape, Sprite> shapeSpriteDict;
    private Collider2D currentCollider;
    public void Init()
    {
        spriteRenderer.size = new Vector2(0.5f, 0.5f);
        shapeSpriteDict = new Dictionary<PlayerShape, Sprite>();
        shapeSpriteDict.Add(PlayerShape.Sphere, sphereSprite);
        shapeSpriteDict.Add(PlayerShape.Cube, cubeSprite);
        shapeSpriteDict.Add(PlayerShape.Triangle, triangleSprite);
        currentCollider = PlayerManager.instance.playerCollisionHandler.currentCollider;
        ChangeShape(PlayerShape.Sphere, PlayerManager.instance.playerCollisionHandler.currentCollider);
    }

    
    public void AdjustSphereStart() //This script is made for using in Restart function. To make player sphere in the beginning.
    {
        spriteRenderer.sprite = sphereSprite;
        currentShape = PlayerShape.Sphere;
    }

    public void ChangeShape(PlayerShape shape, Collider2D collider)
    {
        currentShape = shape;

        currentCollider.enabled = false;
        currentCollider = collider;
        currentCollider.enabled = true;
    }

    //public IEnumerator SlingShotImmunity()
    //{
    //    Physics2D.IgnoreLayerCollision(6, 8,true); //Player and Enemy
    //    animator.SetBool("isInvis", true);

    //    yield return new WaitForSeconds(player.slingShotYPower / 8);

    //    animator.SetBool("isInvis", false);
    //    Physics2D.IgnoreLayerCollision(6, 8, false);
    //}
}
