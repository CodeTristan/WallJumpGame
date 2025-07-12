using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipEnemy : EnemyBase
{
    public float turnSpeed;
    public float fastTurnSpeed;
    public float slowTurnTime;
    public float fastTurnTime;
    public bool CanTurnFast;

    private bool isFast;
    private float currentSeconds;

    public override void Init()
    {
        base.Init();
        currentSeconds = slowTurnTime;
    }
    private void Update()
    {
        if(Inited == false)
        {
            return;
        }

        if (isFast == false)
            transform.Rotate(new Vector3(0, 0, 1 * turnSpeed * Time.deltaTime));
        else
            transform.Rotate(new Vector3(0, 0, 1 * fastTurnSpeed * Time.deltaTime));

        if(CanTurnFast)
        {
            currentSeconds -= Time.deltaTime;

            if (currentSeconds < 0)
            {

                if (isFast == true)
                {
                    currentSeconds = slowTurnTime;
                    isFast = false;
                }
                else
                {
                    currentSeconds = fastTurnTime;
                    isFast = true;
                }
            }
        }
        
    }
}
