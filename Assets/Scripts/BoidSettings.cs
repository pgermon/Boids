using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject {
    // Settings
    public float minSpeed = 2;
    public float maxSpeed = 5;
    
    public float perceptionRadius = 2.5f; // radius in which the boid can sense its neighbors
    public float avoidanceRadius = 1; // threshold radius to avoid other boids
    public float maxSteerForce = 3; // max force that can't be exceeded by streeing forces applied to the boid

    [Header ("Steering weights")]
    public float alignWeight = 1; // weight for global flock heading 
    public float cohesionWeight = 1; // weight for global flock cohesion
    public float seperateWeight = 1; // weight for avoidance of other boids

    public float targetWeight = 1; // weight for target attraction

    [Header ("Collisions")]
    public LayerMask obstacleMask; // layers of the colliders to avoid
    public float boundsRadius = .27f; // radius of the boid
    public float avoidCollisionWeight = 10; // weigth for avoidance of obstacles
    public float collisionAvoidDst = 5; // max distance to avoid obstacles

}