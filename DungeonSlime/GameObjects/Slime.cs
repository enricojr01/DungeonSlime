using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DungeonSlime.GameObjects
{
    public class Slime
    {
        /// <summary>
        /// A constant value that represents the amount of time to wait between
        /// movement updates
        /// </summary>
        private static readonly TimeSpan s_movementTime = TimeSpan.FromMilliseconds(200);

        /// <summary>
        /// The amount of time that has elapsed since the last movement update
        /// </summary>
        private TimeSpan _movementTimer;
        /// <summary>
        /// Normalized value (0 - 1) representing progress between movement
        /// ticks for visual interpolation
        /// </summary>
        private float _movementProgress;

        /// <summary>
        /// The next direction to apply to the head of the slime chain during
        /// the next movement update.
        /// </summary>
        private Vector2 _nextDirection;

        /// <summary>
        /// The number of pixels to move the head segment
        /// during the movement cycle.
        /// </summary>
        private float _stride;

        /// <summary>
        /// Tracks the segments of the slime chain.
        /// </summary>
        private List<SlimeSegment> _segments;

        /// <summary>
        /// The AnimatedSprite used when drawing each slime segment.
        /// </summary>
        private AnimatedSprite _sprite;

        private Queue<Vector2> _inputBuffer;
        private const int MAX_BUFFER_SIZE = 2;

        public event EventHandler BodyCollision;

        public Slime(AnimatedSprite sprite)
        {
            this._sprite = sprite;
        }

        public void Initialize(Vector2 startingPosition, float stride)
        {
            // init segment collection
            this._segments = new List<SlimeSegment>();
            // set stride value
            this._stride = stride;

            // Create the head of the chain
            SlimeSegment head = new SlimeSegment();
            head.At = startingPosition;
            head.To = startingPosition + new Vector2(this._stride, 0);
            head.Direction = Vector2.UnitX;

            // add the head to the segments
            this._segments.Add(head);
            // Set the next direction as the same direction the
            // head is moving.
            this._nextDirection = head.Direction;
            // Zero out the movement timer
            this._movementTimer = TimeSpan.Zero;

            // init the input buffer
            this._inputBuffer = new Queue<Vector2>(MAX_BUFFER_SIZE);
        }

        public void HandleInput()
        {
            Vector2 potentialNextDirection = Vector2.Zero;

            if (GameController.MoveUp())
            {
                potentialNextDirection = -Vector2.UnitY;
            }
            else if (GameController.MoveDown())
            {
                potentialNextDirection = Vector2.UnitY;
            }
            else if (GameController.MoveLeft())
            {
                potentialNextDirection = -Vector2.UnitX;
            }
            else if (GameController.MoveRight())
            {
                potentialNextDirection = Vector2.UnitX;
            }

            // If a new direction was input, consider adding it to the buffer
            if (potentialNextDirection != Vector2.Zero && this._inputBuffer.Count < MAX_BUFFER_SIZE) 
            {
                // If the input buffer is empty, validate against the current direction,
                // otherwise validate against the last buffered direction
                Vector2 validateAgainst = 
                    this._inputBuffer.Count > 0 
                    ? this._inputBuffer.Last() 
                    : this._segments[0].Direction;

                // Only allow direction change if it is not reversing the current
                // direction. This prevents the slime from backing into itself.
                float dot = Vector2.Dot(potentialNextDirection, validateAgainst);
                if (dot >= 0)
                {
                    this._inputBuffer.Enqueue(potentialNextDirection);
                }
            }
        }

        private void Move()
        {
            if (_inputBuffer.Count > 0)
            {
                this._nextDirection = this._inputBuffer.Dequeue();
            }

            // Capture the value of the head segment
            SlimeSegment head = this._segments[0];

            // Update the direction the head is supposed to move in
            // to the next direction cached
            head.Direction = this._nextDirection;
            
            // update the head's "at" position" to be where it was moving "to";
            head.At = head.To;

            // Update the head's "to" position to the next tile in the direction
            // it's moving.
            head.To = head.At + head.Direction * this._stride;
            
            // Inserts the new adjusted value for the head at the front
            // of the segment, and remove the tail segment.
            // This moves the entire chain forward without having to
            // change each individual segment
            this._segments.Insert(0, head);
            this._segments.RemoveAt(this._segments.Count - 1);
            
            // Check each segement except the head and check if they
            // are at the same position as the head.
            // this is how we check for body collisions
            for (int i = 1; i < this._segments.Count; i++)
            {
                SlimeSegment segment = this._segments[i];

                if (head.At == segment.At)
                {
                    if (BodyCollision != null)
                    {
                        BodyCollision.Invoke(this, EventArgs.Empty);
                    }

                    return;
                }
            }
        }

        public void Grow()
        {
            SlimeSegment tail = this._segments[this._segments.Count - 1];
            SlimeSegment newTail = new SlimeSegment();
            newTail.At = tail.To + tail.ReverseDirection * this._stride;
            newTail.To = tail.At;
            newTail.Direction = Vector2.Normalize(tail.At - newTail.At);

            this._segments.Add(newTail);
        }

        /// <summary>
        /// Updates the slime;
        /// </summary>
        /// <param name="gameTime">A snapshot of timing values for the current update cycle.</param>
        public void Update(GameTime gameTime)
        {
            // Update the animated sprite.
            this._sprite.Update(gameTime);

            // Handle player input;
            HandleInput(); 

            // Increment the movement timer by the frame elapsed time.
            this._movementTimer += gameTime.ElapsedGameTime;

            // If the movement timer has accumulated enough time to be greater than the movement
            // time threshold, the perform a full movement.
            if (_movementTimer >= s_movementTime)
            {
                this._movementTimer -= s_movementTime;
                Move();
            }

            // Update the movement offset amount
            this._movementProgress = (float)(this._movementTimer.TotalSeconds / s_movementTime.TotalSeconds);
        }

        /// <summary>
        /// Draws the slime.
        /// </summary>
        public void Draw()
        {
            // Iterate through each segment and draw it
            foreach (SlimeSegment segment in _segments)
            {
                // Calculate the visual position of the segment at the moment by
                // lerping between the "at" and "to" position by the movement
                // offset lerp amount.
                Vector2 pos = Vector2.Lerp(segment.At, segment.To, this._movementProgress);
                this._sprite.Draw(Core.SpriteBatch, pos);
            }
        }
        
        /// <summary>
        /// Returns a circle that represents the bounding box for this object.
        /// </summary>
        /// <returns></returns>
        public Circle GetBounds()
        {
            SlimeSegment head = this._segments[0];
            Vector2 pos = Vector2.Lerp(head.At, head.To, this._movementProgress);
            Circle bounds = new Circle(
                (int)(pos.X + (this._sprite.Width * 0.5f)),
                (int)(pos.Y + (this._sprite.Height * 0.5f)),
                (int)(this._sprite.Width * 0.5f)
            );

            return bounds;
        }
    }
}
