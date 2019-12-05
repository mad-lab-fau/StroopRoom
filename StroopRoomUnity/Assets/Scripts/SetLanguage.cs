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
*/
using DFTGames.Localization;
using UnityEngine;

public class SetLanguage : MonoBehaviour {


    public void SetGerman()
    {
        Localize.SetCurrentLanguage(SystemLanguage.German);
        SaveInfo.SetLang(SystemLanguage.German);
    }

    public void SetEnglish()
    {
        Localize.SetCurrentLanguage(SystemLanguage.English);
        SaveInfo.SetLang(SystemLanguage.English);
    }

    public void SetDutch()
    {
        Localize.SetCurrentLanguage(SystemLanguage.Dutch);
        SaveInfo.SetLang(SystemLanguage.Dutch);
    }

    /* public void SetSpanish()
     {
         Localize.SetCurrentLanguage(SystemLanguage.Spanish);
         SaveInfo.SetLang(SystemLanguage.Spanish);
     }

     public void SetItalian()
     {
         Localize.SetCurrentLanguage(SystemLanguage.Italian);
         SaveInfo.SetLang(SystemLanguage.Italian);
     }

     public void SetFrench()
     {
         Localize.SetCurrentLanguage(SystemLanguage.French);
         SaveInfo.SetLang(SystemLanguage.French);
     }*/
}
