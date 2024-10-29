using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpscareManager : MonoBehaviour
{
    [SerializeField]
    private void OnEnable()
    {
        FlashlightManager.OnJumpscare += Jumpscare();

    }

    private void OnDisable()
    {
        FlashlightManager.OnJumpscare -= Jumpscare();

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void Jumpscare()
    {
        Debug.Log($"Jumpscare occurred! Type: {jumpscareType});
    }


}
