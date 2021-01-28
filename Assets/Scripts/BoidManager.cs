using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour {

    const int threadGroupSize = 1024;

    public BoidSettings settings;
    public ComputeShader compute;
    List<Boid> boids;
    public Transform target;

    
    void Start () {
        // Find all the boids in the scene at the beginning of the simulation
        boids = new List<Boid>(FindObjectsOfType<Boid>());

        // Initialize all the boids 
        foreach (Boid b in boids) {
            b.Initialize (settings, target);
        }
    }

    // Update all the boids at each frame
    void Update () {
        if (boids != null && boids.Count != 0) {

            int numBoids = boids.Count;

            // create a list of data structures, one for each boid, to be fed to the compute shader
            var boidData = new BoidData[numBoids];

            // set the position and the direction of each boid in the data structure
            for (int i = 0; i < boids.Count; i++) {
                boidData[i].position = boids[i].position;
                boidData[i].direction = boids[i].forward;
            }

            // create a buffer to send information to the compute shader
            var boidBuffer = new ComputeBuffer (numBoids, BoidData.Size);

            boidBuffer.SetData (boidData);

            // set the variables of the compute shader
            compute.SetBuffer (0, "boids", boidBuffer);
            compute.SetInt ("numBoids", boids.Count);
            compute.SetFloat ("viewRadius", settings.perceptionRadius);
            compute.SetFloat ("avoidRadius", settings.avoidanceRadius);

            int threadGroups = Mathf.CeilToInt (numBoids / (float) threadGroupSize);
            compute.Dispatch (0, threadGroups, 1, 1);

            // get back the data computed by the shader
            boidBuffer.GetData (boidData);

            // update the attributes of each boid from the compute shader informations
            for (int i = 0; i < boids.Count; i++) {
                boids[i].avgFlockHeading = boidData[i].flockHeading;
                boids[i].centreOfFlockmates = boidData[i].flockCentre;
                boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

                // update the velocity and the direction of each boid
                boids[i].UpdateBoid ();
            }

            boidBuffer.Release ();
        }
    }

    public struct BoidData {
        public Vector3 position; // position of the boid
        public Vector3 direction; // direction of the boid

        public Vector3 flockHeading; // global direction of the neighbor boids
        public Vector3 flockCentre; // mean position of the neighbor boids
        public Vector3 avoidanceHeading; // global direction to avoid the neighbor boids
        public int numFlockmates; // number of neighbor boids

        public static int Size {
            get {
                return sizeof (float) * 3 * 5 + sizeof (int);
            }
        }
    }

    /* MY CODE */
    // Add a boid to the list and initialize it
    public void AddBoid(Boid b){
        this.boids.Add(b);
        b.Initialize(settings, target);
    }
}