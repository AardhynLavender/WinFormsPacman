
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

        private const int DIGITS                = 10;
        private const int POINT_TILES           = 2;
        private const int GHOSTS                = 4;
        private const int LEVEL_PELLETS         = 240;
        private const int LEVEL_MAX             = 255;
        private const int MODE_SWITCHES         = 7;
        private const int SWITCH_SETS           = 3;
        private const int FLASH_TIME            = Time.TWO_SECOND;
        private const int WALL_CYCLE            = -168;

        private const int READY                 = 571;
        private const int HI_SCORE              = 40;

        private const int CLYDE_PELLET_LIMIT    = 60;
        private const int CLYDE_DECREMENT       = 10;
        private const int INKY_PELLET_LIMIT     = 30;
        private const int FRIGHTEN_TIME         = 6;

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

        private static readonly int[] ghostPoints =
        new int[GHOSTS]
        {
            200,
            400,
            800,
            1600
        };

        public static readonly int[,] pointTiles =
        new int[GHOSTS, POINT_TILES]
        {
            { 531, 536 },
            { 534, 536 },
            { 533, 536 },
            { 535, 536 }
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
        private int eatenGhosts;
        private long exitFrightenTime;
        private int frightenTime;
        private bool flashing;

        private LivesManager livesManager;

        private Menu menu;
        private TileSet tileset;
        private World world;

        private PacMan pacman;

        private Blinky blinky;
        private Pinky pinky;
        private Inky inky;
        private Clyde clyde;
        private List<Ghost> ghosts;

        private int clydePelletLimit;
        private int inkyPelletLimit;

        private Dictionary<int, int> digits;

        // CONSTRUCTOR

        public PacManGame(GameScreen screen, SoundPlayer media, System.Windows.Forms.Timer ticker)
            : base(screen, media, ticker)
        {
            // initalize fields

            digits = new Dictionary<int, int>(DIGITS)
            {
                { 0, 402 },{ 1, 403 },{ 2, 404 },{ 3, 444 },{ 4, 445 },
                { 5, 446 },{ 6, 486 },{ 7, 487 },{ 8, 488 },{ 9, 528 }
            };

            modeTracker     = new Stopwatch();
            scoreDisplay    = new List<TileObject>();
            tileset         = new TileSet("Assets/tileset.tsx", "Assets/tileset.png");

            // calculate default mode absolute switching times
            modeIndex = 0;
            for (int i = 0; i < modeTimes.GetLength(0); i++)
                for (int j = 1; j < modeTimes.GetLength(1) - 1; j++)
                    modeTimes[i, j] += modeTimes[i, j - 1];

            // configure GameObject
            GameObject.Init(this, screen, tileset);

            livesManager    = new LivesManager();

            // create and run a menu
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
                DisplayText(hiScore.ToString(), HI_SCORE);
            }
        }

        public bool Debug
        { 
            get => debug; 
            set => debug = value; 
        }
        
        public Vector2D PacManPosition 
            => pacman.CurrentTile;

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

        // display points earned and hide pacman while freezing the game 
        public void ConsumeGhost()
        {
            pacman.Hide();
            Freeze(Time.SECOND);

            GameObject[] points = new GameObject[POINT_TILES]
            {
                new GameObject(pacman.X - tileset.Size / 2, pacman.Y, pointTiles[eatenGhosts, 0]),
                new GameObject(pacman.X + tileset.Size / 2, pacman.Y, pointTiles[eatenGhosts, 1])
            };

            Score += ghostPoints[eatenGhosts++];

            Array.ForEach(points, p => AddGameObject(p));
            QueueTask(Time.SECOND, () => 
            {
                pacman.Show();
                Array.ForEach(points, p => QueueFree(p)); 
            });
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

        // Uses the pellet counter and preference rank to determine what ghost can be released
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

        // stop mode tracker
        public void PauseModeTracker()
            => modeTracker.Stop();

        // resume mode tracker
        public void ResumeModeTracker()
            => modeTracker.Start();

        // reset mode tracker
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

            eatenGhosts = 0;

            // frighten the ghosts
            ghosts.Where(g => !g.AtHome).ToList().ForEach(g => g.Frighten());

            // stop flashing -- just in case ghosts are already flashing
            flashing = false;

            // chase again after 6 seconds
            exitFrightenTime = RunningTime + Time.SECOND * frightenTime;
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
                    // clear gate
                    world.ClearTile(world.GetTileObject(new Vector2D(13, 15)));
                    world.ClearTile(world.GetTileObject(new Vector2D(14, 15)));

                    world.OffsetTiles(WALL_CYCLE, Time.HALF_SECOND, tile => tile.Wall);
                    QueueTask(Time.SECOND, () =>
                    {
                        world.OffsetTiles(WALL_CYCLE, Time.HALF_SECOND, tile => tile.Wall);
                        QueueTask(Time.SECOND, () =>
                        {
                            world.OffsetTiles(WALL_CYCLE, Time.HALF_SECOND, tile => tile.Wall);

                            // free pacman and recreate the world
                            QueueFree(pacman);
                            QueueFree(world);
                            QueueTask(Time.HALF_SECOND, () => newLevel());
                        });
                    });
                });
            }
            else
            {
                // Well done, all 255 levels...
                EndGame();
            }
        }

        // freeze everything and hide ghosts
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

        // create a new level, set of ghosts, and Pacman
        private void newLevel()
        {
            // reset world
            world = new World("Assets/tilemap.tmx", 0, 0);
            if (!gameObjects.Contains(world)) AddGameObject(world);

            // reset pacman
            pacman  = (PacMan)AddGameObject(new PacMan(world, livesManager));

            // update pellet limits

            if (level == 0)
            {
                clydePelletLimit = CLYDE_PELLET_LIMIT;
                inkyPelletLimit = INKY_PELLET_LIMIT;
            }
            else if (level == 1)
            {
                clydePelletLimit = CLYDE_PELLET_LIMIT - CLYDE_DECREMENT;
                inkyPelletLimit = 0;
            }
            else
            {
                clydePelletLimit = 0;
            }

            // shorten ghosts frighten time
            if (level < 7)
                --frightenTime;

            // create a new set of ghosts
            blinky  = (Blinky)AddGameObject(new Blinky(world, pacman));
            clyde   = (Clyde)AddGameObject(new Clyde(world, pacman, clydePelletLimit));
            pinky   = (Pinky)AddGameObject(new Pinky(world, pacman));
            inky    = (Inky)AddGameObject(new Inky(world, pacman, blinky, inkyPelletLimit));

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
                    world.ClearTile(READY + i);
            });
        }

        // DEBUG

        // Queues a line to be drawn after all other rendering
        public void DrawLine(Vector2D a, Vector2D b, Colour colour)
            => QueueDraw(() => screen.DrawLine(a, b, colour));

        // Queues an elipse to be drawn after all other rendering
        public void DrawEllipse(Vector2D p, float r, Colour colour)
            => QueueDraw(() => screen.DrawEllipse(p, r, colour));

        // ABSTRACT

        // the games beginning point
        public override void StartGame()
        {
            base.StartGame();

            QueueFree(menu);

            frightenTime = FRIGHTEN_TIME;
            score = level = 0;

            AddGameObject(livesManager);
            newLevel();
            DisplayText(hiScore.ToString(), HI_SCORE);
        }

        // the games end point
        public override void EndGame()
        {
            QueueTask(Time.TWO_SECOND, () =>
            {
                ResetModeTracker();
                QueueFree(livesManager);

                ghosts.ForEach(g => QueueFree(g));
                QueueFree(world);
                QueueFree(pacman);

                menu = (Menu)AddGameObject(new Menu());
            });
        }
    }
}