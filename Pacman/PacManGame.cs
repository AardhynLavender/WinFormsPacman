
//
//  PacMan Class : Game
//  Created 22/09/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  Applies the abstract game class to create the game of pacman
//  creating the maze, ghosts, and 
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

using FormsPixelGameEngine.GameObjects;
using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine
{
    class PacManGame : Game
    {
        // FIELDS

        TileSet tileset;
        World world;

        PacMan pacman;
        GameObject bar;
        GameObject targetTile;

        // CONSTRUCTOR

        public PacManGame(GameScreen screen, SoundPlayer media, System.Windows.Forms.Timer ticker)
            : base (screen, media,ticker)
        {
            // create tileset and world

            tileset = new TileSet("Assets/tileset.tsx", "Assets/tileset.png");
            GameObject.Texture = tileset;

            world = new World(this, "Assets/tilemap.tmx", 0, 0, tileset);
            AddGameObject(world);

            pacman = (PacMan)AddGameObject(new PacMan(8, 64, world, this));

            bar = AddGameObject(new GameObject(0, 0, tileset.GetTileSourceRect(323)));
        }

        // PROPERTIES



        // GAME LOOP

        protected override void Process()
        {
            base.Process();
        }

        protected override void Render()
        {
            base.Render();
            world.PlaceObject(bar, pacman.CurrentTile);
        }

        // OBJECT MANAGMENT 

        // adds an object to the games processing pool
        public override GameObject AddGameObject(GameObject gameObject)
        {
            // give the object a reference to its game and screen
            gameObject.Game = this;
            gameObject.Screen = screen;

            // call base method
            return base.AddGameObject(gameObject);
        }

        // EVENTS

        public override void StartGame()
        {
            base.StartGame();
        }

        protected override void SaveGame()
        {
            base.SaveGame();
        }

        public override void EndGame()
        {
            base.EndGame();
        }
    }
}
