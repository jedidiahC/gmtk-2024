using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTag : MonoBehaviour
{
    private string _sceneName;
    public string sceneName { get { return _sceneName; } set { _sceneName = value; } }
}
