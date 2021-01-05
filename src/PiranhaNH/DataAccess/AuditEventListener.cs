using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.DataAccess
{
   public class AuditEventListener : IPreUpdateEventListener, IPreInsertEventListener
   {
      // Idea for this class from
      // http://ayende.com/blog/3987/nhibernate-ipreupdateeventlistener-ipreinserteventlistener
      // [Manfred, 09sep2014]

      // You have to update both the entity and the entity state in these
      // two event listeners (this is not necessarily the case in other 
      // listeners, by the way).

      public Task<bool> OnPreUpdateAsync(PreUpdateEvent @event, CancellationToken cancellationToken)
      {
         return Task.FromResult(OnPreUpdate(@event));
      }

      public bool OnPreUpdate(PreUpdateEvent @event)
      {
         if (@event.Entity is IHaveAuditInformation audit)
         {
            var now = DateTime.Now;

            var index = Array.IndexOf(@event.Persister.PropertyNames, nameof(IHaveAuditInformation.Created));
            if ((DateTime)@event.State.GetValue(index) != audit.Created)
            {
               Set(@event.Persister, @event.State, nameof(IHaveAuditInformation.Created), audit.Created);
            }

            Set(@event.Persister, @event.State, nameof(IHaveAuditInformation.LastModified), now);
            audit.LastModified = now;
         }
         CheckDateTimeWithinSqlRange(@event.Persister, @event.State);
         return false;
      }

      public Task<bool> OnPreInsertAsync(PreInsertEvent @event, CancellationToken cancellationToken)
      {
         return Task.FromResult(OnPreInsert(@event));
      }

      public bool OnPreInsert(PreInsertEvent @event)
      {
         if (@event.Entity is IHaveAuditInformation audit)
         {
            var now = DateTime.Now;
            Set(@event.Persister, @event.State, nameof(IHaveAuditInformation.Created), now);
            Set(@event.Persister, @event.State, nameof(IHaveAuditInformation.LastModified), now);
            audit.Created = now;
            audit.LastModified = now;
         }
         CheckDateTimeWithinSqlRange(@event.Persister, @event.State);
         return false;
      }

      private static void CheckDateTimeWithinSqlRange(IEntityPersister persister, IReadOnlyList<object> state)
      {
         var rgnMin = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
         // There is a small but relevant difference between DateTime.MaxValue and SqlDateTime.MaxValue.
         // DateTime.MaxValue is bigger than SqlDateTime.MaxValue but still within the valid range of
         // values for SQL Server. Therefore we test against DateTime.MaxValue and not against
         // SqlDateTime.MaxValue. [Manfred, 04jul2017]
         //var rgnMax = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;
         var rgnMax = DateTime.MaxValue;
         for (var i = 0; i < state.Count; i++)
         {
            if (state[i] != null
                && state[i] is DateTime time)
            {
               var value = time;
               if (value < rgnMin /*|| value > rgnMax*/)
               { // we don't check max as SQL Server is happy with DateTime.MaxValue [Manfred, 04jul2017]
                  throw new ArgumentOutOfRangeException(persister.PropertyNames[i], value,
                     $"Property '{persister.PropertyNames[i]}' for class '{persister.EntityName}' must be between {rgnMin:s} and {rgnMax:s} but was {value:s}");
               }
            }
         }
      }

      private static void Set(IEntityPersister persister, object[] state, string propertyName, object value)
      {
         var index = Array.IndexOf(persister.PropertyNames, propertyName);
         if (index == -1)
            return;
         state[index] = value;
      }
   }
}
