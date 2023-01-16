using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MatchWidth : MonoBehaviour {

    // https://gamedev.stackexchange.com/questions/167317/scale-camera-to-fit-screen-size-unity

    // Set this to the in-world distance between the left & right edges of your scene

    public enum TF { WIDTH, HEIGHT };

    public TF ScaleByFixed = TF.WIDTH;

    public float scale = 10;

    Camera cam;
    void Start() {
        cam = GetComponent<Camera>();
    }

    // Adjust the camera's height so the desired scene width fits in view
    // even if the screen/window size changes dynamically.
    void Update() {

        if (TF.WIDTH == ScaleByFixed) {
            float unitsPerPixel = scale / Screen.width;

            float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

            cam.orthographicSize = desiredHalfHeight;
        } else {
            float unitsPerPixel = scale / Screen.height;

            float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

            cam.orthographicSize = desiredHalfHeight;
        }
    }
}