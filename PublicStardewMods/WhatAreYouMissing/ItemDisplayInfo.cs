﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public class ItemDisplayInfo
    {
        private enum SeasonIndex
        {
            Spring = 4,
            Summer = 5,
            Fall = 6,
            Winter = 7
        };

        private SObject Item;
        private int ParentSheetIndex;
        private Dictionary<int, string> FishData;
        private Dictionary<string, string> LocationData;
        public ItemDisplayInfo(SObject item)
        {
            Item = item;
            ParentSheetIndex = item.ParentSheetIndex;
            FishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            LocationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
        }

        public string GetItemDisplayInfo()
        {
            if (Item.Category == SObject.FishCategory)
            {
                return GetFishItemDisplayInfo();
            }
            else if(Item.Category == SObject.CookingCategory)
            {
                return GetCookedItemDisplayInfo();
            }
            else
            {
                return "";
            }
        }

        private string GetFishItemDisplayInfo()
        {
            Constants constants = new Constants();

            if (constants.LEGNEDARY_FISH_INFO.ContainsKey(ParentSheetIndex))
            {
                return constants.LEGNEDARY_FISH_INFO[ParentSheetIndex];
            }
            else if (constants.NIGHT_MARKET_FISH.Contains(ParentSheetIndex))
            {
                return Utilities.GetTranslation("NIGHT_MARKET");
            }
            else if (IsFromACrabPot())
            {
                return GetCrabPotDisplayInfo();
            }
            else
            {
                return GetNormalFishDisplayInfo();
            }
        }

        private string GetNormalFishDisplayInfo()
        {
            string displayInfo = "";
            string periodToCatch = GetAllPeriodsToCatchDisplayInfo();
            string weather = GetWeatherDisplayInfoForFish();

            List<string> seasons = GetSeasonsToCatch();
            List<string> locations = GetFishLocations();

            foreach (string location in locations)
            {
                displayInfo += location + ", ";
            }

            foreach (string season in seasons)
            {
                displayInfo += season + ", ";
            }

            return displayInfo + weather + ", " + periodToCatch;
        }

        private string GetCookedItemDisplayInfo()
        {
            Dictionary<int, SObject> recipeIngredients = ModEntry.RecipesIngredients.GetRecipeIngredients(ParentSheetIndex);

            string displayInfo = Utilities.GetTranslation("INGREDIENTS") + ": ";

            foreach (KeyValuePair<int, SObject> ingredient in recipeIngredients)
            {
                SObject ingredientObj = ingredient.Value;
                string displayName = GetIngredientDisplayName(ingredient.Key, ingredientObj);
                if (IsMissingIngredient(ingredient.Key))
                {
                    displayInfo += "~" + ingredientObj.Stack.ToString() + " " + displayName + ", ";
                }
                else
                {
                    displayInfo += ingredientObj.Stack.ToString() + " " + displayName  +  ", ";
                }
            }
            return displayInfo.Substring(0, displayInfo.Length - 2);
        }

        private bool IsMissingIngredient(int ingredientParentSheetIndex)
        {
            Dictionary<int, Dictionary<int, SObject>> missingIngredients = ModEntry.MissingItems.GetMissingRecipeIngredients();

            if (missingIngredients.ContainsKey(ParentSheetIndex))
            {
                return missingIngredients[ParentSheetIndex].ContainsKey(ingredientParentSheetIndex);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// The key is the "parent sheet index" from CookingRecipies.json
        /// It can also be a category which means the SObject wouldn't 
        /// have the correct parent sheet index since it would be meaningless
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ingredient"></param>
        /// <returns></returns>
        private string GetIngredientDisplayName(int key, SObject ingredient)
        {
            if (IsSepcialCookingItem(key))
            {
                return GetSpecialCookingItemDisplayName(key);
            }
            else
            {
                return ingredient.DisplayName;
            }
        }

        private string GetSpecialCookingItemDisplayName(int itemCategory)
        {
            switch (itemCategory)
            {
                case Constants.FISH_CATEGORY:
                    return Utilities.GetTranslation("ANY_FISH");
                case Constants.EGG_CATEGORY:
                    return Utilities.GetTranslation("ANY_EGG");
                case Constants.MILK_CATEGORY:
                    return Utilities.GetTranslation("ANY_MILK");
                default:
                    //should never get here
                    return "Oopsies";
            }
        }

        private bool IsSepcialCookingItem(int itemCategory)
        {
            Constants constants = new Constants();
            return constants.SPECIAL_COOKING_IDS.Contains(itemCategory);
        }

        private List<string> GetSeasonsToCatch()
        {
            List<string> seasons = new List<string>();
            foreach (KeyValuePair<string, string> data in LocationData)
            {
                for (int season = (int)SeasonIndex.Spring; season < (int)SeasonIndex.Winter; ++season)
                {
                    string[] seasonalFish = data.Value.Split('/')[season].Split(' ');
                    string seasonStr = GetTranslatedSeason((SeasonIndex)season);
                    if (!seasons.Contains(seasonStr) && seasonalFish.Contains(ParentSheetIndex.ToString()))
                    {
                        seasons.Add(seasonStr);
                    }
                }
            }
            return seasons;
        }

        private string GetTranslatedSeason(SeasonIndex seasonIndex)
        {
            switch (seasonIndex)
            {
                case SeasonIndex.Spring:
                    return Utilities.GetTranslation("SPRING");
                case SeasonIndex.Summer:
                    return Utilities.GetTranslation("SUMMER");
                case SeasonIndex.Fall:
                    return Utilities.GetTranslation("FALL");
                case SeasonIndex.Winter:
                    return Utilities.GetTranslation("WINTER");
                default:
                    return "Oopsies";
            }
        }

        private string GetCrabPotDisplayInfo()
        {
            if(FishData[ParentSheetIndex].Split('/')[4] == "ocean")
            {
                return Utilities.GetTranslation("OBTAINED_FROM_CRAB_POT_IN_OCEAN");
            }
            else
            {
                return Utilities.GetTranslation("OBTAINED_FROM_CRAB_POT_IN_FRESHWATER");
            }
        }

        private bool IsFromACrabPot()
        {
            return FishData[ParentSheetIndex].Split('/')[1] == "trap";
        }

        private List<string> GetFishLocations()
        {
            List<string> possibleLocations = new List<string>();

            foreach (KeyValuePair<string, string> data in LocationData)
            {
                for(int season = (int)SeasonIndex.Spring; season < (int)SeasonIndex.Winter; ++season)
                {
                    string[] seasonalFish = data.Value.Split('/')[season].Split(' ');
                    if (seasonalFish.Contains(ParentSheetIndex.ToString()))
                    {
                        int areaCode = GetAreaCode(seasonalFish);

                        string[] locationDisplayNames = GetLocationDisplayNames(data.Key, areaCode);
                        foreach (string name in locationDisplayNames)
                        {
                            if (name != "" && !possibleLocations.Contains(name))
                            {
                                possibleLocations.Add(name);
                            }
                        }
                    }
                }
            }

            //The forest farm pond is not stored in locations data and 
            //it is the same as the cindersap pond but with the addition
            //of the woodskip
            if(ParentSheetIndex == Constants.WOODSKIP)
            {
                possibleLocations.Add(Utilities.GetTranslation("FOREST_FARM_POND_DIAPLAY_NAME"));
            }

            return possibleLocations;
        }

        private int GetAreaCode(string[] data)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                if(data[i] == ParentSheetIndex.ToString())
                {
                    return ParseAreaCode(data[i + 1]);
                }
            }
            //Should never get here
            return Constants.DEFAULT_AREA_CODE;
        }

        private int ParseAreaCode(string code)
        {
            bool successful = int.TryParse(code, out int areaCode);
            if (!successful)
            {
                ModEntry.Logger.LogAreaCodeParseFail(code);
                return Constants.DEFAULT_AREA_CODE;
            }
            return areaCode;
        }

        private string[] GetLocationDisplayNames(string gameName, int areaCode)
        {
            LocationDisplayNames locationDisplayNames = new LocationDisplayNames();
            return locationDisplayNames.GetLocationDisplayNames(gameName, areaCode);
        }

        private string GetWeatherDisplayInfoForFish()
        {
            string weather = FishData[ParentSheetIndex].Split('/')[7];
            switch (weather)
            {
                case "sunny":
                    return Utilities.GetTranslation("SUNNY_WEATHER");
                case "rainy":
                    return Utilities.GetTranslation("RAINY_WEATHER");
                case "both":
                    return Utilities.GetTranslation("ANY_WEATHER");
                default:
                    return "Oopsies";
            }
        }

        private string GetAllPeriodsToCatchDisplayInfo()
        {
            string periodsToCatch = GetPeriodToCatchDisplayInfo(0);
            string[] timesToCatch = FishData[ParentSheetIndex].Split('/')[5].Split(' ');

            for (int i = 2; i < timesToCatch.Length; ++i)
            {
                if(i % 2 == 0)
                {
                    periodsToCatch = periodsToCatch + ", " + GetPeriodToCatchDisplayInfo(i);
                }
            }
            return periodsToCatch;
        }

        private string GetPeriodToCatchDisplayInfo(int startTimeIndex)
        {
            string[] timesToCatch = FishData[ParentSheetIndex].Split('/')[5].Split(' ');

            string earliestTime = Convert24TimeTo12(timesToCatch[startTimeIndex]);
            string latestTime = Convert24TimeTo12(timesToCatch[startTimeIndex + 1]);

            if (earliestTime == "6:00 am" && latestTime == "2:00 am")
            {
                return Utilities.GetTranslation("ANYTIME");
            }
            else
            {
                return earliestTime + " - " + latestTime;
            }
        }

        private string Convert24TimeTo12(string twentyFourHourTime)
        {
            bool successful = int.TryParse(GetImportantDigits(twentyFourHourTime), out int importantDigits);
            
            if(!successful)
            {
                ModEntry.Logger.LogTimeParseFail(twentyFourHourTime);
                return "";
            }

            if (importantDigits < 12)
            {
                return importantDigits.ToString() + ":" + "00 am";
            }
            else if(importantDigits == 12)
            {
                return importantDigits.ToString() + ":" + "00 pm";
            }
            else if (importantDigits < 24)
            {
                return (importantDigits - 12).ToString() + ":" + "00 pm";
            }
            else if (importantDigits == 24)
            {
                return (importantDigits - 12).ToString() + ":" + "00 am";
            }
            else
            {
                return (importantDigits % 12).ToString() + ":" + "00 am";
            }
        }

        private string GetImportantDigits(string twentyFourHourTime)
        {
            return twentyFourHourTime.Substring(0, twentyFourHourTime.Length - 2);
        }
    }
}
