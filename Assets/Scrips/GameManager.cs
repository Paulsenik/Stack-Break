using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public bool isPaused = false;

    public SwipeManager swipeManager;
    public float secondsPerTick, secondsPerTick_Fast;
    public Transform spawnPos;

    private float currentTickSpeed;

    public List<Shape> shapes; // available shapes to spawn

    private float totalTime = 0f;
    [HideInInspector]
    public Shape currentShape = null;
    private bool isFastTickSpeed = false;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        currentTickSpeed = secondsPerTick;
        Shape.spawnPoint = spawnPos;
    }

    public void pause() {
        isPaused = !isPaused;
    }


    public void resetLevel() {
        pause();
        stopFastSpeed();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        pause();
    }

    void Update() {

        if (!isPaused) {

            totalTime += Time.deltaTime;
            if (totalTime >= currentTickSpeed) {
                totalTime = 0.0f;
                tick();
            }

            if (currentShape == null) {
                //Debug.Log("GEN NEW Shape!!");
                int rand = Random.Range(0, shapes.Count);
                currentShape = Instantiate(shapes[rand]);
            }

            bool isAllFreeze = true;
            foreach (TileScript t in currentShape.tiles) {
                if (t == null)
                    continue;
                if (t.isActiveAndEnabled && t.canMove) {
                    isAllFreeze = false;
                    break;
                }
            }

            if (isAllFreeze || currentShape == null) {
                stopFastSpeed();
                currentShape = null;
            }

            useInput();
        }
    }

    void tick() {
        //Debug.Log("tick");
        // move green & red
        List<TileScript> tilesToMove = TileScript.getTiles(TileScript.Type.GREEN);
        tilesToMove.AddRange(TileScript.getTiles(TileScript.Type.RED));
        tilesToMove.AddRange(TileScript.getTiles(TileScript.Type.EXPLOSION_1));
        tilesToMove.AddRange(TileScript.getTiles(TileScript.Type.EXPLOSION_2));
        tilesToMove.AddRange(TileScript.getTiles(TileScript.Type.EXPLOSION_3));
        tilesToMove.Sort();

        //moveDown
        foreach (TileScript t in tilesToMove) {
            t.setCoord(t.getX(), t.getY() - 1);
        }
    }

    private bool hasPressedUp = false, hasPressedDown = false, hasPressedRight = false, hasPressedLeft = false;

    void useInput() {
        //Swipe
        if (currentShape != null && !isFastTickSpeed) {
            // Android
            if (swipeManager.SwipeLeft) {
                currentShape.moveLeft();
            }
            if (swipeManager.SwipeRight) {
                currentShape.moveRight();
            }
            if (swipeManager.Tap) {
                currentShape.rotate();
            }
            if (swipeManager.SwipeDown) {
                Debug.Log("swipe down");
                startFastSpeed();
            }

            //Standalone
            if (Input.GetKey(KeyCode.UpArrow)) {
                if (!hasPressedUp) {
                    hasPressedUp = true;
                    currentShape.rotate();
                }
            } else {
                hasPressedUp = false;
            }
            if (Input.GetKey(KeyCode.DownArrow)) {
                if (!hasPressedDown) {
                    hasPressedDown = true;
                    startFastSpeed();
                }
            } else {
                hasPressedDown = false;
            }
            if (Input.GetKey(KeyCode.LeftArrow)) {
                if (!hasPressedLeft) {
                    hasPressedLeft = true;
                    currentShape.moveLeft();
                }
            } else {
                hasPressedLeft = false;
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
                if (!hasPressedRight) {
                    hasPressedRight = true;
                    currentShape.moveRight();
                }
            } else {
                hasPressedRight = false;
            }
        }
    }
    public void startFastSpeed() {
        isFastTickSpeed = true;
        currentTickSpeed = secondsPerTick_Fast;
    }

    public void stopFastSpeed() {
        Debug.Log("stop");
        isFastTickSpeed = false;
        currentTickSpeed = secondsPerTick;
    }
}
