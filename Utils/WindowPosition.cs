using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class WindowPosition : MonoBehaviour 
{
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern IntPtr FindWindow(System.String className, System.String windowName);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    public static void SetPosition(string winName, int depth, int x, int y, int resX = 0, int resY = 0)
    {
        SetWindowPos(FindWindow(null, winName), depth, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
    }

    public string windowName = "App Name";
    public int posX = 0;
    public int posY = 0;
    public int resWid = 1920;
    public int resHei = 1080;
    public bool isFullscreen = false;
    public bool isTopMost = false;

    void Awake () 
	{
#if !UNITY_EDITOR
        int depth = 0;
        if (isTopMost) depth = -1;
        SetPosition(windowName, depth, posX, posY, resWid, resHei);
        SetForegroundWindow(FindWindow(null, windowName));
#endif
    }

    void Start ()
    {
        //StartCoroutine("SetFocusWindow");
    }

    IEnumerator SetFocusWindow()
    {
        yield return new WaitForSeconds(10.0f);

#if !UNITY_EDITOR
        int depth = 0;
        if (isTopMost) depth = -1;
        SetPosition(windowName, depth, posX, posY, resWid, resHei);
        SetForegroundWindow(FindWindow(null, windowName));
#endif

        StartCoroutine("SetFocusWindow");
    }

}
