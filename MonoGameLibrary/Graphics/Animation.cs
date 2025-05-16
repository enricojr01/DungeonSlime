using System;
using System.Collections.Generic;

namespace MonoGameLibrary.Graphics
{
    public class Animation
    {
        public List<TextureRegion> Frames { get; set; }
        public TimeSpan Delay { get; set; }

        public Animation()
        {
            this.Frames = new List<TextureRegion>();
            this.Delay = TimeSpan.FromMilliseconds(100);
        }

        public Animation(List<TextureRegion> frames, TimeSpan delay)
        {
            this.Frames = frames;
            this.Delay = delay;
        }
    }
}
