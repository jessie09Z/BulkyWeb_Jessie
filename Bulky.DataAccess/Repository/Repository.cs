﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Bulky.DataAccess.Repository
{
    public class Repository<T> : IRepository<T>  where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db) { 
        _db = db;
            this.dbSet = _db.Set<T>();
            _db.Products.Include(u => u.category);
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null,bool tracked =false)
        {
            IQueryable<T> query;
            if (tracked == true)
            {
               query = dbSet;
               
            }
            else {
                 query = dbSet.AsNoTracking();
               
                }
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includePro in includeProperties.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries))
                {

                    query = query.Include(includePro);
                }
                
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
      
            {
                IQueryable<T> query = dbSet;
            if (filter != null) { query = query.Where(filter); }
            
            if (!string.IsNullOrEmpty(includeProperties)) {
                foreach (var includePro in includeProperties.Split(new char[] { ','},
                    StringSplitOptions.RemoveEmptyEntries))
                {

                    query=query.Include(includePro);
                }
            }
           
            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
    }

}
