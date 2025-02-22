using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;

    public Vector3[] path;

    private Vector3 MoveTarget;

    [SerializeField] private int index;

    private void Start()
    {
        MoveTarget = path[0];
        MoveTarget = transform.position + MoveTarget;
    }

    private void Update()
    {
        if (Vector3.Distance(MoveTarget,transform.position) > 0.01)
            transform.position = Vector3.MoveTowards(transform.position, MoveTarget, speed * Time.deltaTime);
        else
        {
            index++;
            if(index >= path.Length)
            {
                index = 0;
            }
            MoveTarget = path[index];
            MoveTarget = transform.position + MoveTarget;
        }
    }

}
