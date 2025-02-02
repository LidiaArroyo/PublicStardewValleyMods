﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace WhatAreYouMissing
{
    public class ModConfig
    {
        public SButton button { get; set; }
        public bool ShowItemsFromLockedPlaces { get; set; }
        public bool ShowAllFishFromCurrentSeason { get; set; }
        public bool ShowAllRecipes { get; set; }
        public bool AlwaysShowAllRecipes { get; set; }
        public int CommonAmount { get; set; }
        public int HighestQualityAmount { get; set; }

        public ModConfig()
        {
            button = SButton.F2;
            ShowItemsFromLockedPlaces = true;
            ShowAllFishFromCurrentSeason = false;
            ShowAllRecipes = false;
            AlwaysShowAllRecipes = false;
            CommonAmount = 5;
            HighestQualityAmount = 5;
        }
    }
}
