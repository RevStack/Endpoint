using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web.OData.Query;
using RevStack.Mvc;
using RevStack.Pattern;

namespace RevStack.Endpoint
{
    public class ODataApiController<TEntity,TKey> : ODataBaseController<TEntity,TKey>
        where TEntity : class, IEntity<TKey>
    {
        public ODataApiController(IService<TEntity,TKey> service) :base(service)
        { }

        public ODataApiController(IService<TEntity, TKey> service,bool allowCreate,bool allowUpdate,bool allowDelete) : base(service,allowCreate,allowUpdate,allowDelete)
        { }


        public virtual async Task<IHttpActionResult> Get(ODataQueryOptions<TEntity> options)
        {
            var result = await _service.GetAsync();
            var pagedResult = PagedResult(result, options);
            return Content(HttpStatusCode.OK, pagedResult);
        }

        public virtual async Task<IHttpActionResult> Get(TKey id)
        {
            var result = await _service.FindAsync(x => x.Compare(x.Id, id));
            var entity = result.FirstOrDefault();
            return Ok(entity);
        }

        public virtual async Task<IHttpActionResult> Post(TEntity entity)
        {
            if(!AllowCreate)
            {
                return new ContentErrorResult(Request, HttpStatusCode.Forbidden, "Create not allowed");
            }
            if (!ModelState.IsValid)
            {
                var modelErrors = ModelState.Values.SelectMany(x => x.Errors);
                return new ModelErrorResult(Request, modelErrors);
            }
            
            entity.Id = NewId;
            var validated = Validate(entity);
            if (!validated.Item1)
            {
                return new ContentErrorResult(Request, validated.Item2);
            }
            entity = validated.Item3;
            entity = await _service.AddAsync(entity);
            return Ok(entity);
        }

        public virtual async Task<IHttpActionResult> Put(TEntity entity)
        {
            if (!AllowUpdate)
            {
                return new ContentErrorResult(Request, HttpStatusCode.Forbidden, "Update not allowed");
            }
            if (!ModelState.IsValid)
            {
                var modelErrors = ModelState.Values.SelectMany(x => x.Errors);
                return new ModelErrorResult(Request, modelErrors);
            }
            entity = await _service.UpdateAsync(entity);
            return Ok(entity);
        }

        public virtual async Task<IHttpActionResult> Delete(TKey id)
        {
            if (!AllowDelete)
            {
                return new ContentErrorResult(Request, HttpStatusCode.Forbidden, "Delete not allowed");
            }
            var result = await _service.FindAsync(x => x.Compare(x.Id, id));
            var entity = result.FirstOrDefault();
            await _service.DeleteAsync(entity);
            return Ok();
        }

    }
}
