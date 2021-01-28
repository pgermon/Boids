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
        if (Input.GetKey("space"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //Debug.Log("space down");
            if (Physics.Raycast(ray, out hit/*, LayerMask.NameToLayer("Wall")*/)){
                //Debug.Log("instantiate boid");
                Boid boid = Instantiate(prefab);
                Vector3 pos = new Vector3(hit.point.x, hit.point.y + 2, hit.point.z);
                boid.transform.position = pos;
                boid.transform.forward = Random.insideUnitSphere;
                boid.SetColour (Random.ColorHSV());
                BoidManager manager = FindObjectOfType<BoidManager>();
                manager.AddBoid(boid);
                boidsCount++;
            }
        }
    }
}