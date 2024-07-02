using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LevelData
{
    public int level_number;
    public int grid_width;
    public int grid_height;
    public int move_count;
    public string[] grid;
}

public class LevelLoader : MonoBehaviour
{
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
        Debug.Log("Attempting to load level from URL: " + url);
        UnityWebRequest webRequest = UnityWebRequest.Get(url);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error downloading level data: " + webRequest.error);
        }
        else
        {
            string dataAsJson = webRequest.downloadHandler.text;
            Debug.Log("Downloaded JSON data: " + dataAsJson);

            if (string.IsNullOrEmpty(dataAsJson))
            {
                Debug.LogError("Downloaded JSON data is null or empty!");
                yield break;
            }

            try
            {
                currentLevelData = JsonUtility.FromJson<LevelData>(dataAsJson);
                if (currentLevelData == null)
                {
                    Debug.LogError("Parsed LevelData is null!");
                    yield break;
                }

                Debug.Log("Level: " + currentLevelData.level_number);
                // Load your level data into the game here
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Exception during JSON parsing: " + ex.Message);
            }
        }
    }
}
