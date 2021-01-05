using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.Services
{
   internal interface IContentService<TContent, TField, TModelBase>
       where TContent : ContentBaseEntity<TField>
       where TField : ContentFieldBaseEntity
       where TModelBase : Piranha.Models.ContentBase
   {
      /// <summary>
      /// Transforms the given data into a new model.
      /// </summary>
      /// <typeparam name="T">The model type</typeparam>
      /// <param name="content">The content entity</param>
      /// <param name="type">The content type</param>
      /// <param name="process">Optional func that should be called after transformation</param>
      /// <returns>The page model</returns>
      Task<T> TransformAsync<T>(TContent content, Piranha.Models.ContentTypeBase type, Func<TContent, T, Task> process = null)
          where T : Piranha.Models.ContentBase, TModelBase;

      /// <summary>
      /// Transforms the given model into content data.
      /// </summary>
      /// <param name="model">The model</param>
      /// <param name="type">The conten type</param>
      /// <param name="dest">The optional dest object</param>
      /// <returns>The content data</returns>
      TContent Transform<T>(T model, Piranha.Models.ContentTypeBase type, TContent dest /*= null*/)
          where T : Piranha.Models.ContentBase, TModelBase;

      /// <summary>
      /// Transforms the given block data into block models.
      /// </summary>
      /// <param name="blocks">The data</param>
      /// <returns>The transformed blocks</returns>
      IList<Piranha.Extend.Block> TransformBlocks(IEnumerable<BlockEntity> blocks);

      /// <summary>
      /// Transforms the given blocks to the internal data model.
      /// </summary>
      /// <param name="models">The blocks</param>
      /// <returns>The data model</returns>
      IList<BlockEntity> TransformBlocks(IList<Piranha.Extend.Block> models, ISession session);
   }
}
