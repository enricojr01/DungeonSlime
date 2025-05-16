﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameLibrary.Graphics
{
    public class TextureRegion
    {
        public Texture2D Texture { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public int Width => SourceRectangle.Width;
        public int Height => SourceRectangle.Height;

        public float TopTextureCoordinate => SourceRectangle.Top / (float)Texture.Height;
        public float BottomTextureCoordinate => SourceRectangle.Bottom / (float)Texture.Height;
        public float LeftTextureCoordinate => SourceRectangle.Left / (float)Texture.Width;
        public float RightTextureCoordinate => SourceRectangle.Right / (float)Texture.Width;

        public TextureRegion(Texture2D texture, int x, int y, int width, int height)
        {
            this.Texture = texture;
            this.SourceRectangle = new Rectangle(x, y, width, height);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color) 
        {
            this.Draw(spriteBatch, position, color, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            this.Draw(
                spriteBatch,
                position,
                color,
                rotation,
                origin,
                new Vector2(scale, scale),
                effects,
                layerDepth
            );
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            spriteBatch.Draw(
                Texture,
                position,
                this.SourceRectangle,
                color,
                rotation,
                origin,
                scale,
                effects,
                layerDepth
            );
        }
    }
}
