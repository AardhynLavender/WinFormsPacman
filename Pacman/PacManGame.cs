
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
using System.Linq;

using FormsPixelGameEngine.GameObjects;
using FormsPixelGameEngine.GameObjects.Tiles;
using FormsPixelGameEngine.GameObjects.Sprites;
using FormsPixelGameEngine.GameObjects.Sprites.Ghosts;

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;
using System.Diagnostics;

namespace FormsPixelGameEngine
{
    class PacManGame : Game
    {
        // CONSTANT AND STATIC MEMBERS

        private const int DIGITS = 10;

        private static readonly int[,] modeTimes = new int[3, 8]
        {
            {
                // level 1
                Time.SEVEN_SECOND,
                Time.TWENTY_SECOND,
                Time.SEVEN_SECOND,
                Time.TWENTY_SECOND,
                Time.FIVE_SECOND,
                Time.TWENTY_SECOND,
                Time.FIVE_SECOND,
                -1

            },{ // levels 2 > 4
                Time.SEVEN_SECOND,
                Time.TWENTY_SECOND,
                Time.SEVEN_SECOND,
                Time.TWENTY_SECOND,
                Time.FIVE_SECOND,
                1033,
                1,
                -1

            },{ // Levels 5 > 255
                Time.FIVE_SECOND,
                Time.TWENTY_SECOND,
                Time.FIVE_SECOND,
                Time.TWENTY_SECOND,
                Time.FIVE_SECOND,
                1037,
                1,
                -1
            }
        };

        // FIELDS

        private int score;
        private int hiScore;

        private List<TileObject> scoreDisplay;

        private Stopwatch modeTracker;
        private Mode currentMode;
        private int modeIndex;

        private TileSet tileset;
        private World world;

        private PacMan pacman;

        private Blinky blinky;
        private Pinky pinky;
        private Inky inky;
        private Clyde clyde;
        private List<Ghost> ghosts;

        private Dictionary<int, int> digits;

        // CONSTRUCTOR

        public PacManGame(GameScreen screen, SoundPlayer media, System.Windows.Forms.Timer ticker)
            : base(screen, media, ticker)
        {
            // initalize fileds

            modeTracker = new Stopwatch();

            digits = new Dictionary<int, int>(DIGITS)
            {
                { 0, 402 },{ 1, 403 },{ 2, 404 },{ 3, 444 },{ 4, 445 },
                { 5, 446 },{ 6, 486 },{ 7, 487 },{ 8, 488 },{ 9, 528 }
            };

            scoreDisplay = new List<TileObject>();

            // create tileset

            tileset = new TileSet("Assets/tileset.tsx", "Assets/tileset.png");

            // configure GameObject

            GameObject.Init(this, screen, tileset);

            // calculate default mode absolute switching times

            modeIndex = 0;
            for (int i = 0; i < modeTimes.GetLength(0); i++)
                for (int j = 1; j < modeTimes.GetLength(1) - 1; j++)
                    modeTimes[i, j] += modeTimes[i, j - 1];

            for (int i = 0; i < modeTimes.GetLength(1); i++)
                Console.WriteLine(modeTimes[0, i] + "\t" + i);

            // create world

            world = new World("Assets/tilemap.tmx", 0, 0);

            AddGameObject(world);

            // add pacman

            pacman = (PacMan)AddGameObject(new PacMan(world));

            // add ghosts

            blinky      = (Blinky)AddGameObject(new Blinky(world, pacman));
            clyde       = (Clyde)AddGameObject(new Clyde(world, pacman));
            pinky       = (Pinky)AddGameObject(new Pinky(world, pacman));
            inky        = (Inky)AddGameObject(new Inky(world, pacman, blinky));

            ghosts = new List<Ghost>(4)
            { blinky, inky, pinky, clyde };

            world.Ghosts = ghosts;

            // start game

            PlaySound(Properties.Resources.game_start);
            QueueTask(Time.FOUR_SECOND, () =>
            {
                ResumeModeTracker();

                pacman.Locked   = false;
                blinky.Locked   = false;

                QueueTask(Time.SECOND, () => 
                {
                    inky.Locked = false;    

                    QueueTask(Time.SECOND, () => 
                    {
                        pinky.Locked = false; 

                        QueueTask(Time.SECOND, () => 
                        {
                            clyde.Locked = false;
                        });
                    });
                });

                WinLevel();

                for (int i = 0; i < "ready!".Length; i++)
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

        public Mode CurrentMode 
            => currentMode;

        // GAME LOOP

        protected override void Process()
        {
            base.Process();

            if (modeIndex < 7 && modeTracker.ElapsedMilliseconds > modeTimes[0, modeIndex])
            {
                currentMode = modeIndex++ % 2 == 0
                    ? Mode.CHASE
                    : Mode.SCATTER;

                Console.WriteLine(currentMode + " : " + modeIndex);

                ghosts.Where(g => g.Mode != Mode.EATEN).ToList().ForEach(g => g.Revert());
            }
        }

        protected override void Render()
        {
            base.Render();
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

        // EVENTS

        public void PauseModeTracker()
            => modeTracker.Stop();

        public void ResumeModeTracker()
            => modeTracker.Start();

        public void ResteModeTracker()
            => modeTracker.Reset();

        // Frighten the ghosts
        public void Frighten()
        {
            // pause mode tracker
            PauseModeTracker();

            // frighten the ghosts
            ghosts.ForEach(g => g.Frighten());

            // chase again after 6 seconds
            QueueTask(Time.SECOND * 6, () =>
            {
                ResumeModeTracker();
                ghosts.Where(g => g.Mode != Mode.EATEN).ToList().ForEach(g => g.Revert());
            });
        }

        public void Scatter()
        {
            ghosts.Where(g => g.Mode != Mode.EATEN).ToList()
            .ForEach(g => g.Scatter());
        }

        // freezes all sprites except eaten ghosts
        public void Freeze(int milliseconds)
        {
            pacman.Frozen = true;

            ghosts.Where(g => g.Mode != Mode.EATEN)
            .ToList().ForEach(g =>
            {
                g.Frozen = true;
                QueueTask(milliseconds, () => g.Frozen = false);

                if (g.CurrentAnimation.Animating)
                {
                    g.CurrentAnimation.Animating = false;
                    QueueTask(milliseconds, () => g.CurrentAnimation.Animating = true);
                }
            });

            QueueTask(milliseconds, () => pacman.Frozen = false);

        }

        // reset eveything and update the level counter
        public void WinLevel()
        {
            QueueTask(Time.SECOND, () =>
            {
                world.OffsetTiles(-168, Time.HALF_SECOND, tile => tile.Wall);
                QueueTask(Time.SECOND, () =>
                {
                    world.OffsetTiles(-168, Time.HALF_SECOND, tile => tile.Wall);
                    QueueTask(Time.SECOND, () =>
                    {
                        world.OffsetTiles(-168, Time.HALF_SECOND, tile => tile.Wall);
                    });
                });
            });
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
