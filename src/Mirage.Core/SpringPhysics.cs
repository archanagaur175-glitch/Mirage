using System;

namespace Mirage.Core
{
    /// <summary>
    /// Spring-physics magnification model for the macOS-style Dock. Items scale up
    /// as the cursor approaches, using a damped harmonic oscillator (Hooke's law)
    /// integrated per frame so motion feels elastic rather than linear.
    ///
    /// Pure, framework-independent math so it can be unit-tested headlessly.
    /// </summary>
    public static class SpringPhysics
    {
        /// <summary>
        /// Target magnification for an item at <paramref name="distance"/> pixels from
        /// the cursor, peaking at <paramref name="maxScale"/> and falling off over
        /// <paramref name="influenceRadius"/>. Returns 1.0 (base size) beyond the radius.
        /// </summary>
        public static double TargetScale(double distance, double influenceRadius, double maxScale)
        {
            if (influenceRadius <= 0)
            {
                return 1.0;
            }

            double d = Math.Abs(distance);
            if (d >= influenceRadius)
            {
                return 1.0;
            }

            // Smooth bell curve -> feels more "macOS" than a hard linear ramp.
            double t = 1.0 - (d / influenceRadius);
            double bell = t * t * (3.0 - 2.0 * t); // smoothstep
            return 1.0 + (maxScale - 1.0) * bell;
        }

        /// <summary>
        /// A single damped-spring state integrated toward a target each frame.
        /// </summary>
        public sealed class Spring
        {
            public double Position { get; private set; } = 1.0;
            public double Velocity { get; private set; }

            public double Step(double target, double stiffness, double damping, double dt)
            {
                // F = -k(x - target) - c*v
                double force = stiffness * (target - Position) - damping * Velocity;
                Velocity += force * dt;
                Position += Velocity * dt;
                return Position;
            }

            public void Settle(double value)
            {
                Position = value;
                Velocity = 0;
            }
        }
    }
}
