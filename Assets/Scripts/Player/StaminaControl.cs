using System.Collections;
using UnityEngine;

public class StaminaControl : Singleton<StaminaControl>
{
    [SerializeField] private float staminaRecoverySpeed = 1f;
    [SerializeField] private float maxStamina = 0f;

    [Range(0, 100)]
    [SerializeField] private float currentStamina;
    [HideInInspector] public bool tryingSprint = false;
    public bool infinityStamina = false;
    
    private  bool usingStamina = false;
    private bool canUsingStamina = true;
    private float tempStamina = 0;

    protected override void Awake()
    {
        base.Awake();
        currentStamina = maxStamina; 
        tempStamina = currentStamina; 
    }

    public float GetCurrentStamina() => currentStamina;

    public void SetMaxStamina(float _max) => maxStamina = _max;

    public void SetStaminaRecoverySpeed(float _speed) => staminaRecoverySpeed = _speed;

    private void StaminaDirection()
    {
        if (currentStamina > tempStamina) 
            usingStamina = false;

        if (currentStamina < tempStamina)
            usingStamina = true;

        if (usingStamina && tempStamina == currentStamina)
        {
            usingStamina = false;
        }

        tempStamina = currentStamina;
    }

    public bool UseStamina(float _staminaDrain)
    {   
        if(!canUsingStamina)
            return false;

        if(currentStamina <= 0f)
            StartCoroutine(CanRecolverStamina());

        if (currentStamina >= _staminaDrain)
        {
            if(!infinityStamina)
                currentStamina -= _staminaDrain;
            return true;
        }
        else if (usingStamina)
        {
            currentStamina -= _staminaDrain;
            return true;
        }
        
        return false;
    }

    public void RecoverStamina()
    {   
        if (tryingSprint && currentStamina <= 0)
            return;

        if (!canUsingStamina || usingStamina)
            return;

        if (currentStamina < maxStamina)
            currentStamina += Time.deltaTime * staminaRecoverySpeed;
    }

    private void FixedUpdate() 
    {
        StaminaDirection();
        RecoverStamina();
    }

    private IEnumerator CanRecolverStamina()
    {
        currentStamina = 0f;
        canUsingStamina = false;

        yield return new WaitForSeconds(0.4f);
        canUsingStamina = true;
    }
}
