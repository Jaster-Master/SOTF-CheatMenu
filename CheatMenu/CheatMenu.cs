﻿using Il2Cpp;
using Il2CppSons.Gui;
using Il2CppSons.Input;
using Il2CppSons.Items.Core;
using Il2CppTheForest;
using Il2CppTheForest.Items.Inventory;
using Il2CppTheForest.Utils;
using MelonLoader;
using UnityEngine;

namespace CheatMenu;

public class CheatMenu : MelonMod
{
    private static readonly HashSet<FirstPersonCharacter> Persons = new();
    private static readonly HashSet<LocalPlayer> LocalPlayers = new();
    private static readonly HashSet<PlayerInventory> Inventories = new();
    private static readonly HashSet<SimpleMouseRotator> MouseRotators = new();
    private static readonly HashSet<PauseMenu> PauseMenus = new();
    private static bool _isCheatMenuShown;
    private static bool _isDebugConsoleShown;
    public static bool IsGodModeEnabled;
    public static bool IsInstantChopTreeEnabled;
    private static bool _isFlyModeEnabled;
    private static float _speedMultiplier = 1;
    private static float _jumpMultiplier = 1;
    private static bool _isAllItemsButtonPressed;
    private static bool _isResetButtonPressed;

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        ReadData();
        foreach (var pauseMenu in Resources.FindObjectsOfTypeAll<PauseMenu>())
        {
            PauseMenus.Add(pauseMenu);
        }
    }

    private static void ReadData()
    {
        ReadPersons();
        ReadLocalPlayers();
        ReadInventories();
        ReadMouseRotators();
    }

    public override void OnLateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.F1))
        {
            _isDebugConsoleShown = !_isDebugConsoleShown;
            DebugConsole.Instance.ShowConsole(_isDebugConsoleShown);
            DebugConsole.Instance.enabled = _isDebugConsoleShown;
        }

        if (Input.GetKeyUp(KeyCode.F10))
        {
            _isCheatMenuShown = !_isCheatMenuShown;
            LockCursor();
        }

        if (!_isCheatMenuShown)
        {
            ApplyFlyMode();
        }

        // Only in pause menu
        if (Input.GetKeyUp(KeyCode.F2))
        {
            foreach (var pauseMenu in PauseMenus)
            {
                pauseMenu.SetSaveMenuActive(!pauseMenu._saveMenuActive);
            }
        }
        else if (Input.GetKeyUp(KeyCode.F3))
        {
            foreach (var pauseMenu in PauseMenus)
            {
                pauseMenu.SetLoadMenuActive(!pauseMenu._loadMenuActive);
            }
        }
    }

    private static void LockCursor()
    {
        if (_isCheatMenuShown)
        {
            InputSystem.Cursor.Enable(true);
            foreach (var mouseRotator in MouseRotators)
            {
                mouseRotator.lockRotation = true;
            }
        }
        else
        {
            InputSystem.Cursor.Enable(false);
            foreach (var mouseRotator in MouseRotators)
            {
                mouseRotator.lockRotation = false;
            }
        }
    }

    public override void OnGUI()
    {
        if (_isCheatMenuShown)
        {
            ShowMenu();
        }
    }

    private static void ReadPersons()
    {
        foreach (var person in Resources.FindObjectsOfTypeAll<FirstPersonCharacter>())
        {
            Persons.Add(person);
        }

        var firstPerson = Persons.FirstOrDefault();
        ReadDefaultPersonValues(firstPerson);
    }

    private static void ReadLocalPlayers()
    {
        foreach (var localPlayer in Resources.FindObjectsOfTypeAll<LocalPlayer>())
        {
            LocalPlayers.Add(localPlayer);
        }
    }

    private static void ReadDefaultPersonValues(FirstPersonCharacter? person)
    {
        if (person == null) return;
        PersonValues.DefaultStaminaCost = person._runStaminaCostPerSec;
        PersonValues.DefaultSpeed = person._speed;
        PersonValues.DefaultWalkSpeed = person._walkSpeed;
        PersonValues.DefaultRunSpeed = person._runSpeed;
        PersonValues.DefaultSwimSpeed = person._swimSpeed;
        PersonValues.DefaultStrafeSpeed = person._strafeSpeed;
        PersonValues.DefaultCrouchSpeed = person._crouchSpeed;
        PersonValues.DefaultGravity = person.gravity;
        PersonValues.DefaultJumpMultiplier = person._jumpMultiplier;
        if (person.Stats == null) return;
        PersonValues.DefaultJumpStaminaCost = person.Stats._jumpStaminaCost;
        PersonValues.DefaultTired = person.Stats._tired;
        if (person.Stats._vitals == null) return;
        PersonValues.DefaultBaseHealth = person.Stats._vitals._health?._baseValue ?? 0;
        PersonValues.DefaultBaseVitality = person.Stats._vitals._vitality?._baseValue ?? 0;
        PersonValues.DefaultBaseFullness = person.Stats._vitals._fullness?._baseValue ?? 0;
        PersonValues.DefaultBaseHydration = person.Stats._vitals._hydration?._baseValue ?? 0;
        PersonValues.DefaultBaseStamina = person.Stats._vitals._stamina?._baseValue ?? 0;
        PersonValues.DefaultBaseStrength = person.Stats._vitals._strength?._baseValue ?? 0;
        PersonValues.DefaultBaseRested = person.Stats._vitals._rested?._baseValue ?? 0;
        PersonValues.DefaultBaseTemperature = person.Stats._vitals._temperature?._baseValue ?? 0;
    }

    private static void ReadInventories()
    {
        foreach (var inventory in Resources.FindObjectsOfTypeAll<PlayerInventory>())
        {
            Inventories.Add(inventory);
        }
    }

    private static void ReadMouseRotators()
    {
        foreach (var mouseRotator in Resources.FindObjectsOfTypeAll<SimpleMouseRotator>())
        {
            MouseRotators.Add(mouseRotator);
        }
    }

    private static void ShowMenu()
    {
        GUI.Box(new Rect(20, 20, 300, 450), "Cheat-Menu\nBy Jaster_Master");

        IsGodModeEnabled = GUI.Toggle(new Rect(30, 60, 280, 40), IsGodModeEnabled, "God-Mode");

        _isFlyModeEnabled = GUI.Toggle(new Rect(30, 100, 280, 40), _isFlyModeEnabled, "Fly-Mode");

        IsInstantChopTreeEnabled =
            GUI.Toggle(new Rect(30, 140, 280, 40), IsInstantChopTreeEnabled, "Instant chop tree");

        GUI.Label(new Rect(30, 180, 280, 40), "Speed-Multiplier:");
        _speedMultiplier = GUI.HorizontalSlider(new Rect(30, 220, 280, 40), _speedMultiplier, 0, 100);

        GUI.Label(new Rect(30, 260, 280, 40), "Jump-Multiplier:");
        _jumpMultiplier = GUI.HorizontalSlider(new Rect(30, 300, 280, 40), _jumpMultiplier, 0, 100);

        _isAllItemsButtonPressed = GUI.Button(new Rect(30, 340, 280, 40), "Get all items");

        _isResetButtonPressed = GUI.Button(new Rect(30, 380, 280, 40), "Reset all values");
        if (!GUI.changed) return;
        ApplyAll();
        CheckGetAllItemsButton();
        CheckResetButton();
    }

    private static void ApplyGodMode()
    {
        foreach (var person in Persons)
        {
            person._runStaminaCostPerSec = IsGodModeEnabled ? 0 : PersonValues.DefaultStaminaCost;
            if (person.Stats == null) continue;
            person.Stats._damageController.SetDamageEnabled(!IsGodModeEnabled);
            person.Stats._jumpStaminaCost = IsGodModeEnabled ? 0 : PersonValues.DefaultJumpStaminaCost;
            person.Stats._tired = IsGodModeEnabled ? 100 : PersonValues.DefaultTired;
            person.Stats._vitals._health._baseValue = IsGodModeEnabled
                ? person.Stats._vitals._health._max
                : PersonValues.DefaultBaseHealth;
            person.Stats._vitals._health._currentValue = IsGodModeEnabled
                ? person.Stats._vitals._health._max
                : person.Stats._vitals._health._currentValue;
            person.Stats._vitals._vitality._baseValue = IsGodModeEnabled
                ? person.Stats._vitals._vitality._max
                : PersonValues.DefaultBaseVitality;
            person.Stats._vitals._fullness._baseValue = IsGodModeEnabled
                ? person.Stats._vitals._fullness._max
                : PersonValues.DefaultBaseFullness;
            person.Stats._vitals._hydration._baseValue = IsGodModeEnabled
                ? person.Stats._vitals._hydration._max
                : PersonValues.DefaultBaseHydration;
            person.Stats._vitals._stamina._baseValue =
                IsGodModeEnabled
                    ? person.Stats._vitals._stamina._max
                    : PersonValues.DefaultBaseStamina;
            person.Stats._vitals._strength._baseValue = IsGodModeEnabled
                ? person.Stats._vitals._strength._max
                : PersonValues.DefaultBaseStrength;
            person.Stats._vitals._rested._baseValue =
                IsGodModeEnabled
                    ? person.Stats._vitals._rested._max
                    : PersonValues.DefaultBaseRested;
            person.Stats._vitals._temperature._baseValue = IsGodModeEnabled
                ? person.Stats._vitals._temperature._max
                : PersonValues.DefaultBaseTemperature;
        }
    }

    private static void ApplySpeed()
    {
        foreach (var person in Persons)
        {
            person._speed = PersonValues.DefaultSpeed * _speedMultiplier;
            person._walkSpeed = PersonValues.DefaultWalkSpeed * _speedMultiplier;
            person._runSpeed = PersonValues.DefaultRunSpeed * _speedMultiplier;
            person._swimSpeed = PersonValues.DefaultSwimSpeed * _speedMultiplier;
            person._strafeSpeed = PersonValues.DefaultStrafeSpeed * _speedMultiplier;
            person._crouchSpeed = PersonValues.DefaultCrouchSpeed * _speedMultiplier;
        }
    }

    private static void ApplyJump()
    {
        foreach (var person in Persons)
        {
            person._jumpMultiplier = _jumpMultiplier;
            person._setJumpMultiplierAfterJump = _jumpMultiplier;
        }
    }

    private static void ApplyFlyMode()
    {
        float xMove = 0;
        float yMove = 0;
        float zMove = 0;
        if (_isFlyModeEnabled)
        {
            if (Input.GetKey(KeyCode.W))
            {
                xMove = -0.1f;
            }

            if (Input.GetKey(KeyCode.S))
            {
                xMove = 0.1f;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                yMove = 0.1f;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                yMove = -0.1f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                zMove = 0.1f;
            }

            if (Input.GetKey(KeyCode.A))
            {
                zMove = -0.1f;
            }
        }

        foreach (var localPlayer in LocalPlayers)
        {
            if (localPlayer == null) continue;
            if (localPlayer._camFollowHead == null) continue;
            if (localPlayer._camFollowHead._cameraShakeController == null) continue;
            if (_isFlyModeEnabled)
            {
                localPlayer._camFollowHead._cameraShakeController.StopAllShakeAnimations();
            }
        }

        foreach (var person in Persons)
        {
            if (person._rigidbody == null) continue;
            person._rigidbody.useGravity = !_isFlyModeEnabled;
            person.gravity = _isFlyModeEnabled ? 0 : PersonValues.DefaultGravity;
            var personPosition = person.transform.position;
            person.AddMovement(new Vector2(xMove, zMove));
            person._rigidbody.MovePosition(new Vector3(personPosition.x, personPosition.y + yMove,
                personPosition.z));
        }
    }

    private static void CheckGetAllItemsButton()
    {
        if (!_isAllItemsButtonPressed) return;
        GetAllItems();
    }

    private static void GetAllItems()
    {
        foreach (var inventory in Inventories)
        {
            foreach (var item in ItemDatabaseManager._instance._itemDataList)
            {
                if (item == null) continue;
                if (item._editorName.ToLower().Contains("emergency")) continue;
                try
                {
                    inventory.AddItem(item._id, int.MaxValue, true);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }

    private static void CheckResetButton()
    {
        if (!_isResetButtonPressed) return;
        ResetValues();
    }

    private static void ResetValues()
    {
        _speedMultiplier = 1;
        _jumpMultiplier = PersonValues.DefaultJumpMultiplier;
        IsGodModeEnabled = false;
        _isFlyModeEnabled = false;
        IsInstantChopTreeEnabled = false;
        ApplyAll();
    }

    private static void ApplyAll()
    {
        ApplyGodMode();
        ApplySpeed();
        ApplyJump();
        ApplyFlyMode();
    }
}