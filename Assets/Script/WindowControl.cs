using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ExampleList
{
    public enum ExampleType
    {
        None,
        MultiClient,
        Exit,
        KeyEvent,
        ScreenFullRect,
        MessageBox,
        AspectRatio,
    }

    public ExampleType Type;
    public List<GameObject> ObjList;
}

public class WindowControl : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    private List<ExampleList> ExampleList = null;

    [SerializeField]
    private ExampleList.ExampleType ExampleOpenType = global::ExampleList.ExampleType.None;

    void Start()
    {
        for(int i = 0; i < ExampleList.Count; i++)
        {
            var example = ExampleList[i];

            if (example.Type == ExampleOpenType)
            {
                for(int j = 0; j < example.ObjList.Count; j++)
                {
                    example.ObjList[j].SetActive(true);
                }
            }
            else
            {
                for (int j = 0; j < example.ObjList.Count; j++)
                {
                    example.ObjList[j].SetActive(false);
                }
            }
        }
    }
}
