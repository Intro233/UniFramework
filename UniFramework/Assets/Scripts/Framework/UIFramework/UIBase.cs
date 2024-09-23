// using DG.Tweening;

using System.Collections.Generic;
using CodeBind;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UniFramework.UI
{
    /// <summary>
    /// ��崰�ڻ���
    /// </summary>
    public class UIBase : PanelBehaviour
    {
        private List<Button> mAllButtonList = new List<Button>(); // ���� Button �б�
        private List<Toggle> mToggleList = new List<Toggle>(); // ���е� Toggle �б�
        private List<InputField> mInputList = new List<InputField>(); // ���е�������б�

        // �õ���Ӧ�����
        private CanvasGroup mUIMask;
        private CanvasGroup mCanvasGroup;
        protected Transform mPanelTrans;
        protected CSCodeBindMono mBindMono;

        /// <summary>
        /// ��ʼ���������
        /// </summary>
        private void InitializeBaseComponent()
        {
            mCanvasGroup = transform.GetComponent<CanvasGroup>();
            mUIMask = transform.Find("UIMask")?.GetComponent<CanvasGroup>();
            mPanelTrans = transform.Find("Panel")?.transform;
        }

        /// <summary>
        /// �������ֵ�����
        /// </summary>
        /// <param name="isVisble"></param>
        public void SetMaskVisible(bool isVisble)
        {
            //TODO ��ʱĬ�ϲ�����������
            return;

            // ���ں͵���ģʽ����
            if (!UISetting.Instance.SINGMASK_SYSTEM)
            {
                return;
            }

            mUIMask.alpha = isVisble ? 1 : 0;
        }


        #region ��д��������

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

        #region ����¼�����

        /// <summary>
        /// ��� Button ����¼�����
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="action"></param>
        public void AddButtonClickListener(Button btn, UnityAction action)
        {
            if (btn != null)
            {
                if (!mAllButtonList.Contains(btn))
                {
                    // ��� btn ������б���ͳһ����
                    mAllButtonList.Add(btn);
                }

                // �Ȱ� btn �����¼��Ƴ� ������µ��¼�
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(action);
            }
        }

        /// <summary>
        /// ��� Toggle ����¼�����
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="action"></param>
        public void AddToggleClickListener(Toggle toggle, UnityAction<bool, Toggle> action)
        {
            if (toggle != null)
            {
                if (!mToggleList.Contains(toggle))
                {
                    // ��� Toggle ������б���ͳһ����
                    mToggleList.Add(toggle);
                }

                // �Ȱ� Toggle �����¼��Ƴ� ������µ��¼�
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    // �� Toggle ��������� isOnֵһ�𴫵ݳ�ȥ
                    action?.Invoke(isOn, toggle);
                });
            }
        }

        /// <summary>
        /// ���������¼�����
        /// </summary>
        /// <param name="input"></param>
        /// <param name="onChangeAction">�����޸��¼�</param>
        /// <param name="endAction">��������¼�</param>
        public void AddInputFieldListener(InputField input, UnityAction<string> onChangeAction, UnityAction<string> endAction)
        {
            if (input != null)
            {
                if (!mInputList.Contains(input))
                {
                    // ��� InputField ������б���ͳһ����
                    mInputList.Add(input);
                }

                // ���Ƴ��¼� �ټ������¼�
                input.onValueChanged.RemoveAllListeners();
                input.onEndEdit.RemoveAllListeners();
                input.onValueChanged.AddListener(onChangeAction);
                input.onEndEdit.AddListener(endAction);
            }
        }

        /// <summary>
        /// �Ƴ� Button �¼�����
        /// </summary>
        public void RemoveAllButtonListener()
        {
            foreach (var item in mAllButtonList)
            {
                item.onClick.RemoveAllListeners();
            }
        }

        /// <summary>
        /// �Ƴ� Toggle �¼�����
        /// </summary>
        public void RemoveAllToggleListener()
        {
            foreach (var item in mToggleList)
            {
                item.onValueChanged.RemoveAllListeners();
            }
        }

        /// <summary>
        /// �Ƴ� InputField �¼�����
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

        #region ��������

        // protected bool mDisableAnim = false; // ���ö���


        //
        // public void ShowAnimation()
        // {
        //     //������������Ҫ����
        //     if (Canvas.sortingOrder > 90 && mDisableAnim == false)
        //     {
        //         //Mask����
        //         mUIMask.alpha = 0;
        //         mUIMask.DOFade(1, 0.2f);
        //         //���Ŷ���
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