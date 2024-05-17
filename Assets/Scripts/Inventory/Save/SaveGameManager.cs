using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager Instance;
    public static SaveData data;

    private void Awake()
    {
        data = new SaveData();
        Instance = this;
        SaveLoad.OnLoadGame += LoadData;
        TryLoadDate();
    }

    private void Start()
    {
    }

    public void DeleteData()
    {
        SaveLoad.DeleteSaveData();
    }

    public void SaveData()
    {
        var saveData = data;
        SaveLoad.Save(saveData);
    }

    public static void LoadData(SaveData _data)
    {
        data = _data;
    }

    public void TryLoadDate()
    {
        SaveLoad.Load();
    }

    private void OnApplicationQuit()
    {
        //SaveData();
    }

}
