
//
//  World : Game Object Class
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

using FormsPixelGameEngine.Utility;
using FormsPixelGameEngine.GameObjects.Tiles;
using FormsPixelGameEngine.GameObjects.Sprites.Ghosts;
using System.Linq;

namespace FormsPixelGameEngine.GameObjects
{
    class World : GameObject
    {
        // CONSTANTS

        private const int PELLET = 22;
        private const int ENERGIZER = 23;

        // FIELDS

        private int widthTiles;
        private List<TileObject> tiles;
        private List<Ghost> ghosts;

        // CONSTRUCTOR

        public World(string tilemap, float x, float y)
            : base(x, y)
        {
            // Create world from tilemap

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
                this.width = width * size;

                // get map data
                string[] map = xTileset
                    .Element("layer")
                    .Element("data")
                    .Value.Split(',');

                // create game objects from map
                int i = 0;
                tiles = new List<TileObject>();
                Array.ForEach(map, tile =>
                {
                    if (int.TryParse(tile, out int index))
                    {
                        if (--index == -1)
                            AddTile(new TileObject(i, this, -1));

                        else if (index == PELLET)
                            AddTile(new Pellet(i, this));

                        else if (index == ENERGIZER)
                            AddTile(new Energizer(i, this));

                        else
                            AddTile(new TileObject(i, this, index))
                            .Wall = tileset.IsTileWall(index);
                    }
                    i++;
                });
            }
        }

        // PROPERTIES

        public int WidthTiles 
            => widthTiles;

        public List<Ghost> Ghosts
        { set => ghosts = value; }

        // METHODS

        // removes each TileObject from the game
        public override void OnFreeGameObject()
            => tiles.ForEach(tile => game.QueueFree(tile));

        // pushes a new tile to the worlds tile vector
        public TileObject AddTile(TileObject tileObject)
        {
            tiles.Add(tileObject);
            return (TileObject)game.AddGameObject(tileObject);
        }

        // replaces the specified tile with the provided tile object
        public TileObject SetTile(TileObject tileObject, int index)
        {
            game.QueueFree(tiles[index]);
            return tiles[index] = (TileObject)game.AddGameObject(tileObject);
        }

        // replaces the specified tile with a blank tile
        public void ClearTile(TileObject tileObject)
        {
            // remove tile from game and set world tile to blank
            tiles[tiles.FindIndex(tile => tile == tileObject)] 
                = new TileObject(tileObject.X, tileObject.Y);

            game.QueueFree(tileObject);
        }      
        
        // replaces the specified tile with a blank tile
        public void ClearTile(int index)
        {
            game.QueueFree(tiles[index]);
            tiles[index] = AddTile(new TileObject(index, this, -1));
        }

        // Gets the Tile Coordinate for the specified GameObject
        public Vector2D GetTile(GameObject gameObject)
            => new Vector2D()
            {
                X = (float)Math.Floor((gameObject.X - x) / tileset.Size),
                Y = (float)Math.Floor((gameObject.Y - y)  / tileset.Size)
            };
        
        // Gets the Tile for the provided absolute pixel coordainte
        public Vector2D GetTile(float x, float y)
            => new Vector2D()
            {
                X = (float)Math.Floor((x - this.x) / tileset.Size),
                Y = (float)Math.Floor((y - this.y) / tileset.Size)
            };

        // gets the absolute pixel coordinte of the provided tile
        public Vector2D GetCoordinate(Vector2D tile)
            => new Vector2D()
            {
                X = tile.X * tileset.Size,
                Y = tile.Y * tileset.Size
            };

        // Returns the TileObject at the specified tile coordinate
        public TileObject GetTileObject(Vector2D tile)
            => tiles[((int)tile.X % widthTiles) + widthTiles * (int)tile.Y];

        // places the provided game object on a tile coordinate
        // as if it was a tile object
        public void PlaceObject(GameObject gameObject, Vector2D tile)
        {
            gameObject.X = tile.X * tileset.Size;
            gameObject.Y = tile.Y * tileset.Size;
        }

        // cycles the maps tiles in the specified direction 
        public void Slide(Direction direction, int step = 1)
        {
            // freeze the ghosts during the slide -- things'll go real bad if we dont do this ;-)
            ghosts.ForEach(ghost => ghost.Frozen = true);

            if (direction == Direction.LEFT)
            {
                game.QueueTask(0, () =>
                {
                    // cycle tiles to the right
                    tiles.ForEach(tile =>
                        tile.X = (tile.X + tileset.Size * 2) % width
                    );

                    // cycle ghosts to the right
                    ghosts.ForEach(ghost => ghost.Hide());

                    // recursively slide until a full cycle has compleated
                    if (step < widthTiles / 2)
                        Slide(direction, ++step);

                    else
                        // unfreeze the ghosts
                        ghosts.ForEach(ghost =>
                        {
                            ghost.Show();
                            ghost.Frozen = false;
                        });
                });
            }
            else if (direction == Direction.RIGHT)
            {
                game.QueueTask(0, () =>
                {
                    // cycle tiles to the left
                    tiles.ForEach(tile =>
                        tile.X = (tile.X + width - tileset.Size * 2) % width
                    );

                    // cycle ghosts to the left
                    ghosts.ForEach(ghost => ghost.Hide());

                    // recursively slide until a full cycle has compleated
                    if (step < widthTiles / 2)
                        Slide(direction, ++step);

                    else
                        // unfreeze the ghosts
                        ghosts.ForEach(ghost =>
                        {
                            ghost.Show();
                            ghost.Frozen = false; 
                        });
                });
            }
        }

        // offsets the specified tiles in a world for a set time
        public void OffsetTiles(int offset, int time, Func<TileObject, bool> predicate)
        {
            tiles.Where(predicate).ToList().ForEach(tile =>
            {
                tile.SourceRect = tileset.GetTileSourceRect(tile.TextureIndex + offset);

                game.QueueTask(time, () =>
                    tile.SourceRect = tileset.GetTileSourceRect(tile.TextureIndex)
                );
            });
        }
    }
}
