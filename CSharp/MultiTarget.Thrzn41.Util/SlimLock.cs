/* 
 * MIT License
 * 
 * Copyright(c) 2017 thrzn41
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Thrzn41.Util
{

    /// <summary>
    /// Slim lock that is allowing multiple threads for reading or exclusive writing.
    /// </summary>
    public class SlimLock : IDisposable
    {

        /// <summary>
        /// ReaderWriter lock.
        /// </summary>
        private ReaderWriterLockSlim rwLock;




        /// <summary>
        /// Creates Slim lock.
        /// </summary>
        public SlimLock()
        {
            this.rwLock = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Creates Slim lock with policy.
        /// </summary>
        /// <param name="policy"><see cref="LockRecursionPolicy"/> that specifies whether a lock can be entered multiple times by the same thread.</param>
        public SlimLock(LockRecursionPolicy policy)
        {
            this.rwLock = new ReaderWriterLockSlim(policy);
        }




        /// <summary>
        /// Executes in reader lock.
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="func">Function that returns value.This function is executed in reader lock.</param>
        /// <returns>Result value of the func function parameter.</returns>
        public TResult ExecuteInReaderLock<TResult>(Func<TResult> func)
        {
            this.rwLock.EnterReadLock();
            try
            {
                return func();
            }
            finally
            {
                this.rwLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Executes in reader lock.
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="func">Function that returns value.This function is executed in reader lock.</param>
        /// <returns>Result value of the func function parameter.</returns>
        public TResult ExecuteInWriterLock<TResult>(Func<TResult> func)
        {
            this.rwLock.EnterWriteLock();
            try
            {
                return func();
            }
            finally
            {
                this.rwLock.ExitWriteLock();
            }
        }


        /// <summary>
        /// Executes in reader lock.
        /// </summary>
        /// <param name="func">Function that returns value.This function is executed in reader lock.</param>
        public void ExecuteInReaderLock(Action func)
        {
            this.rwLock.EnterReadLock();
            try
            {
                func();
            }
            finally
            {
                this.rwLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Executes in reader lock.
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="func">Function that returns value.This function is executed in reader lock.</param>
        public void ExecuteInWriterLock(Action func)
        {
            this.rwLock.EnterWriteLock();
            try
            {
                func();
            }
            finally
            {
                this.rwLock.ExitWriteLock();
            }
        }




        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    using (this.rwLock)
                    {
                        // Disposed.
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SlimLock() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

}
