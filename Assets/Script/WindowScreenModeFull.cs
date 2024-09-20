using System;
using UnityEngine;
using UnityEngine.UI;
using UnityWindowAPI;

/// <summary>
/// 유니티에서 StandAlone 빌드시 ResizableWindow 옵션을 켰을때, 
/// 최대 창 크기가 비활성화 된 경우 활성화 시킬때 사용
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
        // max버튼 활성화
        MaxSizeWindowBtn();

        // 창크기를 1280 720 으로 변경해서 비교할 것 초기화
        Screen.SetResolution(1280, 720, false);
    }

    // 버튼 클릭시 창크기 최대화로
    public void OnClickWindowScreenModeFullRect()
    {
        MaximizeWindow();
    }

    // 최대 창 크기 active true 시키는 함수
    private void MaxSizeWindowBtn()
    {
        IntPtr hWnd = WindowAPI.GetActiveWindow();

        // 현재 창 스타일 가져오기
        int style = WindowAPI.GetWindowLong(hWnd, GWL_STYLE);

        // 최대화 버튼 활성화하기
        style |= WS_MAXIMIZEBOX;

        // 창 스타일 변경 적용
        WindowAPI.SetWindowLong(hWnd, GWL_STYLE, style);
    }

    private void MaximizeWindow()
    {
        IntPtr hWnd = WindowAPI.GetActiveWindow();

        // 현재 모니터의 작업 영역 크기 가져오기
        WindowAPI.RECT rect = new WindowAPI.RECT();
        WindowAPI.SystemParametersInfo(SPI_GETWORKAREA, 0, ref rect, 0);
        int workAreaWidth = rect.Right - rect.Left;
        int workAreaHeight = rect.Bottom - rect.Top;

        // 창을 최대화 상태로 설정
        WindowAPI.SetWindowPos(hWnd, IntPtr.Zero, rect.Left, rect.Top, workAreaWidth, workAreaHeight, SWP_FRAMECHANGED | SWP_SHOWWINDOW);
    }
}