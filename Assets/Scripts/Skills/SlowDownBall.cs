using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownBall : Skill
{
    public float slowDownDuration = 2f;
    public float slowDownRatio = 0.5f;

    protected override void UseAbility(int paddle)
    {
        return;
    }
}
