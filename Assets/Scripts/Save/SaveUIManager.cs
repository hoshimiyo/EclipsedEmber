using UnityEngine;

public class SaveUIManager : MonoBehaviour
{
    public static SaveUIManager instance;

    [SerializeField] private GameObject saveUIprefab;
    private SaveSystemUI saveUIinstance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Instantiate the save UI
        GameObject saveUIObj = Instantiate(saveUIprefab);
        saveUIObj.transform.SetParent(transform);
        saveUIinstance = saveUIObj.GetComponent<SaveSystemUI>();
    }

    public void ToggleSaveUI()
    {
        if (saveUIinstance.gameObject.activeInHierarchy &&
            saveUIinstance.gameObject.activeSelf)
        {
            saveUIinstance.Hide();
        }
        else
        {
            saveUIinstance.Show();
        }
    }

    // Show the UI (can be called from other scripts)
    public void ShowSaveUI()
    {
        saveUIinstance.Show();
    }

    // Hide the UI
    public void HideSaveUI()
    {
        saveUIinstance.Hide();
    }
}