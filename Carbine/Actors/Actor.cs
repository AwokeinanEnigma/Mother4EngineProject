using System;
using SFML.System;

namespace Carbine.Actors
{
    public abstract class Actor : IDisposable
    {
        public virtual Vector2f Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }

        public virtual Vector2f Velocity
        {
            get
            {
                return this.velocity;
            }
        }

        public virtual float ZOffset
        {
            get
            {
                return this.zOffset;
            }
            set
            {
                this.zOffset = value;
            }
        }

        ~Actor()
        {
            this.Dispose(false);
        }

        public virtual void Input()
        {
        }

        public virtual void Update()
        {
            this.position += this.velocity;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
            }
            this.disposed = true;
        }

        protected Vector2f position;

        protected float zOffset;

        protected Vector2f velocity;

        protected bool disposed;
    }
}