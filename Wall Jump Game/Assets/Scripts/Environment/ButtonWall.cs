using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonWall : MonoBehaviour
{
    public Vector2 force;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject wall;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Player")
        {
            StartCoroutine(closeAnim());
            wall.SetActive(false);
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = force;
        }
    }

    private IEnumerator closeAnim()
    {
        animator.SetTrigger("buttonPressed");
        yield return new WaitForSeconds(0.4f);
        gameObject.SetActive(false);
    }
}
