using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameLibrary.Graphics
{
    public class AnimatedSprite : Sprite
    {
        private int _currentFrame;
        private TimeSpan _elapsed;
        private Animation _animation;

        public Animation Animation
        {
            get => this._animation;
            set
            {
                this._animation = value;
                this.Region = _animation.Frames[0];
            }
        }

        public AnimatedSprite() {}
        public AnimatedSprite(Animation animation)
        {
            this.Animation = animation;
        }

        public void Update(GameTime gameTime)
        {
            this._elapsed += gameTime.ElapsedGameTime;

            if (this._elapsed >= this._animation.Delay)
            {
                this._elapsed -= _animation.Delay;
                this._currentFrame++;

                if (this._currentFrame >= this._animation.Frames.Count)
                {
                    this._currentFrame = 0;
                }

                this.Region = this._animation.Frames[this._currentFrame];
            }
        }
    }
}
