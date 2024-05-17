using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    [SerializeField] private Image interactiveObjeteDebug = null;

    private void Awake() 
    {
        interactiveObjeteDebug.enabled = false; 
    }

    public void SetInteractiveObjeteDebug(bool state) => interactiveObjeteDebug.enabled = state;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
