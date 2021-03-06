﻿using System;
using Weapsy.Infrastructure.Domain;

namespace Weapsy.Apps.Text.Domain.Events
{
    public class TextModuleCreated : Event
    {
        public Guid SiteId { get; set; }
        public Guid ModuleId { get; set; }
        public Guid VersionId { get; set; }
        public string Content { get; set; }
    }
}
