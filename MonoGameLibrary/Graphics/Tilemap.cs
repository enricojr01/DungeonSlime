using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics
{
    public class Tilemap
    {
        private readonly Tileset _tileset;
        private readonly int[] _tiles;

        /// <summary>
        /// Gets the total number of rows in this tilemap.
        /// </summary>
        public int Rows { get; }

        /// <summary>
        /// Gets the total number of columns in this tilemap.
        /// </summary>
        public int Columns { get; }

        /// <summary>
        /// Gets the total number of tiles in this tilemap.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the total number of tiles in this tilemap.
        /// </summary>
        public Vector2 Scale { get; set; }
        
        /// <summary>
        /// Gets or Sets the scale factor to draw each tile at.
        /// </summary>
        public float TileWidth => this._tileset.TileWidth * Scale.X;
           
        /// <summary>
        /// Gets the width, in pixels, each tile is drawn at.
        /// </summary>
        public float TileHeight => this._tileset.TileHeight * Scale.Y;

        public Tilemap(Tileset tileset, int columns, int rows)
        {
            this._tileset = tileset;
            this.Rows = rows;
            this.Columns = columns;
            this.Count = this.Columns * this.Rows;
            this.Scale = Vector2.One;
            this._tiles = new int[this.Count];
        }

        public void SetTile(int index, int tilesetID)
        {
            this._tiles[index] = tilesetID;
        }

        public void SetTile(int column, int row, int tilesetID)
        {
            int index = row * this.Columns + column;
            SetTile(index, tilesetID);
        }

        public TextureRegion GetTile(int index)
        {
            return this._tileset.GetTile(this._tiles[index]);
        }

        public TextureRegion GetTile(int column, int row)
        {
            int index = row * this.Columns + column;
            return this.GetTile(index);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < this.Count; i++)
            {
                int tileSetIndex = this._tiles[i];
                TextureRegion tile = this._tileset.GetTile(tileSetIndex);

                int x = i % this.Columns;
                int y = i / this.Columns;

                Vector2 position = new Vector2(x * TileWidth, y * TileHeight);
                tile.Draw(spriteBatch, position, Color.White, 0.0f, Vector2.Zero, this.Scale, SpriteEffects.None, 1.0f);
            }
        }
        
        /// <summary>
        /// Creates a new tilemap based on a tilemap xml configuration file.
        /// </summary>
        /// <param name="content">The content manager used to load the texture for the tileset.</param>
        /// <param name="filename">The path to the xml file, relative to the content root directory.</param>
        /// <returns>The tilemap created by this method.</returns>
        public static Tilemap FromFile(ContentManager content, string filename)
        {
            string filePath = Path.Combine(content.RootDirectory, filename);

            using (Stream stream = TitleContainer.OpenStream(filePath))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    XDocument doc = XDocument.Load(reader);
                    XElement root = doc.Root;
                    // Data about the tileset is stored in an xml element, called
                    // <Tileset>.
                    // 
                    // Example:
                    // <Tileset region="0 0 100 100" tileWidth="10" tileHeight="10">contentPath</Tileset>
                    //
                    // The 'region' attribute stores x, y, width, and height, and
                    // can be passed to the TextureRegion constructor.
                    // 
                    // The tileWidth and tileHeight attributes specify the width and height
                    // of each tile in the tileset.
                    //
                    // the contentPath value is the contentPath to the texture to load that
                    // contains the tileset.
                    XElement tilesetElement = root.Element("Tileset");

                    string regionAttribute = tilesetElement.Attribute("region").Value;

                    string[] split = regionAttribute.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    int x = int.Parse(split[0]);
                    int y = int.Parse(split[1]);
                    int width = int.Parse(split[2]);
                    int height = int.Parse(split[3]);

                    int tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
                    int tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
                    string contentPath = tilesetElement.Value;

                    Texture2D texture = content.Load<Texture2D>(contentPath);

                    TextureRegion textureRegion = new TextureRegion(texture, x, y, width, height);

                    Tileset tileset = new Tileset(textureRegion, tileWidth, tileHeight);

                    // Data about each individual tile in a Tileset is stored in the
                    // <Tiles> element. Each <Tiles> element contains lines of strings
                    // where each line represents a row in a tilemap.
                    // Example:
                    // <Tiles>
                    //     00 01 01 02
                    //     03 04 04 05
                    //     03 04 04 05
                    //     06 07 07 08
                    // </Tiles>
                    XElement tilesElement = root.Element("Tiles");
                    // NOTE: Trim() (probably) removes the leading / trailing spaces
                    //       Split() splits the string into lines along newlines;
                    // NOTE: The goal here is to get the # of rows.
                    string[] rows = tilesElement.Value.Trim().Split("\n", StringSplitOptions.RemoveEmptyEntries);

                    // NOTE: The Split() here grabs the individual values.
                    // NOTE: The goal here is to get the # of columns, by splitting the first row 
                    int columnCount = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;
                    Tilemap tilemap = new Tilemap(tileset, columnCount, rows.Length);

                    for (int row = 0; row < rows.Length; row++)
                    {
                        string[] columns = rows[row].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        for (int column = 0; column < columnCount; column++)
                        {
                            int tilesetIndex = int.Parse(columns[column]);
                            TextureRegion region = tileset.GetTile(tilesetIndex);
                            tilemap.SetTile(column, row, tilesetIndex);
                        }
                    }
                    return tilemap;
                }
            }
        }
    }
}
