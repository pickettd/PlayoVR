﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using NetBase;

public class SetupPlayerAvatar : Photon.MonoBehaviour {
    [Tooltip("The avatar's left hand to sync with the left controller. If empty, a child named 'Left Hand' will be used.")]
    public GameObject LeftHand;
    [Tooltip("The avatar's right hand to sync with the right controller. If empty, a child named 'Right Hand' will be used.")]
    public GameObject RightHand;

    void Awake () {
        if (!photonView.isMine) {
            return;
        }
        // Move the camera rig to where the player was spawned
        VRTK_SDKManager sdk = VRTK_SDKManager.instance;
        sdk.loadedSetup.actualBoundaries.transform.position = transform.position;
        sdk.loadedSetup.actualBoundaries.transform.rotation = transform.rotation;

        // Add PhotonViewLink objects to the VR controller objects and link them to the avatar's hands
        LeftHand = LeftHand != null ? LeftHand : NetUtils.Find(gameObject, "Left Hand");
        RightHand = RightHand != null ? RightHand : NetUtils.Find(gameObject, "Right Hand");
        SetUpControllerHandLink(LeftHand, VRTK_DeviceFinder.Devices.LeftController);
        SetUpControllerHandLink(RightHand, VRTK_DeviceFinder.Devices.RightController);
    }

    private static void SetUpControllerHandLink(GameObject avatarComponent, VRTK_DeviceFinder.Devices device) {
        var photonView = avatarComponent.GetComponent<PhotonView>();
        if (photonView == null) {
            Debug.LogError(string.Format("The network representation '{0}' has no {1} component on it.", avatarComponent.name, typeof(PhotonView).Name));
            return;
        }

        GameObject controller = VRTK_DeviceFinder.DeviceTransform(device).gameObject;
        GameObject actual = VRTK_DeviceFinder.GetActualController(controller);
        var link = actual.AddComponent<PhotonViewLink>();
        link.linkedView = photonView;
    }
}
