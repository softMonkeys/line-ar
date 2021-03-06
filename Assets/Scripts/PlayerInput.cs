﻿using System;
using System.Collections;
using System.Collections.Generic;
using LineAR;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Input manager class
/// </summary>
public class PlayerInput : MonoBehaviour {
    // Should have an instance of the player.
    [SerializeField] private MeshGenerator player;
    [SerializeField] private bool isAlive = true;
    [SerializeField] private GameObject rocket;

    private float horizontal;

    [SerializeField] private float bombTimer = 1.5f, bombTicker = 0;
    [SerializeField] private bool isReadyForBomb = true;

    [SerializeField] private GameObject canvasIndicator;

    public float Horizontal
    {
        get => horizontal;
        set => horizontal = value;
    }

    public bool IsAlive
    {
        get => isAlive;
        set {
            if (value == false) {
                player.StartGrow = false;
            }
        }
    }

    public MeshGenerator Player
    {
        get => player;
        set => player = value;
    }

    private void OnEnable() {
        // Subscribe to powerup
        PlayerPowerUpUI.OnPowerUpPressed += LaunchBomb;
        
    }

    private void OnDisable() {
        PlayerPowerUpUI.OnPowerUpPressed -= LaunchBomb;
    }

    private void LaunchBomb() {
        // Instantiate bomb
        if (!isReadyForBomb) return;
        var bomb = Instantiate(rocket, player.RbCircle.transform.position + (player.RbCircle.transform.forward * 0.05f),
            Quaternion.Euler(player.RbCircle.transform.rotation.eulerAngles));
        isReadyForBomb = false;
        HUD.Instance.TogglePowerUp(false);

    }

    private void Start() {
        player = GetComponent<MeshGenerator>();
        player.IsPlayer = true;
        
        canvasIndicator.transform.SetParent(player.RbCircle.transform);
        
    }

    private void Update() {
        bombTicker += Time.deltaTime;
        if (bombTicker >= bombTimer) {
            isReadyForBomb = true;
            bombTicker = 0;
            HUD.Instance.TogglePowerUp(true);
        }

        // Testing in Editor alone
#if UNITY_EDITOR_64 || UNITY_EDITOR || UNITY_EDITOR_OSX
        if (Input.GetButtonDown("Jump")) {
            // Create bomb
            if (isAlive)
                LaunchBomb();
        }
#endif
        // Platform specific input.
#if UNITY_EDITOR_OSX || UNITY_EDITOR || UNITY_EDITOR_64
        horizontal = Input.GetAxis("Horizontal");
#endif
        // control input for mobile.
#if UNITY_ANDROID
        if (!player.StartGrow) return;
        if (Input.touchCount > 0) {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began || t.phase == TouchPhase.Stationary || t.phase != TouchPhase.Ended) {
                if (t.position.x < Screen.width / 2) {
                    horizontal = -1;
                }
                else if (t.position.x > Screen.width / 2) {
                    horizontal = 1;
                }
            }
            else {
                horizontal = 0;
            }
        }
#endif
    }
}