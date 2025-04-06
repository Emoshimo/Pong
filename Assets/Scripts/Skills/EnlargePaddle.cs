using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnlargePaddle : Skill
{
    public float enlargementFactor = 1.4f;
    public float duration = 5f;

    protected override void UseAbility()
    {
        return;
    }
    protected override void UseAbility(int paddle)
    {
        if (PaddleManager.instance)
        {
            //Use PaddleManager.instance.EnlargePaddle()
            PaddleManager.instance.EnlargePaddle(duration, enlargementFactor, paddle);
        }
    }


}
