using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;

namespace InfiniteCity.Model
{
    internal sealed class MenuItem
    {
        public readonly MenuItemType MenuItemType;

        private readonly int _sliderStep;

        private int _sliderValue;
        private int _selectedAIIndex;

        /// <summary>
        ///   Create a transition MenuItem
        /// </summary>
        /// <param name = "name"></param>
        public MenuItem(String name)
        {
            MenuItemType = MenuItemType.Transition;
            Name = name;
            Enabled = true;
        }

        /// <summary>
        ///   Create a Bool MenuItem
        /// </summary>
        /// <param name = "name"></param>
        /// <param name = "value"></param>
        public MenuItem(String name, bool value)
        {
            MenuItemType = MenuItemType.Bool;
            Name = name;
            BoolValue = value;
            Enabled = true;
        }

        /// <summary>
        ///   Create a slider MenuItem. A slider is a menuItem that has an integer value that 
        ///   can be set anywhere within a range. For example, a slider menu item might be used
        ///   to ask the user for Mouse Sensitivity.
        /// </summary>
        /// <param name = "name"></param>
        /// <param name = "value"></param>
        /// <param name = "maxValue">The maximum inclusive value for the slider</param>
        /// <param name = "minValue">The minimum inclusive value for the slider</param>
        /// <param name = "step"></param>
        public MenuItem(String name, int value, int maxValue, int minValue, int step)
        {
            MenuItemType = MenuItemType.Slider;
            Name = name;
            MaxSliderValue = maxValue;
            MinSliderValue = minValue;
            SliderValue = value;
            _sliderStep = step;
            Enabled = true;
        }

        /// <summary>
        ///   Create AI Selector
        /// </summary>
        /// <param name = "name"></param>
        /// <param name = "selectedAIIndex"></param>
        public MenuItem(String name, int selectedAIIndex)
        {
            MenuItemType = MenuItemType.AISelector;
            Name = name;
            SelectedAIIndex = selectedAIIndex;
            Enabled = true;
        }

        /// <summary>
        ///   List of of tuples containing.
        ///   First element is the string key to grab the AI's constructor.
        ///   Second Element is whether or not a difficulty parameter is necessary
        /// </summary>
        public static List<Tuple<string, bool>> AIs { get; set; }

        public bool BoolValue { get; set; }
        public MenuItem DifficultySlider { get; set; }
        public bool Enabled { get; set; }
        public int MaxSliderValue { get; set; }
        public int MinSliderValue { get; set; }
        public String Name { get; set; }

        public int SelectedAIIndex
        {
            get { return _selectedAIIndex; }
            set
            {
                _selectedAIIndex = value;
                if (_selectedAIIndex>=AIs.Count)
                    _selectedAIIndex = 0;
                if (_selectedAIIndex<0)
                    _selectedAIIndex = AIs.Count-1;
            }
        }

        public int SliderValue
        {
            get { return _sliderValue; }
            set
            {
                _sliderValue = value;
                if (_sliderValue>MaxSliderValue)
                    _sliderValue = MaxSliderValue;
                if (_sliderValue<MinSliderValue)
                    _sliderValue = MinSliderValue;
            }
        }

        /// <summary>
        ///   Associate a MenuItem that selects an AI and a MenuItem that selects a difficulty.
        /// </summary>
        /// <param name = "aiSelectorMenuItem">A menu item that selects an AI</param>
        /// <param name = "levelMenuItem">A menu item that selects a level</param>
        public static void LinkAIandLevelMenuItems(MenuItem aiSelectorMenuItem, MenuItem levelMenuItem)
        {
            aiSelectorMenuItem.DifficultySlider = levelMenuItem;
        }

        /// <summary>
        ///   Decrement the sliderValue of a slider menu item
        /// </summary>
        public void SliderDecrement()
        {
            SliderValue -= _sliderStep;
        }

        /// <summary>
        ///   Increment the sliderValue of a slider menu item
        /// </summary>
        public void SliderIncrement()
        {
            SliderValue += _sliderStep;
        }

        /// <summary>
        ///   Given a selectedAIIndex from AISelectorMenuItem, toggle the linked levelMenuItem depending on if the
        ///   AI requires a difficulty
        /// </summary>
        public void ToggleLevelSelector()
        {
            if (DifficultySlider == null)
                return;
            DifficultySlider.Enabled = AIs[SelectedAIIndex].Item2;
        }
    }
}