using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace _24ayar.Data.Utility
{
    public interface IRepository<T> where T:class
	{
        List<T> GetAll(string orderby = "");
        List<T> GetAll(string searchText, string orderby = "");
        List<T> GetAll(string searchText, int pageSize, int page, out int totalCount, string orderby = "");
        List<T> GetAll(string searchText, int pageSize, int page, out int totalCount, string orderby ,Expression<Func<T,bool>> filter = null);
        T Get(int id);
        T CreateNewInstance(string initName = "");
        void Save(T surrogate);
        void Delete(T surrogate);
        void Delete(int id);
        void UpdateFileInfo(int id, string imageFileName, string thumbImageFileName);
    }
    public interface IRepository<T,TKey> where T : class
    {
        List<T> GetAll(string orderby = "");
        List<T> GetAll(string searchText, string orderby = "");
        List<T> GetAll(string searchText, int pageSize, int page, out int totalCount, string orderby = "");
        List<T> GetAll(string searchText, int pageSize, int page, out int totalCount, string orderby, Expression<Func<T, bool>> filter = null);
        T Get(TKey id);
        T CreateNewInstance(string initName = "");
        void Save(T surrogate);
        void Delete(T surrogate);
        void Delete(TKey id);
        void UpdateFileInfo(TKey id, string imageFileName, string thumbImageFileName);
    }
    //public interface IRepository<T,TID> where T : class
    //{
    //    List<T> GetAll(string orderby = "");
    //    List<T> GetAll(string searchText, string orderby = "");
    //    List<T> GetAll(string searchText, int pageSize, int page, out int totalCount, string orderby = "");
    //    T Get(TID id);

    //    void Save(T surrogate);
    //    void Delete(T surrogate);
    //    void Delete(TID id);
    //}
}
