using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;

    public Vector3 WhereToMove;
    private Vector3 MoveTarget;
    private void Start()
    {
        MoveTarget = WhereToMove;
        MoveTarget = transform.position + MoveTarget;
    }

    private void Update()
    {
        if (Vector3.Distance(MoveTarget,transform.position) > 0.01)
            transform.position = Vector3.MoveTowards(transform.position, MoveTarget, speed * Time.deltaTime);
        else
        {
            WhereToMove.x = -WhereToMove.x;
            WhereToMove.y = -WhereToMove.y;
            MoveTarget = new Vector3(WhereToMove.x, WhereToMove.y, WhereToMove.z);
            MoveTarget = transform.position + MoveTarget;
        }
    }
}
