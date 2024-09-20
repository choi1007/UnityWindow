using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityWindowAPI;

/// <summary>
/// Unity Window����ÿ� �ѹ��� _clinetMaxCount ��ŭ �����ϰ� �ϴ� �Լ�.
/// �ش� �������� ���� Ŭ���̾�Ʈ�� ������ �� �ִ�.
/// ���� exe���� �̸� ����ÿ� �ش� Ŭ���̾�Ʈ�� ������ϰԵ� �����ϴ�.
/// </summary>
public class WindowMultiClientCheck : MonoBehaviour
{
    [SerializeField]
    private Text _multiClientTitle = null;

    [SerializeField]
    private Text _multiClientCount = null;

    // �ڸ��� ����
    private const int MAX_PATH = 260;
    // ������ ���� �÷���
    private const uint TH32CS_SNAPPROCESS = 0x00000002;
    // Ŭ���̾�Ʈ ���� ���� ��
    private const int CLINET_MAX_COUNT = 2;

    public static List<int> FindProcessesByName(string processName)
    {
        List<int> processIds = new List<int>();
        WindowAPI.PROCESSENTRY32 processEntry = new WindowAPI.PROCESSENTRY32();
        processEntry.dwSize = (uint)Marshal.SizeOf(typeof(WindowAPI.PROCESSENTRY32));

        // ���μ��� ������ ����
        IntPtr snapshotHandle = WindowAPI.CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

        if (WindowAPI.Process32First(snapshotHandle, ref processEntry))
        {
            do
            {
                // ���μ��� �̸� �� (��ҹ��� ����)
                if (string.Equals(System.IO.Path.GetFileNameWithoutExtension(processEntry.szExeFile), processName, StringComparison.OrdinalIgnoreCase))
                {
                    // �ش� �̸��� ���� ���μ����� PID�� ����Ʈ�� �߰�
                    processIds.Add((int)processEntry.th32ProcessID);
                }
            }
            while (WindowAPI.Process32Next(snapshotHandle, ref processEntry));
        }

        // ������ �ڵ� �ݱ�
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
            _multiClientTitle.text = $"���μ��� �̸��� �ٸ��ϴ�";
            _multiClientCount.text = $"{activeWindowTitle} / {processName}";
            return;
        }

        List<int> processIds = FindProcessesByName(processName);

        if (processIds.Count > CLINET_MAX_COUNT)
        {
            _multiClientTitle.text = $"���μ��� '{processName}'�� ������ 2���� �ѽ��ϴ�.";
            _multiClientCount.text = processIds.Count.ToString();
        }
        else
        {
            _multiClientTitle.text = $"���μ��� '{processName}'�� ã�ҽ��ϴ�.";
            _multiClientCount.text = processIds.Count.ToString();
        }
    }
}

