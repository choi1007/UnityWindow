using System;
using UnityEngine;
using UnityWindowAPI;

public class WindowMessageBox : MonoBehaviour
{
    public void OnClickMessageBox()
    {
        var activeWindow = WindowAPI.GetActiveWindow();

        string message = "게임을 종료하시겠습니까?";
        string title = "게임 종료";
        var result = WindowAPI.MessageBox(activeWindow, message, title, (uint)(0x00000001L | 0x00000040L | 0x00040000L));
        
        switch(result)
        {
            case 1:
                {
                    // YES
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                    break;
                }
            case 2:
                {
                    // NO
                    break;
                }
        }
    }
}
