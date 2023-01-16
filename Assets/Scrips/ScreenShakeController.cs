using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenShakeController : MonoBehaviour {

    public static ScreenShakeController instance; // Singleton

    public float rotationMultiplier = 15f;

    private float shakeTimeRemaining, shakePowerX, shakePowerY, shakeFadeTime, shakeRotation;
    private Vector3 oldCameraPos;
    private bool isReset = false;

    private void Start() {
        instance = this;
        oldCameraPos = transform.position;
    }

    private void LateUpdate() {
        if (shakeTimeRemaining > 0) {
            shakeTimeRemaining -= Time.deltaTime;

            float xAmount = Random.Range(-1f, 1f) * shakePowerX;
            float yAmount = Random.Range(-1f, 1f) * shakePowerY;

            transform.position += new Vector3(xAmount, yAmount, 0f);

            shakePowerX = Mathf.MoveTowards(shakePowerX, 0f, shakeFadeTime * Time.deltaTime);
            shakePowerY = Mathf.MoveTowards(shakePowerY, 0f, shakeFadeTime * Time.deltaTime);

            shakeRotation = Mathf.MoveTowards(shakeRotation, 0f, shakeFadeTime * rotationMultiplier * Time.deltaTime);

        } else if (!isReset && oldCameraPos != null) {
            isReset = true;
            transform.position = oldCameraPos;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, shakeRotation * Random.Range(-1f, 1f));
    }

    public void startShake(float length, float powerX, float powerY, float rotation) {
        isReset = false;
        rotationMultiplier = rotation;
        oldCameraPos = transform.position;
        shakeTimeRemaining = length;
        shakePowerX = powerX;
        shakePowerY = powerY;

        float strongest = (powerX > powerY ? powerX : powerY);

        shakeFadeTime = strongest / length;

        shakeRotation = strongest * rotationMultiplier;
    }

    public void startShake(float length, float power, float rotation) {
        startShake(length, power, power, rotation);
    }
}
