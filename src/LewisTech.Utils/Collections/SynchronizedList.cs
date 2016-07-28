using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace LewisTech.Utils.Collections
{
    public class SynchronizedList<T> : ICollection<T>
    {
        private readonly List<T> _list = new List<T>();
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

        #region IDictionary<TKey,TValue> Members

        public SynchronizedList()
        {

        }

        public void Add(T item)
        {
            try
            {

                _rwLock.EnterWriteLock();
                _list.Add(item);
            }
            finally
            {
                _rwLock.ExitWriteLock();


            }
        }

        public void Clear()
        {
            try
            {
                _rwLock.EnterWriteLock();
                _list.Clear();
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public bool Contains(T item)
        {
            try
            {
                _rwLock.EnterReadLock();
                return _list.Contains(item);
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            try
            {
                _rwLock.EnterReadLock();

                _list.ToArray().CopyTo(array, arrayIndex);
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        public int Count
        {
            get
            {
                try
                {
                    _rwLock.EnterReadLock();
                    return _list.Count;
                }
                finally
                {
                    _rwLock.ExitReadLock();
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            try
            {
                _rwLock.EnterWriteLock();
                return _list.Remove(item);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public T this[int index]
        {
            get
            {
                try
                {

                    _rwLock.EnterReadLock();
                    return _list[index];
                }
                finally
                {

                    _rwLock.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    _rwLock.EnterWriteLock();
                    _list[index] = value;
                }
                finally
                {
                    _rwLock.ExitWriteLock();
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            try
            {
                _rwLock.EnterReadLock();
                return _list.GetEnumerator();
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            try
            {
                _rwLock.EnterReadLock();
                return _list.GetEnumerator();
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        #endregion


    }


}
