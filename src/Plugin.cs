﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using System.Security.Permissions;
using UnityEngine;
using RWCustom;
using System.Security;
using System.Reflection;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace PokerNight;
[BepInPlugin(GUID: MOD_ID, Name: MOD_NAME, Version: VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string MOD_ID = "cactus.poker";
    public const string MOD_NAME = "PokerNight";
    public const string VERSION = "0.0";
    public const string AUTHORS = "ASlightlyOverGrownCactus";

    private static bool loaded = false;
    public static FAtlas mainAtlas;
    public static List<FAtlasElement> elements = new List<FAtlasElement>();
    public PokerGame game = new PokerGame();

    public void OnEnable()
    {
        try
        {
            On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
            On.RainWorldGame.Update += RainWorldGameOnUpdate;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError(e);
            throw new Exception("Exception from PokerNight: " + e);
        }
    }

    private void RainWorldGameOnUpdate(On.RainWorldGame.orig_Update orig, RainWorldGame self)
    {
        orig(self);
        if (RWInput.PlayerInput(0, self.rainWorld).jmp)
            game.DebugHand();
        //self.paused = !self.paused;
    }

    public void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            Debug.Log("haiiiiiiii :3");
            PokerDoor.__RegisterPokerDoor();
            mainAtlas ??= Futile.atlasManager.LoadAtlas("atlases/door");
            if (mainAtlas._elementsByName.TryGetValue("mydoor", out var element))
                elements.Add(element);
            
            if (mainAtlas == null)
            {
                Debug.LogError("Door not loading!!!! Please reinstall :3");
            }
            loaded = true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogException(e);
            throw new Exception("Error trying to load OnModsInit PokerWorld 0.0");
        }
    }
}
