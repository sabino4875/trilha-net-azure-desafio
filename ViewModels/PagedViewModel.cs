using System;
using System.Collections.Generic;

namespace TrilhaNetAzureDesafio.ViewModels
{
    public class PagedViewModel<TEntity> where TEntity : class
    {
        private readonly Int32 _page;
        private readonly Int32 _totalPages;
        private readonly IEnumerable<TEntity> _items;

        public PagedViewModel(Int32 count, Int32 limit, Int32 page, IEnumerable<TEntity> items)
        {
            _page = page;
            if(_page < 1)
            {
                _page = 1;
            }

            if (limit < 1) 
            {
                limit = 1;
            }

            if (count < 0) 
            {
                count = 0;
            }

            _totalPages = Convert.ToInt32(Math.Floor(Convert.ToDecimal(count) / Convert.ToDecimal(limit)));
            if (_totalPages < 1) 
            {
                _totalPages = 1;
            }

            if((_totalPages * limit) < count)
            {
                _totalPages += 1;
            }

            _items = items;
        }

        public Int32 Page => _page;
        public Int32 TotalPages => _totalPages;
        public IEnumerable<TEntity> Items => _items;
    }
}
