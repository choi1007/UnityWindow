using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityWindowAPI;

/// <summary>
/// Unity Window 빌드 시, X버튼에 대한 예외처리를 한다.
/// x 버튼을 눌렀을 때, 확인 팝업이 나오고, 확인을 누르면 종료하게 유도한다.
/// 해당 내용을 적용하려면 다른 클래스에서 OnApplicationQuit 함수를 사용할 때,
/// _isQuit의 변수를 static 으로 변경하여 해당 내용을 받아 false일때 return 시키거나 
/// 예외처리를 해주어야 한다.
/// </summary>
public class WindowExit : MonoBehaviour
{
    [SerializeField]
    private Text QuitText = null;

    private Thread _thread = null;

    private bool _isQuit = false;
    private bool _isOpen = false;

    private IntPtr _activeWindow = IntPtr.Zero;

    private void Start()
    {
        Application.wantsToQuit += WantsToQuit;
        _activeWindow = WindowAPI.GetActiveWindow();
        QuitText.text = $"Quit / {_isQuit.ToString()}";
    }

    private bool WantsToQuit()
    {
        QuitText.text = $"Quit / {_isQuit.ToString()}";

        if (_isQuit)
        {
            return true;
        }
        else
        {
            if (_isOpen == false)
            {
                _isOpen = true;
                _thread = new Thread(WaitMessageBox);
                _thread.Start();
            }
            return false;
        }
    }

    /// <summary>
    /// 기존 WindowMessageBox와 조합하여 만듬
    /// </summary>
    private void WaitMessageBox()
    {
        WindowAPI.EnableWindow(_activeWindow, false);

        // 0x00000001L 확인 취소
        // 0x00000040L (i)아이콘
        // 0x00040000L 메시지 창 맨위로
        // 메시지 박스를 띄웁니다. CharSet을 Unicode로 설정하여 한글이 깨지지 않도록 합니다. 
        var messageBoxEvent = WindowAPI.MessageBox(IntPtr.Zero, "종료하시겠습니까?", "종료", (uint)(0x00000001L | 0x00000040L | 0x00040000L));
        if (messageBoxEvent > 0)
        {
            _isQuit = messageBoxEvent == 1;
            _isOpen = messageBoxEvent == 1;
            // 메시지 박스 없어질때 뒤에 클릭되도록
            WindowAPI.EnableWindow(_activeWindow, true);

            if (_isQuit)
            {
                Application.Quit();
            }
        }
    }

    /// <summary>
    /// OnApplicationQuit 함수를 따로 쓰고 있다면 Quit가 false일때 리턴시켜줘야 해당 x버튼 누를시에
    /// 호출되도 제대로 동작한다. x 버튼 누르고 예를 눌렀을때 그때 다시 Application.Quit 를 호출함으로
    /// 찾아서 처리하도록 하자
    /// </summary>
    private void OnApplicationQuit()
    {
        if(_isQuit == false)
        {
            return;
        }
        
        // 원래 구현 부분

    }
}
