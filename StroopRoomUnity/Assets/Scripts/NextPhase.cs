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
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntEvent : UnityEvent<int>
{
}

public class PhaseEvent : UnityEvent<SaveInfo.StroopRoomPhase>
{
}

/// <summary>
/// Handles phase progressions in the Stroop Room.
/// </summary>
public class NextPhase : MonoBehaviour {

    public static SaveInfo.StroopRoomPhase phase;
    public int roomIndex;
    //public IntEvent nextPhase;
    public IntEvent room;
    public IntEvent nextPhase;
    //public int countForNext;
    //private int numStimuli;

    void Start () {
        //phase = 1;
        ProgressToNextPhase();
        roomIndex = SaveInfo.GetRoom();
        room.AddListener(EnableScript);
        room.Invoke(roomIndex);
        nextPhase.Invoke((int)phase);
    }

    void ProgressToNextPhase()
    {
        if (SaveInfo.phaseSequence == SaveInfo.StroopRoomSequence.CONGRUENT_INCONGRUENT)
        {
            if (phase <= SaveInfo.StroopRoomPhase.PREPARATION)
            {
                phase = SaveInfo.StroopRoomPhase.CONGRUENT_PRACTICE;
            }
            else
            {
                phase++;
            }
        }
        else if (SaveInfo.phaseSequence == SaveInfo.StroopRoomSequence.INCONGRUENT_CONGRUENT)
        {
            if (phase <= SaveInfo.StroopRoomPhase.PREPARATION)
                phase = SaveInfo.StroopRoomPhase.INCONGRUENT_PRACTICE;
            else if (phase == SaveInfo.StroopRoomPhase.INCONGRUENT_TRIAL)
                phase = SaveInfo.StroopRoomPhase.RESTING_BREAK;
            else if (phase == SaveInfo.StroopRoomPhase.RESTING_BREAK)
                phase = SaveInfo.StroopRoomPhase.CONGRUENT_PRACTICE;
            else if (phase == SaveInfo.StroopRoomPhase.CONGRUENT_TRIAL)
                phase = SaveInfo.StroopRoomPhase.RESTING_RECOVERY;
            else
                phase++;
        }
    }

    void Update ()
    {
        //ende
        if (phase == SaveInfo.StroopRoomPhase.RESTING_RECOVERY && nextPhase != null)
        {
            return;
        }

        //wechsel ab scene 1
        else if (phase != SaveInfo.StroopRoomPhase.NONE && SaveInfo.roomStatus.TransitionPhase && nextPhase != null)
        {
            ProgressToNextPhase();            
            Debug.Log("nextphase.Invoke " + phase);
            nextPhase.Invoke((int)phase);
        }
    }

    public void EnableScript(int room)
    {
        switch (room)
        {
            case 0:
                nextPhase.SetPersistentListenerState(0, UnityEventCallState.RuntimeOnly);
                nextPhase.SetPersistentListenerState(1, UnityEventCallState.Off);
                nextPhase.SetPersistentListenerState(2, UnityEventCallState.Off);
                nextPhase.SetPersistentListenerState(3, UnityEventCallState.Off);
                this.gameObject.GetComponent<RoomZero>().enabled = true;
                this.gameObject.GetComponent<RoomA>().enabled = false;
                this.gameObject.GetComponent<RoomB>().enabled = false;
                this.gameObject.GetComponent<RoomC>().enabled = false;
                break;
            case 1:
                nextPhase.SetPersistentListenerState(0, UnityEventCallState.Off);
                nextPhase.SetPersistentListenerState(1, UnityEventCallState.RuntimeOnly);
                nextPhase.SetPersistentListenerState(2, UnityEventCallState.Off);
                nextPhase.SetPersistentListenerState(3, UnityEventCallState.Off);
                this.gameObject.GetComponent<RoomZero>().enabled = false;
                this.gameObject.GetComponent<RoomA>().enabled = true;
                this.gameObject.GetComponent<RoomB>().enabled = false;
                this.gameObject.GetComponent<RoomC>().enabled = false;
                break;
            case 2:
                nextPhase.SetPersistentListenerState(0, UnityEventCallState.Off);
                nextPhase.SetPersistentListenerState(1, UnityEventCallState.Off);
                nextPhase.SetPersistentListenerState(2, UnityEventCallState.RuntimeOnly);
                nextPhase.SetPersistentListenerState(3, UnityEventCallState.Off);
                this.gameObject.GetComponent<RoomZero>().enabled = false;
                this.gameObject.GetComponent<RoomA>().enabled = false;
                this.gameObject.GetComponent<RoomB>().enabled = true;
                this.gameObject.GetComponent<RoomC>().enabled = false;
                break;
            case 3:
                nextPhase.SetPersistentListenerState(0, UnityEventCallState.Off);
                nextPhase.SetPersistentListenerState(1, UnityEventCallState.Off);
                nextPhase.SetPersistentListenerState(2, UnityEventCallState.Off);
                nextPhase.SetPersistentListenerState(3, UnityEventCallState.RuntimeOnly);
                this.gameObject.GetComponent<RoomZero>().enabled = false;
                this.gameObject.GetComponent<RoomA>().enabled = false;
                this.gameObject.GetComponent<RoomB>().enabled = false;
                this.gameObject.GetComponent<RoomC>().enabled = true;
                break;
            default:
                nextPhase.SetPersistentListenerState(0, UnityEventCallState.Off);
                nextPhase.SetPersistentListenerState(1, UnityEventCallState.Off);
                nextPhase.SetPersistentListenerState(2, UnityEventCallState.Off);
                this.gameObject.GetComponent<RoomZero>().enabled = false;
                this.gameObject.GetComponent<RoomA>().enabled = false;
                this.gameObject.GetComponent<RoomB>().enabled = false;
                break;
        }
    }
}
