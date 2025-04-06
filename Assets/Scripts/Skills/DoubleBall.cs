using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleBall : Skill
{
    public float duplicationDuration = 5f;
    protected override void UseAbility()
    {
        if (BallManager.instance)
        {
            BallManager.instance.DuplicateBall(duplicationDuration);
        }
    }
    protected override void UseAbility(int paddle)
    {
        return;
    }
}
