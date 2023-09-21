#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ProjectVersionReader : MonoBehaviour
{
    private void Start()
    {
        var projectVersion = PlayerSettings.bundleVersion;
        if (!Config.PROJECT_VERSION.Equals(projectVersion))
        {
            Config.PROJECT_VERSION = projectVersion;
            Debug.Log("Project Version: " + projectVersion);
        }
        
    }
}
#endif