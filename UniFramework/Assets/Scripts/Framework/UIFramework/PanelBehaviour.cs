using System;
using UnityEngine;


namespace UniFramework.UI
{
    /// <summary>
    /// ���� MonoBehaviour ����
    /// </summary>
    public abstract class PanelBehaviour
    {
        public GameObject gameObject { get; set; } // ��ǰ�������� GameObject
        public Transform transform { get; set; } // �����Լ� Transform
        public Canvas Canvas { get; set; } // ��ǰ���ڵ� Canvas
        public string Name { get; set; } // ��ǰ���ڵ�����
        public bool Visible { get; set; } // ��ǰ�����Ƿ�ɼ�
        public bool PopStack { get; set; } //�Ƿ���ͨ����ջϵͳ�����ĵ���
        public Action<UIBase> PopStackListener { get; set; }

        /// <summary>
        /// ֻ�������崴��ʱִ��һ�� ����Mono Awake����ʱ���ʹ�������һ��
        /// </summary>
        public virtual void OnAwake()
        {
        }

        /// <summary>
        /// ��������ʾʱִ��һ�Σ���Mono OnEnableһ��
        /// </summary>
        public virtual void OnShow()
        {
        }

        /// <summary>
        /// ���º��� ��Mono Updateһ��
        /// </summary>
        public virtual void OnUpdate()
        {
        }

        /// <summary>
        /// ����������ʱִ��һ�Σ���Mono OnDisable һ��
        /// </summary>
        public virtual void OnHide()
        {
        }

        /// <summary>
        /// �ڵ�ǰ���汻����ʱ����һ��
        /// </summary>
        public virtual void OnDestroy()
        {
        }

        /// <summary>
        /// ��������Ŀɼ���
        /// </summary>
        /// <param name="isVisble">�Ƿ�ɼ�</param>
        public virtual void SetVisible(bool isVisble)
        {
            Visible = isVisble;
        }
    }
}