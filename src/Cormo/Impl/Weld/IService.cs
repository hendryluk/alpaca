﻿namespace Cormo.Impl.Weld
{
    public interface IService
    {
        /// <summary>
        /// Called by Weld when it is shutting down, allowing the service to
        /// perform any cleanup needed.
        /// </summary>
        void Cleanup();
    }
}