using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Boid prefab;
    public float spawnRadius = 10;
    public int spawnCount = 10;
    public Color colour;

    [SerializeField]
    private int boidsCount = 0;

    void Awake () {

        // Spawns the boids at the beginning
        for (int i = 0; i < spawnCount; i++) {

            // spawns each boid at a random position inside a sphere of radius spawnRadius and with a random direction
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            Boid boid = Instantiate (prefab);
            boid.transform.position = pos;
            boid.transform.forward = Random.insideUnitSphere;
            boid.SetColour (colour);
            boidsCount++;
        }
    }

    /* MY CODE */
    void Update()
    {
        // generate boids while the space key is held and the mouse cursor is on a collider of the level
        if (Input.GetKey("space"))
        {
            // create a ray from the camera in the direction fo the mouse cursor
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            // cast the ray and check if it hits a collider
            if (Physics.Raycast(ray, out hit)){

                // instantiate a boid at the hit position + a little vertical offset
                Boid boid = Instantiate(prefab);
                Vector3 pos = new Vector3(hit.point.x, hit.point.y + 2, hit.point.z);
                boid.transform.position = pos;
                boid.transform.forward = Random.insideUnitSphere;
                boid.SetColour (Random.ColorHSV());

                // add the new boid to the BoidManager
                BoidManager manager = FindObjectOfType<BoidManager>();
                manager.AddBoid(boid);
                boidsCount++;
            }
        }
    }
}