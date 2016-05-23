using System;
using System.Linq;
using System.Web.Http;
using System.Web.OData.Query;
using System.Web.OData;
using System.Collections.Generic;
using System.Web.OData.Extensions;
using RevStack.Mvc;
using RevStack.Pattern;

namespace RevStack.Endpoint
{
    public class ODataNavigationController<TEntity,TKey> : ApiController
        where TEntity : class, IEntity<TKey>
    {



        protected virtual PageResult<TEntity> PagedResult(IEnumerable<TEntity> result, ODataQueryOptions<TEntity> options)
        {
            ODataQuerySettings settings = new ODataQuerySettings() { };
            IQueryable query = options.ApplyTo(result.AsQueryable(), settings);
            return new PageResult<TEntity>(query as IQueryable<TEntity>, Request.ODataProperties().NextLink, Request.ODataProperties().TotalCount);
        }

        protected virtual EntityNavigation<TEntity> NavigationResult(TKey id,TEntity entity,IEnumerable<TEntity> items,string baseUrl)
        {
            var navigationResult = new EntityNavigation<TEntity>
            {
                Item = entity
            };
          
            var nextItem=items.SkipWhile(x => x.Id.ToString() != id.ToString()).Skip(1).FirstOrDefault();
            var prevItem=items.Reverse().SkipWhile(x => x.Id.ToString() != id.ToString()).Skip(1).FirstOrDefault();
            if (nextItem==null)
            {
                navigationResult.NextPageLink = null;
            }
            else
            {
                navigationResult.NextPageLink = baseUrl + "/" + nextItem.Id.ToString();
            }
            if(prevItem==null)
            {
                navigationResult.PrevPageLink = null;
            }
            else
            {
                navigationResult.PrevPageLink = baseUrl + "/" + prevItem.Id.ToString();
            }
            navigationResult.Count = items.Count();

            return navigationResult;
        }

        protected virtual EntityNavigation<TEntity> NavigationResult(TKey id, TEntity entity, IEnumerable<TEntity> items,string baseUrl,string querystring)
        {
            var navigationResult = NavigationResult(id,entity, items, baseUrl);
            var firstChar = querystring.FirstChars(1);
            if (firstChar != "?") querystring = "$" + querystring;
            navigationResult.UrlQueryString = querystring;
            if(!string.IsNullOrEmpty(navigationResult.NextPageLink))
            {
                navigationResult.NextPageLink += querystring;
            }
            if(!string.IsNullOrEmpty(navigationResult.PrevPageLink))
            {
                navigationResult.PrevPageLink += querystring;
            }
            
            return navigationResult;
        }
    }
}
