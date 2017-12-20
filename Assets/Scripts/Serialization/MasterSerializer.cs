using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

public class MasterSerializer : MonoBehaviour {
    const string SAVE_DIRECTORY_PATH = "Saves";
    const string SAVE_FILE_PATH = "Saves/inventory.binary";

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
        FileStream saveFile = File.Create(SAVE_FILE_PATH);

        formatter.Serialize(saveFile, inventory);

        saveFile.Close();
    }

    public static bool CanLoadInventoryData()
    {
        return File.Exists(SAVE_FILE_PATH);
    }

    public static InventoryData LoadInventoryData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream saveFile = File.Open(SAVE_FILE_PATH, FileMode.Open);

        InventoryData loadedInventory = (InventoryData)formatter.Deserialize(saveFile);

        saveFile.Close();
        return loadedInventory;
    }

    #endregion
}
