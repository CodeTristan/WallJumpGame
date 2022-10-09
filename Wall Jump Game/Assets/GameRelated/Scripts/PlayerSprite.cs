using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : MonoBehaviour
{

    private PlayerMovement player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public Sprite sphereSprite;
    public Sprite cubeSprite;
    public Sprite triangleSprite;

    public Collider2D CircleCollider;
    public Collider2D CubeCollider;
    public Collider2D TriangleCollider;

    private string currentShape;
    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        animator = gameObject.GetComponent<Animator>();

        spriteRenderer.size = new Vector2(0.5f, 0.5f);
        //Starts with Sphere form
        spriteRenderer.sprite = sphereSprite;
        AdjustColliders(CircleCollider);
        AdjustBooleans();
        player.isSphere = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "SwapToCube")
        {
            spriteRenderer.sprite = cubeSprite;
            AdjustColliders(CubeCollider);
            AdjustBooleans();
            player.isCube = true;
            currentShape = "cube";
        }
        if (collision.gameObject.name == "SwapToSphere")
        {
            spriteRenderer.sprite = sphereSprite;
            AdjustColliders(CircleCollider);
            AdjustBooleans();
            player.isSphere = true;
            currentShape = "sphere";
        }
        if (collision.gameObject.name == "SwapToTriangle")
        {
            spriteRenderer.sprite = triangleSprite;
            AdjustColliders(TriangleCollider);
            AdjustBooleans();
            player.isTriangle = true;
            currentShape = "triangle";
        }
    }

    public void AdjustSphereStart() //This script is made for using in Restart function. To make player sphere in the beginning.
    {
        spriteRenderer.sprite = sphereSprite;
        AdjustColliders(CircleCollider);
        AdjustBooleans();
        player.isSphere = true;
    }
    private void AdjustBooleans()
    {
        player.isSphere = false;
        player.isCube = false;
        player.isTriangle = false;
    }
    private void AdjustColliders(Collider2D collider)
    {
        CircleCollider.enabled = false;
        CubeCollider.enabled = false;
        TriangleCollider.enabled = false;

        collider.enabled = true;
    }
    public IEnumerator SlingShotImmunity()
    {
        Physics2D.IgnoreLayerCollision(6, 8,true); //Player and Enemy
        animator.SetBool("isInvis", true);

        yield return new WaitForSeconds(player.slingShotYPower / 10);

        animator.SetBool("isInvis", false);
        Physics2D.IgnoreLayerCollision(6, 8, false);
    }
}
