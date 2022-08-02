// Decompiled with JetBrains decompiler
// Type: Carbine.Actors.ActorManager
// Assembly: Carbine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9929100E-21E2-4663-A88C-1F977D6B46C4
// Assembly location: D:\OddityPrototypes\Carbine.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Carbine.Actors
{
    public class ActorManager
    {
        private List<Actor> actors;
        private Stack<Actor> actorsToAdd;
        private Stack<Actor> actorsToRemove;

        public ActorManager()
        {
            this.actors = new List<Actor>();
            this.actorsToAdd = new Stack<Actor>();
            this.actorsToRemove = new Stack<Actor>();
        }

        public void Add(Actor actor) => this.actorsToAdd.Push(actor);

        public void AddAll<T>(List<T> addActors) where T : Actor
        {
            int count = addActors.Count;
            for (int index = 0; index < count; ++index)
                this.Add((Actor)addActors[index]);
        }

        public void Remove(Actor actor) => this.actorsToRemove.Push(actor);

        public Actor Find(Func<Actor, bool> predicate) => this.actors.FirstOrDefault<Actor>(predicate);

        public void Step()
        {
            for (int index = this.actors.Count - 1; index >= 0; --index)
            {
                this.actors[index].Input();
                this.actors[index].Update();
            }
            actors.AddRange(actorsToAdd);
            actorsToAdd.Clear();
            while (this.actorsToRemove.Count > 0)
            {
                Actor actor = this.actorsToRemove.Pop();
                actor.Dispose();
                this.actors.Remove(actor);
            }
            this.actorsToRemove.Clear();
        }

        public void Clear()
        {
            for (int index = 0; index < this.actors.Count; ++index)
                this.actorsToRemove.Push(this.actors[index]);
        }
    }
}
