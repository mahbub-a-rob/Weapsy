﻿using System;
using System.Linq;
using Weapsy.Data;
using Weapsy.Domain.Languages;
using Weapsy.Domain.Menus;
using Weapsy.Domain.Sites.Events;
using Weapsy.Infrastructure.Caching;
using Weapsy.Infrastructure.Domain;

namespace Weapsy.Reporting.Data.Sites
{
    public class SiteEventsHandler : 
        IEventHandler<SiteCreated>,
        IEventHandler<SiteDetailsUpdated>
    {
        private readonly IWeapsyDbContextFactory _dbContextFactory;
        private readonly ICacheManager _cacheManager;

        public SiteEventsHandler(IWeapsyDbContextFactory dbContextFactory,
            ICacheManager cacheManager)
        {
            _dbContextFactory = dbContextFactory;
            _cacheManager = cacheManager;
        }

        public void Handle(SiteCreated @event)
        {
            ClearCache(@event.AggregateRootId, @event.Name);
        }

        public void Handle(SiteDetailsUpdated @event)
        {
            ClearCache(@event.AggregateRootId, @event.Name);
        }

        private void ClearCache(Guid siteId, string name)
        {
            using (var context = _dbContextFactory.Create())
            {
                var languageIds = context.Languages
                    .Where(x => x.SiteId == siteId && x.Status == LanguageStatus.Active)
                    .Select(language => language.Id)
                    .ToList();

                foreach (var languageId in languageIds)
                {
                    _cacheManager.Remove(string.Format(CacheKeys.SiteInfoCacheKey, name, languageId));
                }
                
                _cacheManager.Remove(string.Format(CacheKeys.SiteInfoCacheKey, name, Guid.Empty));

                var menuNames = context.Menus
                    .Where(x => x.SiteId == siteId && x.Status == MenuStatus.Active)
                    .Select(menu => menu.Name)
                    .ToList();

                foreach (var menuName in menuNames)
                {
                    _cacheManager.Remove(string.Format(CacheKeys.MenuCacheKey, siteId, menuName, Guid.Empty));

                    foreach (var languageId in languageIds)
                    {
                        _cacheManager.Remove(string.Format(CacheKeys.MenuCacheKey, siteId, menuName, languageId));
                    }
                }
            }
        }
    }
}
