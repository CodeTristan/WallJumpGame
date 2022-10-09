using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    //Ints
    PlayerPrefs MaxPoint;

    PlayerPrefs MaxJump;
    PlayerPrefs Coin;
    PlayerPrefs MaxCombo;
    PlayerPrefs MaxSpeed;


    //Floats
    PlayerPrefs MaxSlowTime;

    //Bools
    PlayerPrefs CanTurnSquare;
    PlayerPrefs CanTurnTriangle;

    //PowerUps
    PlayerPrefs slingShotPower;
    PlayerPrefs doublePointTimer;
    PlayerPrefs bomberTimer;

    public PlayerMovement player;

    public void SaveGame()
    {
        PlayerPrefs.SetInt("MaxJump", player.maxJumpCount);
        PlayerPrefs.SetInt("MaxCombo", player.MaxCombo);
        PlayerPrefs.SetInt("MaxSpeed", player.maxSpeed);
        PlayerPrefs.SetInt("Coin", player.Coin);
        PlayerPrefs.SetInt("slingShotPower", player.slingShotYPower);


        PlayerPrefs.SetFloat("MaxSlowTime", player.slowTime);
        PlayerPrefs.SetFloat("doublePointTimer", player.doublePointTimer);
        PlayerPrefs.SetFloat("bomberTimer", player.bomberTimer);

    }

    public void LoadGame()
    {
        player.maxJumpCount = PlayerPrefs.GetInt("MaxJump");
        player.MaxCombo = PlayerPrefs.GetInt("MaxCombo");
        player.maxSpeed = PlayerPrefs.GetInt("MaxSpeed");
        player.Coin = PlayerPrefs.GetInt("Coin");
        player.slingShotYPower = PlayerPrefs.GetInt("slingShotPower");

        player.slowTime = PlayerPrefs.GetFloat("MaxSlowTime");
        player.doublePointTimer = PlayerPrefs.GetFloat("doublePointTimer");
        player.bomberTimer = PlayerPrefs.GetFloat("bomberTimer");
    }

}
