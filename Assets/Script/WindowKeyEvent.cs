using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Input�� MouseClick �̺�Ʈ�� ���ÿ� ó���Ǹ� � ������ �Ͼ���� Ȯ��
/// IsEventInputModuleActive false �϶� ���콺�� Ŭ���� �̺�Ʈ�� �����ִٸ�,
/// Ű���� input�ȿ� �ƹ��� �̺�Ʈ�� ��� ���콺�� Ŭ���� �̺�Ʈ�� ȣ��ȴ�.
/// IsEventInputModuleActive�� true�� �����Ͽ� Ȯ���غ���, ���콺 Ŭ�� �̺�Ʈ �����
/// �ش� �̺�Ʈ�� �ʱ�ȭ �Ͽ� Ű���� Input�� ������ ������ ����.
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
        // ��� �����ų�� �����ϴ��� üũ
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
