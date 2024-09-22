using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniFramework.UI
{
    /// <summary>
    /// UI������ - ����ģʽ
    /// </summary>
    public class UIManager
    {
        private static UIManager _instance;

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UIManager();
                }

                return _instance;
            }
        }

        private Camera mUICamera; // ���� UI ���
        private Transform mUIRoot; // UI ������

        private Dictionary<string, UIBase> mAllWindowDic = new Dictionary<string, UIBase>(); // ���д��ڵ�Dic key-��������
        private List<UIBase> mAllWindowList = new List<UIBase>(); // ���д��ڵ��б�
        private List<UIBase> mVisibleWindowList = new List<UIBase>(); // ���пɼ����ڵ��б�

        private Queue<UIBase> mWindowStack = new Queue<UIBase>(); // ���� ������������ѭ������
        private bool mStartPopStackWndStatus; // ��ʼ������ջ�ı�ֻ ������������������ ���磺���ڳ�ջ�����������浯�� ����ֱ�ӷŵ�ջ�ڽ��е��� ��


        /// <summary>
        /// ��ʼ�� UIModule ����������
        /// </summary>
        public void Initialize()
        {
            mUICamera = GameObject.Find("UICamera").GetComponent<Camera>();
            mUIRoot = GameObject.Find("UIRoot").transform;
        }

        #region ���ڹ���

        /// <summary>
        /// ����һ������ ����Ⱦ���Ӵ���ǰ��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ShowPanel<T>() where T : UIBase, new()
        {
            Type type = typeof(T);
            string panelName = type.Name;
            UIBase panel = GetPanel(panelName);
            if (panel != null)
            {
                return ShowWindow(panelName) as T;
            }

            // ������ڲ����� ���½�һ��
            T t = new T();
            return InitializeWindow(t, panelName) as T;
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="wndName">�������</param>
        public void HidePanel(string wndName)
        {
            UIBase panel = GetPanel(wndName);
            HidePanel(panel);
        }

        /// <summary>
        /// ���� �������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void HidePanel<T>() where T : UIBase
        {
            HidePanel(typeof(T).Name);
        }

        /// <summary>
        /// ��ȡ�Ѿ������ĵ���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetPanel<T>() where T : UIBase
        {
            var type = typeof(T);
            // һ�㶼�Ƿ����Ѿ��򿪵Ĵ��� ����һЩ API ����
            foreach (var item in mVisibleWindowList)
            {
                if (item.Name == type.Name)
                {
                    return (T)item;
                }
            }

            Debug.LogError("�ô���û�л�ȡ����" + type.Name);
            return null;
        }

        /// <summary>
        /// ���� �������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DestroyPanel<T>() where T : UIBase
        {
            DestroyPanel(typeof(T).Name);
        }

        /// <summary>
        /// �������е���嶼���ٵ�
        /// </summary>
        /// <param name="filterlist"></param>
        public void DestroyAllPanel(List<string> filterlist = null)
        {
            // ����ѭ�����б�ѭ����ɾ�� �б�ı���������
            for (int i = mAllWindowList.Count - 1; i >= 0; i--)
            {
                UIBase panel = mAllWindowList[i];
                if (panel == null || (filterlist != null && filterlist.Contains(panel.Name)))
                {
                    continue;
                }

                DestroyPanel(panel.Name);
            }

            // ע���ͷ���Դ��ʱ��
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// ����һ������ ����Ⱦ���Ӵ���ǰ��
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        private UIBase ShowPanel(UIBase panel)
        {
            Type type = panel.GetType();
            string wndName = type.Name;
            UIBase wnd = GetPanel(wndName);
            if (wnd != null)
            {
                return ShowWindow(wndName);
            }

            return InitializeWindow(panel, wndName);
        }

        /// <summary>
        /// ��ʼ������
        /// </summary>
        /// <param name="panelBase">���ڶ���</param>
        /// <param name="PanelName">������</param>
        /// <returns></returns>
        private UIBase InitializeWindow(UIBase panelBase, string PanelName)
        {
            // 1.���ɶ�Ӧ�Ĵ���Ԥ����
            GameObject nWnd = LoadPanel(PanelName);
            // 2.��ʼ����Ӧ������
            if (nWnd != null)
            {
                panelBase.gameObject = nWnd;
                panelBase.transform = nWnd.transform;
                panelBase.Canvas = nWnd.GetComponent<Canvas>();
                panelBase.Canvas.worldCamera = mUICamera;
                panelBase.transform.SetAsLastSibling();
                panelBase.Name = nWnd.name;
                // ���øô��ڵ��������ں��� �����ÿɼ�
                panelBase.OnAwake();
                panelBase.SetVisible(true);
                panelBase.OnShow();
                RectTransform rectTrans = nWnd.GetComponent<RectTransform>();
                rectTrans.anchorMax = Vector2.one;
                rectTrans.offsetMax = Vector2.zero;
                rectTrans.offsetMin = Vector2.zero;
                // ��ӵ���Ӧ�б��н��й���
                mAllWindowDic.Add(PanelName, panelBase);
                mAllWindowList.Add(panelBase);
                mVisibleWindowList.Add(panelBase);
                SetWidnowMaskVisible();
                return panelBase;
            }

            Debug.LogError("û�м��ص���Ӧ�Ĵ��� �������֣�" + PanelName);
            return null;
        }

        /// <summary>
        /// show �������Ĵ���
        /// </summary>
        /// <param name="winName">��������</param>
        /// <returns></returns>
        private UIBase ShowWindow(string winName)
        {
            UIBase panel;
            // �Ѿ��򿪹�
            if (mAllWindowDic.ContainsKey(winName))
            {
                panel = mAllWindowDic[winName];
                // �����Ѿ����� �� �ǲ��ɼ���
                if (panel.gameObject != null && panel.Visible == false)
                {
                    // ��ӵ��ɼ��б��й���
                    mVisibleWindowList.Add(panel);
                    // ������λ������Ϊ���һ�� ��������Ⱦ
                    panel.transform.SetAsLastSibling();
                    panel.SetVisible(true);
                    SetWidnowMaskVisible();
                    panel.OnShow();
                }

                return panel;
            }
            else
                Debug.LogError(winName + " ���ڲ����ڣ������PopUpWindow ���е���");

            return null;
        }

        /// <summary>
        /// �õ��Ѿ��������Ĵ���
        /// </summary>
        /// <param name="winName">��������</param>
        /// <returns></returns>
        private UIBase GetPanel(string winName)
        {
            if (mAllWindowDic.ContainsKey(winName))
            {
                return mAllWindowDic[winName];
            }

            return null;
        }


        private void HidePanel(UIBase panel)
        {
            // ��岻Ϊ�� �� �ǿɼ���
            if (panel != null && panel.Visible)
            {
                mVisibleWindowList.Remove(panel);
                panel.SetVisible(false); // ���ص�������
                SetWidnowMaskVisible();
                panel.OnHide();
            }

            // �ڳ�ջ������£���һ����������ʱ���Զ���ջ�ֵ���һ������
            PopNextStackWindow(panel);
        }

        private void DestroyPanel(string wndName)
        {
            UIBase panel = GetPanel(wndName);
            DestoryPanel(panel);
        }


        private void DestoryPanel(UIBase panel)
        {
            if (panel != null)
            {
                if (mAllWindowDic.ContainsKey(panel.Name))
                {
                    // �ڶ�Ӧ������ �Ƴ���Ӧ�����
                    mAllWindowDic.Remove(panel.Name);
                    mAllWindowList.Remove(panel);
                    mVisibleWindowList.Remove(panel);
                }

                panel.SetVisible(false);
                SetWidnowMaskVisible();
                panel.OnHide();
                panel.OnDestroy();
                Object.Destroy(panel.gameObject);
                // �ڳ�ջ������� ��һ����������ʱ �Զ���ջ�ֵ���һ������
                PopNextStackWindow(panel);
            }
        }


        /// <summary>
        /// ��̬���ش���Ԥ�Ƽ�
        /// </summary>
        /// <param name="wndName">������</param>
        /// <returns></returns>
        private GameObject LoadPanel(string wndName)
        {
            //TODO ��Դ����
            // GameObject window = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(mWindowConfig.GetWindowPath(wndName)), mUIRoot);
            GameObject window = Object.Instantiate(Resources.Load<GameObject>(wndName), mUIRoot);
            //window.transform.SetParent(mUIRoot);
            window.transform.localScale = Vector3.one;
            window.transform.localPosition = Vector3.zero;
            window.transform.rotation = Quaternion.identity;
            window.name = wndName;
            return window;
        }

        private void SetWidnowMaskVisible()
        {
            //TODO ��ʱĬ�ϲ�����������
            return;
            // ���ں͵���ģʽ����
            if (!UISetting.Instance.SINGMASK_SYSTEM)
            {
                return;
            }

            UIBase maxOrderWndBase = null; // �����Ⱦ�㼶�Ĵ���
            int maxOrder = 0; // �����Ⱦ�㼶
            int maxIndex = 0; // ��������±� ����ͬ���ڵ��µ�λ���±�
            // 1.�ر����д��ڵ�Mask ����Ϊ���ɼ�
            // 2.�����пɼ��������ҵ�һ���㼶���Ĵ��� ��Mask����Ϊ�ɼ�
            for (int i = 0; i < mVisibleWindowList.Count; i++)
            {
                UIBase panel = mVisibleWindowList[i];
                if (panel != null && panel.gameObject != null)
                {
                    // �Ȱ����д��ڿɼ��Թر�
                    panel.SetMaskVisible(false);
                    if (maxOrderWndBase == null)
                    {
                        maxOrderWndBase = panel;
                        maxOrder = panel.Canvas.sortingOrder;
                        maxIndex = panel.transform.GetSiblingIndex();
                    }
                    else
                    {
                        // �ҵ������Ⱦ�㼶�Ĵ��� �õ���
                        if (maxOrder < panel.Canvas.sortingOrder)
                        {
                            maxOrderWndBase = panel;
                            maxOrder = panel.Canvas.sortingOrder;
                        }
                        // ����������ڵ���Ⱦ�㼶��ͬ ���ҵ�ͬ�ڵ������һ������ ������ȾMask
                        else if (maxOrder == panel.Canvas.sortingOrder && maxIndex < panel.transform.GetSiblingIndex())
                        {
                            maxOrderWndBase = panel;
                            maxIndex = panel.transform.GetSiblingIndex();
                        }
                    }
                }
            }

            if (maxOrderWndBase != null)
            {
                maxOrderWndBase.SetMaskVisible(true);
            }
        }

        #endregion

        #region ��ջϵͳ

        /// <summary>
        /// ��ջһ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="popCallBack"></param>
        public void PushWindowToStack<T>(Action<UIBase> popCallBack = null) where T : UIBase, new()
        {
            T wndBase = new T();
            wndBase.PopStackListener = popCallBack;
            mWindowStack.Enqueue(wndBase);
        }

        /// <summary>
        /// ������ջ�е�һ������
        /// </summary>
        public void StartPopFirstStackWindow()
        {
            if (mStartPopStackWndStatus) return;
            mStartPopStackWndStatus = true; //�Ѿ���ʼ���ж�ջ���������̣�
            PopStackWindow();
        }

        /// <summary>
        /// ѹ�벢�ҵ�����ջ����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="popCallBack"></param>
        public void PushAndPopStackWindow<T>(Action<UIBase> popCallBack = null) where T : UIBase, new()
        {
            PushWindowToStack<T>(popCallBack);
            StartPopFirstStackWindow();
        }

        /// <summary>
        /// ������ջ�е���һ������
        /// </summary>
        /// <param name="panelBase"></param>
        private void PopNextStackWindow(UIBase panelBase)
        {
            if (panelBase != null && mStartPopStackWndStatus && panelBase.PopStack)
            {
                panelBase.PopStack = false;
                PopStackWindow();
            }
        }

        /// <summary>
        /// ������ջ����
        /// </summary>
        /// <returns></returns>
        public bool PopStackWindow()
        {
            if (mWindowStack.Count > 0)
            {
                UIBase panel = mWindowStack.Dequeue();
                UIBase popPanel = ShowPanel(panel);
                popPanel.PopStackListener = panel.PopStackListener;
                popPanel.PopStack = true;
                popPanel.PopStackListener?.Invoke(popPanel);
                popPanel.PopStackListener = null;
                return true;
            }
            else
            {
                mStartPopStackWndStatus = false;
                return false;
            }
        }

        /// <summary>
        /// ��ջ������
        /// </summary>
        public void ClearStackWindows()
        {
            mWindowStack.Clear();
        }

        #endregion
    }
}