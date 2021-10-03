
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

namespace FormsPixelGameEngine.GameObjects
{
    class TileMap : GameObject
    {
        // FIELDS

        private TileSet tileset;
        private int width;
        private int height;
        private int widthTiles;
        private int HeightTiles;

        private List<GameObject> tiles;

        // CONSTRUCTOR

        public TileMap(PacManGame game, string tilemap, float x, float y, TileSet tileset)
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
                // convert width to pixels
                width *= size;

                Console.WriteLine(width);
                Console.WriteLine(size);

                // get map data
                string[] map = xTileset
                    .Element("layer")
                    .Element("data")
                    .Value.Split(',');

                // create game objects from map
                int i = 0;
                Array.ForEach(map, tile =>
                {
                    int objX = i * size % width;
                    int objY = (int)Math.Floor((float)i / (width / size)) * size;
                    i++;

                    if (int.TryParse(tile, out int index) && index != 0)
                        Game.AddGameObject(new GameObject(x + objX, y + objY, tileset.GetTileSourceRect(index - 1)));
                });
            }
        }
        // PROPERTIES



        // METHODS

        public override void OnAddGameObject()
            => tiles.ForEach(tile => Game.AddGameObject(tile));

        public override void OnFreeGameObject()
            => tiles.ForEach(tile => Game.QueueFree(tile));
    }
}
