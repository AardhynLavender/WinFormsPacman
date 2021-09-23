
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

using FormsPixelGameEngine.Render;

namespace FormsPixelGameEngine.GameObjects
{
    class TileMap /* : GameObject */
    {
        // FIELDS

        private TileSet tileset;
        private int width;
        private int height;
        private int widthTiles;
        private int HeightTiles;

        private List<Object> tiles;

        // CONSTRUCTOR

        public TileMap(string filepath, float x, float y, TileSet tileset)
        {
            this.tileset = tileset;
        }

        // PROPERTIES

        

        // METHODS

            // override addGameObject() to add all the tiles as-well

            // ovrride removeGameObject() to remove all the tiles as-well
    }
}
