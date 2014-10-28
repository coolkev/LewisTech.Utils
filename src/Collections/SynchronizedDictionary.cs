using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LewisTech.Utils.Collections
{
    public class SynchronizedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

        #region IDictionary<TKey,TValue> Members
      
          public SynchronizedDictionary()
        {
            
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            try
            {

                _rwLock.EnterWriteLock();
                _dict.Add(item.Key, item.Value);
            }
            finally
            {
                _rwLock.ExitWriteLock();


            }
        }

        public void AddIfNotContains(TKey key, Func<TValue> addFunc)
        {
            try
            {

                _rwLock.EnterUpgradeableReadLock();
                
                if (_dict.ContainsKey(key))
                    return;


                try
                {

                    _rwLock.EnterWriteLock();
                    var result = addFunc();
                    _dict.Add(key, result);

                    return;
                }
                finally
                {


                    _rwLock.ExitWriteLock();

                }

            }
            finally
            {

                _rwLock.ExitUpgradeableReadLock();
            }
        }
        public void Clear()
        {
            try
            {
                _rwLock.EnterWriteLock();
                _dict.Clear();
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            try
            {
                _rwLock.EnterReadLock();
                return _dict.Contains(item);
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            try
            {
                _rwLock.EnterReadLock();

                _dict.ToArray().CopyTo(array, arrayIndex);
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
                    return _dict.Count;
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

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            try
            {
                _rwLock.EnterWriteLock();
                return _dict.Remove(item.Key);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public void Add(TKey key, TValue value)
        {
            try
            {
                _rwLock.EnterWriteLock();
                _dict[key] = value;
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public bool ContainsKey(TKey key)
        {
            try
            {
                _rwLock.EnterReadLock();
                return _dict.ContainsKey(key);
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                try
                {
                
                    _rwLock.EnterReadLock();
                    return _dict[key];
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
                    _dict[key] = value;
                }
                finally
                {
                    _rwLock.ExitWriteLock();
                }
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                try
                {
                    _rwLock.EnterReadLock();
                    return _dict.Keys;
                }
                finally
                {
                    _rwLock.ExitReadLock();
                }
            }
        }

        public bool Remove(TKey key)
        {
            try
            {
                _rwLock.EnterWriteLock();
                return _dict.Remove(key);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            try
            {
                _rwLock.EnterReadLock();
                return _dict.TryGetValue(key, out value);
            }
            finally
            {
                 
                _rwLock.ExitReadLock();
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                try
                {
                    _rwLock.EnterReadLock();
                    return _dict.Values;
                }
                finally
                {
                    _rwLock.ExitReadLock();
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return GetEnumeratorGeneric();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            try
            {
                _rwLock.EnterReadLock();
                return _dict.GetEnumerator();
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        #endregion

        public Dictionary<TKey, TValue>.Enumerator GetEnumeratorGeneric()
        {
            try
            {
                _rwLock.EnterReadLock();
                return _dict.GetEnumerator();
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }



        public void SetAll(Dictionary<TKey, TValue> keyValues)
        {
            try
            {
                _rwLock.EnterWriteLock();
                _dict = keyValues;
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public IEnumerable<TValue> GetAll()
        {
            try
            {
                _rwLock.EnterReadLock();
                return _dict.Values;
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }


        public TValue GetValueOrAdd(TKey key, Func<TValue> addFunc)
        {
            try
            {
                _rwLock.EnterUpgradeableReadLock();
                TValue result;

                if (_dict.TryGetValue(key, out result))
                    return result;


                try
                {
                    _rwLock.EnterWriteLock();
                    result = addFunc();
                    _dict.Add(key, result);

                    return result;
                }
                finally
                {

                     
                    _rwLock.ExitWriteLock();
                }

            }
            finally
            {
                  
                _rwLock.ExitUpgradeableReadLock();
            }
        }


        public TValue GetValueOrAdd(TKey key, Func<TKey, TValue> addFunc)
        {
            try
            {
                _rwLock.EnterUpgradeableReadLock();
                TValue result;

                if (_dict.TryGetValue(key, out result))
                    return result;


                try
                {
                    _rwLock.EnterWriteLock();
                    result = addFunc(key);
                    _dict.Add(key, result);

                    return result;
                }
                finally
                {


                    _rwLock.ExitWriteLock();
                }

            }
            finally
            {

                _rwLock.ExitUpgradeableReadLock();
            }
        }


        public TValue GetValueOrAdd<TParam>(TKey key, Func<TKey,TParam,TValue> addFunc, TParam param)
        {
            try
            {
                _rwLock.EnterUpgradeableReadLock();
                TValue result;

                if (_dict.TryGetValue(key, out result))
                    return result;


                try
                {
                    _rwLock.EnterWriteLock();
                    result = addFunc(key,param);
                    _dict.Add(key, result);

                    return result;
                }
                finally
                {


                    _rwLock.ExitWriteLock();
                }

            }
            finally
            {

                _rwLock.ExitUpgradeableReadLock();
            }
        }


        public TValue GetValueOrAdd<TParam1, TParam2>(TKey key, Func<TParam1, TParam2, TValue> addFunc, TParam1 param1, TParam2 param2)
        {
            try
            {
                _rwLock.EnterUpgradeableReadLock();
                TValue result;

                if (_dict.TryGetValue(key, out result))
                    return result;


                try
                {
                    _rwLock.EnterWriteLock();
                    result = addFunc(param1,param2);
                    _dict.Add(key, result);

                    return result;
                }
                finally
                {


                    _rwLock.ExitWriteLock();
                }

            }
            finally
            {

                _rwLock.ExitUpgradeableReadLock();
            }
        }



        internal void OpenWriteLock(Action<SynchronizedDictionary<TKey,TValue>> action)
        {
            try
            {
                _rwLock.EnterWriteLock();
                action(this);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }
    }

    
}
