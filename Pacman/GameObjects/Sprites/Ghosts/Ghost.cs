﻿
//
//  Ghost : Sprite Class
//  Created 16/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A Sprite that moves about the maze and chooses a new
//  direction at intersections based on AI implimented in
//  child classes. Provides functionality shared across
//  all ghosts such as target tile Pathfinding, and mode
//  switching and subsequent functionality augmentation
//
//  Ghosts can be in multiple MODEs
//
//  *   CHASE mode, where they follow their 
//      implimented target tile finding algorithms.

//  *   SCATTER mode, where their targe tile
//      is set to an constant implimented in derived
//      classes.
//  
//  *   EATEN mode, where their target tile is set to
//      their home tile which implimented in derived
//      classes. Thier texture is set to the EATEN 
//      varients defined.
//
//  *   FRIGHTEND mode, where their movements are based
//      on random turns at intersections rather than
//      a target tile. Ghosts use the FRIGHTENED texture
//      in this mode
//
//  Ghosts change from CHASE || SCATTER to FRIGHTENED when 
//  PacMan:sprite types enter the same tile as Energizer:TileObject
//  types, and remain in said mode until 'Eaten()', or the FrightenedTime
//  elapsed since 'Frighten()'ed;
//
//  Ghosts can be eaten by Pacman:sprite types which triggers
//  the mode from FRIGHTEND to EATEN, reverting back to CHASE 
//  when the Ghost has returned to its Home Tile.
//

using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects.Sprites.Ghosts
{
    abstract class Ghost : Sprite
    {
        // CONSTANT AND STATIC MEMBERS

        private const int INFINITY              = 1000;
        protected const int DIRECTIONS          = 4;
        protected const int SIZE                = 2;
        private const int TUNNEL_DIVISOR        = 2;
        private const int FRIGHTENED_DIVISOR    = 2;
        protected const int ANIMATIONS          = 2;
        protected const int ANIMATION_SPEED     = Time.HUNDREDTH_SECOND;

        protected const int FRIGHTENED          = 504;
        protected const int EATEN_RIGHT         = 512;
        protected const int EATEN_LEFT          = 514;
        protected const int EATEN_UP            = 516;
        protected const int EATEN_DOWN          = 518;

        private const int PSEUDO_WALLS          = 4;
        private const int GATE_TILES            = 2;

        private const int OFFSET_X              = 4;
        private const int OFFSET_Y              = 3;

        // directional trajectories
        protected static readonly Vector2D[] Directions =
        new Vector2D[DIRECTIONS]
        {
            new Vector2D(0,-1),
            new Vector2D(1,0),
            new Vector2D(0,1),
            new Vector2D(-1,0)
        };

        // tiles that ghosts can't turn UP into
        private static readonly Vector2D[] PseudoWall =
        new Vector2D[PSEUDO_WALLS]
        {
            new Vector2D(12,13),
            new Vector2D(15,13),
            new Vector2D(12,25),
            new Vector2D(15,25)
        };

        // tiles that ghosts can't turn DOWN into
        private static readonly Vector2D[] monsterGate =
        new Vector2D[GATE_TILES]
        {
            new Vector2D(13,15),
            new Vector2D(14,15)
        };

        private static readonly int[] eatenTextures =
        new int[DIRECTIONS]
        {
            EATEN_UP,
            EATEN_RIGHT,
            EATEN_DOWN,
            EATEN_LEFT
        };


        // FIELDS

        protected PacMan pacman;
        private Mode mode;

        private int updateDiv;

        protected Vector2D targetTile;
        protected Vector2D scatterTile;
        protected Vector2D homeTile;

        private int houseWaitTime;

        protected Animation frightened;

        protected Animation right;
        protected Animation left;
        protected Animation up;
        protected Animation down;

        protected Animation[] directionalAnimations;

        // CONSTRUCTOR

        public Ghost(float x, float y, int index, World world, PacMan pacman)
            : base(x, y, index, SIZE, SIZE, new Vector2D(), world)
        {
            // initalize fields

            this.pacman = pacman;
            locked      = true;
            mode        = Mode.CHASE; 
            offsetX     = OFFSET_X;
            offsetY     = OFFSET_Y;

            homeTile = world.GetTile(this);

            // create frightened animation
            frightened = new Animation(game, tileset, this, SIZE, FRIGHTENED, 2, Time.TENTH_SECOND);
        }

        // PROPERTIES

        public Vector2D TargetTile
            => targetTile;

        public Mode Mode
            => mode; 

        // METHODS

        // calculate the target tile
        protected abstract Vector2D GetTargetTile();

        // reverse direction and set target tile to
        // applicable map corner
        public void Scatter()
        {
            if (mode == Mode.EATEN && !currentTile.Equals(homeTile));
            else
            {
                CurrentAnimation.Start();
                mode = Mode.SCATTER;
                speed = 1.0f;

                // reverse direction
                Trajectory.X *= -1;
                Trajectory.Y *= -1;
            }
        }

        public void Chase()
        {
            if (mode == Mode.EATEN && !currentTile.Equals(homeTile));
            else
            {
                CurrentAnimation.Start();
                mode = Mode.CHASE;
                speed = 1.0f;
            }
        }

        // reverse direction and set mode to FRIGHTENED
        public void Frighten()
        {
            mode = Mode.FRIGHTENED;

            CurrentAnimation = frightened;
            frightened.Start();

            // reverse direction
            if (!inTunnel)
            {
                Trajectory.X *= -1;
                Trajectory.Y *= -1;
            }
        }

        // set mode to EATEN
        public void Eat()
        {
            if (mode != Mode.EATEN);
            {
                CurrentAnimation.Stop();
                mode = Mode.EATEN;

                game.PlaySound(Properties.Resources.eat_ghost);
                game.Freeze(Time.SECOND);
            }
        }

        public override void Update()
        {
            if (inTunnel)
                updateDiv = TUNNEL_DIVISOR;

            else if (mode == Mode.FRIGHTENED)
                updateDiv = FRIGHTENED_DIVISOR;

            else
                updateDiv = 1;

            // round position
            int x = (int)Math.Round(this.x);
            int y = (int)Math.Round(this.y);

            // teleport in tunnel
            if (x < world.X && Direction == Direction.LEFT) 
                X = world.Width;

            if (x > world.X + world.Width && Direction == Direction.RIGHT) 
                X = world.X;

            // get the ghosts current tile
            currentTile = world.GetTile(x, y);

            if (mode == Mode.FRIGHTENED && currentTile.Equals(pacman.CurrentTile))
                Eat();

            if (mode == Mode.EATEN && currentTile.Equals(homeTile))
                Chase();

            // update target tile if centered on a tile
            if (x % tileset.Size == 0 
                && y % tileset.Size == 0 
                && x < world.X + world.Width 
                && x > world.X)
            {
                // update target tile based on mode
                switch (mode)
                {
                    case Mode.CHASE: 
                        // chase pacman
                        targetTile = GetTargetTile();
                        speed = 1.0f;
                        break;

                    case Mode.SCATTER:
                        // scatter to corner
                        targetTile = scatterTile;
                        speed = 1.0f;
                        break;

                    case Mode.EATEN:
                        // return to ghost house
                        targetTile = homeTile;
                        speed = 2.0f;
                        break;

                    default:break;
                }

                // store distances to 'target tile' from tiles ajacant to the 'current tile'
                List<int> distances = new List<int>(DIRECTIONS);

                // loop ajacant tiles in order
                for (int i = 0; i < DIRECTIONS; i++)
                {
                    Vector2D directionTrajectory = Directions[i];
                    Vector2D adjacent = new Vector2D(CurrentTile.X + directionTrajectory.X, CurrentTile.Y + directionTrajectory.Y);

                    // get ajacant tile distance
                    int distance = (int)Vector2D.GetAbsDistance(world.GetCoordinate(adjacent), world.GetCoordinate(targetTile));

                    // set distance to INFINITY if the ajacant tile is a wall, 'pseudowall', gate, or flank direction tile
                    if (world.GetTileObject(adjacent).Wall || Trajectory.Invert().Equals(directionTrajectory)
                        || (CurrentTile.Y > adjacent.Y && PseudoWall.Contains(adjacent))
                        || (CurrentTile.Y < adjacent.Y && mode != Mode.EATEN && monsterGate.Contains(adjacent)))
                    {
                        distance = INFINITY;
                    }

                    distances.Add(distance);
                }

                // set the ghosts trajectory and animation to the index of the first
                //  instance of the shortest distance in the directions list<vector>
                if (!inTunnel)
                {
                    int directionIndex;
                    if (mode == Mode.FRIGHTENED)
                        // pick a random tile that is not an INFINITE distance from pacman
                        do 
                            directionIndex = game.Random.Next(DIRECTIONS);

                        while (distances[directionIndex] == INFINITY);

                    else
                        // pick the first closest tile to PacMan
                        directionIndex = distances.IndexOf(distances.Min());

                    Trajectory = Directions[directionIndex];
                    direction = (Direction)directionIndex;
                }

                // set animation / texture

                if (mode == Mode.EATEN)
                    sourceRect = tileset.GetTileSourceRect(eatenTextures[(int)direction], SIZE, SIZE);

                else if (mode != Mode.FRIGHTENED)
                    CurrentAnimation = directionalAnimations[(int)direction];
            }

            // update if div is 0
            if (!locked && game.Tick % updateDiv == 0)
                base.Update();
        }
    } 
}