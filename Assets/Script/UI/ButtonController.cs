using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class ButtonController : MonoBehaviour,
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    // クリック時の非同期コールバック
    public Func<Task> onClickAsync;

    Image _image;
    bool _isPushed = false;

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    // ボタンが押されドラッグせずに離れる（タップ）
    public void OnPointerClick(PointerEventData eventData)
    {
        if ( _isPushed ) return;
        
        _Execute().Forget();
        async Task _Execute()
        {
            try
            {
                _isPushed = true;

                // 処理を実行
                await onClickAsync?.Invoke();
            }
            finally
            {
                _isPushed = false;
            }
        }
    }

    // ボタンが押される（タップダウン）
    public void OnPointerDown(PointerEventData eventData)
    {
        if ( _isPushed ) return;
    }
    
    // ボタンが離される（タップアップ）
    public void OnPointerUp(PointerEventData eventData)
    {
        if ( _isPushed ) return;
    }

    // カーソルが乗る
    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    // カーソルが外れる
    public void OnPointerExit(PointerEventData eventData)
    {
    }
}

public static class TaskExtensions
{
    /// <summary>
    /// タスクの完了を待たないことを明示的にする
    /// 例外をログに出すためのもの
    /// </summary>
    public static async void Forget(this Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e);
        }
    }
}
