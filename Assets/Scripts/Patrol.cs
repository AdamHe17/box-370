using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour {

    public float speed;
    private Rigidbody2D rb;
    private GameObject player;

    public GameObject[] waypoints;
    private Vector3[] path;
    private int destIndex, dir;
    private Vector3 currDest;

    private int rotationIndex;
    List<Quaternion> rotations;

    bool seesPlayer;

    GameObject visionCone;
    public float visionAngle;
    public float visionRange;


    void Start () {
        path = new Vector3[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++) {
            path[i] = waypoints[i].transform.position;
        }
        rb = this.GetComponent<Rigidbody2D>();
        destIndex = 0;
        dir = -1;
        nextDest();
        rotationIndex = -1;
        player = GameObject.FindGameObjectWithTag("Player");
        seesPlayer = false;

        visionCone = new GameObject("cone");
        MeshRenderer mr = visionCone.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        Material mat = Resources.Load("Flashlight", typeof(Material)) as Material;
        mr.material = mat;
        MeshFilter filter = visionCone.AddComponent(typeof(MeshFilter)) as MeshFilter;
        visionCone.AddComponent<MeshCollider>();
    }

    //private void OnDrawGizmos() {
    //    Gizmos.color = Color.blue;
    //    for (float i = -2f; i < 2f; i += 0.05f) {
    //        Vector3 target = transform.up * 5f + transform.right * i;
    //        RaycastHit2D rh = Physics2D.Raycast(transform.position, target, 5f);
    //        if (rh.collider && rh.collider.gameObject.CompareTag("wall")) {
    //            Gizmos.DrawLine(transform.position, rh.point);
    //        } else {
    //            Gizmos.DrawRay(transform.position, target);
    //        }
    //    }
    //}

    void FixedUpdate () {
        generateVision();

        //target detection
        float sightOfPlayer = Vector2.Angle(player.transform.position - transform.position, transform.up);
        //Debug.DrawLine(transform.position, player.transform.position, Color.green);
        //Debug.Log(sightOfPlayer);
        if (sightOfPlayer < visionAngle) {
            //Debug.DrawLine(transform.position, player.transform.position, Color.yellow);
            RaycastHit2D rh = Physics2D.Raycast(transform.position, player.transform.position - transform.position, visionRange);
            RaycastHit2D rh_test = Physics2D.Raycast(transform.position, player.transform.position - transform.position);
            if (rh_test.collider && rh_test.collider.gameObject == player) {
                seesPlayer = true;
            } else {
                seesPlayer = false;
            }
            //Debug.DrawRay(transform.position, Vector3.Normalize(player.transform.position - transform.position) * 5f, Color.yellow);
            //Debug.Log(rh.collider);
            if (rh.collider && rh.collider.gameObject == player) {
                //Debug.DrawLine(transform.position, player.transform.position, Color.red);
                float angleToPlayer = Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
                Quaternion rot = Quaternion.Euler(new Vector3(0, 0, angleToPlayer - 90));
                transform.rotation = rot;
                rb.AddForce(transform.up * speed * 2);
                return;
            }
        } else { seesPlayer = false; }

        

        //patrolling movement
        if (Vector3.Distance(currDest, transform.position) < 0.1) {
            transform.position = currDest;
            nextDest();
            rotationIndex = -1;
            return;
        }

        float angle = Mathf.Atan2(currDest.y - transform.position.y, currDest.x - transform.position.x) * Mathf.Rad2Deg;

        if (rotationIndex == -1) {
            Quaternion rot = Quaternion.Euler(new Vector3(0, 0, angle - 90));
            rotations = getRotationAngles(rot);
            rotationIndex = 0;
        }

        if (rotationIndex < rotations.Count) {
            Quaternion rot = rotations[rotationIndex];

            transform.rotation = rot;
            rotationIndex++;
        }
        else {
            Quaternion rot = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            transform.rotation = rot;
            rb.AddForce(transform.up * speed);
        }
    }

    private void nextDest() {
        if (destIndex <= 0) {
            dir = 1;
        } else if (destIndex >= path.Length - 1) {
            dir = -1;
        }
        destIndex += dir;
        currDest = path[destIndex];
    }

    private List<Quaternion> getRotationAngles(Quaternion target) {
        List<Quaternion> res = new List<Quaternion>();

        for (float t = 0f; t < 1f; t += 0.05f) {
            res.Add(Quaternion.Lerp(transform.rotation, target, t));
        }
        return res;
    }

    public bool canSeePlayer() {
        return seesPlayer;
    }

    private void generateVision() {
        List<Vector2> vertices2D = new List<Vector2>();

        vertices2D.Add(transform.up * 0.5f);

        float h_limit = visionRange * Mathf.Tan(visionAngle * Mathf.Deg2Rad);


        for (float i = -h_limit; i < h_limit; i += 2f * h_limit / 40f) {
            Vector3 target = Vector3.Normalize(transform.up * visionRange + transform.right * i) * visionRange;
            //vertices2D.Add(target);
            RaycastHit2D rh = Physics2D.Raycast(transform.position, Vector3.Normalize(target), visionRange);
            //Debug.DrawRay(transform.position, Vector3.Normalize(target) * 5f, Color.grey);
            if (rh.collider && rh.collider.gameObject.CompareTag("wall")) {
                //Debug.DrawLine(transform.position, rh.point, Color.green);
                vertices2D.Add(target * rh.fraction);
            }
            else {
                //Debug.DrawLine(transform.position, target, Color.blue);
                vertices2D.Add(target);
            }
        }

        // Use the triangulator to get indices for creating triangles
        triangulator tr = new triangulator(vertices2D.ToArray());
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Count];
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0) + transform.position;
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        Color[] colors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            colors[i] = Color.Lerp(Color.red, Color.green, vertices[i].y);

        // assign the array of colors to the Mesh.
        msh.colors = colors;

        // Set up game object with mesh;
        visionCone.GetComponent<MeshFilter>().mesh = msh;
        visionCone.GetComponent<MeshCollider>().sharedMesh = msh;
    }
}
