using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public float cooldownTime = 5f;
    private bool isOnCooldown = false;
    public KeyCode leftPaddleActivationKey = KeyCode.None;
    public KeyCode rightPaddleActivationKey = KeyCode.None;
    public void Activate()
    {
        if (!isOnCooldown)
        {
            UseAbility();
            StartCoroutine(CooldownCoroutine());
        }
    }
    public void Activate(int paddle)
    {
        if (!isOnCooldown)
        {
            UseAbility(paddle);
            StartCoroutine(CooldownCoroutine());
        }
    }
    protected abstract void UseAbility();
    protected abstract void UseAbility(int paddle);
    private IEnumerator CooldownCoroutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}
