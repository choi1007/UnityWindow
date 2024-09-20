using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Input과 MouseClick 이벤트가 동시에 처리되면 어떤 문제가 일어나는지 확인
/// IsEventInputModuleActive false 일때 마우스로 클릭한 이벤트가 남아있다면,
/// 키보드 input안에 아무런 이벤트가 없어도 마우스로 클릭한 이벤트가 호출된다.
/// IsEventInputModuleActive를 true로 변경하여 확인해보면, 마우스 클릭 이벤트 실행시
/// 해당 이벤트를 초기화 하여 키보드 Input을 눌러도 영향이 없다.
/// </summary>
public class WindowKeyEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject TestCubeObj = null;

    [SerializeField]
    private Text EventInputSpaceTextLog = null;

    [SerializeField]
    private Text EventClickBtnTextLog = null;

    [SerializeField]
    private bool IsEventInputModuleActive = false;

    private int _inputCount = 0;
    private int _clickCount = 0;

    private void OnEnable()
    {
        EventInputSpaceTextLog.text = $"InputSpaceBtn / {_inputCount++}";
        EventClickBtnTextLog.text = $"OnClickEventBtn / {_clickCount++}";
    }

    private void Update()
    {
        // 모듈 적용시킬때 동작하는지 체크
        if (IsEventInputModuleActive)
        {
            if (IsDeactiveModuleCheck())
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.currentInputModule.DeactivateModule();
            }
            else
            {
                EventSystem.current.currentInputModule.ActivateModule();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventInputSpaceTextLog.text = $"InputSpaceBtn / {_inputCount++} / {Time.time}";
        }
    }

    public void OnClickEventButton()
    {
        EventClickBtnTextLog.text = $"OnClickEventBtn / {_clickCount++} / {Time.time}";
        TestCubeObj.SetActive(TestCubeObj.activeSelf == false);
    }

    private bool IsDeactiveModuleCheck()
    {
        return Input.GetMouseButton(0) == false 
            && EventSystem.current != null 
            && EventSystem.current.currentSelectedGameObject != null 
            && EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() == null;
    }
}
