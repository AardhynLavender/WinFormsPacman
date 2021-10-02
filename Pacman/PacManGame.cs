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



        // CONSTRUCTOR

        public PacManGame(GameScreen screen, SoundPlayer media, System.Windows.Forms.Timer ticker)
            : base (screen, media,ticker)
        {

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
