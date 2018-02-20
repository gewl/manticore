using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

public class MasterSerializer : MonoBehaviour {
    static string SAVE_DIRECTORY_PATH { get { return Application.dataPath + "/Saves"; } }
    static string INVENTORY_SAVE_FILE_PATH { get { return SAVE_DIRECTORY_PATH + "/inventory.binary"; } }
    static string MOMENTUM_SAVE_FILE_PATH { get { return SAVE_DIRECTORY_PATH + "/momentum.binary"; } }

    static string DATA_DIRECTORY_PATH { get { return Application.dataPath + "/Data"; } }
    static string HARDWARE_DESCRIPTIONS_DIRECTORY_PATH { get { return DATA_DIRECTORY_PATH + "/HardwareDescriptions"; } }

    static Dictionary<HardwareType, JSONObject> hardwareTypeToDescriptionsMap;

    private void Awake()
    {
        hardwareTypeToDescriptionsMap = new Dictionary<HardwareType, JSONObject>();
    }

    private void OnEnable()
    {
        InventoryController.OnInventoryUpdated += SaveInventoryData;
        MomentumManager.OnMomentumUpdated += SaveMomentumData;
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

    static JSONObject RetrieveDescriptionsObject(HardwareType hardwareType)
    {
        string hardwareDescriptionsObjectString = File.ReadAllText(HARDWARE_DESCRIPTIONS_DIRECTORY_PATH + "/" + hardwareType.ToString() + ".json");
        JSONObject hardwareDescriptionsObject = new JSONObject(hardwareDescriptionsObjectString);
        return hardwareDescriptionsObject;
    }
        
}
