using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipEnemy : MonoBehaviour
{
    public float turnSpeed;
    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, 1 * turnSpeed * Time.deltaTime));
    }
}
