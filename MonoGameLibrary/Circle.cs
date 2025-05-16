using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary
{
    public readonly struct Circle
    {
        public Circle(int x, int y, int radius)
        {
            this.X = x;
            this.Y = y;
            this.Radius = radius;
        }

        public Circle(Point location, int radius)
        {
            this.X = location.X;
            this.Y = location.Y;
            this.Radius = radius;
        }

        /// <summary>
        /// A reusable empty circle.
        /// </summary>
        private static readonly Circle s_empty = new Circle();

        /// <summary>
        /// The X-Coorinate of the center of the circle
        /// </summary>
        public readonly int X;

        /// <summary>
        /// The Y-Coorinate of the center of the circle
        /// </summary>
        public readonly int Y;

        /// <summary>
        /// The length, in pixels, from the center of this circle to the edge.
        /// </summary>
        public readonly int Radius;

        /// <summary>
        /// Gets the location of the center of this circle.
        /// </summary>
        public readonly Point Location => new Point(this.X, this.Y);
        /// <summary>
        /// Gets a circle with X=0, Y=0, and Radius=0;
        /// </summary>
        public static Circle Empty => s_empty;
        /// <summary>
        /// Gets a value that indicates whether this circle has a radius of 0 and a location of (0, 0)
        /// </summary>
        public readonly bool IsEmpty => this.X == 0 && this.Y == 0 && Radius == 0;

        /// <summary>
        /// Gets the y-coordinate of the highest point of this circle.
        /// </summary>
        public readonly int Top => Y - this.Radius;
        /// <summary>
        /// Gets the y-coordinate of the lowest point of this circle.
        /// </summary>
        public readonly int Bottom => Y + this.Radius;
        /// <summary>
        /// Gets the y-coordinate of the leftmost point of this circle.
        /// </summary>
        public readonly int Left => X - this.Radius;
        /// <summary>
        /// Gets the y-coordinate of the rightmost point of this circle.
        /// </summary>
        public readonly int Right => X + this.Radius;
   
        /// <summary>
        /// Returns a value that indicates whether the specified circle intersects with this circle.
        /// </summary>
        /// <param name="other">The other circle to check</param>
        /// <returns>returns true if other intersects with this; otherwise, false.</returns>
        public bool Intersects(Circle other)
        {
            int radiiSquared = (this.Radius + other.Radius) * (this.Radius + other.Radius);
            float distanceSquared = Vector2.DistanceSquared(this.Location.ToVector2(), other.Location.ToVector2());
            return distanceSquared < radiiSquared;
        }

        /// <summary>
        /// Returns a value that indicates whether this circle and the specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with this circle</param>
        /// <returns>true if this circle and the specified object are equal; otherwise false;</returns>
        public override readonly bool Equals(object obj) => obj is Circle other && Equals(other);

        /// <summary>
        /// Returns a value that indicates whether this circle and the specified object are equal.
        /// </summary>
        /// <param name="other">The other circle to compare with this circle.</param>
        /// <returns></returns>
        public readonly bool Equals(Circle other) => this.X == other.X && this.Y == other.Y && this.Radius == other.Radius;

        /// <summary>
        /// Returns the hash code for this circle.
        /// </summary>
        /// <returns>The hash code for this circle as a 32-bit signed integer.</returns>
        public override readonly int GetHashCode() => HashCode.Combine(X, Y, Radius);
    }
}
