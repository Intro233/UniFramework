// using DG.Tweening;

using System.Collections.Generic;
using CodeBind;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UniFramework.UI
{
    /// <summary>
    /// 面板窗口基类
    /// </summary>
    public class UIBase : PanelBehaviour
    {
        private List<Button> mAllButtonList = new List<Button>(); // 所有 Button 列表
        private List<Toggle> mToggleList = new List<Toggle>(); // 所有的 Toggle 列表
        private List<InputField> mInputList = new List<InputField>(); // 所有的输入框列表

        // 拿到对应的组件
        private CanvasGroup mUIMask;
        private CanvasGroup mCanvasGroup;
        protected Transform mPanelTrans;
        protected CSCodeBindMono mBindMono;

        /// <summary>
        /// 初始化基类组件
        /// </summary>
        private void InitializeBaseComponent()
        {
            mCanvasGroup = transform.GetComponent<CanvasGroup>();
            mUIMask = transform.Find("UIMask")?.GetComponent<CanvasGroup>();
            mPanelTrans = transform.Find("Panel")?.transform;
        }

        /// <summary>
        /// 设置遮罩的显隐
        /// </summary>
        /// <param name="isVisble"></param>
        public void SetMaskVisible(bool isVisble)
        {
            //TODO 暂时默认不启动单遮罩
            return;

            // 单遮和叠遮模式处理
            if (!UISetting.Instance.SINGMASK_SYSTEM)
            {
                return;
            }

            mUIMask.alpha = isVisble ? 1 : 0;
        }


        #region 重写生命周期

        public override void OnAwake()
        {
            base.OnAwake();
            if (gameObject.TryGetComponent<CSCodeBindMono>(out var csCodeBindMono))
            {
                mBindMono = csCodeBindMono;
            }
        }

        public override void OnShow()
        {
            base.OnShow();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RemoveAllButtonListener();
            RemoveAllToggleListener();
            RemoveAllInputListener();
            mAllButtonList.Clear();
            mToggleList.Clear();
            mInputList.Clear();
        }

        #endregion

        public void HideSelf()
        {
            var type = GetType();
            UIManager.Instance.HidePanel(type.Name);
        }

        public override void SetVisible(bool isVisble)
        {
            base.SetVisible(isVisble);
            gameObject.SetActive(isVisble);
        }

        #region 组件事件管理

        /// <summary>
        /// 添加 Button 组件事件监听
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="action"></param>
        public void AddButtonClickListener(Button btn, UnityAction action)
        {
            if (btn != null)
            {
                if (!mAllButtonList.Contains(btn))
                {
                    // 添加 btn 组件到列表中统一管理
                    mAllButtonList.Add(btn);
                }

                // 先把 btn 所有事件移除 再添加新的事件
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(action);
            }
        }

        /// <summary>
        /// 添加 Toggle 组件事件监听
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="action"></param>
        public void AddToggleClickListener(Toggle toggle, UnityAction<bool, Toggle> action)
        {
            if (toggle != null)
            {
                if (!mToggleList.Contains(toggle))
                {
                    // 添加 Toggle 组件到列表中统一管理
                    mToggleList.Add(toggle);
                }

                // 先把 Toggle 所有事件移除 再添加新的事件
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    // 把 Toggle 自身和他的 isOn值一起传递出去
                    action?.Invoke(isOn, toggle);
                });
            }
        }

        /// <summary>
        /// 添加输入框事件监听
        /// </summary>
        /// <param name="input"></param>
        /// <param name="onChangeAction">输入修改事件</param>
        /// <param name="endAction">输入完成事件</param>
        public void AddInputFieldListener(InputField input, UnityAction<string> onChangeAction, UnityAction<string> endAction)
        {
            if (input != null)
            {
                if (!mInputList.Contains(input))
                {
                    // 添加 InputField 组件到列表中统一管理
                    mInputList.Add(input);
                }

                // 先移除事件 再监听新事件
                input.onValueChanged.RemoveAllListeners();
                input.onEndEdit.RemoveAllListeners();
                input.onValueChanged.AddListener(onChangeAction);
                input.onEndEdit.AddListener(endAction);
            }
        }

        /// <summary>
        /// 移除 Button 事件监听
        /// </summary>
        public void RemoveAllButtonListener()
        {
            foreach (var item in mAllButtonList)
            {
                item.onClick.RemoveAllListeners();
            }
        }

        /// <summary>
        /// 移除 Toggle 事件监听
        /// </summary>
        public void RemoveAllToggleListener()
        {
            foreach (var item in mToggleList)
            {
                item.onValueChanged.RemoveAllListeners();
            }
        }

        /// <summary>
        /// 移除 InputField 事件监听
        /// </summary>
        public void RemoveAllInputListener()
        {
            foreach (var item in mInputList)
            {
                item.onValueChanged.RemoveAllListeners();
                item.onEndEdit.RemoveAllListeners();
            }
        }

        #endregion

        #region 动画管理

        // protected bool mDisableAnim = false; // 禁用动画


        //
        // public void ShowAnimation()
        // {
        //     //基础弹窗不需要动画
        //     if (Canvas.sortingOrder > 90 && mDisableAnim == false)
        //     {
        //         //Mask动画
        //         mUIMask.alpha = 0;
        //         mUIMask.DOFade(1, 0.2f);
        //         //缩放动画
        //         mUIContent.localScale = Vector3.one * 0.8f;
        //         mUIContent.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        //     }
        // }
        //
        // public void HideAnimation()
        // {
        //     if (Canvas.sortingOrder > 90 && mDisableAnim == false)
        //     {
        //         mUIContent.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
        //         {
        //             UIModule.Instance.HideWindow(Name);
        //         });
        //     }
        //     else
        //     {
        //         UIModule.Instance.HideWindow(Name);
        //     }
        // }
        //

        #endregion
    }
}