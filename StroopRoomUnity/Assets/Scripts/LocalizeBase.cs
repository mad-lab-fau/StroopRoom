/* 
* This file is part of the Stroop Room distribution (https://github.com/mad-lab-fau/StroopRoom).
* Copyright (c) 2019 Machine Learning and Data Analytics Lab, Friedrich-Alexander-Universität Erlangen-Nürnberg (FAU).
* 
* This program is free software: you can redistribute it and/or modify  
* it under the terms of the GNU General Public License as published by  
* the Free Software Foundation, version 3.
*
* This program is distributed in the hope that it will be useful, but 
* WITHOUT ANY WARRANTY; without even the implied warranty of 
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
* General Public License for more details.
*
* You should have received a copy of the GNU General Public License 
* along with this program. If not, see <http://www.gnu.org/licenses/>.
* 
* This file was copied from https://github.com/DFTGames/UnityLocalization.
*/
using UnityEngine;

/// <summary>
/// Class managing UI text localization. Language specific strings shall be saved following this
/// folder structure:
///
///     Resources/localization/English.txt 
///     Resources/localization/Italian.txt 
///     Resources/localization/Japanese.txt
///
/// ... and so on, where the file name (and consequently the resource name) is the string version of
/// the SystemLanguage enumeration.
///
/// The file format is as follows:
///
///     key=value
///
/// A TAB character is also accepted as key/value separator. 
/// In the value you can use the standard /// notation for newline: \n
/// </summary>

namespace DFTGames.Localization
{
    public abstract class LocalizeBase : MonoBehaviour
    {
        #region Public Fields

        public string localizationKey;

        #endregion Public Fields


        #region Public Properties


        #endregion Public Properties

        #region Private Properties


        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Update the value of the Text we are attached to.
        /// </summary>
        public abstract void UpdateLocale();

        protected virtual void Start()
        {
            // The first Text object getting here inits the CultureInfo data and loads the language file,
            // if any
            if (!Locale.currentLanguageHasBeenSet)
            {
                Locale.currentLanguageHasBeenSet = true;
                SetCurrentLanguage(Locale.PlayerLanguage);
            }
            UpdateLocale();
        }

        /// <summary>
        /// Returns the localized string for a given key
        /// </summary>
        /// <param name="key">the key to lookup</param>
        /// <returns></returns>
        public static string GetLocalizedString(string key)
        {
            if (Locale.CurrentLanguageStrings.ContainsKey(key))
                return Locale.CurrentLanguageStrings[key];
            else
                return string.Empty;
        }

        /// <summary>
        /// This is to set the language by code. It also update all the Text components in the scene.
        /// </summary>
        /// <param name="language">The new language</param>
        public static void SetCurrentLanguage(SystemLanguage language)
        {
            Locale.CurrentLanguage = language.ToString();
            Locale.PlayerLanguage = language;
            Localize[] allTexts = GameObject.FindObjectsOfType<Localize>();
           // LocalizeTM[] allTextsTM = GameObject.FindObjectsOfType<LocalizeTM>();
            for (int i = 0; i < allTexts.Length; i++)
                allTexts[i].UpdateLocale();
          /*  for (int i = 0; i < allTextsTM.Length; i++)
                allTextsTM[i].UpdateLocale();*/
        }




        #endregion Public Methods

    }
}