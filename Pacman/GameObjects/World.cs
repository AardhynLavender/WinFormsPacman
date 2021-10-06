
//
//  TileMap Class
//  Created 22/09/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  Uses LINQ-XML to parse tilemap data combined with a tileset to create a collection
//  of Tile objects correctly arranged on the screen with the right texture and collision
//  infomation. Tilemaps of type .tmx were created using Tiled (https://www.mapeditor.org/).
//

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects
{
    class World : GameObject
    {
        // FIELDS

        private TileSet tileset;
        private int width;
        private int height;
        private int widthTiles;
        private int HeightTiles;

        private List<GameObject> tiles;

        // CONSTRUCTOR

        public World(PacManGame game, string tilemap, float x, float y, TileSet tileset)
            : base(x, y)
        {
            Game = game;

            this.tileset = tileset;

            XElement xTileset;

            // try to load data as XML elements
            try
            {
                xTileset = XElement.Load(tilemap);
            }
            catch (Exception err)
            {
                throw new Exception($"[Tilemap] > could not load file '{tilemap}' > {err.Message}");
            }

            // fetch elements

            if (int.TryParse((string)xTileset.Attribute("tilewidth"), out int size)
                && int.TryParse((string)xTileset.Attribute("width"), out int width))
            {
                // calculate width and widthTiles;
                widthTiles = width;
                width *= size;

                Console.WriteLine(width);

                // get map data
                string[] map = xTileset
                    .Element("layer")
                    .Element("data")
                    .Value.Split(',');

                // create game objects from map
                int i = 0;
                tiles = new List<GameObject>();
                Array.ForEach(map, tile =>
                {
                    int objX = i * size % width;
                    int objY = (int)Math.Floor((float)i++ / (width / size)) * size;

                    if (int.TryParse(tile, out int index))
                    {
                        if (index == 0)
                        {
                            tiles.Add(new GameObject(x + objX, y + objY));
                        }
                        else
                        {
                            tiles.Add(new GameObject(x + objX, y + objY, tileset.GetTileSourceRect(index - 1)));
                            tiles.Last().Wall = tileset.IsTileWall(index - 1);
                        }
                    }
                });
            }
        }

        // PROPERTIES



        // METHODS

        // adds each TileObject to the game
        public override void OnAddGameObject()
            => tiles.ForEach(tile => Game.AddGameObject(tile));

        // removes each TileObject from the game
        public override void OnFreeGameObject()
            => tiles.ForEach(tile => Game.QueueFree(tile));

        /// <summary>
        /// Gets the Tile Coordinate for the specified GameObject
        /// </summary>
        /// <param name="gameObject">The GameObject</param>
        /// <returns>Tile Coordinate</returns>
        public Vector2D GetTile(GameObject gameObject)
            => new Vector2D()
            {
                X = (float)Math.Floor((gameObject.X - x) / tileset.Size),
                Y = (float)Math.Floor((gameObject.Y - y)  / tileset.Size)
            };
        
        /// <summary>
        /// Gets the Tile for the provided absolute pixel coordainte
        /// </summary>
        /// <param name="x">x absolute pixel coordinate</param>
        /// <param name="y">y absolute pixel coordiante</param>
        /// <returns>tile coordiante</returns>
        public Vector2D GetTile(float x, float y)
            => new Vector2D()
            {
                X = (float)Math.Floor((x - this.x) / tileset.Size),
                Y = (float)Math.Floor((y - this.y) / tileset.Size)
            };

        /// <summary>
        /// Returns the TileObject at the specified tile coordinate
        /// </summary>
        /// <param name="tile">coordinate in tiles</param>
        /// <returns>TileObject at that coordinate</returns>
        public GameObject GetTileObject(Vector2D tile)
            => tiles[((int)tile.X % widthTiles) + widthTiles * (int)tile.Y];

        public void PlaceObject(GameObject gameObject, Vector2D tile)
        {
            gameObject.X = tile.X * tileset.Size;
            gameObject.Y = tile.Y * tileset.Size;
        }
    }
}
