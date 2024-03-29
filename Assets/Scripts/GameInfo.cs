﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Big static class full of shared info about the current game state.

public static class GameInfo
{
    public static Player Player;
    public static int Score;
    public static float TimeToBossSpawn = 20f;
    public static float RunTime;
    public static int LevelNumber;
    public static float Difficulty = 1;
    public static bool Paused;
    public static GameState State;

    private static List<Card> _savedDeck;

    public enum GameState
    {
        Active,
        Victory,
        Failure
    }
    
    public static float GetDifficultyModifier()
    {
        float timeFactor = RunTime / 180f;
        float levelFactor = LevelNumber;
        return (timeFactor + levelFactor) * Difficulty;
    }

    public static void NewLevel()
    {
        SaveDeck();
        LevelNumber++;
        TimeToBossSpawn += 180f;
    }

    public static void Reset(float difficulty)
    {
        Difficulty = difficulty;
        TimeToBossSpawn = 180f;
        LevelNumber = 1;
        RunTime = 0f;
        Score = 0;
        _savedDeck = null;
    }

    public static void SaveDeck()
    {
        Debug.Log("Saved Deck");
        _savedDeck = Player.deck.GetCards();
    }

    public static List<Card> LoadSavedDeck()
    {
        
        return _savedDeck;
    }
}

