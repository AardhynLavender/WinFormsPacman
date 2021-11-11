
//
//  Game Object Class
//  Created 02/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A group of buttons that call functions when selected
//  using a pacman style menu tilemap.
//

using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects.Menu
{
    sealed class Menu : GameObject
    {
        // CONSTANTS

        private const int MENU_ITEMS        = 2;
        private const int MENU_X            = 0;
        private const int MENU_Y            = 0;

        private const int SELECTOR_RIGHT    = 530;
        private const int SELECTOR_LEFT     = 529;
        private const int SELECTOR_START_X  = 9;
        private const int SELECTOR_START_Y  = 18;
        private const int SELECTION_WIDTH   = 10;

        // FIELDS

        private World menu;

        private GameObject selectorRight;
        private GameObject selectorLeft;

        private int _selectionIndex;
        private bool keyHeld;
        bool selected;

        // CONSTRUCTOR

        public Menu()
            : base(MENU_X, MENU_Y)
        {
            menu            = new World("../../Assets/menu.tmx", X, Y);
            selectorRight   = new GameObject(SELECTOR_START_X * tileset.Size, SELECTOR_START_Y, SELECTOR_RIGHT);
            selectorLeft    = new GameObject(SELECTOR_START_X * tileset.Size + SELECTION_WIDTH * tileset.Size, SELECTOR_START_Y + SELECTION_WIDTH, SELECTOR_LEFT);
        }

        // PROPERTIES

        private int selectionIndex
        {
            get => _selectionIndex;
            set
            {
                game.PlaySound(Properties.Resources.waka);
                _selectionIndex = (value + MENU_ITEMS) % MENU_ITEMS;
            }
        }

        // METHODS

        private void Select()
        {
            selected = true;
            game.PlaySound(Properties.Resources.eat_ghost);
            game.QueueTask(Time.HALF_SECOND + Time.QUARTER_SECOND, () =>
            {
                switch (selectionIndex)
                {
                    case 0:

                        selected = false;
                        game.QueueFree(this);
                        game.StartGame();
                        break;

                    case 1:

                        game.Debug = true;

                        goto case 0;

                    default: break;
                }
            });
        }

        public override void Input()
        {
            if (InputManager.Up)
            {
                if (!keyHeld)
                { 
                    --selectionIndex;
                    keyHeld = true;
                }
            }
            else if (InputManager.Down)
            {
                if (!keyHeld)
                {
                    ++selectionIndex;
                    keyHeld = true;
                }
            }
            else if (InputManager.Select)
            {
                if (!keyHeld && !selected)
                {
                    Select();
                    keyHeld = true;
                }
            }
            else keyHeld = false;
        }

        public override void Update()
        {
            selectorLeft.Y = SELECTOR_START_Y * tileset.Size + selectionIndex * tileset.Size * 2;
            selectorRight.Y = SELECTOR_START_Y * tileset.Size + selectionIndex * tileset.Size * 2;
        }

        public override void OnAddGameObject()
        {
            // add menu objects
            game.AddGameObject(menu);
            game.AddGameObject(selectorLeft);
            game.AddGameObject(selectorRight);
        }

        public override void OnFreeGameObject()
        {
            // remove menu objects
            game.QueueFree(menu);
            game.QueueFree(selectorLeft);
            game.QueueFree(selectorRight);
        }
    }
}