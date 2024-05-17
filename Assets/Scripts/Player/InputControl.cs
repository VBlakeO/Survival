using UnityEngine;
using UnityEngine.Events;

public class InputControl : Singleton<InputControl>
{
    [SerializeField] private bool hotbarIsEnable = true;
    [SerializeField] private bool canOpenBackpack = true;
    [Space]

    public UnityAction<int> OnNumPressed = null;
    public UnityAction<int> OnScrollRoll = null;
    [Space]

    public UnityAction OnPressEscape = null;
    public UnityAction OnPressTab = null;

    public UnityAction OnPressMouse0 = null;
    public UnityAction OnReleasingMouse0 = null;
    public UnityAction WhilePressMouse0 = null;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if(hotbarIsEnable)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1)) SetButtonPressed(0);
            if(Input.GetKeyDown(KeyCode.Alpha2)) SetButtonPressed(1);
            if(Input.GetKeyDown(KeyCode.Alpha3)) SetButtonPressed(2);
            if(Input.GetKeyDown(KeyCode.Alpha4)) SetButtonPressed(3);
            if(Input.GetKeyDown(KeyCode.Alpha5)) SetButtonPressed(4);
            if(Input.GetKeyDown(KeyCode.Alpha6)) SetButtonPressed(5);
            if(Input.GetKeyDown(KeyCode.Alpha7)) SetButtonPressed(6);
            if(Input.GetKeyDown(KeyCode.Alpha8)) SetButtonPressed(7);
            if(Input.GetKeyDown(KeyCode.Alpha9)) SetButtonPressed(8);

            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0f)
                SetScrollRotation((int) Mathf.Sign(scroll));
        }

        if (canOpenBackpack)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                OnPressTab?.Invoke();
        }
    
        if (Input.GetKeyDown(KeyCode.Escape)) OnPressEscape?.Invoke();
        
        
        if (Input.GetKeyDown(KeyCode.Mouse0)) OnPressMouse0?.Invoke();

        if (Input.GetKeyUp(KeyCode.Mouse0)) OnReleasingMouse0?.Invoke();
       
        if (Input.GetKey(KeyCode.Mouse0)) WhilePressMouse0?.Invoke();
    }

    private void SetButtonPressed(int num)
    {
        OnNumPressed?.Invoke(num);
    }

    private void SetScrollRotation(int num)
    {
        OnScrollRoll?.Invoke(num);
    }
}
