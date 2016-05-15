using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Http;
using System.Web.OData.Query;
using System.Linq.Expressions;
using RevStack.Pattern;
using RevStack.Mvc;


namespace RevStack.Endpoint
{
    public class ODataApiPredicateController<TEntity,TKey,TProperty> : ODataBaseController<TEntity,TKey>
        where TEntity : class, IEntity<TKey>
    {
        public ODataApiPredicateController(IService<TEntity, TKey> service) :base(service)
        { }

        public ODataApiPredicateController(IService<TEntity, TKey> service, bool allowCreate, bool allowUpdate, bool allowDelete) : base(service,allowCreate,allowUpdate,allowDelete)
        { }

       
        public virtual async Task<IHttpActionResult> Get(ODataQueryOptions<TEntity> options, TProperty id)
        {
            var exp = Lambda(id);
            var result = await _service.FindAsync(exp);
            var pagedResult = PagedResult(result, options);
            return Content(HttpStatusCode.OK, pagedResult);
        }

        public virtual async Task<IHttpActionResult> Get(TKey id,bool signature)
        {
            var result = await _service.FindAsync(x => x.Compare(x.Id, id));
            var entity = result.FirstOrDefault();
            return Ok(entity);
        }

        public virtual async Task<IHttpActionResult> post(TEntity entity)
        {
            if (!AllowCreate)
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

        public virtual async Task<IHttpActionResult> put(TEntity entity)
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

        public virtual async Task<IHttpActionResult> delete(TKey id)
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


        protected virtual Expression<Func<TEntity, bool>> Lambda(TProperty id)
        {
            throw new NotImplementedException();
            
        }


    }

    
}