using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionUser : MonoBehaviour {

    public void makeStandardTransition() {
        TransitionHandler.singleton.makeStandardTransition();
    }
}
