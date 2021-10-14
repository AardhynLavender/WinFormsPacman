
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
using System.Media;

using FormsPixelGameEngine.GameObjects;
using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine
{
    class PacManGame : Game
    {
        // FIELDS

        private int score;
        private int hiScore;

        private List<TileObject> scoreDisplay;

        private TileSet tileset;
        private World world;

        private PacMan pacman;

        private Dictionary<int, int> digits;

        // CONSTRUCTOR

        public PacManGame(GameScreen screen, SoundPlayer media, System.Windows.Forms.Timer ticker)
            : base (screen, media,ticker)
        {
            // initalize fileds

            digits = new Dictionary<int, int>(10)
            {
                { 0, 402 },{ 1, 403 },{ 2, 404 },{ 3, 444 },{ 4, 445 },
                { 5, 446 },{ 6, 486 },{ 7, 487 },{ 8, 488 },{ 9, 528 }
            };

            scoreDisplay = new List<TileObject>();

            // create tileset and world

            tileset = new TileSet("Assets/tileset.tsx", "Assets/tileset.png");
            GameObject.Texture = tileset;

            world = new World(this, "Assets/tilemap.tmx", 0, 0, tileset);
            AddGameObject(world);

            // add pacman

            pacman = (PacMan)AddGameObject(new PacMan(8, 64, world, this));

            PlaySound(Properties.Resources.game_start);
            QueueTask(4000, () =>
            {
                pacman.Locked = false;
                for (int i = 0; i < 5; i++)
                    world.QueueTileFree(571 + i);
            });
        }

        // PROPERTIES

        public int Score
        {
            get => score;
            set
            {
                score = value;

                DisplayText(score.ToString(), 30);

                if (score >= hiScore)
                    HiScore = score;
            }
        }

        public int HiScore
        {
            get => hiScore;
            private set
            {
                hiScore = value;
                DisplayText(hiScore.ToString(), 40);
            }
        }

        public Vector2D PacManPosition 
            => pacman.CurrentTile;

        // GAME LOOP

        protected override void Process()
        {
            base.Process();
        }

        protected override void Render()
        {
            base.Render();
            //world.PlaceObject(bar, pacman.CurrentTile);
        }

        // TEXT MANAGMENT

        private void DisplayText(string text, int index)
        {
            char[] strScore = text.ToString().ToCharArray();
            Array.ForEach(strScore, character =>
            {
                if (int.TryParse(character.ToString(), out int digit))
                    world.SetTileObject(
                        new TileObject(index, world, tileset.GetTileSourceRect(digits[digit])),
                        index++
                    );
                else
                    throw new Exception("text rendering has not yet been implimented!");
            });
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
