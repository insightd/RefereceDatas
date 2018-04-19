using UnityEngine;
using System.Collections;

public class UseMultiScene : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}
