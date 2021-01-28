using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {

    BoidSettings settings;

    // State
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    Vector3 velocity;

    // To update:
    Vector3 acceleration;
    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;
    [HideInInspector]
    public int numPerceivedFlockmates;

    // Cached
    Material material;
    Transform cachedTransform;
    Transform target;

    void Awake () {
        material = transform.GetComponentInChildren<MeshRenderer> ().material;
        cachedTransform = transform;
    }

    // Called by the BoidManager at the beginning of the simulation
    public void Initialize (BoidSettings settings, Transform target) {

        // target to follow if not null
        this.target = target;

        // settings of the boids
        this.settings = settings;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        // set the startSpeed of the boid to the mean of min and max speed
        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }

    public void SetColour (Color col) {
        if (material != null) {
            material.color = col;
        }
    }

    // Called by the Boid Manager at each frame to update the velocity and the direction of the boid
    public void UpdateBoid () {
        Vector3 acceleration = Vector3.zero;

        // If there is a target to follow, compute the acceleration toward this target according to the target weight
        if (target != null) {
            Vector3 offsetToTarget = (target.position - position);
            acceleration = SteerTowards (offsetToTarget) * settings.targetWeight;
        }

        // if the boid has at leats one other boid in its view range
        if (numPerceivedFlockmates != 0) {

            // compute the mean position of the boids in its view range
            centreOfFlockmates /= numPerceivedFlockmates;
            Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            /* compute the forces to be applied to the boid according to the settings weights
             * alignmentForce: move toward the global same direction of its neighbor boids
             * cohesionForce: move toward the mean position of its neighbors boids
             * seperationForce: move to avoid collisions with its neighbors boids
             */
            
            var alignmentForce = SteerTowards (avgFlockHeading) * settings.alignWeight;
            var cohesionForce = SteerTowards (offsetToFlockmatesCentre) * settings.cohesionWeight;
            var seperationForce = SteerTowards (avgAvoidanceHeading) * settings.seperateWeight;

            // add the computed forces to the acceleration
            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }

        // if the boid is about to collide an obstacle
        if (IsHeadingForCollision ()) {
            // select a direction to avoid the obstacle
            Vector3 collisionAvoidDir = ObstacleRays ();

            // compute the force to be applied to the boid and add it to the acceleration
            Vector3 collisionAvoidForce = SteerTowards (collisionAvoidDir) * settings.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }

        // set the velocity of the boid
        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp (speed, settings.minSpeed, settings.maxSpeed);
        velocity = dir * speed;

        // update the position and direction of the boid
        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
        forward = dir;
    }

    // Check if the boid is about to collide an obstacle
    bool IsHeadingForCollision () {
        RaycastHit hit;

        // Cast a sphere boundsRadius in the forward direction until the collision avoid distance and check if it hits an obstacle
        if (Physics.SphereCast (position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask)) {
            return true;
        } else { }
        return false;
    }

    // Return the first direction that avoid obstacles
    Vector3 ObstacleRays () {

        // get a spherical distribution of directions around the boid
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++) {
            Vector3 dir = cachedTransform.TransformDirection (rayDirections[i]);

            // for each direction, create a ray in this direction from the positon of the boid
            Ray ray = new Ray (position, dir);

            // cast a sphere along the ray and return the direction if the sphere does not hit any obstacle
            if (!Physics.SphereCast (ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask)) {
                return dir;
            }
        }

        // return forward if no valid direction has been found... RIP boid :'(
        return forward;
    }

    // Compute the force to be applied to the boid in the direction given by vector such as the resulting
    // force does not exceed the maxSteerForce
    Vector3 SteerTowards (Vector3 vector) {
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude (v, settings.maxSteerForce);
    }

}