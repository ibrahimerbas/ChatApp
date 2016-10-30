using _24ayar.Data.Utility;
using ChatApp.Data.Surrogates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Data.Entity.Validation;

namespace ChatApp.Data.Repository
{
    public abstract class BaseRepository<TKey, TSurrogate, dContext, TDataModel> : IRepository<TSurrogate,TKey> where TSurrogate : BaseSurrogate<TKey> where TDataModel : class where dContext : DbContext
    {

        public virtual List<TSurrogate> GetAll(string orderby = "")
        {
            return GetAll((string)null, orderby);
        }

        protected abstract IQueryable<TSurrogate> EntryToSurrogate(IQueryable<TDataModel> query);
        protected abstract TDataModel SurrogateToEntry(TSurrogate surrogate, TDataModel entry);
        protected abstract IQueryable<TDataModel> Query(dContext dataContext, string searchText = "", string orderby = "");
        protected abstract IQueryable<TDataModel> Query(dContext dataContext, TKey id);
        public virtual IQueryable<TSurrogate> Order(IQueryable<TSurrogate> query, string orderby)
        {
            return query;
            //switch (orderby)
            //{
            //    case "id":
            //        query = query.OrderBy(u => u.ID);
            //        break;
            //    case "id desc":
            //        query = query.OrderByDescending(u => u.id);
            //        break;
            //    case "name":
            //        query = query.OrderBy(u => u.Name);
            //        break;
            //    case "name desc":
            //        query = query.OrderByDescending(u => u.Name);
            //        break;
            //    default:
            //        break;
            //}
            //return query;
        }

        public virtual List<TSurrogate> GetAll(string searchText, string orderby = "")
        {
            searchText = string.IsNullOrWhiteSpace(searchText) ? null : searchText;
            List<TSurrogate> result = new List<TSurrogate>();
            using (dContext dataContext = (dContext)Activator.CreateInstance(typeof(dContext)))
            {
                var q = Query(dataContext, searchText, orderby);
                result = EntryToSurrogate(q).ToList();
            }
            return result;
        }

        public virtual List<TSurrogate> GetAll(string searchText, int pageSize, int page, out int totalCount, string orderby = "")
        {
            List<TSurrogate> result = new List<TSurrogate>();
            searchText = string.IsNullOrWhiteSpace(searchText) ? null : searchText;
            using (dContext dataContext = (dContext)Activator.CreateInstance(typeof(dContext)))
            {
                var q = Query(dataContext, searchText, orderby);
                result = EntryToSurrogate(q).ToPaged(pageSize, page, out totalCount).ToList();
            }
            return result;
        }
        public virtual List<TSurrogate> GetAll(string searchText, int pageSize, int page, out int totalCount, string orderby = "", Expression<Func<TSurrogate, bool>> filter = null)
        {
            List<TSurrogate> result = new List<TSurrogate>();
            searchText = string.IsNullOrWhiteSpace(searchText) ? null : searchText;
            using (dContext dataContext = (dContext)Activator.CreateInstance(typeof(dContext)))
            {

                var q = Query(dataContext, searchText, orderby);
                var s = EntryToSurrogate(q);
                if (filter != null)
                    s = s.Where(filter);
                s = Order(s, orderby);
                result = s.ToPaged(pageSize, page, out totalCount).ToList();
            }
            return result;
        }
        //public virtual List<TSurrogate> GetAll(string searchText, int pageSize, int page, out int totalCount, string orderby = "", Expression<Func<TDataModel, bool>> filter = null)
        //{
        //    List<TSurrogate> result = new List<TSurrogate>();
        //    using (dContext dataContext = (dContext)Activator.CreateInstance(typeof(dContext)))
        //    {

        //        var q = Query(dataContext, searchText, orderby);

        //        if (filter != null)
        //            q = q.Where(filter);
        //        var s = EntryToSurrogate(q);
        //        s = Order(s, orderby);
        //        result = s.ToPaged(pageSize, page, out totalCount).ToList();
        //    }
        //    return result;
        //}
        public virtual List<TSurrogate> GetAll(Expression<Func<TSurrogate, bool>> filter)
        {
            return GetAll(filter, null);
        }
        public virtual List<TSurrogate> GetAll(Expression<Func<TSurrogate, bool>> filter, string orderby)
        {
            List<TSurrogate> result = new List<TSurrogate>();
            using (dContext dataContext = (dContext)Activator.CreateInstance(typeof(dContext)))
            {

                var q = Query(dataContext, null, orderby);
                var s = EntryToSurrogate(q);
                if (filter != null)
                    s = s.Where(filter);
                s = Order(s, orderby);
                result = s.ToList();
            }
            return result;
        }

        public virtual TSurrogate Get(TKey id)
        {
            TSurrogate result = null;
            using (dContext dataContext = (dContext)Activator.CreateInstance(typeof(dContext)))
            {
                result = EntryToSurrogate(Query(dataContext, id)).SingleOrDefault();
            }
            return result;
        }

        public TSurrogate CreateNewInstance()
        {
            return CreateNewInstance(null);
        }
        public abstract TSurrogate CreateNewInstance(string initName);

        protected abstract TDataModel CreateNewInstanceDataModel();


        public virtual void Save(TSurrogate surrogate)
        {
            using (dContext dataContext = (dContext)Activator.CreateInstance(typeof(dContext)))
            {
                try
                {
                    TDataModel entry = Query(dataContext, surrogate.ID).SingleOrDefault();
                    if (entry == null)
                    {
                        entry = CreateNewInstanceDataModel();
                        dataContext.Entry(entry).State = EntityState.Added;
                    }
                    entry = SurrogateToEntry(surrogate, entry);
                    Save(dataContext, entry, surrogate);
                }
                catch (DbEntityValidationException dbEx)
                {
                    string errors = "";
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            errors += String.Format("Property: {0} Error: {1}{2}", validationError.PropertyName, validationError.ErrorMessage, Environment.NewLine);
                        }
                    }
                    throw new Exception(errors);
                }
            }
        }

        protected abstract void Save(dContext dataContext, TDataModel entry, TSurrogate surrogate);

        public virtual void Delete(TSurrogate surrogate)
        {
            this.Delete(surrogate.ID);
        }

        public virtual void Delete(TKey id)
        {
            using (dContext dataContext = (dContext)Activator.CreateInstance(typeof(dContext)))
            {
                var entry = Query(dataContext, id).SingleOrDefault();
                if (entry != null)
                {
                    dataContext.Entry(entry).State = System.Data.Entity.EntityState.Deleted;
                    dataContext.SaveChanges();
                }
            }

        }



        public virtual void UpdateFileInfo(TDataModel entry, string ImageFileName, string ThumbImageFileName)
        {

        }

        public void UpdateFileInfo(TKey id, string ImageFileName, string thumbFileName)
        {
            using (dContext dataContext = (dContext)Activator.CreateInstance(typeof(dContext)))
            {
                var q = Query(dataContext, id);
                TDataModel model = q.SingleOrDefault();
                UpdateFileInfo(model, ImageFileName, thumbFileName);
                dataContext.SaveChanges();
            }
        }
    }
}
