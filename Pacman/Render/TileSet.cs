
//
//  TileSet Class
//  Created 22/09/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  Uses LINQ-XML to parse tileset data into texture rectangles and collision infomation
//  tilesets of type .tsx were created using Tiled (https://www.mapeditor.org/).
//

using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Xml.Linq;
using System.Collections.Generic;

namespace FormsPixelGameEngine.Render
{
    public class TileSet
    {
        // FIELDS

        private string imageSource;
        private int tileSize;
        private int tileCount;
        private int width;
        private int height;

        private List<Rectangle> tileTextures;
        private List<bool> tileCollisions;

        // CONSTRUCTOR

        public TileSet(string tileset)
        {
            XElement xTileset;

            // try to load data as XML elements
            try
            {
                xTileset = XElement.Load(tileset);
            }
            catch (Exception err)
            {
                throw new Exception($"[Tileset] > could not load file '{tileset}' > {err.Message}");
            }

            // fetch elements

            XElement xImage = xTileset.Element("image");
            List<XElement> xTiles = xTileset.Elements("tile").ToList();

            // parse numerical data

            tileSize    = int.Parse((string)xTileset.Attribute("tilewidth"));
            tileCount   = int.Parse((string)xTileset.Attribute("tilecount"));
            width       = int.Parse((string)xImage.Attribute("width"));
            height      = int.Parse((string)xImage.Attribute("height"));

            imageSource 
                = Directory.GetCurrentDirectory() + '/'
                + tileset + '/'
                + xImage.Attributes("source").First()
                .ToString().Replace('/', '\\');

            // create tile source rectangles and collision infomation

            tileTextures = new List<Rectangle>(tileCount);
            tileCollisions = new List<bool>(tileCount);
            for (int i = 0; i < tileCount; i++)
            {
                // create tile texture source rectangle

                int x = i * tileSize % width;
                int y = (int)Math.Floor(i / (float)tileSize) / width;
                tileTextures.Add(new Rectangle(x, y, tileSize, tileSize));

                // determine if the tile is a wall

                try
                {
                    bool wall =
                        (string)xTiles[i]
                        .Descendants("properties")
                        .Descendants("property")
                        .Attributes("value").First() == "true";

                    tileCollisions.Add(wall);
                }
                catch { }

            }
        }

        // PROPERTIES

            // return reference to image ("image" type from winForms)

        // METHODS

        // fetches the location of the texture data
        public Rectangle GetTileSourceRect(int index) 
        {
            if (index > tileCount) 
                throw new Exception("[TileSet] > tile index out of range");

            return tileTextures[index];
        }

        // checks if the tile is a 'wall'
        public bool IsTileWall(int index)
            => index < tileCount && tileCollisions[index];
    }
}