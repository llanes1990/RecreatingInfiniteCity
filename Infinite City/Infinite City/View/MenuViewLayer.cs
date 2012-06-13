using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model;
using InfiniteCity.Model.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteCity.View
{
    internal class MenuViewLayer
    {
        //private SpriteBatch _menuSpriteBatch;
        private const int MenuItemSubtextXOffset = 300;
        private const int MenuItemYStep = 20;
        private readonly Menu _menu;
        private readonly SpriteFont _font;
        private readonly Vector2 _firstMenuItemPosition = new Vector2(100, 100);

        public MenuViewLayer(Menu menu, SpriteFont font)
        {
            _menu = menu;
            _font = font;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int currentMenuItemIndex = 0; currentMenuItemIndex<_menu.MenuItems.Count; currentMenuItemIndex++)
            {
                MenuItem currentMenuItem = _menu.MenuItems[currentMenuItemIndex];

                //Do not draw the menu item if it is disabled.
                if (!currentMenuItem.Enabled)
                    continue;

                Color menuItemColor = Color.White;
                if (currentMenuItemIndex == _menu.SelectedMenuItemIndex)
                    menuItemColor = Color.Red;

                String menuItemText = currentMenuItem.Name;
                String menuItemSubText = String.Empty;
                switch (currentMenuItem.MenuItemType)
                {
                    case MenuItemType.AISelector:
                        menuItemSubText = " <- "+MenuItem.AIs[currentMenuItem.SelectedAIIndex].Item1+" -> ";
                        break;
                    case MenuItemType.Slider:
                        menuItemSubText = " <- "+currentMenuItem.SliderValue+" -> ";
                        break;
                    case MenuItemType.Bool:
                        menuItemSubText = " <- "+currentMenuItem.BoolValue+" -> ";
                        break;
                }

                spriteBatch.DrawString(_font,
                                       menuItemText,
                                       new Vector2(_firstMenuItemPosition.X, _firstMenuItemPosition.Y+(currentMenuItemIndex*MenuItemYStep)),
                                       menuItemColor);
                spriteBatch.DrawString(_font,
                                       menuItemSubText,
                                       new Vector2(_firstMenuItemPosition.X+MenuItemSubtextXOffset,
                                                   _firstMenuItemPosition.Y+(currentMenuItemIndex*MenuItemYStep)),
                                       menuItemColor);
            }
        }
    }
}