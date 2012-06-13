using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;
using Microsoft.Xna.Framework;

namespace InfiniteCity.Model
{
    internal class Menu
    {
        private int _selectedMenuItemIndex;

        /// <summary>
        ///   Initialize the menu and opens up the uppermost submenu, the main menu.
        ///   Also, initialize the static list of AIs that will be used by MenuItems
        ///   that can select AIs.
        /// </summary>
        public Menu()
        {
            MenuItems = new List<MenuItem>();
            TransitionToMainMenu();

            MenuItem.AIs = new List<Tuple<string, bool>>
                {
                    new Tuple<string, bool>("Human", false), new Tuple<string, bool>("Wheatley", false), new Tuple<string, bool>("HAL", true),
                    new Tuple<string, bool>("Skynet", false), new Tuple<string, bool>("Disabled", false)
                };
        }

        public SubMenu CurrentSubMenu { get; private set; }
        public List<MenuItem> MenuItems { get; private set; }

        public int SelectedMenuItemIndex
        {
            get { return _selectedMenuItemIndex; }
            set
            {
                while (value<0)
                    value += MenuItems.Count;
                _selectedMenuItemIndex = (value%MenuItems.Count);
            }
        }

        /// <summary>
        ///   Load previously selected AI settings into their respective
        ///   menu items.
        /// </summary>
        /// <param name = "aiParams">Previously selected AI settings</param>
        public void LoadSavedAISettings(IEnumerable<Tuple<string, int>> aiParams)
        {
            int currentMenuItemIndex = 0;
            foreach (Tuple<string, int> t in aiParams)
            {
                switch (t.Item1)
                {
                    case "Human":
                        MenuItems[currentMenuItemIndex].SelectedAIIndex = 0;
                        break;
                    case "Wheatley":
                        MenuItems[currentMenuItemIndex].SelectedAIIndex = 1;
                        break;
                    case "HAL":
                        MenuItems[currentMenuItemIndex].SelectedAIIndex = 2;
                        break;
                    case "Skynet":
                        MenuItems[currentMenuItemIndex].SelectedAIIndex = 3;
                        break;
                    case "Disabled":
                        MenuItems[currentMenuItemIndex].SelectedAIIndex = 4;
                        break;
                }
                MenuItems[currentMenuItemIndex].ToggleLevelSelector();

                currentMenuItemIndex++;
                MenuItems[currentMenuItemIndex].SliderValue = t.Item2;
                currentMenuItemIndex++;
            }
        }

        /// <summary>
        ///   Open up the upper most submenu, the main menu.
        /// </summary>
        public void TransitionToMainMenu()
        {
            CurrentSubMenu = SubMenu.Main;
            MenuItems.Clear();

            MenuItems.Add(new MenuItem("Start New game"));
            MenuItems.Add(new MenuItem("Options"));
            MenuItems.Add(new MenuItem("Exit"));

            SelectedMenuItemIndex = 0;
        }

        /// <summary>
        ///   Open up the Options submenu. If AIParams is supplied, the AImenu
        ///   initializes its menuitems to equal previously selected settings.
        /// </summary>
        /// <param name = "aiParams"></param>
        public void TransitionToOptions(IEnumerable<Tuple<string, int>> aiParams)
        {
            CurrentSubMenu = SubMenu.Options;

            MenuItems.Clear();
            MenuItem currentAILevelSelector;

            for (int currentPlayer = 1; currentPlayer<=6; currentPlayer++)
            {
                var currentAISelector = new MenuItem("Player["+currentPlayer+"] AI", 0);
                currentAILevelSelector = new MenuItem("Player["+currentPlayer+"] AI Level: ", 1, 9, 1, 1);
                MenuItems.Add(currentAISelector);
                MenuItems.Add(currentAILevelSelector);
                MenuItem.LinkAIandLevelMenuItems(currentAISelector, currentAILevelSelector);
                currentAILevelSelector.Enabled = false;
            }

            MenuItems.Add(new MenuItem("Back to Main Menu"));

            if (aiParams != null)
                LoadSavedAISettings(aiParams);

            SelectedMenuItemIndex = 0;
        }
    }
}