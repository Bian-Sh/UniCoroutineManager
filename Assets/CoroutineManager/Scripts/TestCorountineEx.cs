using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using zFrame.Extend;

public class TestCorountineEx : MonoBehaviour
{
    CoroutineHandler task;
    bool finish = false;

    public void StartTask()
    {
        if (null == task || !task.Running)
        {
            finish = false;
            Debug.Log("协程开始");
            task = MyAwesomeTask().Start();
            task.OnCompleted.AddListener(v => //第一种事件注册方式
            {
                if (v)
                {
                    Debug.Log("操作完成：用户取消了操作！");
                }
                else
                {
                    Debug.Log("操作完成！");
                }
            });
            task.OnComplete(v => Debug.Log("喵呜~ ---" + v)); //第二种事件注册方式（链式）
        }
        else
        {
            Debug.Log("不需要启动的Task");
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("启动协程"))
        {
            StartTask();
        }

        if (null == task || !task.Running) return;
        if (GUILayout.Button("强制停止"))
        {
            Debug.Log("强制停止");
            task.Stop();
        }
        if (GUILayout.Button("完成协程"))
        {
            Debug.Log("模拟协程完成操作");
            finish = true;
            if (task.Paused)
            {
                Debug.Log("模拟协程完成操作--事件在恢复Task时发出~");
            }
        }

        if (GUILayout.Button(task.Paused ? "继续" : "暂停"))
        {
            if (task.Paused)
            {
                Debug.Log("继续");
                task.Resume();
            }
            else
            {
                Debug.Log("暂停");
                task.Pause();
            }
        }

    }

    IEnumerator MyAwesomeTask()
    {
        while (!finish)
        {
            Debug.Log("运行中...");
            yield return null;
        }
    }
}
