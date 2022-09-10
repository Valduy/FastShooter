using UnityEngine;

public class CooldownTimer
{
    private float _cooldownCompleteTime;

    public float coolDownAmount = 1.0f;

    public bool CoolDownComplete => Time.time > _cooldownCompleteTime;

    public void StartColldown()
    {
        _cooldownCompleteTime = Time.time + coolDownAmount;
    }
}
