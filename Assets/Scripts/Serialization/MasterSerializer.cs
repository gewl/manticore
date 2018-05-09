﻿using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterSerializer : MonoBehaviour {
    static string SAVE_DIRECTORY_PATH { get { return Application.dataPath + "/Saves"; } }
    static string INVENTORY_SAVE_FILE_PATH { get { return SAVE_DIRECTORY_PATH + "/inventory.binary"; } }
    static string MOMENTUM_SAVE_FILE_PATH { get { return SAVE_DIRECTORY_PATH + "/momentum.binary"; } }

    static string DATA_DIRECTORY_PATH { get { return Application.dataPath + "/Data"; } }
    static string HARDWARE_DESCRIPTIONS_DIRECTORY_PATH { get { return DATA_DIRECTORY_PATH + "/HardwareDescriptions"; } }
    static string RENEWABLE_DESCRIPTIONS_DIRECTORY_PATH { get { return DATA_DIRECTORY_PATH + "/RenewableDescriptions"; } }
    static string DIALOGUE_OBJECT_DIRECTORY_PATH { get { return DATA_DIRECTORY_PATH + "/DialogueText"; } }
    static string LEVEL_STATE_OBJECT_DIRECTORY_PATH { get { return DATA_DIRECTORY_PATH + "/LevelStates"; } }

    static string DIALOGUE_OBJECT_SUFFIX = "_Dialogue";
    static string LEVEL_STATE_OBJECT_SUFFIX = "_State";

    static Dictionary<HardwareType, JSONObject> hardwareTypeToDescriptionsMap;
    static Dictionary<RenewableTypes, JSONObject> renewableTypeToDescriptionsMap;

    static string _activeSceneName;
    static JSONObject _activeSceneObject;

    static JSONObject ActiveSceneObject
    {
        get
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (_activeSceneObject == null || _activeSceneName != currentSceneName)
            {
                string stateFileName = currentSceneName + "_State";
                string sceneObjectString = File.ReadAllText(LEVEL_STATE_OBJECT_DIRECTORY_PATH + "/" + stateFileName + ".json");
                _activeSceneObject = new JSONObject(sceneObjectString);
            }

            return _activeSceneObject;
        }
    }

    private void Awake()
    {
        hardwareTypeToDescriptionsMap = new Dictionary<HardwareType, JSONObject>();
        renewableTypeToDescriptionsMap = new Dictionary<RenewableTypes, JSONObject>();
    }

    private void OnEnable()
    {
        InventoryController.OnInventoryUpdated += SaveInventoryData;
        MomentumManager.OnMomentumUpdated += SaveMomentumData;
    }

    private void OnDisable()
    {
        InventoryController.OnInventoryUpdated -= SaveInventoryData;
        MomentumManager.OnMomentumUpdated -= SaveMomentumData;
    }

    #region Data serialization/retrieval
    public static void SaveInventoryData(InventoryData inventory)
    {
        if (!Directory.Exists(SAVE_DIRECTORY_PATH))
        {
            Directory.CreateDirectory(SAVE_DIRECTORY_PATH);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream saveFile = File.Create(INVENTORY_SAVE_FILE_PATH);

        formatter.Serialize(saveFile, inventory);

        saveFile.Close();
    }

    public static bool CanLoadInventoryData()
    {
        return File.Exists(INVENTORY_SAVE_FILE_PATH);
    }

    public static InventoryData LoadInventoryData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream saveFile = File.Open(INVENTORY_SAVE_FILE_PATH, FileMode.Open);

        InventoryData loadedInventory = (InventoryData)formatter.Deserialize(saveFile);

        saveFile.Close();
        return loadedInventory;
    }

    public static void SaveMomentumData(MomentumData momentumData)
    {
        if (!Directory.Exists(SAVE_DIRECTORY_PATH))
        {
            Directory.CreateDirectory(SAVE_DIRECTORY_PATH);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream saveFile = File.Create(MOMENTUM_SAVE_FILE_PATH);

        formatter.Serialize(saveFile, momentumData);

        saveFile.Close();
    }

    public static bool CanLoadMomentumData()
    {
        return File.Exists(MOMENTUM_SAVE_FILE_PATH);
    }

    public static MomentumData LoadMomentumData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream saveFile = File.Open(MOMENTUM_SAVE_FILE_PATH, FileMode.Open);

        MomentumData loadedInventory = (MomentumData)formatter.Deserialize(saveFile);

        saveFile.Close();
        return loadedInventory;
    }
    #endregion

    #region Hardware Descriptions
    public static string GetGeneralHardwareDescription(HardwareType hardwareType)
    {
        if (!hardwareTypeToDescriptionsMap.ContainsKey(hardwareType))
        {
            hardwareTypeToDescriptionsMap[hardwareType] = RetrieveDescriptionsObject(hardwareType);
        }
        JSONObject hardwareDescriptions = hardwareTypeToDescriptionsMap[hardwareType];
        string hardwareDescription = hardwareDescriptions["BaseDescription"].str + "\n\n" + hardwareDescriptions[hardwareType.ToString()].str + "\n\n" + hardwareDescriptions["GeneralPassiveDescription"].str;

        if (hardwareDescription == null)
        {
            hardwareDescription = "Hardware description not found";
            Debug.LogError(hardwareDescription);
        }

        return hardwareDescription;
    }

    public static string GetSpecificHardwareDescription(HardwareType hardwareType, HardwareType activeHardwareType)
    {
        if (!hardwareTypeToDescriptionsMap.ContainsKey(hardwareType))
        {
            hardwareTypeToDescriptionsMap[hardwareType] = RetrieveDescriptionsObject(hardwareType);
        }
        JSONObject hardwareDescriptions = hardwareTypeToDescriptionsMap[hardwareType];
        string hardwareDescription = hardwareDescriptions["BaseDescription"].str + "\n\n" + hardwareDescriptions[activeHardwareType.ToString()].str;

        if (hardwareDescription == null)
        {
            hardwareDescription = "Hardware description not found";
            Debug.LogError(hardwareDescription);
        }

        return hardwareDescription;
    }

    public static string GetRenewableDescription(RenewableTypes renewableType)
    {
        if (!renewableTypeToDescriptionsMap.ContainsKey(renewableType))
        {
            renewableTypeToDescriptionsMap[renewableType] = RetrieveDescriptionsObject(renewableType);
        }
        JSONObject renewableDescriptionObject = renewableTypeToDescriptionsMap[renewableType];
        string renewableDescription = renewableDescriptionObject["Body"].str;

        if (renewableDescription == null)
        {
            renewableDescription = "Renewable description not found";
            Debug.LogError(renewableDescription);
        }

        return renewableDescription;
    }

    static JSONObject RetrieveDescriptionsObject(HardwareType hardwareType)
    {
        string hardwareDescriptionsObjectString = File.ReadAllText(HARDWARE_DESCRIPTIONS_DIRECTORY_PATH + "/" + hardwareType.ToString() + ".json");
        JSONObject hardwareDescriptionsObject = new JSONObject(hardwareDescriptionsObjectString);
        return hardwareDescriptionsObject;
    }

    static JSONObject RetrieveDescriptionsObject(RenewableTypes renewableType)
    {
        string renewableDescriptionObjectString = File.ReadAllText(RENEWABLE_DESCRIPTIONS_DIRECTORY_PATH + "/" + renewableType.ToString() + ".json");
        JSONObject renewableDescriptionObject = new JSONObject(renewableDescriptionObjectString);
        return renewableDescriptionObject;
    }
    #endregion

    #region Dialogue
    public static JSONObject RetrieveDialogueObject(string conversationalPartnerID)
    {
        string dialogueObjectString = File.ReadAllText(DIALOGUE_OBJECT_DIRECTORY_PATH + "/" + conversationalPartnerID + DIALOGUE_OBJECT_SUFFIX + ".json");
        JSONObject dialogueObject = new JSONObject(dialogueObjectString);
        return dialogueObject;
    }
    #endregion

    #region Level states

    public static bool GetSceneState(string StateTag)
    {
        if (!ActiveSceneObject.HasField(StateTag))
        {
            Debug.LogError("State tag not found in scene state object: " + StateTag);
            return false;
        }
        bool tagState = ActiveSceneObject[StateTag].b;
        return tagState;
    }

    #endregion
}
