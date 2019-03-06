# UniCoroutineManager
Coroutine Manager For Unity 3d

![Test](Image/CoroutineManager.gif)

# 怎么使用
1. 接管一个协程
```
 CoroutineHandler task; 
 public void StartTask()
    {
        if (null == task || !task.Running) //避免指定协程的重复开启
        {
            Debug.Log("协程开始");
            task = MyAwesomeTask().Start(); //MyAwsomeTask() 是一个协程
            task.OnCompleted.AddListener(v => //第一种事件注册方式
            {
                if (v) //可以通过返回值知道协程结束，以及结束是有谁主导的
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
```
2. 协程的生命周期控制
```
  task.Start() //协程的运行，推荐使用扩展方法 IEnumerator.Start() 方式接管协程的运行。
  task.Stop(); //协程的手动干预导致的停止
  task.Pause(); //暂停
  task.Resume();//协程的恢复运行
```

3. 核心代码来源于[Unity-TaskManager](https://github.com/krockot/Unity-TaskManager/blob/master/TaskManager.cs)：
```
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
        }
```

# Reference
. [Unity-TaskManager](https://github.com/krockot/Unity-TaskManager/blob/master/TaskManager.cs)

