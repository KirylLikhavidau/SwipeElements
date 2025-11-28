using Grid.Components;
using System.IO;
using UnityEngine;

public class DataSaver : MonoBehaviour
{
    private const string SaveFileName = "Save.json";

    private string _filePath;
    private SaveData _data;

    private bool _isLayoutRequested = false;

    private void Start()
    {
        _filePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        _data = Load();
    }

    private void Save(SaveData data)
    {
        var json = JsonUtility.ToJson(data);
        File.WriteAllText(_filePath, json);
    }

    private SaveData Load()
    {
        if (File.Exists(_filePath))
        {
            string json = File.ReadAllText(_filePath);

            if (string.IsNullOrEmpty(json))
            {
                return new SaveData();
            }

            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            return new SaveData();
        }
    }

    public void SaveGridLayout(GridSection[,] sections)
    {
        if (sections == null)
        {
            _data.InitialLayout = null;
            return;
        }

        _data.InitialLayout = new int[sections.GetLength(0) * sections.GetLength(1)];
        int layoutIndex = 0;

        for (int y = 0; y < sections.GetLength(1); y++)
            for (int x = 0; x < sections.GetLength(0); x++)
            {
                _data.InitialLayout[layoutIndex] = (int)sections[x, y].CurrentBlockType; 
                layoutIndex++;
            }
    }

    public void SaveSceneIndex(int sceneIndex)
    {
        _data.ActiveSavedScene = sceneIndex;
    }

    public int[] RequestInitialLayout()
    {
        if (_isLayoutRequested)
        {
            return null;
        }
        else
        {
            _isLayoutRequested = true;
            return _data.InitialLayout;
        }
    }

    public int RequestSceneIndex()
    {
        return _data.ActiveSavedScene;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save(_data);
        }
    }

    private void OnApplicationQuit()
    {
        Save(_data);
    }
}
