using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle
{
    public Vector3 Position;
    public Vector3 Velocity;
    public float Size;
    public Color Col;
    public float Mass = 100f;
    public float MinVelocity = 0f;
    public float MaxVelocity = float.MaxValue;

    public Particle() {
        
    }

    public Particle(Vector3 pos, Vector3 vel, float size, float mass, Color color, float minVel, float maxVel) {
        Position = pos;
        Velocity = vel;
        Size = size;
        Mass = mass;
        Col = color;
        MinVelocity = minVel;
        MaxVelocity = maxVel;
    }

    public void Simulate(Vector3 force) {
        //F = m*a
        //V' = V + a*dt
        //P' = P + V'
        Vector3 a = force / Mass;
        Velocity += a * Time.fixedDeltaTime;
        Velocity = Mathf.Clamp(Velocity.magnitude, MinVelocity, MaxVelocity) * Velocity.normalized;
        Position += Velocity;
    }

    public void Draw(float size, Color color) {
        UltiDraw.Begin();
        UltiDraw.DrawSphere(Position, Quaternion.identity, size, color);
        UltiDraw.End();
    }
}
