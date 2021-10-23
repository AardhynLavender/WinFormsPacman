﻿
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
using FormsPixelGameEngine.GameObjects.Tiles;
using FormsPixelGameEngine.GameObjects.Sprites;
using FormsPixelGameEngine.GameObjects.Sprites.Ghosts;

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine
{
    class PacManGame : Game
    {
        // CONSTANTS

        private const int DIGITS = 10;

        // FIELDS

        private int score;
        private int hiScore;

        private List<TileObject> scoreDisplay;

        private TileSet tileset;
        private World world;

        private PacMan pacman;
        private Blinky blinky;
        private Pinky pinky;
        private Inky inky;
        private Clyde clyde;

        private GameObject InkyTarget;

        private Dictionary<int, int> digits;

        // CONSTRUCTOR

        public PacManGame(GameScreen screen, SoundPlayer media, System.Windows.Forms.Timer ticker)
            : base (screen, media,ticker)
        {
            // initalize fileds

            digits = new Dictionary<int, int>(DIGITS)
            {
                { 0, 402 },{ 1, 403 },{ 2, 404 },{ 3, 444 },{ 4, 445 },
                { 5, 446 },{ 6, 486 },{ 7, 487 },{ 8, 488 },{ 9, 528 }
            };

            scoreDisplay = new List<TileObject>();

            // create tileset and world

            tileset = new TileSet("Assets/tileset.tsx", "Assets/tileset.png");

            GameObject.Init(this, screen, tileset);

            world = new World("Assets/tilemap.tmx", 0, 0);
            AddGameObject(world);

            // add pacman

            pacman = (PacMan)AddGameObject(new PacMan(world));

            // add ghosts

            blinky      = (Blinky)AddGameObject(new Blinky(world, pacman));
            clyde       = (Clyde)AddGameObject(new Clyde(world, pacman));
            pinky       = (Pinky)AddGameObject(new Pinky(world, pacman));
            inky        = (Inky)AddGameObject(new Inky(world, pacman, blinky));
            InkyTarget  = AddGameObject(new GameObject(0, 0, 324));

            // start game
            
            PlaySound(Properties.Resources.game_start);
            QueueTask(Time.FOUR_SECOND, () =>
            {
                pacman.Locked   = false;
                blinky.Locked   = false;

                QueueTask(Time.SECOND, () => 
                {
                    inky.Locked     = false;    

                    QueueTask(Time.SECOND, () => 
                    {
                        pinky.Locked    = false; 

                        QueueTask(Time.SECOND, () => 
                        {
                            clyde.Locked    = false;        
                        });
                    });
                });

                for (int i = 0; i < 5; i++)
                    world.ClearTile(571 + i);
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

        public TileSet TileSet
            => tileset;

        // GAME LOOP

        protected override void Process()
        {
            base.Process();
        }

        protected override void Render()
        {
            base.Render();
            world.PlaceObject(InkyTarget, inky.TargetTile);
        }

        // TEXT MANAGMENT

        private void DisplayText(string text, int index)
        {
            char[] strScore = text.ToString().ToCharArray();
            Array.ForEach(strScore, character =>
            {
                if (int.TryParse(character.ToString(), out int digit))
                    world.SetTile(
                        new TileObject(index, world, digits[digit]),
                        index++
                    );

                else throw new Exception("text rendering has not yet been implimented!");
            });
        }

        // OBJECT MANAGMENT 

        // adds an object to the games processing pool
        public override GameObject AddGameObject(GameObject gameObject)
        {
            // call base method
            return base.AddGameObject(gameObject);
        }

        // EVENTS

        // Frighten the ghosts
        public void Frighten()
        {
            blinky.Frighten();
            pinky.Frighten();
            inky.Frighten();
            clyde.Frighten();
        }

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
