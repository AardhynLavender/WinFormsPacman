
//
//  Lives Manager Class
//  Created 09/11/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  Manages the visual display of lives on the game screen
//

using FormsPixelGameEngine.GameObjects;
using System;

namespace FormsPixelGameEngine.Utility
{
    class LivesManager : GameObject
    {
        // CONSTANTS

        private const int START_LIVES   = 3;
        private const int SIZE          = 2;
        private const int TEXTURE       = 92;

        private static readonly Vector2D 
        startVector = new Vector2D(16, 272);

        // FIELDS

        private GameObject[] livesDisplay;
        private int lives;

        // CONSTRUCTOR

        public LivesManager()
            : base(startVector.X, startVector.Y)
        {
            // create lives
            livesDisplay = new GameObject[START_LIVES];
            for (int i = 0; i < START_LIVES; i++)
                livesDisplay[i] = new GameObject(startVector.X + i * tileset.Size * SIZE, startVector.Y, TEXTURE, 100, SIZE, SIZE);
        }

        // PROPERTIES

        public int Lives
            => lives;

        // METHODS

        // deducts a life, returning if pacman can be revived
        public bool DeductLife()
        {
            game.QueueFree(livesDisplay[--lives]);
            return lives > 0;
        }

        // add lives display
        public override void OnAddGameObject()
        {
            lives = START_LIVES;
            Array.ForEach(livesDisplay, l => game.AddGameObject(l));
        }

        // remove lives display
        public override void OnFreeGameObject()
            => Array.ForEach(livesDisplay, l => game.QueueFree(l));
    }
}
