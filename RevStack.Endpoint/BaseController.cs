using System;
using System.Web.Http;
using System.Web.OData.Query;
using System.Web.OData;
using System.Web.OData.Extensions;
using RevStack.Pattern;
using System.Collections.Generic;
using System.Linq;

namespace RevStack.Endpoint
{
    public class ODataBaseController<TEntity,TKey> : ApiController
         where TEntity : class, IEntity<TKey>
    {
        protected IService<TEntity, TKey> _service;
        public ODataBaseController(IService<TEntity, TKey> service)
        {
            _service = service;
            AllowCreate = true;
            AllowDelete = true;
            AllowUpdate = true;
        }

        public ODataBaseController(IService<TEntity, TKey> service,bool allowCreate,bool allowUpdate,bool allowDelete)
        {
            _service = service;
            AllowCreate = allowCreate;
            AllowDelete = allowDelete;
            AllowUpdate = allowUpdate;
        }

        private bool _allowUpdate;
        public bool AllowUpdate
        {
            get
            {
                return _allowUpdate;
            }
            set
            {
                _allowUpdate = value;
            }
        }
        private bool _allowCreate;
        public bool AllowCreate
        {
            get
            {
                return _allowCreate;
            }
            set
            {
                _allowCreate = value;
            }
        }
        private bool _allowDelete;
        public bool AllowDelete
        {
            get
            {
                return _allowDelete;
            }
            set
            {
                _allowDelete = value;
            }
        }

        protected PageResult<TEntity> PagedResult(IEnumerable<TEntity> result, ODataQueryOptions<TEntity> options)
        {
            ODataQuerySettings settings = new ODataQuerySettings() { };
            IQueryable query = options.ApplyTo(result.AsQueryable(), settings);
            return new PageResult<TEntity>(query as IQueryable<TEntity>, Request.ODataProperties().NextLink, Request.ODataProperties().TotalCount);
        }

        public virtual TKey NewId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected virtual Tuple<bool,string,TEntity> Validate(TEntity entity)
        {
            return new Tuple<bool,string,TEntity>(true,"",entity);
        }

    }
}