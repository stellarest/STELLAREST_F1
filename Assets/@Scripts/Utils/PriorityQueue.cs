using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace STELLAREST_F1
{
    #region Enqueue, Dequeue Example
    /*
        또 까묵을 것 같아서 메모
        pi = (ci - 1) / 2
        Add List: 23, 10, 8, 9, 3, 5
        
        ### Enqueue
        * [23]
        * [23][10]
        // - [10][23]
        * [10][23][8]
        // - PI: (2 - 1) / 2 : 0
        // - [8][23][10]
        * [8][23][10][9]
        // - PI: (3 - 1) / 2 : 1
        // [8][9][10][23]
        // - PI: (1 - 1) / 2 : 0
        * [8][9][10][23][3]
        // - PI: (4 - 1) / 2 : 1
        // [8][3][10][23][9]
        // - PI: (1 - 1) / 2 : 0
        // [3][8][10][23][9]
        * [3][8][10][23][9][5]
        // - PI: (5 - 1) / 2 : 2
        // [3][8][5][23][9][10]
        // - PI: (2 - 1) / 2 : 0
        * [3][8][5][23][9][10] --> 종료

        ### Dequeue(최상단의 값이 다시 최소값이 되도록 유지시켜줘야함, 계속 이진트리 형태를 유지시켜야함)
        * 3
        // - [ ][8][5][23][9][10]
        // - [10][8][5][23][9]: 가장 마지막 요소를 '일단' 루트로 옮겨줌
        // - [5][8][10][23][9]: 10의 자식(왼쪽, 오른쪽)중에서 더 작은 값이랑 스왑
        // - 10은 더이상 비교할 자식이 없기 때문에 Dequeue 종료, 루트 노드에 최소값(5)이 설정이 되었음
        * 3, 5
        // - [ ][8][10][23][9]
        // - [9][8][10][23]
        // - [8][9][10][23]: 9가 자식 23보다 작으니까 그대로 종료
        * 3, 5, 8
        // - [ ][9][10][23]
        // - [23][9][10]
        // - [9][23][10]: 23의 자식이 없으므로 종료
        * 3, 5, 8, 9
        // - [ ][23][10]
        // - [10][23]
        * 3, 5, 8, 9, 10
        // - [ ][23]
        // - [23]
        * 3, 5, 8, 9, 10, 23 종료

        List vs 이진힙(이진트리)
        - 검색: List:O(N), BinaryHeap:O(1)
        - 추가 삭제: List:O(1), BinaryHeap:O(logN) - 부모,자식 이진 트리 구조로 인해 올리거나 내려야하지만 거듭제곱의 반대 log로 인해 많이 느리진 않아서 좋음.
    */

    #endregion
    // A*오픈리스트에서 활용(최소, 최대값 검색의 시간 복잡도는 무조건 O(1)).
    // (A*에서 오픈셋에 등록되어 있는 노드들중에서 작은 값만 얻어내면 되기 때문에 유용함).
    // 우선순위 큐는 가장 상위의 요소에 대해서만 보장하면 장땡이라, 일반적인 정렬이랑은 다른 구조임.
    // 데이터 데이터 관리를 배열로 관리하면 장땡. (이진트리(이진힙))
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> _heap = new List<T>();
        public List<T> Heap => _heap;

        // O(logN)
        public void Push(T data)
        {
            _heap.Add(data); // 데이터 추가는 무조건 맨 끝 부터..
            int childIdx = _heap.Count - 1;
            while (childIdx > 0)
            {
                // [23][10]
                // pi: (ci - 1) / 2
                int parentIdx = (childIdx - 1) / 2;

                // parent H가 더 클 경우 1을 반환함.
                // [23][10] 여기서 23이 더 크면 1을 반환해서 Sawp을 진행하는 것이고, 
                // 23이 child 보다 작으면 -1을 반환해서 그대로 break.
                // ---> 새로 들어온 휴리스틱이 크면 1을 반환함. 그래서 아래와 같이 Swap 진행해서 가장 낮은 휴리스틱을 가진 노드를 위로 올림.
                if (_heap[childIdx].CompareTo(_heap[parentIdx]) < 0)
                    break;

                /*
                	public int CompareTo(PQNode other)
                    {
                        if (H == other.H)
                            return 0;
                        return H < other.H ? 1 : -1;
                    }
                */

                T temp = _heap[childIdx];
                _heap[childIdx] = _heap[parentIdx];
                _heap[parentIdx] = temp;
                childIdx = parentIdx;
            }            
        }

        public T Pop()
        {
            T ret = _heap[0];
            int lastIdx = _heap.Count - 1;
            _heap[0] = _heap[lastIdx];
            _heap.RemoveAt(lastIdx);
            lastIdx--;

            int now = 0;
            while (true)
            {
                int left = 2 * now + 1;
                int right = 2 * now + 2;
                
                int next = now;

                // 왼쪽 자식이 현재값보다 크다면...
                if (left <= lastIdx && _heap[next].CompareTo(_heap[left]) < 0)
                    next = left; // next는 왼쪽이 됨

                // 오른쪽 자식이 현재(왼쪽으로 이동된거 포함)보다 크다면,
                if (right <= lastIdx && _heap[next].CompareTo(_heap[right]) < 0)
                    next = right; // next는 오른쪽이 됨

                // 왼쪽, 오른쪽 자식 전부 now보다 작으면 종료
                if (next == now)
                    break;

                T temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;
                now = next;
            }

            return ret;
        }

        public int Count => _heap.Count;
    }
}
