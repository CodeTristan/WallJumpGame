using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawEnemy : EnemyBase
{
    public float RotateSpeed = 100f;


    private void Update()
    {
        if(!Inited) return;

        transform.Rotate(Vector3.forward * RotateSpeed * Time.deltaTime);
    }
}
