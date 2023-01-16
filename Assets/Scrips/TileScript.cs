using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileScript : MonoBehaviour, IComparable<TileScript> {

    public static List<TileScript> tiles = new List<TileScript>();

    public enum Type { BLACK, GREEN, RED, WHITE, EXPLOSION_1, EXPLOSION_2, EXPLOSION_3 };
    public enum ExplosionType { ONE, TWO, THREE };

    public Type tileType = Type.BLACK;
    public GameObject deathEffect;
    public bool canMove = true, isDestructable = true;

    private const float distToSnap = 0.05f; // distance for animation
    private const float standardSpeed = 10f;
    private int x, y; // x & y in coordinateSystem

    private void Awake() {
        updatePosition();
        tiles.Add(this);
    }

    private void OnDestroy() {
        tiles.Remove(this);
    }

    public void updatePosition() {
        x = (int)transform.position.x;
        y = (int)transform.position.y;
    }

    // Update is called once per frame
    void Update() {
        Vector3 pos = transform.position;
        if (pos.x != x || pos.y != y) {
            Vector3 v = new Vector3(x, y) - pos;
            transform.Translate(v * standardSpeed * Time.deltaTime);

            //rounding
            if (pos.x < x && pos.x + distToSnap > x)
                transform.position = new Vector3(x, pos.y);
            if (pos.x > x && pos.x - distToSnap < x)
                transform.position = new Vector3(x, pos.y);
            if (pos.y < y && pos.y + distToSnap > y)
                transform.position = new Vector3(pos.x, y);
            if (pos.y > y && pos.y - distToSnap < y)
                transform.position = new Vector3(pos.x, y);
        }
    }

    public void explode(ExplosionType type) {
        List<TileScript> ts = new List<TileScript>();
        if (type == ExplosionType.ONE || type == ExplosionType.TWO || type == ExplosionType.THREE) {
            Debug.Log("EXPLODE_1");
            ts.Add(getTile(x - 1, y));
            ts.Add(getTile(x, y - 1));
            ts.Add(getTile(x + 1, y));
            ts.Add(getTile(x, y + 1));
        }
        if (type == ExplosionType.TWO || type == ExplosionType.THREE) {
            Debug.Log("EXPLODE_2");
            ts.Add(getTile(x - 1, y - 1));
            ts.Add(getTile(x + 1, y - 1));
            ts.Add(getTile(x + 1, y + 1));
            ts.Add(getTile(x - 1, y + 1));
        }
        if (type == ExplosionType.THREE) {
            Debug.Log("EXPLODE_3");
            ts.Add(getTile(x - 1, y + 2));
            ts.Add(getTile(x, y + 2));
            ts.Add(getTile(x + 1, y + 2));
            ts.Add(getTile(x - 1, y - 2));
            ts.Add(getTile(x, y - 2));
            ts.Add(getTile(x + 1, y - 2));
            ts.Add(getTile(x - 2, y + 1));
            ts.Add(getTile(x - 2, y));
            ts.Add(getTile(x - 2, y - 1));
            ts.Add(getTile(x + 2, y + 1));
            ts.Add(getTile(x + 2, y));
            ts.Add(getTile(x + 2, y - 1));
        }
        foreach (TileScript t in ts)
            if (t != null)
                t.destroy();
    }

    public void destroy() {
        if (!isDestructable)
            return;
        isDestructable = false;
        switch (tileType) {
            case Type.EXPLOSION_1:
                explode(ExplosionType.ONE);
                ScreenShakeController.instance.startShake(0.3f, 0.1f, 0f, 10f);
                break;
            case Type.EXPLOSION_2:
                explode(ExplosionType.TWO);
                ScreenShakeController.instance.startShake(0.3f, 0.1f, 0f, 35f);
                break;
            case Type.EXPLOSION_3:
                explode(ExplosionType.THREE);
                ScreenShakeController.instance.startShake(0.3f, 0.1f, 0f, 80f);
                break;
        }
        canMove = false;
        switch (tileType) {
            case Type.BLACK:
                //Death-Effect
                deathEffect.transform.position = transform.position;
                GameObject eff = Instantiate(deathEffect);
                Destroy(eff, 1);
                break;
                //No Effect
        }

        try {
            tiles.Remove(this);
            Destroy(gameObject);
        } catch (MissingReferenceException e) {
            Debug.Log("Tilescript :: missingReference - error " + e.GetBaseException());
        }
    }


    public void freeze() {

        Debug.Log("freeze " + x + " " + y);
        canMove = false;
    }

    public void setCoord(int x, int y) {
        if (!canMove)
            return;

        TileScript tileOnCoord = getTile(x, y);
        if (tileOnCoord != null && !tileOnCoord.canMove) {
            switch (tileOnCoord.tileType) {
                case Type.BLACK: // collides on BLACK or Green
                case Type.GREEN:
                    switch (tileType) {
                        case Type.EXPLOSION_1: // EXPLOSION_1 collides on black
                        case Type.EXPLOSION_2: // EXPLOSION_2 collides on black
                        case Type.EXPLOSION_3: // EXPLOSION_3 collides on black
                        case Type.GREEN: // green collides on black
                            freeze();
                            break;
                        case Type.RED: // red collides on black

                            destroy();
                            tileOnCoord.destroy();
                            break;
                    }
                    break;
                case Type.RED: // collides on RED
                    switch (tileType) {
                        case Type.EXPLOSION_1: // EXPLOSION_1 collides on red
                            destroy();
                            tileOnCoord.destroy();
                            break;
                        case Type.EXPLOSION_2: // EXPLOSION_2 collides on red
                            destroy();
                            tileOnCoord.destroy();
                            break;
                        case Type.EXPLOSION_3: // EXPLOSION_3 collides on red
                            destroy();
                            tileOnCoord.destroy();
                            break;
                        case Type.GREEN: // green collides on red
                            destroy();
                            tileOnCoord.destroy();
                            break;
                        case Type.RED: // red collides on red
                            freeze();
                            break;
                    }
                    break;
                case Type.EXPLOSION_1: // collides on EXPLOSION_1
                    switch (tileType) {
                        case Type.RED: // red collides on EXPLOSION_1
                            destroy();
                            tileOnCoord.destroy();
                            break;
                        default:
                            freeze();
                            break;
                    }
                    break;
                case Type.EXPLOSION_2: // collides on EXPLOSION_2
                    switch (tileType) {
                        case Type.RED: // red collides on EXPLOSION_2
                            destroy();
                            tileOnCoord.destroy();
                            break;
                        default:
                            freeze();
                            break;
                    }
                    break;
                case Type.EXPLOSION_3: // collides on EXPLOSION_3
                    switch (tileType) {
                        case Type.RED: // red collides on EXPLOSION_3
                            destroy();
                            tileOnCoord.destroy();
                            break;
                        default:
                            freeze();
                            break;
                    }
                    break;
                case Type.WHITE: // anything collides on white
                    freeze();
                    break;
            }
        } else {
            this.x = x;
            this.y = y;
        }
    }

    public int getX() {
        return x;
    }
    public int getY() {
        return y;
    }

    public static TileScript getTile(int x, int y) {
        foreach (TileScript t in tiles) {
            if (t.getX() == x && t.getY() == y)
                return t;
        }
        return null;
    }

    public static List<TileScript> getTiles(Type type) {
        List<TileScript> t = new List<TileScript>();
        foreach (TileScript ts in tiles) {
            if (ts.tileType == type)
                t.Add(ts);
        }
        return t;
    }

    public int CompareTo(TileScript other) {
        if (other.y == y)
            return 0;
        if (other.y > y)
            return -1;
        return 1;
    }
}
