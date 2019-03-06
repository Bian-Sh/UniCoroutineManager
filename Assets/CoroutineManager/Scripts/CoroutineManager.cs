using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace zFrame.Extend
{
    public static class CoroutineEx
    {
        public static CoroutineHandler Start(this IEnumerator enumerator)
        {
            CoroutineHandler handler = new CoroutineHandler(enumerator);
            handler.Start();
            return handler;
        }
    }
    public class CoroutineHandler
    {
        public IEnumerator Coroutine { get; private set; } = null;
        public bool Paused { get; private set; } = false;
        public bool Running { get; private set; } = false;
        public bool Stopped { get; private set; } = false;
        public class FinishedHandler : UnityEngine.Events.UnityEvent<bool> { }
        public FinishedHandler OnCompleted = new FinishedHandler();
        public CoroutineHandler(IEnumerator c)
        {
            Coroutine = c;
        }

        public void Pause()
        {
            Paused = true;
        }

        public void Resume()
        {
            Paused = false;
        }

        public void Start()
        {
            if (null != Coroutine)
            {
                Running = true;
                CoroutineDriver.Run(CallWrapper());
            }
            else
            {
                Debug.Log("Coroutine 未指定，避免直接调用该方法。");
            }
        }

        public void Stop()
        {
            Stopped = true;
            Running = false;
        }

        /// <summary>
        /// 完成回调并断引用
        /// </summary>
        private void Finish()
        {
            OnCompleted?.Invoke(Stopped);
            this.OnCompleted.RemoveAllListeners();
            this.Coroutine = null;
        }
        /// <summary>
        /// 添加协程执行完成事件
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public CoroutineHandler OnComplete(UnityAction<bool> action)
        {
            this.OnCompleted.AddListener(action);
            return this;
        }

        #region IEnumerator Wrapper
        IEnumerator CallWrapper()
        {
            yield return null;
            IEnumerator e = Coroutine;
            while (Running)
            {
                if (Paused)
                    yield return null;
                else
                {
                    if (e != null && e.MoveNext())
                    {
                        yield return e.Current;
                    }
                    else
                    {
                        Running = false;
                    }
                }
            }
            Finish();
        }
        #endregion

        internal class CoroutineDriver : MonoBehaviour
        {
            static CoroutineDriver driver;
            static CoroutineDriver Driver
            {
                get
                {
                    if (null == driver)
                    {
                        GameObject go = new GameObject("[CoroutineDriver]");
                        driver = go.AddComponent<CoroutineDriver>();
                        GameObject.DontDestroyOnLoad(go);
                        go.hideFlags = HideFlags.HideAndDontSave;
                    }
                    return driver;
                }
            }
            private void Awake()
            {
                if (null != driver && driver != this) //避免了跳场景导致的重复生成问题
                {
                    GameObject.Destroy(gameObject);
                }
            }
            public static Coroutine Run(IEnumerator target)
            {
                return Driver.StartCoroutine(target);
            }
        }
    }
}
