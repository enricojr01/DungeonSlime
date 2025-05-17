using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DungeonSlime.GameObjects
{
    public class Bat
    {
        private const float MOVEMENT_SPEED = 5.0f;

        private Vector2 _velocity;
        private AnimatedSprite _sprite;
        private SoundEffect _bounceSoundEffect;

        public Vector2 Position { get; set; }

        public Bat(AnimatedSprite sprite, SoundEffect bounceSoundEffect)
        {
            this._sprite = sprite;
            this._bounceSoundEffect = bounceSoundEffect;
        }
        public void RandomizeVelocity()
        {
            // Generate a random angle;
            float angle = (float)(Random.Shared.NextDouble() * MathHelper.TwoPi);

            // Convert the angle to a direction vector;
            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);
            Vector2 direction = new Vector2(x, y);

            // Multiply the direction vector by the movement speed to get
            // the final velocity.
            this._velocity = direction * MOVEMENT_SPEED;
        }

        public void Bounce(Vector2 normal)
        {
            Vector2 newPosition = Position;

            // Adjust the position based on the normal to prevent sticking
            // on walls
            if(normal.X != 0)
            {
                // When bouncing off a vertical wall, move slightly away from the
                // wall in the direction of the normal
                newPosition.X += normal.X * (this._sprite.Width * 0.1f);
            }

            if(normal.Y != 0)
            {
                // When bouncing off a horizontal wall, move slightly away from the
                // wall in the direction of the normal.
                newPosition.Y += normal.Y * (this._sprite.Height * 0.1f);
            }

            // Apply the new position
            this.Position = newPosition;

            // Apply reflection based on the normal.
            this._velocity = Vector2.Reflect(this._velocity, normal);

            // Play the bounce sound effect
            Core.Audio.PlaySoundEffect(this._bounceSoundEffect);
        }
        
        /// <summary>
        /// Returns a circle value that represents the collision bounds of the bat.
        /// </summary>
        /// <returns>A Circle.</returns>
        public Circle GetBounds()
        {
            int x = (int)(Position.X + this._sprite.Width * 0.5f);
            int y = (int)(Position.Y + this._sprite.Height * 0.5f);
            int radius = (int)(this._sprite.Width * 0.25f);

            return new Circle(x, y, radius);
        }

        /// <summary>
        /// Updates the bat.
        /// </summary>
        /// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
        public void Update(GameTime gameTime)
        {
            // Update the animated sprite.
            this._sprite.Update(gameTime);

            // Update the position of the bat based on the velocity.
            this.Position += this._velocity;
        }

        /// <summary>
        /// Draws the bat.
        /// </summary>
        public void Draw()
        {
            this._sprite.Draw(Core.SpriteBatch, Position);
        }
    }
}
