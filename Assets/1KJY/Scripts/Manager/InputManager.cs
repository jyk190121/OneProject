using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    //옵저버패턴: C# delegate/event, Action을 사용하는 경우 가장 대표적인것이 옵저버패턴이다.
    //옵저버패턴이란 객체의 상태 변화를 관찰해서 상태가 변할때 그와 연관된 객체들에게 알림을 보내는 디자인패턴이다.
    //결국 인풋매니저에서 관리하려는 Action들을 등록해두고 관찰중인 객체들에서 발생하는 모든 이벤트에
    //변화가 생기면 자동으로 알림을 보낸다.

    //파사드패턴: 어렵게 생각할것 없이, 복잡한 내부 코드를 여러분이 사용하기 쉽게 변경을 하거나
    //꼭 사용해야 하는 간단한 인터페이스만(함수) 노출시킨다 => 추상클래스 인터페이스 아님!!!

    private PlayerInput playerInput;
    // 외부 공개용 인터페이스, 옵저버들 (1회성 입력은 액션으로, 연속 입력되는 이동관련은 프로퍼티로)

    // 연속 입력되는 액션 - 프로퍼티로 노출
    public static Vector2 Input { get; private set; }               //이동 프로퍼티
    public static bool IsSprint {  get; private set; }              //왼쪽쉬프트 프로퍼티

    // 1회성 액션 - 이벤트로 노출
    public static event Action OnJump;                              //점프 입력 이벤트
    public static event Action OnAttack;                            //공격 입력 이벤트
 
    // Input System의 액션들 (외부에서 몰라도 됨)
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction attackAction;

    // 콜백 함수들을 저장할 변수들
    private Action<InputAction.CallbackContext> onMovePerformed;
    private Action<InputAction.CallbackContext> onMoveCanceled;
    private Action<InputAction.CallbackContext> onJumpPerformed;
    private Action<InputAction.CallbackContext> onSprintPerformed;
    private Action<InputAction.CallbackContext> onSprintCanceled;
    private Action<InputAction.CallbackContext> onAttackPerformed;


    void OnEnable()
    {
        // PlayerInput 컴포넌트 초기화
        playerInput = GetComponent<PlayerInput>();
        playerInput.defaultActionMap = "Player";
        playerInput.defaultControlScheme = "Keyboard&Mouse"; //"Default"
        playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;

        // 각각의 액션들 찾기
        moveAction = playerInput.actions.FindAction("Move");
        jumpAction = playerInput.actions.FindAction("Jump");
        sprintAction = playerInput.actions.FindAction("Sprint");
        attackAction = playerInput.actions.FindAction("Attack");

        // Move Action 콜백등록
        if (moveAction != null)
        {
            onMovePerformed = ctx => Input = ctx.ReadValue<Vector2>(); 
            onMoveCanceled = ctx => Input = Vector2.zero;
            moveAction.performed += onMovePerformed;
            moveAction.canceled += onMoveCanceled;
        }

        // Sprint Action 콜백등록
        if (sprintAction != null)
        {
            onSprintPerformed = ctx => IsSprint = true;
            onSprintCanceled = ctx => IsSprint = false;
            sprintAction.performed += onSprintPerformed;
            sprintAction.canceled += onSprintCanceled;
        }

        // Jump Action 콜백등록
        if (jumpAction != null)
        {
            onJumpPerformed = ctx => OnJump?.Invoke();
            jumpAction.performed += onJumpPerformed;
        }

        // Attack Action 콜백등록
        if(attackAction != null)
        {
            onAttackPerformed = ctx => OnAttack?.Invoke();
            attackAction.performed += onAttackPerformed;
        }

    }//end of OnEnable()

    void OnDisable()
    {
        // Move Action 콜백해제
        if (moveAction != null)
        {
            if (onMovePerformed != null)
            {
                moveAction.performed -= onMovePerformed;
                onMovePerformed = null;
            }

            if (onMoveCanceled != null)
            {
                moveAction.canceled -= onMoveCanceled;
                onMoveCanceled = null;
            }
        }

        // Jump Action 콜백해제
        if (jumpAction != null && onJumpPerformed != null)
        {
            jumpAction.performed -= onJumpPerformed;
            onJumpPerformed = null;
        }

        // Sprint Action 콜백해제
        if (sprintAction != null)
        {
            if (onSprintPerformed != null)
            {
                sprintAction.performed -= onSprintPerformed;
                onSprintPerformed = null;
            }

            if (onSprintCanceled != null)
            {
                sprintAction.canceled -= onSprintCanceled;
                onSprintCanceled = null;
            }
        }

        // Attack Action 콜백해제
        if (attackAction != null && onAttackPerformed != null)
        {
            attackAction.performed -= onAttackPerformed;
            onAttackPerformed = null;
        }
    }//end of OnDisable()
}
