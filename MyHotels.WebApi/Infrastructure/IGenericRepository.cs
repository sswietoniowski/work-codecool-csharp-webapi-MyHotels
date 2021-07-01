﻿using MyHotels.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace MyHotels.WebApi.Infrastructure
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null);
        Task<IPagedList<T>> GetPagedAll(RequestParams requestParams, List<string> includes = null);
        Task<T> Get(Expression<Func<T, bool>> expression = null, List<string> includes = null);
        Task Add(T entity);
        Task AddRange(IEnumerable<T> entities);
        Task Remove(int id);
        void RemoveRange(IEnumerable<T> entities);
        void Modify(T entity);
    }
}
