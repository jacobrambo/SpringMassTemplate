# Assignment2Part2 – Softbody Simulation System  
**Prepared by:** Jacob Rambo  

This README outlines the work completed for **Assignment 2 – Part 2 (Undergraduate Section)**.  
The goal of this project was to implement a complete **softbody spring-mass physics simulator** in Unity using only mathematical and scripting logic, without relying on Unity’s built-in physics engine. The system simulates deformable 3D meshes using particles connected by springs, with support for gravity, ground contact handling, damping, and Symplectic Euler integration.

## Overview
All functionality was implemented within the **BParticleSimMesh.cs** script.  
This class generates a particle system directly from a mesh, connects particles with springs, simulates all forces (spring, damping, gravity, and ground penalty), and updates the mesh in real time to visualize deformation.

Three cube meshes were tested with different stiffness and damping configurations to demonstrate distinct softbody behaviors.

## ParticleInitialization (2 Marks)
- Each vertex in the mesh is automatically converted into a **BParticle**.  
- Particles are initialized in **world space** using `transform.TransformPoint()`.  
- Each particle stores position, velocity, forces, and a list of attached springs.  
- Default mass and gravity properties are configurable via the Unity Inspector.  
- The simulation supports toggling gravity and adjusting its vector direction.

## SpringConfiguration (3 Marks)
- Every unique pair of particles is connected by one **BSpring**.  
- Each spring stores its stiffness (`ks`), damping (`kd`), and rest length.  
- Rest lengths are calculated from the initial mesh geometry in world space.  
- Duplicates are avoided using an inner loop that connects only `i + 1 ... count - 1`.  
- Each particle maintains a list of attached springs for efficient force computation.

## GroundPlaneInitialization (1 Mark)
- The ground plane is defined using a **Transform** reference in the Inspector.  
- If none is provided, the system defaults to a flat plane at world origin (Y = 0) with an upward normal.  
- Plane data is stored in a **BPlane** struct containing position and normal vectors.

## GroundContactPenaltyForces (3 Marks)
- Implemented penalty-based collision response with the ground.  
- For any particle below the plane (negative distance), a spring-like restoring force and damping force are applied.  
- Forces are proportional to penetration depth and velocity into the plane.  
- Each particle stores a **BContactSpring** to manage its collision response and flag for active contact.

## SpringForceComputation (2 Marks)
- Hooke’s Law and damping are applied to all particle pairs connected by springs.  
- Each spring applies equal and opposite forces to its connected particles, preventing redundant calculations.  
- Forces depend on extension/compression (`dist - restLength`) and relative velocity along the spring’s axis.  
- This maintains physical stability and energy conservation within the system.

## Gravity (Implemented via ApplyGravity)
- Gravity applies a constant force to each particle proportional to its mass.  
- Controlled by a public flag (`useGravity`) and adjustable via the `gravity` vector field.

## ForceResetAndIntegration (2 Marks)
- Before every frame, forces are reset to zero to prevent accumulation.  
- All forces (spring, damping, gravity, ground) are then computed in order.  
- Symplectic Euler integration updates each particle’s velocity and position.  
- Integration is time-step consistent through Unity’s `FixedUpdate()`.

## MeshUpdate (2 Marks)
- After each physics step, new particle positions are transformed back to **local space** and written into the mesh vertex array.  
- Mesh bounds and normals are recalculated to correctly display deformation and shading changes.

## SimulatorLoop (3 Marks)
The main simulation loop executes in **FixedUpdate()** to ensure stable physics timing.  
Each iteration performs:
1. `ResetParticleForces()`  
2. `ApplyGravity()`  
3. `ApplySpringForces()`  
4. `ApplyPlanePenaltyForces()`  
5. `Integrate()`  
6. `UpdateMesh()`  

This structure guarantees deterministic updates, consistent integration, and visually accurate softbody motion.

## DebugRendering
A debug visualization was added to display:
- Blue lines for active force vectors  
- Red lines for spring connections between particles  

## TestcaseCubes (3 Marks)
Three softbody cubes were set up with different stiffness and damping parameters to observe physical variation:  
- **Blue Cube:** Very soft (low ks, high damping)  
- **Red Cube:** Balanced (medium ks and kd)  
- **Green Cube:** Stiff and reactive (high ks, low damping)  

Each cube’s behavior visually confirms proper spring and collision dynamics.

## FileSubmission
- Project files follow Unity’s **.gitignore** structure to exclude caches and temporary files.  
- Submitted archive: **rambo-jacob-a2-p2.zip**  
- Includes all scripts, meshes, and scene configurations required to run the simulation.

## Summary
All required elements of **Assignment 2 Part 2** have been successfully implemented:  
- Particle creation and spring connectivity  
- Ground collision detection and penalty response  
- Hooke’s Law and damping with reflected forces  
- Symplectic Euler integration for time-stable updates  
- Real-time mesh deformation with correct bounds and normals  
- Gravity control and debug rendering  

The result is a functional softbody simulation system demonstrating realistic deformation and stable physics behavior within Unity.

**End of README – Assignment 2 Part 2 (Softbody Simulation System)**
