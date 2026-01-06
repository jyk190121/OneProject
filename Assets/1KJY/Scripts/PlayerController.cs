using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    CharacterController cc;

    float walkSpeed = 3f;
    float runSpeed = 6f;

    // Animator Hash ID
    int hashMoveX;
    int hashMoveY;
    int hashJump;
    int hashAttack;


    private void Awake()
    {
        //컴포넌트 참조 초기화
        anim = GetComponentInChildren<Animator>();
        cc = GetComponent<CharacterController>();

        // Animator Hash Init
        hashMoveX = Animator.StringToHash("MoveX");
        hashMoveY = Animator.StringToHash("MoveY");
        hashJump = Animator.StringToHash("Jump");
        hashAttack = Animator.StringToHash("Attack");

    }

    private void OnEnable()
    {
        //InputManager 이벤트 등록
        //1회성 입력되는 액션들만 등록한다
        InputManager.OnJump += HandleJump;
        InputManager.OnAttack += HandleAttack;
    }


    private void OnDisable()
    {
        //InputManager 이벤트 해제
        //1회성 입력되는 액션들만 해제한다
        InputManager.OnJump -= HandleJump;
        InputManager.OnAttack -= HandleAttack;
    }
    
    void Update()
    {
        //플레이어 이동
        PlayerMove(InputManager.Input, InputManager.IsSprint);

    }
    
    /// <summary>
    /// 플레이어 이동처리
    /// </summary>
    /// <param name="input">InputManager.Input 사용하기</param>
    /// <param name="isLeftShiftPressed">InputManager.IsSprint 사용하기</param>
    void PlayerMove(Vector2 input, bool isLeftShiftPressed)
    {
        if(input.magnitude > 0.1f)
        {
            //이동 처리
            Vector3 dir = new Vector3(input.x, 0f, input.y);
            dir.Normalize();
            float curSpeed = isLeftShiftPressed ? runSpeed : walkSpeed;
            cc.Move(dir * curSpeed * Time.deltaTime);

            //이동 애니메이션
            float animSpeed = isLeftShiftPressed ? 2f : 1f;
            anim.SetFloat(hashMoveX, input.x);
            anim.SetFloat(hashMoveY, input.y * animSpeed);
        }
        else
        {
            anim.SetFloat(hashMoveX, 0f);
            anim.SetFloat(hashMoveY, 0f);
        }

    }


    private void HandleJump()
    {
        print("점프");
        anim.SetTrigger(hashJump);
    }

    private void HandleAttack()
    {
        print("공격");
        anim.SetTrigger(hashAttack);
    }
}
