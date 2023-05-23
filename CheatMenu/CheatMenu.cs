using Il2Cpp;
using Il2CppSons.Ai.Vail;
using Il2CppSons.Gui;
using Il2CppSons.Input;
using Il2CppSons.Items.Core;
using Il2CppTheForest;
using Il2CppTheForest.Items.Inventory;
using MelonLoader;
using UnityEngine;

namespace CheatMenu;

public class CheatMenu : MelonMod
{
    private static readonly HashSet<FirstPersonCharacter> Persons = new();
    private static readonly HashSet<PlayerInventory> Inventories = new();
    private static readonly HashSet<SimpleMouseRotator> MouseRotators = new();
    public static bool IsShown;
    public static bool IsGodModeEnabled;
    public static bool IsInstantChopTreeEnabled;
    private static bool _isFlyModeEnabled;
    public static bool IsInstantKillEnabled;
    private static float _speedMultiplier = 1;
    private static float _jumpMultiplier = 1;
    private static bool _isAllItemsButtonPressed;
    private static bool _isResetButtonPressed;

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        ReadData();
    }

    private static void ReadData()
    {
        ReadPersons();
        ReadInventories();
        ReadMouseRotators();
    }

    public override void OnLateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.F1))
        {
            DebugConsole.Instance.ShowConsole(!DebugConsole.Instance._showConsole);
        }

        if (Input.GetKeyUp(KeyCode.F10))
        {
            IsShown = !IsShown;
            LockCursor();
        }

        if (!IsShown)
        {
            ApplyFlyMode();
        }
    }

    private static void LockCursor()
    {
        if (IsShown)
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
        if (IsShown)
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

    private static void ReadDefaultPersonValues(FirstPersonCharacter? person)
    {
        if (person == null) return;
        DefaultValues.DefaultStaminaCost = person._runStaminaCostPerSec;
        DefaultValues.DefaultSpeed = person._speed;
        DefaultValues.DefaultWalkSpeed = person._walkSpeed;
        DefaultValues.DefaultRunSpeed = person._runSpeed;
        DefaultValues.DefaultSwimSpeed = person._swimSpeed;
        DefaultValues.DefaultStrafeSpeed = person._strafeSpeed;
        DefaultValues.DefaultCrouchSpeed = person.crouchSpeed;
        DefaultValues.DefaultGravity = person.gravity;
        DefaultValues.DefaultJumpMultiplier = person._jumpMultiplier;
        if (person.Stats == null) return;
        DefaultValues.DefaultJumpStaminaCost = person.Stats._jumpStaminaCost;
        DefaultValues.DefaultTired = person.Stats._tired;
        if (person.Stats._vitals == null) return;
        DefaultValues.DefaultBaseHealth = person.Stats._vitals._health?._baseValue ?? 0;
        DefaultValues.DefaultBaseVitality = person.Stats._vitals._vitality?._baseValue ?? 0;
        DefaultValues.DefaultBaseFullness = person.Stats._vitals._fullness?._baseValue ?? 0;
        DefaultValues.DefaultBaseHydration = person.Stats._vitals._hydration?._baseValue ?? 0;
        DefaultValues.DefaultBaseStamina = person.Stats._vitals._stamina?._baseValue ?? 0;
        DefaultValues.DefaultBaseStrength = person.Stats._vitals._strength?._baseValue ?? 0;
        DefaultValues.DefaultBaseRested = person.Stats._vitals._rested?._baseValue ?? 0;
        DefaultValues.DefaultBaseTemperatur = person.Stats._vitals._temperature?._baseValue ?? 0;
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
        GUI.Box(new Rect(20, 20, 300, 500), "Cheat-Menu\nBy Jaster_Master");

        IsGodModeEnabled = GUI.Toggle(new Rect(30, 60, 280, 40), IsGodModeEnabled, "God-Mode");

        _isFlyModeEnabled = GUI.Toggle(new Rect(30, 100, 280, 40), _isFlyModeEnabled, "Fly-Mode");

        // IsInstantKillEnabled = GUI.Toggle(new Rect(30, 140, 280, 40), IsInstantKillEnabled, "Instant-Kill");

        IsInstantChopTreeEnabled =
            GUI.Toggle(new Rect(30, 180, 280, 40), IsInstantChopTreeEnabled, "Instant chop tree");

        GUI.Label(new Rect(30, 220, 280, 40), "Speed-Multiplier:");
        _speedMultiplier = GUI.HorizontalSlider(new Rect(30, 260, 280, 40), _speedMultiplier, 0, 100);

        GUI.Label(new Rect(30, 300, 280, 40), "Jump-Multiplier:");
        _jumpMultiplier = GUI.HorizontalSlider(new Rect(30, 340, 280, 40), _jumpMultiplier, 0, 100);

        _isAllItemsButtonPressed = GUI.Button(new Rect(30, 380, 280, 40), "Get all items");

        _isResetButtonPressed = GUI.Button(new Rect(30, 420, 280, 40), "Reset all values");
        if (!GUI.changed) return;
        ApplyAll();
        CheckGetAllItemsButton();
        CheckResetButton();
    }

    private static void ApplyGodMode()
    {
        foreach (var person in Persons)
        {
            person._runStaminaCostPerSec = IsGodModeEnabled ? 0 : DefaultValues.DefaultStaminaCost;
            if (person.Stats == null) continue;
            person.Stats._damageController.SetDamageEnabled(!IsGodModeEnabled);
            person.Stats._jumpStaminaCost = IsGodModeEnabled ? 0 : DefaultValues.DefaultJumpStaminaCost;
            person.Stats._tired = IsGodModeEnabled ? 100 : DefaultValues.DefaultTired;
            person.Stats._vitals._health._baseValue = IsGodModeEnabled
                ? person.Stats._vitals._health._max
                : DefaultValues.DefaultBaseHealth;
            person.Stats._vitals._health._currentValue = IsGodModeEnabled
                ? person.Stats._vitals._health._max
                : person.Stats._vitals._health._currentValue;
            person.Stats._vitals._vitality._baseValue = IsGodModeEnabled
                ? person.Stats._vitals._vitality._max
                : DefaultValues.DefaultBaseVitality;
            person.Stats._vitals._fullness._baseValue = IsGodModeEnabled
                ? person.Stats._vitals._fullness._max
                : DefaultValues.DefaultBaseFullness;
            person.Stats._vitals._hydration._baseValue = IsGodModeEnabled
                ? person.Stats._vitals._hydration._max
                : DefaultValues.DefaultBaseHydration;
            person.Stats._vitals._stamina._baseValue =
                IsGodModeEnabled
                    ? person.Stats._vitals._stamina._max
                    : DefaultValues.DefaultBaseStamina;
            person.Stats._vitals._strength._baseValue = IsGodModeEnabled
                ? person.Stats._vitals._strength._max
                : DefaultValues.DefaultBaseStrength;
            person.Stats._vitals._rested._baseValue =
                IsGodModeEnabled
                    ? person.Stats._vitals._rested._max
                    : DefaultValues.DefaultBaseRested;
            person.Stats._vitals._temperature._baseValue = IsGodModeEnabled
                ? person.Stats._vitals._temperature._max
                : DefaultValues.DefaultBaseTemperatur;
        }
    }

    private static void ApplySpeed()
    {
        foreach (var person in Persons)
        {
            person._speed = DefaultValues.DefaultSpeed * _speedMultiplier;
            person._walkSpeed = DefaultValues.DefaultWalkSpeed * _speedMultiplier;
            person._runSpeed = DefaultValues.DefaultRunSpeed * _speedMultiplier;
            person._swimSpeed = DefaultValues.DefaultSwimSpeed * _speedMultiplier;
            person._strafeSpeed = DefaultValues.DefaultStrafeSpeed * _speedMultiplier;
            person.crouchSpeed = DefaultValues.DefaultCrouchSpeed * _speedMultiplier;
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
        float yMove = 0;
        if (_isFlyModeEnabled)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                yMove = 0.1f;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                yMove = -0.1f;
            }
        }

        foreach (var person in Persons)
        {
            if (person._rigidbody == null) continue;
            person._rigidbody.useGravity = !_isFlyModeEnabled;
            person.gravity = _isFlyModeEnabled ? 0 : DefaultValues.DefaultGravity;
            var personPosition = person.transform.position;
            person._rigidbody.MovePosition(new Vector3(personPosition.x, personPosition.y + yMove, personPosition.z));
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
        _jumpMultiplier = DefaultValues.DefaultJumpMultiplier;
        IsGodModeEnabled = false;
        _isFlyModeEnabled = false;
        IsInstantKillEnabled = false;
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