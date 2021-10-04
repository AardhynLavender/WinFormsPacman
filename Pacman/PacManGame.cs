
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

        Sprite foo;
        GameObject bar;

        // CONSTRUCTOR

        public PacManGame(GameScreen screen, SoundPlayer media, System.Windows.Forms.Timer ticker)
            : base (screen, media,ticker)
        {
            // create tileset and world

            tileset = new TileSet("Assets/tileset.tsx", "Assets/tileset.png");
            GameObject.Texture = tileset;

            world = new World(this, "Assets/tilemap.tmx", 0, 0, tileset);
            AddGameObject(world);

            foo = (Sprite)AddGameObject(new Sprite(8, 64, 84, 2, 2, new Vector2D(0, 0), world));
            bar = AddGameObject(new GameObject(0, 0, tileset.GetTileSourceRect(323)));

            Animation a = AddAnimation(new Animation(this, bar, new List<System.Drawing.Rectangle>
            {
                tileset.GetTileSourceRect(323),
                tileset.GetTileSourceRect(324),
                tileset.GetTileSourceRect(325),
                tileset.GetTileSourceRect(326),
                tileset.GetTileSourceRect(327),
                tileset.GetTileSourceRect(328),
                tileset.GetTileSourceRect(329)
            }, 50));

            a.Start();
        }

        // PROPERTIES



        // GAME LOOP

        protected override void Process()
        {
            base.Process();

            foo.CurrentTile = world.GetTile(foo);

            if (InputManager.Up)
            {
                foo.Direction = Direction.UP;
                foo.Trajectory.Y = -1;
            }

            if (InputManager.Down)
            {
                foo.Direction = Direction.DOWN;
                foo.Trajectory.Y = 1;
            }

            if (InputManager.Left)
            {
                foo.Direction = Direction.LEFT;
                foo.Trajectory.X = -1;
            }

            if (InputManager.Right)
            {
                foo.Direction = Direction.RIGHT;
                foo.Trajectory.X = 1;
            }

            world.PlaceObject(bar, world.GetTile(foo));
        }

        protected override void Render()
        {
            base.Render();
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
