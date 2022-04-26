using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-200)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public bool singleSceneForAllLevels;
    public int startLevelCountForLoop;
    public bool isSDKComplete;
    public bool isThisLoaderScene;

    public GameObject[] levels;
    [HideInInspector]
    public int levelCountOfSDK;

    private const string level = "level";
    private const string gem = "gem";

    public int currentLevel;
    public float gemCountInLevel;
    public float gemCountTotal;
    public float gemCountTotalTemp;
    public float gemMultiplier;
    [HideInInspector]
    public bool isMultiplied;

    public bool isLevelStarted;
    public bool isLevelCompleted;
    public bool isLevelFailed;

    private List<int> levelNumbers;
    private int[] randomLevels;
    private int totalLevelCount;
    public GameObject loaderPanel;

    void Awake()
    {
        CreateInstance();

        CheckSDKCompletion();

        RandomizeLevels();

        AssignSaveLoadParameters();

        SelectLoadType();
    }

    void Start()
    {
        Application.targetFrameRate = 60;

        isLevelStarted = false;
        gemMultiplier = 1;
        isMultiplied = false;
    }

    private void CreateInstance()
    {
        // Create instance on awake
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void CheckSDKCompletion()
    {
        // Allocate first indexes in the build settings for SDK scenes
        if (isSDKComplete)
        {
            levelCountOfSDK = 2;
        }
        else
        {
            levelCountOfSDK = 0;
        }
    }

    private void RandomizeLevels()
    {
        // Decide total level count
        if (singleSceneForAllLevels)
        {
            totalLevelCount = levels.Length;
        }
        else
        {
            totalLevelCount = SceneManager.sceneCountInBuildSettings - levelCountOfSDK;
        }

        // Limit starting level count for the loop after main levels
        if (totalLevelCount <= startLevelCountForLoop)
        {
            startLevelCountForLoop = Mathf.Clamp(totalLevelCount - 1, 1, 100);
            // Debug.LogError("Start level of the loop can't be the last level");
        }

        // Define necessary lists for randomization
        levelNumbers = new List<int>();

        if (singleSceneForAllLevels)
        {
            for (int i = startLevelCountForLoop; i < levels.Length + 1; i++)
            {
                levelNumbers.Add(i);
            }

            randomLevels = new int[levels.Length - startLevelCountForLoop + 1];
        }
        else
        {
            for (int i = startLevelCountForLoop; i < totalLevelCount + 1; i++)
            {
                levelNumbers.Add(i);
            }

            randomLevels = new int[totalLevelCount - startLevelCountForLoop + 1];
        }

        // Store current state of the RNG
        Random.State originalRandomState = Random.state;

        if (singleSceneForAllLevels)
        {
            // Use a specific seed to get the same outcome every time
            Random.InitState(levels.Length);
        }
        else
        {
            // Use a specific seed to get the same outcome every time
            Random.InitState(totalLevelCount);
        }

        // Randomize level list for after manin levels
        for (int i = 0; i < randomLevels.Length; i++)
        {
            int randomIndex = Random.Range(0, levelNumbers.Count);

            if (levelNumbers.Count > 1)
            {
                while (randomIndex == randomLevels.Length - 1)
                {
                    randomIndex = Random.Range(0, levelNumbers.Count);
                }
            }

            randomLevels[i] = levelNumbers[randomIndex];

            levelNumbers.RemoveAt(randomIndex);
        }

        // Return the RNG to how it was before
        Random.state = originalRandomState;
    }

    private void AssignSaveLoadParameters()
    {
        // Set current level and gem count numbers by loading saved data
        if (!PlayerPrefs.HasKey(level))
        {
            PlayerPrefs.SetInt(level, 1);
        }

        if (!PlayerPrefs.HasKey(gem))
        {
            PlayerPrefs.SetInt(gem, 0);
        }

        currentLevel = PlayerPrefs.GetInt(level);
        gemCountTotal = PlayerPrefs.GetInt(gem);
    }
    private void SelectLoadType()
    {
        // Decide load method based on level management system
        if (isThisLoaderScene)
        {
            loaderPanel.SetActive(true);
            LevelLoad();
        }
        else if (singleSceneForAllLevels)
        {
            SelectLevel();
        }
    }
    private void LevelLoad()
    {
        // Decide which scene to load
        if (currentLevel > totalLevelCount)
        {
            currentLevel = randomLevels[(currentLevel - (startLevelCountForLoop - 1) - 1) % randomLevels.Length];
        }

        if (singleSceneForAllLevels && !isThisLoaderScene)
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
        else if (singleSceneForAllLevels && isThisLoaderScene)
        {
            SceneManager.LoadSceneAsync(levelCountOfSDK);
        }
        else
        {
            SceneManager.LoadSceneAsync(currentLevel + (levelCountOfSDK - 1));
        }
    }
    private void SelectLevel()
    {
        // Decide which level to load on the same scene
        if (currentLevel > totalLevelCount)
        {
            currentLevel = randomLevels[(currentLevel - (startLevelCountForLoop - 1) - 1) % randomLevels.Length];
        }

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(false);
        }

        levels[currentLevel - 1].SetActive(true);

        AssignSaveLoadParameters();
    }

    public void LevelComplete()
    {
        if (!(isLevelCompleted || isLevelFailed))
        {
            UIManager.instance.LevelCompleteForUI(0.5f);
            currentLevel++;
            gemCountTotalTemp = gemCountTotal;
            gemCountTotal += gemCountInLevel;
            Save();
            isLevelCompleted = true;
        }
    }

    public void LevelComplete(float multiplier)
    {
        if (!(isLevelCompleted || isLevelFailed))
        {
            UIManager.instance.LevelCompleteForUI(0.5f);
            currentLevel++;
            gemMultiplier = multiplier;
            gemCountInLevel *= gemMultiplier;
            isMultiplied = true;
            UIManager.instance.multiplierText.GetComponent<TextMeshProUGUI>().text = gemMultiplier + "X";
            gemCountTotalTemp = gemCountTotal;
            gemCountTotal += gemCountInLevel;
            Save();
            isLevelCompleted = true;
        }
    }

    public void LevelFail()
    {
        if (!(isLevelFailed || isLevelCompleted))
        {
            UIManager.instance.LevelFailForUI(0.5f);
            isLevelFailed = true;
        }
    }

    public void CollectGem(Vector3 position, int count)
    {
        if (UIManager.instance.gameHasGems)
        {
            Vector3 worldToScreen = Camera.main.WorldToScreenPoint(position);
            GameObject flyingGem = Instantiate(UIManager.instance.gemIcons[0], worldToScreen, Quaternion.identity, transform);
            flyingGem.transform.GetChild(0).gameObject.SetActive(false);
            flyingGem.AddComponent<GemIcon>().FlyGem(0, 2f, 1f);
            gemCountInLevel += count;
        }
    }

    private void Save()
    {
        PlayerPrefs.SetInt(level, currentLevel);
        PlayerPrefs.SetInt(gem, (int)gemCountTotal);
    }

    public void RestartLevel()
    {
        LevelLoad();
    }

    public void NextLevel()
    {
        LevelLoad();
    }
}
