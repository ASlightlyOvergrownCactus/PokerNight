using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;

namespace PokerNight;
[BepInPlugin(GUID: MOD_ID, Name: MOD_NAME, Version: VERSION)]
sealed class Plugin : BaseUnityPlugin
{
    public const string MOD_ID = "cactus.poker";
    public const string MOD_NAME = "PokerNight";
    public const string VERSION = "0.0";
    public const string AUTHORS = "ASlightlyOverGrownCactus";


    public void OnEnable()
    {
        try
        {
            On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError(e);
            throw new Exception("Exception from PokerNight: " + e);
        }
    }

    private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            Debug.Log("haiiiiiiii :3");
            PokerDoor.__RegisterPokerDoor();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogException(e);
            throw new Exception("Error trying to load OnModsInit PokerWorld 0.0");
        }
    }
}
