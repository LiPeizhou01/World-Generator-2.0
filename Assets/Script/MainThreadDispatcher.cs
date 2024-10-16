using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using Unity.VisualScripting;
using System.Linq;

public class PriorityQueue<T> where T : class
{
    private List<(T Value, int Priority)> _elements = new List<(T Value, int Priority)>();

    public void Enqueue(T item, int priority)
    {
        _elements.Add((item, priority));
        int currentIdex = _elements.Count - 1;
        int parentIndex;

        while (currentIdex > 0)
        {
            // 查询父元素索引
            parentIndex = (currentIdex - 1) / 2;

            // 是否符合最小堆性质
            if (_elements[currentIdex].Priority >= _elements[parentIndex].Priority)
            {
                //是 跳出逻辑
                break;
            }

            //否 交换元素
            (_elements[currentIdex], _elements[parentIndex]) = (_elements[parentIndex], _elements[currentIdex]);
            currentIdex = parentIndex;
        }
    }

    public T Dequeue()
    {
        if (_elements.Count == 0)
        {
            throw new InvalidOperationException("The priority queue is empty");
        }

        // 取出根元素
        var root = _elements[0];

        // 取出链表末尾元素
        var lastElement = _elements[_elements.Count - 1];
        _elements.RemoveAt(_elements.Count-1);

        //如果此时为0
        if (_elements.Count == 0)
        {
            return root.Value;
        }

        // 把最后一个元素放置到根节点，然后调整元素位置
        _elements[0] = lastElement;
        Heapify(0);

        return root.Value;
    }

    public bool TryDequeue(out T value)
    {
        try
        {
            value = Dequeue();
            return true;
        }
        catch
        {
            value = null;
            return false;
        }
    }

    //  查看当前队列尾部方法
    public T Peek()
    {
        if (_elements.Count == 0)
        {
            throw new InvalidOperationException("The priority queue is empty");
        }

        return _elements[0].Value;
    }

    public bool TryPeek(out T value)
    {
        try
        {
            value = Peek();
            return true;
        }
        catch
        {
            value = null;
            return false;
        }
    }

    // 检查队列是否为空
    public bool IsEmpty() 
    {
        return _elements.Count == 0;
    }

    // 调整最小堆性质的方法
    private void Heapify(int index) 
    {
        int leftChildIndex;
        int rightChildIndex;
        int smallestIndex = index;

        while (true)
        {
            leftChildIndex = 2 * index + 1;
            rightChildIndex = 2 * index + 2;

            //观察当前节点最小值 并取得其索引
            if (leftChildIndex < _elements.Count && _elements[leftChildIndex].Priority < _elements[smallestIndex].Priority) 
            { 
                smallestIndex = leftChildIndex;
            }
            if (rightChildIndex < _elements.Count && _elements[rightChildIndex].Priority < _elements[smallestIndex].Priority)
            {
                smallestIndex = rightChildIndex;
            }

            //当前已经满足最小堆性质时跳出
            if (smallestIndex == index) 
            {
                break;
            }

            //交换两个节点的位置
            (_elements[index], _elements[smallestIndex]) = (_elements[smallestIndex], _elements[index]);
            index = smallestIndex;
        }
    }
}

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher instance;
    private static readonly Dictionary<Chunk, Action> tasks = new Dictionary<Chunk, Action>();

    static readonly private int sleepFlame = MaxTasksPerFrame * Chunk.chunkHight / MapLoader.rate;
    private int sleepCount = 0;

    public static MainThreadDispatcher Instance()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("MainThreadDispatcher");
            instance = go.AddComponent<MainThreadDispatcher>();
            DontDestroyOnLoad(go); // 保持在场景切换中存在
        }
        return instance;
    }

    public static void InitializeMainThreadDispatcher() 
    {
        if (instance == null)
        {
            GameObject go = new GameObject("MainThreadDispatcher");
            instance = go.AddComponent<MainThreadDispatcher>();
            DontDestroyOnLoad(go); // 保持在场景切换中存在
        }
    }

    private IEnumerable<KeyValuePair<Chunk, Action>> SortedTasks
    {
        get
        {
            return tasks.OrderBy(kvp => kvp.Key); // 按 Chunk 排序
        }
    }

    public void Enqueue(Action action, Chunk chunk)
    {
        if (action == null)
        {
            return;
        }
        tasks.Add(chunk,action);
    }

    private const int MaxTasksPerFrame = 3;

    private Action Pop()
    {
        if (tasks.Count == 0)
        {
            return null; // 没有任务可供执行
        }

        // 获取第一个任务（按 Chunk 排序的最小值）
        var firstTask = SortedTasks.First();
        tasks.Remove(firstTask.Key); // 从字典中移除该任务
        return firstTask.Value; // 返回任务
    }

    private void Update()
    {
        sleepCount += 1;

        if (sleepCount > sleepFlame)
        {
            int taskCount = 0;
            while (taskCount < MaxTasksPerFrame)
            {
                var task = Pop(); // 获取下一个任务
                if (task != null)
                {
                    task.Invoke(); // 执行任务
                    taskCount++;
                }
                else 
                { 
                    break;
                }
            }

            sleepCount = 0;
        }
    }
}
