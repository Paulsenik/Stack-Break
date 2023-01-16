using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour {

    public static Transform spawnPoint;

    public List<TileScript> tiles;
    // erstes Element => Zentrumy
    bool canPlayerInteract = true;

    private void Awake() {
        Vector3 dir = spawnPoint.position - tiles[0].transform.position;
        tiles[0].transform.position = spawnPoint.position;
        for (int i = 1; i < tiles.Count; i++) {
            tiles[i].transform.position += dir;
            tiles[i].updatePosition();
        }
    }

    public void Update() {
        if (canPlayerInteract)
            foreach (TileScript ts in tiles)
                if (ts == null || !ts.isActiveAndEnabled || !ts.canMove) {
                    canPlayerInteract = false;
                    break;
                }

        if (!hasActiveTiles()) {
            Debug.Log("del");
            if (GameManager.instance.currentShape == this) {
                GameManager.instance.stopFastSpeed();
                GameManager.instance.currentShape = null;
            }
            Destroy(gameObject);
        }
    }

    public bool hasActiveTiles() {
        foreach (TileScript ts in tiles) {
            if (ts != null) {
                return true;
            }
        }
        return false;
    }

    public bool rotate() {
        if (canPlayerInteract) {
            List<float[]> newPos = new List<float[]>();
            float xRoot = tiles[0].getX();
            float yRoot = tiles[0].getY();
            for (int i = 1; i < tiles.Count; i++) {
                if (tiles[i] != null && tiles[i].isActiveAndEnabled) {
                    float[] point = new float[2];
                    float a = tiles[i].getX() - xRoot;
                    float b = tiles[i].getY() - yRoot;
                    point[0] = xRoot + b;
                    point[1] = yRoot - a;

                    if (TileScript.getTile((int)point[0], (int)point[1]) != null)
                        return false;

                    newPos.Add(point);
                }
            }

            for (int i = 1; i < tiles.Count; i++) {
                if (tiles[i] != null && tiles[i].isActiveAndEnabled) {
                    float[] p = newPos[i - 1];
                    tiles[i].setCoord((int)p[0], (int)p[1]);
                }
            }

            return true;
        }
        return false;
    }

    public bool moveLeft() {
        if (canPlayerInteract) {
            List<float[]> newPos = new List<float[]>();
            foreach (TileScript t in tiles) {
                if (t != null && t.isActiveAndEnabled) {
                    float[] p = { t.getX() - 1, t.getY() };
                    TileScript ts = TileScript.getTile((int)p[0], (int)p[1]);
                    if (ts == null || (ts != null && (ts.canMove)))
                        newPos.Add(p);
                    else
                        return false;
                }
            }

            for (int i = 0; i < newPos.Count; i++) {
                float[] p = newPos[i];
                tiles[i].setCoord((int)p[0], (int)p[1]);
            }

            return true;
        }
        return false;
    }

    public bool moveRight() {
        if (canPlayerInteract) {
            List<float[]> newPos = new List<float[]>();
            foreach (TileScript t in tiles) {
                if (t != null && t.isActiveAndEnabled) {
                    float[] p = { t.getX() + 1, t.getY() };
                    TileScript ts = TileScript.getTile((int)p[0], (int)p[1]);
                    if (ts == null || (ts != null && (ts.canMove)))
                        newPos.Add(p);
                    else
                        return false;
                }
            }

            for (int i = 0; i < newPos.Count; i++) {
                float[] p = newPos[i];
                tiles[i].setCoord((int)p[0], (int)p[1]);
            }

            return true;
        }
        return false;
    }

}
