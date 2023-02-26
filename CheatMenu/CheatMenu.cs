using Il2Cpp;
using Il2CppSons.Gui;
using Il2CppSons.Items.Core;
using Il2CppTheForest.Items.Inventory;
using MelonLoader;
using UnityEngine;

namespace CheatMenu;

public class CheatMenu : MelonMod
{
    private readonly List<FirstPersonCharacter> _persons = new();
    private readonly List<PlayerInventory> _inventories = new();
    private readonly List<PauseMenu> _pauseMenus = new();
    private bool _isShown;
    public static bool IsGodModeEnabled;
    private float _playerSpeed = 1;
    private bool _isAllItemsButtonPressed;
    private bool _isResetButtonPressed;

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        ReadPauseMenu();
        ReadPersons();
        ReadInventories();
    }

    private void ReadPauseMenu()
    {
        _pauseMenus.AddRange(Resources.FindObjectsOfTypeAll<PauseMenu>());
    }

    private void ReadPersons()
    {
        _persons.AddRange(Resources.FindObjectsOfTypeAll<FirstPersonCharacter>());
        var person = _persons.FirstOrDefault();
        if (person == null) return;
        DefaultValues.DefaultStaminaCost = person.staminaCostPerSec;
        DefaultValues.DefaultSpeed = person._speed;
        DefaultValues.DefaultWalkSpeed = person._walkSpeed;
        DefaultValues.DefaultRunSpeed = person._runSpeed;
        DefaultValues.DefaultSwimSpeed = person._swimSpeed;
        DefaultValues.DefaultStrafeSpeed = person._strafeSpeed;
        DefaultValues.DefaultCrouchSpeed = person.crouchSpeed;
        if (person.Stats == null) return;
        DefaultValues.DefaultJumpStaminaCost = person.Stats._jumpStaminaCost;
    }

    private void ReadInventories()
    {
        _inventories.AddRange(Resources.FindObjectsOfTypeAll<PlayerInventory>());
    }

    public override void OnLateUpdate()
    {
        var isPaused = _pauseMenus.Any(x => x._isActive);
        if (!isPaused)
        {
            _isShown = false;
            return;
        }

        if (Input.GetKeyUp(KeyCode.F10))
        {
            _isShown = !_isShown;
        }
    }

    public override void OnGUI()
    {
        if (_isShown)
        {
            ShowMenu();
        }
    }

    private void ShowMenu()
    {
        GUI.Box(new Rect(20, 20, 300, 500), "Cheat-Menu\nBy Jaster_Master");
        var wasGodModeEnabled = IsGodModeEnabled;
        IsGodModeEnabled = GUI.Toggle(new Rect(30, 60, 280, 40), IsGodModeEnabled, "God-Mode");
        GUI.Label(new Rect(30, 100, 280, 40), "Speed:");
        _playerSpeed = GUI.HorizontalSlider(new Rect(30, 140, 280, 40), _playerSpeed, 0, 100);
        _isAllItemsButtonPressed = GUI.Button(new Rect(30, 180, 280, 40), "Get all items");
        _isResetButtonPressed = GUI.Button(new Rect(30, 220, 280, 40), "Reset");
        if (wasGodModeEnabled != IsGodModeEnabled)
        {
            ApplyGodMode();
        }

        ApplySpeed();
        CheckGetAllItemsButton();
        CheckResetButton();
    }

    private void ApplyGodMode()
    {
        foreach (var person in _persons)
        {
            person.staminaCostPerSec = IsGodModeEnabled ? 0 : DefaultValues.DefaultStaminaCost;
            if (person.Stats == null) continue;
            person.Stats._damageController.SetDamageEnabled(!IsGodModeEnabled);
            person.Stats._jumpStaminaCost = IsGodModeEnabled ? 0 : DefaultValues.DefaultJumpStaminaCost;
        }
    }

    private void ApplySpeed()
    {
        foreach (var person in _persons)
        {
            person._speed = DefaultValues.DefaultSpeed * _playerSpeed;
            person._walkSpeed = DefaultValues.DefaultWalkSpeed * _playerSpeed;
            person._runSpeed = DefaultValues.DefaultRunSpeed * _playerSpeed;
            person._swimSpeed = DefaultValues.DefaultSwimSpeed * _playerSpeed;
            person._strafeSpeed = DefaultValues.DefaultStrafeSpeed * _playerSpeed;
            person.crouchSpeed = DefaultValues.DefaultCrouchSpeed * _playerSpeed;
        }
    }

    private void CheckGetAllItemsButton()
    {
        if (!_isAllItemsButtonPressed) return;
        GetAllItems();
    }

    private void GetAllItems()
    {
        foreach (var inventory in _inventories)
        {
            foreach (var item in ItemDatabaseManager._instance._itemDataList)
            {
                if (item == null) continue;
                try
                {
                    if (!item._editorName.ToLower().Contains("emergency"))
                    {
                        inventory.AddItem(item._id, 99, true);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }

    private void CheckResetButton()
    {
        if (!_isResetButtonPressed) return;
        ResetValues();
    }

    private void ResetValues()
    {
        _playerSpeed = 1;
        IsGodModeEnabled = false;
        ApplyGodMode();
        ApplySpeed();
    }
}