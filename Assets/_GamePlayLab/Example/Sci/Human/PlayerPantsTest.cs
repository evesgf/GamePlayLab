using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPantsTest : MonoBehaviour
{
    public Animator playerAni;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.runtimeAnimatorController = playerAni.runtimeAnimatorController;
    }
}
