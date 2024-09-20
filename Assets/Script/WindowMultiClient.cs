using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityWindowAPI;

/// <summary>
/// Unity Window빌드시에 한번에 _clinetMaxCount 만큼 실행하게 하는 함수.
/// 해당 내용으로 다중 클라이언트를 제어할 수 있다.
/// 또한 exe파일 이름 변경시에 해당 클라이언트를 실행못하게도 가능하다.
/// </summary>
public class WindowMultiClientCheck : MonoBehaviour
{
    [SerializeField]
    private Text _multiClientTitle = null;

    [SerializeField]
    private Text _multiClientCount = null;

    // 자릿수 제한
    private const int MAX_PATH = 260;
    // 스냅샷 생성 플래그
    private const uint TH32CS_SNAPPROCESS = 0x00000002;
    // 클라이언트 생성 제한 수
    private const int CLINET_MAX_COUNT = 2;

    public static List<int> FindProcessesByName(string processName)
    {
        List<int> processIds = new List<int>();
        WindowAPI.PROCESSENTRY32 processEntry = new WindowAPI.PROCESSENTRY32();
        processEntry.dwSize = (uint)Marshal.SizeOf(typeof(WindowAPI.PROCESSENTRY32));

        // 프로세스 스냅샷 생성
        IntPtr snapshotHandle = WindowAPI.CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

        if (WindowAPI.Process32First(snapshotHandle, ref processEntry))
        {
            do
            {
                // 프로세스 이름 비교 (대소문자 무시)
                if (string.Equals(System.IO.Path.GetFileNameWithoutExtension(processEntry.szExeFile), processName, StringComparison.OrdinalIgnoreCase))
                {
                    // 해당 이름을 가진 프로세스의 PID를 리스트에 추가
                    processIds.Add((int)processEntry.th32ProcessID);
                }
            }
            while (WindowAPI.Process32Next(snapshotHandle, ref processEntry));
        }

        // 스냅샷 핸들 닫기
        WindowAPI.CloseHandle(snapshotHandle);

        return processIds;
    }

    public void Start()
    {
        string processName = Application.productName;
        string activeWindowTitle = string.Empty;

        IntPtr hModule = WindowAPI.GetModuleHandle(null);

        if (hModule != IntPtr.Zero)
        {
            StringBuilder sb = new StringBuilder(MAX_PATH);
            uint size = WindowAPI.GetModuleFileName(hModule, sb, MAX_PATH);
            if (size != 0)
            {
                string filePath = sb.ToString();
                activeWindowTitle = System.IO.Path.GetFileName(filePath);
            }
        }

        if (String.IsNullOrWhiteSpace(activeWindowTitle) == false)
        {
            activeWindowTitle = activeWindowTitle.Replace(".exe", string.Empty);
        }

        if (activeWindowTitle != processName)
        {
            _multiClientTitle.text = $"프로세스 이름이 다릅니다";
            _multiClientCount.text = $"{activeWindowTitle} / {processName}";
            return;
        }

        List<int> processIds = FindProcessesByName(processName);

        if (processIds.Count > CLINET_MAX_COUNT)
        {
            _multiClientTitle.text = $"프로세스 '{processName}'의 개수가 2개가 넘습니다.";
            _multiClientCount.text = processIds.Count.ToString();
        }
        else
        {
            _multiClientTitle.text = $"프로세스 '{processName}'를 찾았습니다.";
            _multiClientCount.text = processIds.Count.ToString();
        }
    }
}

