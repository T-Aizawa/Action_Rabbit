using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerStateBase
{
	// ステートを開始したときに呼ばれる
	public virtual void OnEnter(Player owner, PlayerStateBase prevState) {}

	// 毎フレーム呼ばれる
	public virtual void OnUpdate(Player owner) {}

	// ステートを終了したときに呼ばれる
	public virtual void OnExit(Player owner, PlayerStateBase nextState) {}

    // ドラッグ開始
    public virtual void OnBeginDrag(Player owner, Vector2 beginPos) {}

    // ドラッグ中
    public virtual void OnDrag(Player owner, Vector2 dragPos) {}

    // ドラッグ終了
    public virtual void OnEndDrag(Player owner, Vector2 endPos) {}
}
