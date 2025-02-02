﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public class WinterSpecificItems : Items, ISpecificItems
    {
        public WinterSpecificItems() : base() { }

        public Dictionary<int, SObject> GetItems()
        {
            return items;
        }

        protected override void AddItems()
        {
            AddForagables();
            AddFish();
        }

        private void AddForagables()
        {
            AddOneCommonObject(Constants.NAUTILUS_SHELL);
            AddOneCommonObject(Constants.WINTER_ROOT);
            AddOneCommonObject(Constants.CRYSTAL_FRUIT);
            AddOneCommonObject(Constants.SNOW_YAM);
            AddOneCommonObject(Constants.CROCUS);
            AddOneCommonObject(Constants.HOLLY);
        }

        private void AddFish()
        {
            AddFish(Constants.TUNA);
            AddFish(Constants.SARDINE);
            AddFish(Constants.PERCH);
            AddFish(Constants.PIKE);
            AddFish(Constants.RED_MULLET);
            AddFish(Constants.HERRING);
            AddFish(Constants.SQUID);
            AddFish(Constants.SEA_CUCUMBER);
            AddFish(Constants.STURGEON);
            AddFish(Constants.TIGER_TROUT);
            AddFish(Constants.ALBACORE);
            AddFish(Constants.LINGCOD);
            AddFish(Constants.RED_SNAPPER);
            AddFish(Constants.HALIBUT);

            if (Config.ShowAllFishFromCurrentSeason || (Game1.player.getEffectiveSkillLevel(1) > 6 && !Game1.player.fishCaught.ContainsKey(Constants.GLACIERFISH)))
            {
                AddFish(Constants.GLACIERFISH);
            }

            if (Config.ShowAllFishFromCurrentSeason || (Game1.Date.DayOfMonth > 14 && Game1.Date.DayOfMonth < 18))
            {
                AddFish(Constants.MIDNIGHT_SQUID);
                AddFish(Constants.SPOOK_FISH);
                AddFish(Constants.BLOBFISH);
            }
        }
    }
}
