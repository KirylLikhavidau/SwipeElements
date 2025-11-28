using System;

[Serializable]
public class SaveData
{
    public int ActiveSavedScene;
    public int[] InitialLayout;

    public SaveData()
    {
        ActiveSavedScene = 0;
        InitialLayout = null;
    }
}