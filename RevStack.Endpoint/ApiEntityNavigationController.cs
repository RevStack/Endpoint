using System;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.OData.Query;
using System.Web.Http;
using RevStack.Pattern;
using RevStack.Mvc;

namespace RevStack.Endpoint
{
    class ODataEntityNavigationApiController<TEntity,TKey> : ODataBaseController<TEntity,TKey>
        where TEntity : class, IEntity<TKey>
    {
        private string _baseUrl;
        public ODataEntityNavigationApiController(IService<TEntity,TKey> service,string baseUrl) :base(service)
        {
            _baseUrl = baseUrl;
            AllowCreate = false;
            AllowDelete = false;
            AllowUpdate = false;
        }

        public ODataEntityNavigationApiController(IService<TEntity, TKey> service,bool allowCreate,bool allowUpdate,bool allowDelete,string baseUrl) : base(service,allowCreate,allowUpdate,allowDelete)
        {
            _baseUrl = baseUrl;
        }

        public virtual async Task<IHttpActionResult> Get(ODataQueryOptions<TEntity> options)
        {
            var result = await _service.GetAsync();
            var pagedResult = PagedResult(result, options);
            return Ok(pagedResult);
        }

        public virtual async Task<IHttpActionResult> Get(TKey id, ODataQueryOptions<TEntity> options)
        {
            var list = await _service.GetAsync();
            var item = list.Where(x => x.Compare(x.Id, id)).FirstOrDefault();
            var pageResult = PagedResult(list, options);
            var navigationResult = NavigationResult(id, item, pageResult.Items, _baseUrl);
            return Ok(navigationResult);
        }

        public virtual async Task<IHttpActionResult> Get(TKey id, string querystring, ODataQueryOptions<TEntity> options)
        {
            var list = await _service.GetAsync();
            var item = list.Where(x => x.Compare(x.Id, id)).FirstOrDefault();
            var pageResult = PagedResult(list, options);
            var navigationResult = NavigationResult(id, item, pageResult.Items, _baseUrl, HttpUtility.UrlDecode(querystring));
            return Ok(navigationResult);
        }
    }
}
