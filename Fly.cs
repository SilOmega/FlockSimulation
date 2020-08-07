using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour
{

    public Particle particle;
    private Transform target;
    private float spawnRadius;
    public float separationWeight= 1f;
    public float cohesionWeight = 1f;
    public float avoidanceWeight = 0.3f;
    public float alignmentWeight = 1f;
    public Vector3 wind = new Vector3(0, 0, 0);

    // Fly component that fully initializes the called GameObject with all parameters needed to simulate flock
    public static Fly CreateComponent(GameObject gmObj, float particleSize, float percepRadius, Transform tg, 
                                        float spawnSize, Color color, float maxSpeed, float particleMass, Vector3 windForce)
    {
        Fly bird = gmObj.AddComponent<Fly>();
        bird.target = tg;
        Vector3 pos = Random.insideUnitSphere * spawnSize + bird.target.transform.position;
        bird.particle = new Particle(pos, new Vector3(0, 0, 0), particleSize, particleMass, color, 0, maxSpeed);   
        bird.spawnRadius = spawnSize;
        bird.tag = "Bird";
        bird.gameObject.transform.position = bird.particle.Position;
        gmObj.AddComponent<SphereCollider>();
        gmObj.GetComponent<SphereCollider>().radius = 3 * particleSize;
        gmObj.AddComponent<Rigidbody>();
        gmObj.GetComponent<Rigidbody>().useGravity = false;
        return bird;
    }

    public void Start()
    {

    }

    
    void FixedUpdate()
    {
        Vector3 force = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        Vector3 move = (target.transform.position - particle.Position) * particle.Mass;
        Vector3 cohesionForce = new Vector3(0, 0, 0);
        Vector3 alignmentForce = new Vector3(0, 0, 0);
        Collider[] col = Physics.OverlapSphere(transform.position, 10 * particle.Size);
        
        // Update cohesion and alignment forces in order to maintain the bird coherent with the group locally and globally
        if (col.Length > 5)
        {
            float counter = 0;
            for (int i = 0; i < 5; i++) {
                if(col[i].gameObject.tag == "Bird")
                {
                    cohesionForce += col[i].transform.position;
                    alignmentForce += (col[i].GetComponent<Fly>().particle.Velocity - this.particle.Velocity) * 
                                      particle.Mass / Time.fixedDeltaTime;
                    counter++;
                }
                
            }
            if (counter > 0) 
                cohesionForce = (cohesionForce / counter - transform.position) * particle.Mass / Mathf.Pow(Time.fixedDeltaTime, 2); 
        }
        if (col.Length > 0 && col.Length < 5)
        {
            float counter = 0;
            for (int i = 0; i < col.Length; i++) {
                if(col[i].gameObject.tag == "Bird")
                {
                    cohesionForce += col[i].transform.position;
                    alignmentForce += (col[i].GetComponent<Fly>().particle.Velocity - this.particle.Velocity) * 
                                      particle.Mass / Time.fixedDeltaTime;
                    counter++;
                }
                
            }
            if (counter > 0)
                cohesionForce = (cohesionForce / counter - transform.position);// * particle.Mass / Mathf.Pow(Time.fixedDeltaTime, 2);
        }

        particle.Simulate(force + move * 0.7f + cohesionForce*cohesionWeight + alignmentForce * alignmentWeight);
        transform.position = particle.Position;
    }

    // Draws the particle on screen
    void OnRenderObject()
    {
        particle.Draw(particle.Size, particle.Col);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        /*
        Rigidbody rigb = gameObject.GetComponent<Rigidbody>();
        rigb.velocity = Vector3.zero;
        rigb.rotation = Quaternion.identity;
        */
        transform.rotation = Quaternion.identity;
        Rigidbody rigb = gameObject.GetComponent<Rigidbody>();
        rigb.velocity = Vector3.zero;
        rigb.rotation = Quaternion.identity;
        Vector3 separationForce = Vector3.zero;
        Vector3 avoidanceForce = Vector3.zero;
        Vector3 distance = this.transform.position - collision.transform.position;
        ContactPoint[] contact = new ContactPoint[1];
        collision.GetContacts(contact);

        // Reacts to obstacles or other birds avoiding them before an actual collision happens
        if ( collision.gameObject.tag == "Bird")
        {
            // Amplifies the separation force in order to avoid the bird
            separationForce = distance / Mathf.Pow(distance.magnitude, 2) * particle.Mass / Mathf.Pow(Time.fixedDeltaTime, 2);

        } else
        {
            Debug.Log("obstacle");
            // Amplifies the avoidance force to avoid the obstacle
            avoidanceForce = (contact[0].normal / distance.magnitude) * particle.Mass / Mathf.Pow(Time.fixedDeltaTime, 2);
        }
    
        particle.Simulate(separationForce * separationWeight + avoidanceForce*avoidanceWeight);
        this.transform.position = particle.Position;
    }

}
