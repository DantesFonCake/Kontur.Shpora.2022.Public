﻿using System.Threading;

namespace LockFree
{
    public class SimpleStack<T> : IStack<T>
    {
        private readonly object syncObj = new object();
        private Node<T> head;

        public void Push(T obj)
        {
            Node<T> oldHead;
            var newHead = new Node<T> { Value = obj };

            do
            {
                oldHead = head;
                newHead.Next = oldHead;
            } while (Interlocked.CompareExchange(ref head, newHead, oldHead) != oldHead);
        }

        public T Pop()
        {
            Node<T> oldHead;
            Node<T> newHead;
            do
            {
                oldHead = head;
                newHead = oldHead.Next;


            } while (Interlocked.CompareExchange(ref head, newHead, oldHead) != oldHead);

            return oldHead.Value;
        }
    }
}