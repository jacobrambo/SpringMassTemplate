using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Check this out we can require components be on a game object!
[RequireComponent(typeof(MeshFilter))]

public class BParticleSimMesh : MonoBehaviour
{
    public struct BSpring
    {
        public float kd;                        // damping coefficient
        public float ks;                        // spring coefficient
        public float restLength;                // rest length of this spring
        public int attachedParticle;            // index of the attached other particle (use me wisely to avoid doubling springs and sprign calculations)
    }

    public struct BContactSpring
    {
        public float kd;                        // damping coefficient
        public float ks;                        // spring coefficient
        public float restLength;                // rest length of this spring (think about this ... may not even be needed o_0
        public Vector3 attachPoint;             // the attached point on the contact surface
    }

    public struct BParticle
    {
        public Vector3 position;                // position information
        public Vector3 velocity;                // velocity information
        public float mass;                      // mass information
        public BContactSpring contactSpring;    // Special spring for contact forces
        public bool attachedToContact;          // is thi sparticle currently attached to a contact (ground plane contact)
        public List<BSpring> attachedSprings;   // all attached springs, as a list in case we want to modify later fast
        public Vector3 currentForces;           // accumulate forces here on each step        
    }

    public struct BPlane
    {
        public Vector3 position;                // plane position
        public Vector3 normal;                  // plane normal
    }

    public float contactSpringKS = 1000.0f;     // contact spring coefficient with default 1000
    public float contactSpringKD = 20.0f;       // contact spring daming coefficient with default 20

    public float defaultSpringKS = 100.0f;      // default spring coefficient with default 100
    public float defaultSpringKD = 1.0f;        // default spring daming coefficient with default 1

    public bool debugRender = false;            // To render or not to render


    /*** 
     * I've given you all of the above to get you started
     * Here you need to publicly provide the:
     * - the ground plane transform (Transform)
     * - handlePlaneCollisions flag (bool)
     * - particle mass (float)
     * - useGravity flag (bool)
     * - gravity value (Vector3)
     * Here you need to privately provide the:
     * - Mesh (Mesh)
     * - array of particles (BParticle[])
     * - the plane (BPlane)
     ***/

    public Transform groundPlaneTransform;   // the ground plane transform
    public bool handlePlaneCollisions = true; // handle plane collisions flag
    public float particleMass = 1.0f;        // particle mass
    public bool useGravity = true;           // use gravity flag
    public Vector3 gravity = new Vector3(0, -9.81f, 0); // gravity value
    private Mesh mesh;                      // the mesh
    private BParticle[] particles;          // array of particles
    private BPlane plane;             // the plane



    /// <summary>
    /// Init everything
    /// HINT: in particular you should probbaly handle the mesh, init all the particles, and the ground plane
    /// HINT 2: I'd for organization sake put the init particles and plane stuff in respective functions
    /// HINT 3: Note that mesh vertices when accessed from the mesh filter are in local coordinates.
    ///         This script will be on the object with the mesh filter, so you can use the functions
    ///         transform.TransformPoint and transform.InverseTransformPoint accordingly 
    ///         (you need to operate on world coordinates, and render in local)
    /// HINT 4: the idea here is to make a mathematical particle object for each vertex in the mesh, then connect
    ///         each particle to every other particle. Be careful not to double your springs! There is a simple
    ///         inner loop approach you can do such that you attached exactly one spring to each particle pair
    ///         on initialization. Then when updating you need to remember a particular trick about the spring forces
    ///         generated between particles. 
    /// </summary>
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        InitParticles();
        InitPlane();
    }

    void InitParticles()
    {
        Vector3[] vertsLocal = mesh.vertices;
        int count = vertsLocal.Length;
        particles = new BParticle[count];

        // 1️⃣ Create a particle per vertex
        for (int i = 0; i < count; i++)
        {
            BParticle p = new BParticle();
            p.position = transform.TransformPoint(vertsLocal[i]);   // local → world
            p.velocity = Vector3.zero;
            p.mass = particleMass;
            p.currentForces = Vector3.zero;
            p.attachedSprings = new List<BSpring>();
            p.attachedToContact = false;

            p.contactSpring.ks = contactSpringKS;
            p.contactSpring.kd = contactSpringKD;
            p.contactSpring.restLength = 0f;
            p.contactSpring.attachPoint = Vector3.zero;

            particles[i] = p;
        }

        // 2️⃣ Connect each unique pair with one spring
        for (int i = 0; i < count; i++)
        {
            for (int j = i + 1; j < count; j++)
            {
                float restLen = Vector3.Distance(particles[i].position, particles[j].position);
                BSpring s = new BSpring
                {
                    ks = defaultSpringKS,
                    kd = defaultSpringKD,
                    restLength = restLen,
                    attachedParticle = j
                };
                particles[i].attachedSprings.Add(s);
            }
        }

        Debug.Log($"Initialized {count} particles with {count * (count - 1) / 2} springs.");
    }

    void InitPlane()
    {
        // If a Transform is assigned, use its position and orientation
        if (groundPlaneTransform != null)
        {
            plane.position = groundPlaneTransform.position;
            plane.normal = groundPlaneTransform.up.normalized;
        }
        // Otherwise default to world-origin horizontal plane
        else
        {
            plane.position = Vector3.zero;
            plane.normal = Vector3.up;
        }
    }

    /*** BIG HINT: My solution code has as least the following functions
     * InitParticles()
     * InitPlane()
     * UpdateMesh() (remember the hint above regarding global and local coords)
     * ResetParticleForces()
     * ...
     ***/



    /// <summary>
    /// Draw a frame with some helper debug render code
    /// </summary>
    public void Update()
    {
        /* This will work if you have a correctly made particles array
        if (debugRender)
        {
            int particleCount = particles.Length;
            for (int i = 0; i < particleCount; i++)
            {
                Debug.DrawLine(particles[i].position, particles[i].position + particles[i].currentForces, Color.blue);

                int springCount = particles[i].attachedSprings.Count;
                for (int j = 0; j < springCount; j++)
                {
                    Debug.DrawLine(particles[i].position, particles[particles[i].attachedSprings[j].attachedParticle].position, Color.red);
                }
            }
        }
        */
    }
}
