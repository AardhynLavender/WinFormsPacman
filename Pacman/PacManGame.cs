
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
using FormsPixelGameEngine.GameObjects.Menu;

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;
using System.Diagnostics;

namespace FormsPixelGameEngine
{
    class PacManGame : Game
    {
        // CONSTANT AND STATIC MEMBERS

        private const int DIGITS        = 10;
        private const int GHOSTS        = 4;
        private const int LEVEL_PELLETS = 240;
        private const int LEVEL_MAX     = 255;
        private const int MODE_SWITCHES = 7;
        private const int SWITCH_SETS   = 3;
        private const int FLASH_TIME    = Time.TWO_SECOND;

        private static readonly int[,] modeTimes = 
        new int[SWITCH_SETS, MODE_SWITCHES + 1]
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

        private bool debug;

        private List<TileObject> scoreDisplay;
        private int score;
        private int hiScore;

        private int pelletCount;
        private int level;

        private Stopwatch modeTracker;
        private Mode currentMode;
        private int modeIndex;
        private long exitFrightenTime;
        private bool flashing;

        private Menu menu;
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

            // create and run menu

            menu = (Menu)AddGameObject(new Menu());
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

        public bool Debug
        { 
            get => debug; 
            set => debug = value; 
        }

        public PacMan Pacman
            => pacman;
        
        public Vector2D PacManPosition 
            => pacman.CurrentTile;

        public TileSet TileSet
            => tileset;

        public Mode CurrentMode 
            => currentMode;

        private bool frightened
            => exitFrightenTime != 0;

        // GAME LOOP

        protected override void Process()
        {
            base.Process();

            tryExitFrighten();
            setGhostMode();
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

        // LEVEL MANAGMENT

        public void ConsumePellet()
        {
            tryReleaseGhosts();

            if (++pelletCount == LEVEL_PELLETS)
                winLevel();
        }

        // Determines the mode the ghosts should default
        // to based on the level and level timer.
        private void setGhostMode()
        {
            // determine the correct mode set
            int modeSet = (modeIndex < 2)
                ? 0
                : (modeIndex < 5)
                    ? 1
                    : 2;

            // set the ghosts default mode
            if (modeIndex < MODE_SWITCHES
                && modeTracker.ElapsedMilliseconds > modeTimes[modeSet, modeIndex])
            {
                currentMode = modeIndex++ % 2 == 0
                    ? Mode.CHASE
                    : Mode.SCATTER;

                ghosts.Where(g => g.Mode != Mode.EATEN).ToList().ForEach(g => g.Revert());
            }
        }

        private void tryReleaseGhosts()
        {
            // find the highest ranked ghost in the ghost house
            List<Ghost> housedGhosts = (
                from    ghost
                in      ghosts
                where   ghost.AtHome
                orderby ghost.PreferenceRank
                select  ghost
            ).ToList();

            if (housedGhosts.Count > 0)
            {
                // release the ghost if it reaches it's pellet count limit
                Ghost preferedGhost = housedGhosts.First();
                if (!preferedGhost.IncrementPelletCounter())
                {
                    preferedGhost.AtHome =
                    preferedGhost.Locked =
                    preferedGhost.Frozen = false;
                }
            }
        }

        // EVENTS

        public void PauseModeTracker()
            => modeTracker.Stop();

        public void ResumeModeTracker()
            => modeTracker.Start();

        public void ResetModeTracker()
        {
            modeTracker.Reset();
            modeIndex = 0;
        }

        // Resume standard mode switch, revert the ghosts, and reset the exit frighten flag
        private void tryExitFrighten()
        {
            if (frightened)
            {
                // is it time to exit FRIGHTENED mode?
                if (RunningTime > exitFrightenTime)
                {
                    ResumeModeTracker();
                    ghosts.Where(g => g.Mode != Mode.EATEN).ToList().ForEach(g => g.Revert());
                    exitFrightenTime = 0;
                    flashing = false;
                } 
                // is it nearly time to exit FRIGHTENED mode?
                else if (!flashing && exitFrightenTime - RunningTime < FLASH_TIME)
                {
                    flashing = true;
                    ghosts.Where(g => g.Mode != Mode.EATEN).ToList().ForEach(g => g.Flash());
                }
            }
        }

        // Frighten the ghosts
        public void Frighten()
        {
            // pause mode tracker
            PauseModeTracker();

            // frighten the ghosts
            ghosts.Where(g => !g.AtHome).ToList().ForEach(g => g.Frighten());

            // stop flashing -- just in case ghosts are already flashing
            flashing = false;

            // chase again after 6 seconds
            exitFrightenTime = RunningTime + Time.SECOND * 6;
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
        private void winLevel()
        {
            if (++level < LEVEL_MAX)
            {
                ResetModeTracker();
                pacman.Frozen = true;

                ghosts.ForEach(ghost =>
                {
                    ghost.Frozen = true;
                    QueueFree(ghost);
                });

                // wall strobe animation
                QueueTask(Time.SECOND, () =>
                {
                    world.OffsetTiles(-168, Time.HALF_SECOND, tile => tile.Wall);
                    QueueTask(Time.SECOND, () =>
                    {
                        world.OffsetTiles(-168, Time.HALF_SECOND, tile => tile.Wall);
                        QueueTask(Time.SECOND, () =>
                        {
                            world.OffsetTiles(-168, Time.HALF_SECOND, tile => tile.Wall);

                            QueueFree(pacman);
                            QueueFree(world);

                            QueueTask(Time.HALF_SECOND, () =>
                            {
                                newLevel();
                            });
                        });
                    });
                });
            }
            else
            {
                // special level 255 end game screen...
            }
        }

        public void LooseLevel()
        {
            ghosts.ForEach(g => 
            {
                g.Hide();
                g.Frozen = true;
            });

            pacman.Frozen = true;
        }

        // resets the positions of all the ghosts and pacman
        public void RestartLevel()
        {
            QueueTask(Time.TWO_SECOND, () =>
            {
                if (!pacman.Alive)
                {  
                    ghosts.ForEach(g =>
                    {
                        g.Reset();
                        g.Frozen = false;
                    });

                    pacman.Revive();
                }
            });
        }

        private void newLevel()
        {
            // reset world
            world = new World("Assets/tilemap.tmx", 0, 0);
            if (!gameObjects.Contains(world)) AddGameObject(world);

            // reset pacman
            pacman  = (PacMan)AddGameObject(new PacMan(world));

            // create a new set of ghosts and pacman
            blinky  = (Blinky)AddGameObject(new Blinky(world, pacman));
            clyde   = (Clyde)AddGameObject(new Clyde(world, pacman, 60));
            pinky   = (Pinky)AddGameObject(new Pinky(world, pacman));
            inky    = (Inky)AddGameObject(new Inky(world, pacman, blinky, 30));

            ghosts  = new List<Ghost>(GHOSTS)
                    { blinky, inky, pinky, clyde };

            world.Ghosts = ghosts;

            pelletCount = 0;

            PlaySound(Properties.Resources.game_start);
            QueueTask(Time.FOUR_SECOND, () =>
            {
                ResumeModeTracker();

                blinky.Locked =
                pacman.Locked = 
                pacman.Frozen = false;

                for (int i = 0; i < "ready!".Length; i++)
                    world.ClearTile(571 + i);
            });
        }

        // DEBUG

        public void DrawLine(Vector2D a, Vector2D b, Colour colour)
            => QueueDraw(() => screen.DrawLine(a, b, colour));

        public void DrawEllipse(Vector2D p, float r, Colour colour)
            => QueueDraw(() => screen.DrawEllipse(p, r, colour));

        // ABSTRACT

        public override void StartGame()
        {
            base.StartGame();


            QueueFree(menu);
            newLevel();

            score = 0;
            DisplayText(hiScore.ToString(), 40);
        }

        protected override void SaveGame()
        {
            base.SaveGame();
        }

        public override void EndGame()
        {
            ResetModeTracker();
            level = 0;

            ghosts.ForEach(g => QueueFree(g));
            QueueFree(world);
            QueueFree(pacman);

            menu = (Menu)AddGameObject(new Menu());
        }
    }
}
