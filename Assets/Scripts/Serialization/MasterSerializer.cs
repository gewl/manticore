using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

public class MasterSerializer : MonoBehaviour {
    const string SAVE_DIRECTORY_PATH = "Saves";
    const string INVENTORY_SAVE_FILE_PATH = "Saves/inventory.binary";
    const string MOMENTUM_SAVE_FILE_PATH = "Saves/momentum.binary";

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
}
