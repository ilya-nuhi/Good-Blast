using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;

public class LevelData
{
    public int level_number;
    public int grid_width;
    public int grid_height;
    public int move_count;
    public string[] rand_colors;
    public string[][] grid;
}

public class LevelLoader : Singleton<LevelLoader>
{
    public delegate void OnLevelDataLoaded(LevelData levelData);
    public static event OnLevelDataLoaded LevelDataLoaded;
    [SerializeField] string serverUrl = "https://ilya-nuhi.github.io/GoodBlast-leveldata/"; // Base URL of my Github Page
    [SerializeField] int levelNumber = 1; // The level we want to get
    public LevelData currentLevelData;
    void Start()
    {
        string url = serverUrl + "Level_" + levelNumber + ".json";
        StartCoroutine(LoadLevel(url));
    }

    IEnumerator LoadLevel(string url)
    {
        //Debug.Log("Attempting to load level from URL: " + url);
        UnityWebRequest webRequest = UnityWebRequest.Get(url);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error downloading level data: " + webRequest.error);
            yield break;
        }
        else
        {
            string dataAsJson = webRequest.downloadHandler.text;

            if (string.IsNullOrEmpty(dataAsJson))
            {
                Debug.LogError("Downloaded JSON data is null or empty!");
                yield break;
            }
            currentLevelData = JsonConvert.DeserializeObject<LevelData>(dataAsJson);
                
            if (currentLevelData == null)
            {
                Debug.LogError("Parsed LevelData is null!");
                yield break;
            }

            if(currentLevelData.grid_width > 10 || currentLevelData.grid_width < 2){
                Debug.LogError("Fetched level grid width is out of the bonds! Needs to be 2 to 10.");
            }
            if(currentLevelData.grid_height > 10 || currentLevelData.grid_height < 2){
                Debug.LogError("Fetched level grid height is out of the bonds! Needs to be 2 to 10.");
            }

            // Notify all listeners that the level data is loaded
            LevelDataLoaded?.Invoke(currentLevelData);
            
        }
    }
}
