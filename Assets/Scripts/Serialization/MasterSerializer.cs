using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

public class MasterSerializer : MonoBehaviour {
    const string SAVE_DIRECTORY_PATH = "Saves";
    const string INVENTORY_SAVE_FILE_PATH = "Saves/inventory.binary";

    const string ENTITY_DATA_FILE_PATH = "/Data/JSON/EntityData.json";

    private void OnEnable()
    {
        InventoryController.OnInventoryUpdated += SaveInventoryData;
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

    public static JSONObject GetEntityDataByID(string entityID)
    {
        string entityDataString = File.ReadAllText(Application.dataPath + ENTITY_DATA_FILE_PATH);
        JSONObject entityData = new JSONObject(entityDataString)[entityID];
        if (entityData==null)
        {
            Debug.LogError("Entity data not found");
        }
        return entityData;
    }

    #endregion
}
