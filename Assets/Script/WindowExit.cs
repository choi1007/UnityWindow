using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityWindowAPI;

/// <summary>
/// Unity Window ���� ��, X��ư�� ���� ����ó���� �Ѵ�.
/// x ��ư�� ������ ��, Ȯ�� �˾��� ������, Ȯ���� ������ �����ϰ� �����Ѵ�.
/// �ش� ������ �����Ϸ��� �ٸ� Ŭ�������� OnApplicationQuit �Լ��� ����� ��,
/// _isQuit�� ������ static ���� �����Ͽ� �ش� ������ �޾� false�϶� return ��Ű�ų� 
/// ����ó���� ���־�� �Ѵ�.
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
    /// ���� WindowMessageBox�� �����Ͽ� ����
    /// </summary>
    private void WaitMessageBox()
    {
        WindowAPI.EnableWindow(_activeWindow, false);

        // 0x00000001L Ȯ�� ���
        // 0x00000040L (i)������
        // 0x00040000L �޽��� â ������
        // �޽��� �ڽ��� ���ϴ�. CharSet�� Unicode�� �����Ͽ� �ѱ��� ������ �ʵ��� �մϴ�. 
        var messageBoxEvent = WindowAPI.MessageBox(IntPtr.Zero, "�����Ͻðڽ��ϱ�?", "����", (uint)(0x00000001L | 0x00000040L | 0x00040000L));
        if (messageBoxEvent > 0)
        {
            _isQuit = messageBoxEvent == 1;
            _isOpen = messageBoxEvent == 1;
            // �޽��� �ڽ� �������� �ڿ� Ŭ���ǵ���
            WindowAPI.EnableWindow(_activeWindow, true);

            if (_isQuit)
            {
                Application.Quit();
            }
        }
    }

    /// <summary>
    /// OnApplicationQuit �Լ��� ���� ���� �ִٸ� Quit�� false�϶� ���Ͻ������ �ش� x��ư �����ÿ�
    /// ȣ��ǵ� ����� �����Ѵ�. x ��ư ������ ���� �������� �׶� �ٽ� Application.Quit �� ȣ��������
    /// ã�Ƽ� ó���ϵ��� ����
    /// </summary>
    private void OnApplicationQuit()
    {
        if(_isQuit == false)
        {
            return;
        }
        
        // ���� ���� �κ�

    }
}
