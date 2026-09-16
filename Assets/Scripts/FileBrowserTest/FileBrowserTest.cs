using UnityEngine;
using SFB;

public class FileBrowserTest : MonoBehaviour
{
    //functional base working that shows file path
    public void OpenFile()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel(
            "Select a File",
            "",
            "",
            false
        );

        if (paths.Length > 0)
        {
            Debug.Log("Selected file: " + paths[0]);
        }
    }
}
