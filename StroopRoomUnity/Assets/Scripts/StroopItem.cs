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
* 
* The Stroop Room has been published as
* 
* Stefan Gradl, Markus Wirth, Nico Mächtlinger, Romina Poguntke, Andrea Wonner, Nicolas Rohleder, and Bjoern M. Eskofier. 2019.
* The Stroop Room: A Virtual Reality-Enhanced Stroop Test.
* In 25th ACM Symposium on Virtual Reality Software and Technology (VRST '19), Tomas Trescak, Simeon Simoff, Deborah Richards,
* Anton Bogdanovych, Thierry Duval, Torsten Kuhlen, Huyen Nguyen, Shigeo Morishima, Yuichi Itoh, Richard Skarbez,
* Anton Bogdanovych, and Martin Masek (Eds.). ACM, New York, NY, USA, Article 28, 12 pages.
* DOI: https://doi.org/10.1145/3359996.3364247
* 
* Please cite this work if you use this for your own research.
*/
using System;
using UnityEngine;

public class StroopItem
{
    public string Name { get; set; }
    public string ColorString { get; set; }
    public Color Color { get; set; }
    public string NameEn { get; set; }
    public string ColorEn { get; set; }

    string language = SaveInfo.GetLanguage().ToString();
    SystemLanguage lang = SaveInfo.GetLanguage();

    public enum StroopColors
    {
        Blue = 0,
        Red = 1,
        Green = 2,
        Yellow = 3,
        Orange = 4,
        Purple = 5
    }

    public static string[] colorWordsNl = { "BLAUW", "ROOD", "GROEN", "GEEL", "ORANJE", "PAARS" };
    public static string[] colorWordsEn = { "BLUE", "RED", "GREEN", "YELLOW", "ORANGE", "PURPLE" };
    public static string[] colorWordsGer = { "BLAU", "ROT", "GRÜN", "GELB", "ORANGE", "VIOLETT" };
    public static Color[] colors = { Color.blue, Color.red, Color.green, Color.yellow, new Color(1.0f, 126 / 255.0f, 0f, 1f), new Color(183 / 255.0f, 60 / 255.0f, 129 / 255.0f, 1f) };


    public StroopItem(string name, string colorString)
    {
        Name = name;
        ColorString = colorString;
        Color = StringToColor(colorString);
        //NameEn = NameEnglish(name);
        NameEn = NameInEnglish(name);
        //ColorEn = ColorEnglish(colorString);
        ColorEn = NameInEnglish(colorString);
    }

    private Color StringToColor(string col)
    {
        Color color = Color.black;
        string[] words = null;

        if (SaveInfo.GetLanguage().ToString() == "German")
            words = colorWordsGer;
        else if (SaveInfo.GetLanguage().ToString() == "English")
            words = colorWordsEn;
        else if (SaveInfo.GetLanguage().ToString() == "Dutch")
            words = colorWordsNl;

        int idx = Array.IndexOf(words, col);
        return colors[idx];
        
        /*if (SaveInfo.GetLanguage().ToString() == "German")
        {
            for (int i = 0; i < colorWordsGer.Length; i++)
            {
                if (col == colorWordsGer[i])
                {
                    color = colors[i];
                    break;
                }
            }
        }
        else if (SaveInfo.GetLanguage().ToString() == "English")
        {
            for (int i = 0; i < colorWordsEn.Length; i++)
            {
                if (col == colorWordsEn[i])
                {
                    color = colors[i];
                    break;
                }
            }
        }
        return color;*/
    }

    private string NameInEnglish(string name)
    {
        /*string nameEn = "";
        if (SaveInfo.GetLanguage().ToString() == "English")
        {
            nameEn = name;
        }
        else if (SaveInfo.GetLanguage().ToString() == "German")
        {
            for (int i = 0; i < colorWordsGer.Length; i++)
            {
                if (name == colorWordsGer[i])
                {
                    nameEn = colorWordsEn[i];
                    break;
                }
            }
        }
        return nameEn;*/


        string[] words = null;

        if (SaveInfo.GetLanguage().ToString() == "German")
            words = colorWordsGer;
        else if (SaveInfo.GetLanguage().ToString() == "English")
            return name;
        else if (SaveInfo.GetLanguage().ToString() == "Dutch")
            words = colorWordsNl;

        int idx = Array.IndexOf(words, name);
        return colorWordsEn[idx];
    }

    /*private string ColorEnglish(string color)
    {
        string colorEn = "";
        if (SaveInfo.GetLanguage().ToString() == "English")
        {
            colorEn = color;
        }
        else if (SaveInfo.GetLanguage().ToString() == "German")
        {
            for (int i = 0; i < colorWordsGer.Length; i++)
            {
                if (color == colorWordsGer[i])
                {
                    colorEn = colorWordsEn[i];
                    break;
                }
            }
        }
        return colorEn;
    }*/
}
