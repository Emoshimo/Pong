using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkPaddle : Skill
{
    public float shrinkFactor = 0.3f;
    public float duration = 4f;

    protected override void UseAbility()
    {
        return;
    }
    protected override void UseAbility(int paddle)
    {
        if (PaddleManager.instance)
        {
            //Use PaddleManager.instance.EnlargePaddle()
            PaddleManager.instance.ShrinkPaddle(duration, shrinkFactor, paddle);
        }
    }
}
