using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameLibrary.Graphics
{
    public class Tileset
    {
        private readonly TextureRegion[] _tiles;

        /// <summary>
        /// Gets the width, in pixels, of each tile in the tileset.
        /// </summary>
        public int TileHeight { get; }

        /// <summary>
        /// Gets the width, in pixels, of each tile in the tileset.
        /// </summary>
        public int TileWidth { get; }

        /// <summary>
        /// Gets the total number of columns in the tileset.
        /// </summary>
        public int Columns { get; }

        /// <summary>
        /// Gets the total number of rows in the tileset.
        /// </summary>
        public int Rows { get; }

        /// <summary>
        /// Gets the total number of tiles in the tileset.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Creates a new tileset based on the given texture region with the specified tile width and height.
        /// </summary>
        /// <param name="textureRegion">The texture region that contains the tiles for the tileset.</param>
        /// <param name="tileWidth">The width of each tile in the tileset.</param>
        /// <param name="tileHeight">The height of each tile in the tileset.</param>
        public Tileset(TextureRegion textureRegion, int tileWidth, int tileHeight)
        {
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Columns = textureRegion.Width / tileWidth;
            this.Rows = textureRegion.Height / tileHeight;
            this.Count = this.Columns * this.Rows;

            this._tiles = new TextureRegion[Count];

            for (int i = 0; i < this.Count; i++)
            {
                int x = i % this.Columns * tileWidth;
                int y = i / this.Columns * tileHeight;
                this._tiles[i] = new TextureRegion(
                    textureRegion.Texture,
                    textureRegion.SourceRectangle.X + x,
                    textureRegion.SourceRectangle.Y + y,
                    tileWidth,
                    tileHeight
                );
            }
        }

        /// <summary>
        /// Gets the texture region for the tile from this tileset at the given index. 
        /// </summary>
        /// <param name="index">The index of the texture region in the tile set.</param>
        /// <returns>The texture region for the tile from this tileset at the given index.</returns>
        public TextureRegion GetTile(int index) => this._tiles[index];
        
        /// <summary>
        /// Gets the texture region for the tile from this tileset at the given location.
        /// </summary>
        /// <param name="column">The column in this tileset of the texture region.</param>
        /// <param name="row">The row in this tileset of the texture region.</param>
        /// <returns>The texture region for the tile from this tileset at the given location.</returns>
        public TextureRegion GetTile(int column, int row)
        {
            int index = row * Columns + column;
            return this.GetTile(index);
        }
    }
}
