using System;
using Newtonsoft.Json;

namespace RimuTec.PiranhaNH.Entities
{
   internal abstract class ContentRevisionBaseEntity
   {
      /// <summary>
      /// Gets/sets the unique id.
      /// </summary>
      public Guid Id { get; set; }

      /// <summary>
      /// Gets/sets the data of the revision serialized
      /// as JSON.
      /// </summary>
      public string Data { get; set; }

      /// <summary>
      /// Gets/sets the created date.
      /// </summary>
      public DateTime Created { get; set; }

      /// <summary>
      /// Gets the revision data deserialized as the
      /// specified type.
      /// </summary>
      /// <typeparam name="T">The type</typeparam>
      /// <returns>The deserialized revision data</returns>
      public T GetData<T>()
      {
         if (!string.IsNullOrEmpty(Data))
            return JsonConvert.DeserializeObject<T>(Data);
         return default;
      }
   }
}
