﻿using System;
using Alpaca.Contexts;

namespace Alpaca.Weld.Context
{
    public abstract class AbstractContext: IContext
    {
        public virtual void Deactivate()
        {
            Destroy();
        }

        protected abstract void GetComponentStore();
        protected virtual void Destroy()
        {
            
        }

        public abstract Type Scope { get; }
    }
}