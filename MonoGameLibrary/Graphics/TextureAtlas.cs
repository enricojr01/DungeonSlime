﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MonoGameLibrary.Graphics
{
    public class TextureAtlas
    {
        private Dictionary<string, TextureRegion> _regions;
        private Dictionary<string, Animation> _animations;
        public Texture2D Texture { get; set; }

        public TextureAtlas()
        {
            this._regions = new Dictionary<string, TextureRegion>();
            this._animations = new Dictionary<string, Animation>();
        }

        public TextureAtlas(Texture2D texture)
        {
            this.Texture = texture;
            this._regions = new Dictionary<string, TextureRegion>();
            this._animations = new Dictionary<string, Animation>();
        }

        public void AddRegion(string name, int x, int y, int width, int height)
        {
            TextureRegion region = new TextureRegion(Texture, x, y, width, height);
            this._regions.Add(name, region);
        }

        public TextureRegion GetRegion(string name)
        {
            return this._regions[name];
        }

        public bool RemoveRegion(string name)
        {
            return this._regions.Remove(name);
        }

        public void AddAnimation(string animationName, Animation animation)
        {
            this._animations.Add(animationName, animation);
        }

        public Animation GetAnimation(string animationName)
        {
            return _animations[animationName];
        }

        public bool RemoveAnimation(string animationName)
        {
            return this._animations.Remove(animationName);
        }

        public void Clear()
        {
            this._regions.Clear();
        }

        public static TextureAtlas FromFile(ContentManager content, string fileName) 
        {
            TextureAtlas atlas = new TextureAtlas();
            string filePath = Path.Combine(content.RootDirectory, fileName);

            using (Stream stream = TitleContainer.OpenStream(filePath))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    XDocument doc = XDocument.Load(reader);
                    XElement root = doc.Root;

                    string texturePath = root.Element("Texture").Value;
                    atlas.Texture = content.Load<Texture2D>(texturePath);

                    var regions = root.Element("Regions")?.Elements("Region");

                    if (regions != null)
                    {
                        foreach (var region in regions)
                        {
                            string name = region.Attribute("name")?.Value;
                            int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                            int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                            int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                            int height = int.Parse(region.Attribute("height")?.Value ?? "0");
                            
                            if (!string.IsNullOrEmpty(name))
                            {
                                atlas.AddRegion(name, x, y, width, height);
                            }
                        }
                    }

                    var animationElements = root.Element("Animations").Elements("Animation");

                    if (animationElements != null)
                    {
                        foreach (var animationElement in animationElements)
                        {
                            string name = animationElement.Attribute("name")?.Value;
                            float delayInMilliseconds = float.Parse(animationElement.Attribute("delay")?.Value ?? "0");
                            TimeSpan delay = TimeSpan.FromMilliseconds(delayInMilliseconds);

                            List<TextureRegion> frames = new List<TextureRegion>();

                            var frameElements = animationElement.Elements("Frame");

                            if (frameElements != null)
                            {
                                foreach (var frameElement in frameElements)
                                {
                                    string regionName = frameElement.Attribute("region").Value;
                                    TextureRegion region = atlas.GetRegion(regionName);
                                    frames.Add(region);
                                }
                            }
                            Animation animation = new Animation(frames, delay);
                            atlas.AddAnimation(name, animation);
                        }
                    }

                    return atlas;
                }
            }
        }

        public Sprite CreateSprite(string regionName)
        {
            TextureRegion region = this.GetRegion(regionName);
            return new Sprite(region);
        }

        public AnimatedSprite CreateAnimatedSprite(string animationName)
        {
            Animation animation = this.GetAnimation(animationName);
            return new AnimatedSprite(animation);
        }
    }
}
