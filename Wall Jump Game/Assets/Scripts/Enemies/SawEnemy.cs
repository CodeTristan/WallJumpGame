using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawEnemy : EnemyBase
{
    public float RotateSpeed = 100f;

    [SerializeField] private Transform SawBlades;

    private void Update()
    {
        if(!Inited) return;

        SawBlades.Rotate(Vector3.forward * RotateSpeed * Time.deltaTime);
    }
}
