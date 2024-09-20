using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityWindowAPI;

/// <summary>
/// 해당 내용은 GitHub에 있는 AspectRatio를 기반으로 작업함
/// </summary>
public class AspectRatioController : MonoBehaviour
{
    [SerializeField]
    private Text LogText = null;

    // 16 : 9 해상도 비율
    public const float aspectRatioWidth = 16;
    public const float aspectRatioHeight = 9;

    public const int WM_SIZING = 0x214;
    public const int WMSZ_LEFT = 1;
    public const int WMSZ_RIGHT = 2;
    public const int WMSZ_TOP = 3;
    public const int WMSZ_BOTTOM = 6;
    public const int GWLP_WNDPROC = -4;

    private const int SM_CXFULLSCREEN = 16;  // 작업 영역 너비
    private const int SM_CYFULLSCREEN = 17;  // 작업 영역 높이

    // 최소 해상도 값
    public static int minWidthPixel = 640;
    public static int minHeightPixel = 360;

    // 최대 해상도값
    public static int maxWidthPixel
    {
        get
        {
            return Display.main.systemWidth;
        }
    }
    public static int maxHeightPixel
    {
        get
        {
            return Display.main.systemHeight;
        }
    }

    public static float _aspect = 0;

    public static int _setWidth = -1;
    public static int _setHeight = -1;

    private static IntPtr _activeWindow;
    private static IntPtr _oldWndProcPtr;
    private static IntPtr _newWndProcPtr;

    // 창 모드 최대화 시 실제 해상도 적용 width / Height
    private int _taskWidth = 0;
    private int _taskHeight = 0;

    public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    public WndProcDelegate wndProcDelegate;

    void Start()
    {
        // 에디터에서 적용하면 에디터도 16:9로 변경됨
        if (Application.isEditor)
        {
            return;
        }

        _activeWindow = WindowAPI.GetActiveWindow();

        _taskWidth = WindowAPI.GetSystemMetrics(SM_CXFULLSCREEN);
        _taskHeight = WindowAPI.GetSystemMetrics(SM_CYFULLSCREEN);

        SetAspectRatio(aspectRatioWidth, aspectRatioHeight, true);

        wndProcDelegate = wndProc;
        _newWndProcPtr = Marshal.GetFunctionPointerForDelegate(wndProcDelegate);
        _oldWndProcPtr = SetWindowLong(_activeWindow, GWLP_WNDPROC, _newWndProcPtr);
    }

    public void SetAspectRatio(float newAspectWidth, float newAspectHeight, bool apply)
    {
        _aspect = aspectRatioWidth / aspectRatioHeight;

        if (apply)
        {
            Screen.SetResolution(Screen.width, Mathf.RoundToInt(Screen.width / _aspect), Screen.fullScreen);
        }
    }

    [MonoPInvokeCallback]
    private static IntPtr wndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WM_SIZING)
        {
            WindowAPI.RECT rc = (WindowAPI.RECT)Marshal.PtrToStructure(lParam, typeof(WindowAPI.RECT));

            WindowAPI.RECT windowRect = new WindowAPI.RECT();
            WindowAPI.GetWindowRect(_activeWindow, out windowRect);

            WindowAPI.RECT clientRect = new WindowAPI.RECT();
            WindowAPI.GetClientRect(_activeWindow, ref clientRect);

            int borderWidth = windowRect.Right - windowRect.Left - (clientRect.Right - clientRect.Left);
            int borderHeight = windowRect.Bottom - windowRect.Top - (clientRect.Bottom - clientRect.Top);

            rc.Right -= borderWidth;
            rc.Bottom -= borderHeight;

            int newWidth = Mathf.Clamp(rc.Right - rc.Left, minWidthPixel, maxWidthPixel);
            int newHeight = Mathf.Clamp(rc.Bottom - rc.Top, minHeightPixel, maxHeightPixel);

            switch (wParam.ToInt32())
            {
                case WMSZ_LEFT:
                    rc.Left = rc.Right - newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / _aspect);
                    break;
                case WMSZ_RIGHT:
                    rc.Right = rc.Left + newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / _aspect);
                    break;
                case WMSZ_TOP:
                    rc.Top = rc.Bottom - newHeight;
                    rc.Right = rc.Left + Mathf.RoundToInt(newHeight * _aspect);
                    break;
                case WMSZ_BOTTOM:
                    rc.Bottom = rc.Top + newHeight;
                    rc.Right = rc.Left + Mathf.RoundToInt(newHeight * _aspect);
                    break;
                case WMSZ_RIGHT + WMSZ_BOTTOM:
                    rc.Right = rc.Left + newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / _aspect);
                    break;
                case WMSZ_RIGHT + WMSZ_TOP:
                    rc.Right = rc.Left + newWidth;
                    rc.Top = rc.Bottom - Mathf.RoundToInt(newWidth / _aspect);
                    break;
                case WMSZ_LEFT + WMSZ_BOTTOM:
                    rc.Left = rc.Right - newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / _aspect);
                    break;
                case WMSZ_LEFT + WMSZ_TOP:
                    rc.Left = rc.Right - newWidth;
                    rc.Top = rc.Bottom - Mathf.RoundToInt(newWidth / _aspect);
                    break;
            }

            _setWidth = rc.Right - rc.Left;
            _setHeight = rc.Bottom - rc.Top;

            rc.Right += borderWidth;
            rc.Bottom += borderHeight;

            Marshal.StructureToPtr(rc, lParam, true);
        }

        return WindowAPI.CallWindowProc(_oldWndProcPtr, hWnd, msg, wParam, lParam);
    }

    void Update()
    {
        // 에디터에서 적용하면 에디터도 16:9로 변경됨
        if (Application.isEditor)
        {
            return;
        }

        if (!Screen.fullScreen && _setWidth != -1 && _setHeight != -1 && (Screen.width != _setWidth || Screen.height != _setHeight))
        {
            _setHeight = Screen.height;
            _setWidth = Mathf.RoundToInt(Screen.height * _aspect);

            if (!IsAspectRatioValid(Screen.width, Screen.height))
            {
                _setWidth = _taskWidth;
                _setHeight = _taskHeight;

                Screen.SetResolution(_setWidth, _setHeight, Screen.fullScreen);
            }

            bool IsAspectRatioValid(int screenWidth, int screenHeight)
            {
                float currentAspectRatio = (float)screenWidth / screenHeight;
                float targetAspectRatio = (aspectRatioWidth / aspectRatioHeight);
                float tolerance = 0.01f;

                if (Mathf.Abs(currentAspectRatio - targetAspectRatio) > tolerance)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }

    private static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        if (IntPtr.Size == 4)
        {
            return WindowAPI.SetWindowLong32(hWnd, nIndex, dwNewLong);
        }
        return WindowAPI.SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
    }
}