using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    // State Instance
    private static readonly StateStanding stateStanding = new StateStanding();
    private static readonly StateJumping stateJumping = new StateJumping();
    private static readonly StateFlying stateFlying = new StateFlying();
    private static readonly StateDead stateDead = new StateDead();

    private PlayerStateBase currentState = stateStanding;
    public bool IsDead => currentState is StateDead;

    private Rigidbody2D playerRb;

    // タッチ入力の受付
    [SerializeField] TouchHandler inputTouch;

    // 移動を制御
    [SerializeField] float jumpLimitMagnitude;
    [SerializeField] float flyLimitMagnitude;
    [SerializeField] float speed;
    Vector2 startPosition;

    void Awake()
    {
        playerRb = this.GetComponent<Rigidbody2D>();

        inputTouch.OnBeginDragEvent += OnBeginDragMove;
        inputTouch.OnDragEvent += OnDragMove;
        inputTouch.OnEndDragEvent += OnEndDragMove;
    }

    void Start()
    {
        currentState.OnEnter(this, null);
    }

    void Update()
    {
        currentState.OnUpdate(this);
    }

    void OnBeginDragMove(PointerEventData data)
    {
        currentState.OnBeginDrag(this, data.position);
    }

    void OnDragMove(PointerEventData data)
    {
        currentState.OnDrag(this, data.position);
    }

    void OnEndDragMove(PointerEventData data)
    {
        currentState.OnEndDrag(this, data.position);
    }

    /// <summary>
    /// プレイヤーの状態を移行する
    /// </summary>
    /// <param name="nextState">次のプレイヤーの状態</param>
	void ChangeState(PlayerStateBase nextState)
	{
		currentState.OnExit(this, nextState);
		nextState.OnEnter(this, currentState);
		currentState = nextState;
	}

    /// <summary>
    /// プレイヤーを移動する
    /// </summary>
    /// <param name="force">プレイヤーに加える力</param>
    void Move(Vector2 force)
    {
        playerRb.AddForce(force * speed, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 衝突時の動作
    /// </summary>
    /// <param name="other">衝突したオブジェクト</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        // 地面に衝突したとき
        if (other.gameObject.tag == "Ground") {
            ChangeState(stateStanding);
        }
    }

    /// <summary>
    /// 立っている状態
    /// </summary>
    public class StateStanding : PlayerStateBase
    {
        public override void OnBeginDrag(Player owner, Vector2 beginPos)
        {
            owner.startPosition = beginPos;
        }

        public override void OnDrag(Player owner, Vector2 dragPos)
        {
            var diff = dragPos - owner.startPosition;
            var magnitude = diff.magnitude > owner.jumpLimitMagnitude ? owner.jumpLimitMagnitude : diff.magnitude;
        }

        public override void OnEndDrag(Player owner, Vector2 endPos)
        {
            var diff = endPos - owner.startPosition;
            var magnitudeLimitRatio = owner.jumpLimitMagnitude / Mathf.Max(diff.magnitude, owner.jumpLimitMagnitude);

            owner.Move(-diff * magnitudeLimitRatio);
            owner.ChangeState(stateJumping);
        }

    }

    /// <summary>
    /// 跳ねている状態
    /// </summary>
    public class StateJumping : PlayerStateBase
    {
    	// public override void OnEnter(Player owner, PlayerStateBase prevState) {}

	    // public override void OnExit(Player owner, PlayerStateBase nextState) {}

        public override void OnBeginDrag(Player owner, Vector2 beginPos)
        {
            owner.startPosition = beginPos;
        }

        public override void OnDrag(Player owner, Vector2 dragPos)
        {
            var diff = dragPos - owner.startPosition;
            var magnitude = diff.magnitude > owner.flyLimitMagnitude ? owner.flyLimitMagnitude : diff.magnitude;
        }

        public override void OnEndDrag(Player owner, Vector2 endPos)
        {
            var diff = endPos - owner.startPosition;
            var magnitudeLimitRatio = owner.flyLimitMagnitude / Mathf.Max(diff.magnitude, owner.flyLimitMagnitude);

            owner.Move(-diff * magnitudeLimitRatio);
            owner.ChangeState(stateFlying);
        }
    }

    /// <summary>
    /// 飛んでいる状態
    /// </summary>
    public class StateFlying : PlayerStateBase
    {
	    // public override void OnEnter(Player owner, PlayerStateBase prevState) {}
	    // public override void OnExit(Player owner, PlayerStateBase nextState) {}
    }

    /// <summary>
    /// ゲームオーバーの状態
    /// </summary>
    public class StateDead : PlayerStateBase
    {
	    // public override void OnEnter(Player owner, PlayerStateBase prevState) {}
    }
}
