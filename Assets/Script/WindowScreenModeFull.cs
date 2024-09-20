using System;
using UnityEngine;
using UnityEngine.UI;
using UnityWindowAPI;

/// <summary>
/// ����Ƽ���� StandAlone ����� ResizableWindow �ɼ��� ������, 
/// �ִ� â ũ�Ⱑ ��Ȱ��ȭ �� ��� Ȱ��ȭ ��ų�� ���
/// </summary>
public class WindowScreenModeFull : MonoBehaviour
{
    [SerializeField]
    private Text ScreenText = null;

    // Window Styles
    private const int GWL_STYLE = -16;
    private const int WS_MAXIMIZEBOX = 0x00010000;

    // Window position and sizing flags
    private const int SWP_FRAMECHANGED = 0x0020;
    private const int SWP_SHOWWINDOW = 0x0040;
    private const uint SPI_GETWORKAREA = 0x0030;

    private void Start()
    {
        // max��ư Ȱ��ȭ
        MaxSizeWindowBtn();

        // âũ�⸦ 1280 720 ���� �����ؼ� ���� �� �ʱ�ȭ
        Screen.SetResolution(1280, 720, false);
    }

    // ��ư Ŭ���� âũ�� �ִ�ȭ��
    public void OnClickWindowScreenModeFullRect()
    {
        MaximizeWindow();
    }

    // �ִ� â ũ�� active true ��Ű�� �Լ�
    private void MaxSizeWindowBtn()
    {
        IntPtr hWnd = WindowAPI.GetActiveWindow();

        // ���� â ��Ÿ�� ��������
        int style = WindowAPI.GetWindowLong(hWnd, GWL_STYLE);

        // �ִ�ȭ ��ư Ȱ��ȭ�ϱ�
        style |= WS_MAXIMIZEBOX;

        // â ��Ÿ�� ���� ����
        WindowAPI.SetWindowLong(hWnd, GWL_STYLE, style);
    }

    private void MaximizeWindow()
    {
        IntPtr hWnd = WindowAPI.GetActiveWindow();

        // ���� ������� �۾� ���� ũ�� ��������
        WindowAPI.RECT rect = new WindowAPI.RECT();
        WindowAPI.SystemParametersInfo(SPI_GETWORKAREA, 0, ref rect, 0);
        int workAreaWidth = rect.Right - rect.Left;
        int workAreaHeight = rect.Bottom - rect.Top;

        // â�� �ִ�ȭ ���·� ����
        WindowAPI.SetWindowPos(hWnd, IntPtr.Zero, rect.Left, rect.Top, workAreaWidth, workAreaHeight, SWP_FRAMECHANGED | SWP_SHOWWINDOW);
    }
}