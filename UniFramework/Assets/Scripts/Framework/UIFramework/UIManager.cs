using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniFramework.UI
{
    /// <summary>
    /// UI管理器 - 单例模式
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

        private Camera mUICamera; // 场景 UI 相机
        private Transform mUIRoot; // UI 根物体

        private Dictionary<string, UIBase> mAllWindowDic = new Dictionary<string, UIBase>(); // 所有窗口的Dic key-窗口类名
        private List<UIBase> mAllWindowList = new List<UIBase>(); // 所有窗口的列表
        private List<UIBase> mVisibleWindowList = new List<UIBase>(); // 所有可见窗口的列表

        private Queue<UIBase> mWindowStack = new Queue<UIBase>(); // 队列 用来管理弹窗的循环弹出
        private bool mStartPopStackWndStatus; // 开始弹出堆栈的表只 可以用来处理多种情况 比如：正在出栈种有其他界面弹出 可以直接放到栈内进行弹出 等


        /// <summary>
        /// 初始化 UIModule 管理器方法
        /// </summary>
        public void Initialize()
        {
            mUICamera = GameObject.Find("UICamera").GetComponent<Camera>();
            mUIRoot = GameObject.Find("UIRoot").transform;
        }

        #region 窗口管理

        /// <summary>
        /// 弹出一个弹窗 并渲染在视窗最前面
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

            // 如果窗口不存在 则新建一个
            T t = new T();
            return InitializeWindow(t, panelName) as T;
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        /// <param name="wndName">面板类名</param>
        public void HidePanel(string wndName)
        {
            UIBase panel = GetPanel(wndName);
            HidePanel(panel);
        }

        /// <summary>
        /// 泛型 隐藏面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void HidePanel<T>() where T : UIBase
        {
            HidePanel(typeof(T).Name);
        }

        /// <summary>
        /// 获取已经弹出的弹窗
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetPanel<T>() where T : UIBase
        {
            var type = typeof(T);
            // 一般都是访问已经打开的窗口 进行一些 API 调用
            foreach (var item in mVisibleWindowList)
            {
                if (item.Name == type.Name)
                {
                    return (T)item;
                }
            }

            Debug.LogError("该窗口没有获取到：" + type.Name);
            return null;
        }

        /// <summary>
        /// 泛型 销毁面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DestroyPanel<T>() where T : UIBase
        {
            DestroyPanel(typeof(T).Name);
        }

        /// <summary>
        /// 销毁所有的面板都销毁掉
        /// </summary>
        /// <param name="filterlist"></param>
        public void DestroyAllPanel(List<string> filterlist = null)
        {
            // 反向循环进行边循环边删除 列表的本质是数组
            for (int i = mAllWindowList.Count - 1; i >= 0; i--)
            {
                UIBase panel = mAllWindowList[i];
                if (panel == null || (filterlist != null && filterlist.Contains(panel.Name)))
                {
                    continue;
                }

                DestroyPanel(panel.Name);
            }

            // 注意释放资源的时机
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 弹出一个窗口 并渲染在视窗最前面
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
        /// 初始化窗口
        /// </summary>
        /// <param name="panelBase">窗口对象</param>
        /// <param name="PanelName">窗口名</param>
        /// <returns></returns>
        private UIBase InitializeWindow(UIBase panelBase, string PanelName)
        {
            // 1.生成对应的窗口预制体
            GameObject nWnd = LoadPanel(PanelName);
            // 2.初始化对应管理类
            if (nWnd != null)
            {
                panelBase.gameObject = nWnd;
                panelBase.transform = nWnd.transform;
                panelBase.Canvas = nWnd.GetComponent<Canvas>();
                panelBase.Canvas.worldCamera = mUICamera;
                panelBase.transform.SetAsLastSibling();
                panelBase.Name = nWnd.name;
                // 调用该窗口的生命周期函数 并设置可见
                panelBase.OnAwake();
                panelBase.SetVisible(true);
                panelBase.OnShow();
                RectTransform rectTrans = nWnd.GetComponent<RectTransform>();
                rectTrans.anchorMax = Vector2.one;
                rectTrans.offsetMax = Vector2.zero;
                rectTrans.offsetMin = Vector2.zero;
                // 添加到对应列表中进行管理
                mAllWindowDic.Add(PanelName, panelBase);
                mAllWindowList.Add(panelBase);
                mVisibleWindowList.Add(panelBase);
                SetWidnowMaskVisible();
                return panelBase;
            }

            Debug.LogError("没有加载到对应的窗口 窗口名字：" + PanelName);
            return null;
        }

        /// <summary>
        /// show 弹出过的窗口
        /// </summary>
        /// <param name="winName">窗口类名</param>
        /// <returns></returns>
        private UIBase ShowWindow(string winName)
        {
            UIBase panel;
            // 已经打开过
            if (mAllWindowDic.ContainsKey(winName))
            {
                panel = mAllWindowDic[winName];
                // 窗口已经存在 且 是不可见的
                if (panel.gameObject != null && panel.Visible == false)
                {
                    // 添加到可见列表中管理
                    mVisibleWindowList.Add(panel);
                    // 将窗口位置设置为最后一个 最优先渲染
                    panel.transform.SetAsLastSibling();
                    panel.SetVisible(true);
                    SetWidnowMaskVisible();
                    panel.OnShow();
                }

                return panel;
            }
            else
                Debug.LogError(winName + " 窗口不存在，请调用PopUpWindow 进行弹出");

            return null;
        }

        /// <summary>
        /// 得到已经弹出过的窗口
        /// </summary>
        /// <param name="winName">窗口类名</param>
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
            // 面板不为空 且 是可见的
            if (panel != null && panel.Visible)
            {
                mVisibleWindowList.Remove(panel);
                panel.SetVisible(false); // 隐藏弹窗物体
                SetWidnowMaskVisible();
                panel.OnHide();
            }

            // 在出栈的情况下，上一个界面隐藏时，自动打开栈种的下一个界面
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
                    // 在对应容器中 移除对应的面板
                    mAllWindowDic.Remove(panel.Name);
                    mAllWindowList.Remove(panel);
                    mVisibleWindowList.Remove(panel);
                }

                panel.SetVisible(false);
                SetWidnowMaskVisible();
                panel.OnHide();
                panel.OnDestroy();
                Object.Destroy(panel.gameObject);
                // 在出栈的情况下 上一个界面销毁时 自动打开栈种的下一个界面
                PopNextStackWindow(panel);
            }
        }


        /// <summary>
        /// 动态加载窗口预制件
        /// </summary>
        /// <param name="wndName">窗口名</param>
        /// <returns></returns>
        private GameObject LoadPanel(string wndName)
        {
            //TODO 资源加载
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
            //TODO 暂时默认不启动单遮罩
            return;
            // 单遮和叠遮模式处理
            if (!UISetting.Instance.SINGMASK_SYSTEM)
            {
                return;
            }

            UIBase maxOrderWndBase = null; // 最大渲染层级的窗口
            int maxOrder = 0; // 最大渲染层级
            int maxIndex = 0; // 最大排序下标 在相同父节点下的位置下标
            // 1.关闭所有窗口的Mask 设置为不可见
            // 2.从所有可见窗口中找到一个层级最大的窗口 把Mask设置为可见
            for (int i = 0; i < mVisibleWindowList.Count; i++)
            {
                UIBase panel = mVisibleWindowList[i];
                if (panel != null && panel.gameObject != null)
                {
                    // 先把所有窗口可见性关闭
                    panel.SetMaskVisible(false);
                    if (maxOrderWndBase == null)
                    {
                        maxOrderWndBase = panel;
                        maxOrder = panel.Canvas.sortingOrder;
                        maxIndex = panel.transform.GetSiblingIndex();
                    }
                    else
                    {
                        // 找到最大渲染层级的窗口 拿到它
                        if (maxOrder < panel.Canvas.sortingOrder)
                        {
                            maxOrderWndBase = panel;
                            maxOrder = panel.Canvas.sortingOrder;
                        }
                        // 如果两个窗口的渲染层级相同 就找到同节点下最靠下一个物体 优先渲染Mask
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

        #region 堆栈系统

        /// <summary>
        /// 进栈一个界面
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
        /// 弹出堆栈中第一个弹窗
        /// </summary>
        public void StartPopFirstStackWindow()
        {
            if (mStartPopStackWndStatus) return;
            mStartPopStackWndStatus = true; //已经开始进行堆栈弹出的流程，
            PopStackWindow();
        }

        /// <summary>
        /// 压入并且弹出堆栈弹窗
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="popCallBack"></param>
        public void PushAndPopStackWindow<T>(Action<UIBase> popCallBack = null) where T : UIBase, new()
        {
            PushWindowToStack<T>(popCallBack);
            StartPopFirstStackWindow();
        }

        /// <summary>
        /// 弹出堆栈中的下一个窗口
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
        /// 弹出堆栈弹窗
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
        /// 清空缓存队列
        /// </summary>
        public void ClearStackWindows()
        {
            mWindowStack.Clear();
        }

        #endregion
    }
}