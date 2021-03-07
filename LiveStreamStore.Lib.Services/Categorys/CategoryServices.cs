using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Services.Caching;

namespace LiveStreamStore.Lib.Services.Categorys
{
    public class CategoryServices : ICategoryService
    {
        public static string CategoryPrefix => "Category";
        public static CacheKey AllCategoryCacheKey => new CacheKey("Category-All", CategoryPrefix);

        private ICacheService _CacheService;

        public CategoryServices(ICacheService cacheService)
        {
            _CacheService = cacheService;
        }
        public List<Category> GetAllCategory()
        {
            return _CacheService.Get(AllCategoryCacheKey, () =>
            {
                using var context = new LiveStreamStoreContext();
                return context.Category.ToList();
            });
        }
    }
}
