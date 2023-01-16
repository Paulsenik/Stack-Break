using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Events;

public class TransitionHandler : MonoBehaviour {

    public static TransitionHandler singleton;
    public Animator anim;

    public void Awake() {
        DontDestroyOnLoad(gameObject);
        if (singleton != null) {
            Destroy(gameObject);
            return;
        }
        singleton = this;
    }

    [Header("AnimationTriggers")]
    public string startStandardTransition;

    [Header("Events")]
    public UnityEvent onTransitionMid; // gets used at mid of transition

    public void makeStandardTransition() {
        Debug.Log("standardTransition");
        anim.SetTrigger(startStandardTransition);
    }

    public void callOnTransitionMid() {
        onTransitionMid.Invoke();
    }

}
