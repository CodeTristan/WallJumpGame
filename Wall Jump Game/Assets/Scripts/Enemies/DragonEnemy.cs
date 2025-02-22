using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonEnemy : MonoBehaviour
{

    public float sleepBetweenFrames;
    public float FullFireWait;
    public float maxWidth;
    public float increaseRate;
    public Transform FireObject;

    public float attackTimer;
    private float currentAttackTimer;

    private void Start()
    {
        currentAttackTimer = attackTimer;
    }

    private void Update()
    {
        currentAttackTimer -= Time.deltaTime;

        if(currentAttackTimer  < 0)
        {
            currentAttackTimer = 999f;
            StartCoroutine(Fire());
        }
    }

    private IEnumerator Fire()
    {
        while(FireObject.localScale.x < maxWidth)
        {
            FireObject.localScale += new Vector3(increaseRate, 0, 0);
            yield return new WaitForSeconds(sleepBetweenFrames);
        }

        yield return new WaitForSeconds(FullFireWait);

        currentAttackTimer = attackTimer;
        FireObject.localScale = new Vector3(0, 1, 1);
    }

}
