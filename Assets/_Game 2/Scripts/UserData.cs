using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterModel
{
    Warrior,
    Brute,

}
public static class UserData
{
    public static int CurrentModelIndex
    {
        get { return PlayerPrefs.GetInt("Current_Model_Index",0);}
        set
        {
            PlayerPrefs.SetInt("Current_Model_Index", value);
        }
    }
}
