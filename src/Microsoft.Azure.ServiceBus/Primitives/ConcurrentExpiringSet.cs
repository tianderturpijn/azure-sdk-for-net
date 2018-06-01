﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.ServiceBus.Primitives
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    sealed class ConcurrentExpiringSet<TKey>
    {
        readonly ConcurrentDictionary<TKey, DateTime> dictionary;
        readonly object cleanupSynObject = new object();
        bool cleanupScheduled;

        public ConcurrentExpiringSet()
        {
            this.dictionary = new ConcurrentDictionary<TKey, DateTime>();
        }

        public void AddOrUpdate(TKey key, DateTime expiration)
        {
            this.dictionary[key] = expiration;
            this.ScheduleCleanup();
        }

        public bool Contains(TKey key)
        {
            return this.dictionary.TryGetValue(key, out var expiration) && expiration > DateTime.UtcNow;
        }

        void ScheduleCleanup()
        {
            lock (this.cleanupSynObject)
            {
                if (this.cleanupScheduled || this.dictionary.Count <= 0)
                {
                    return;
                }

                this.cleanupScheduled = true;
                Task.Run(async () => await this.CollectExpiredEntries().ConfigureAwait(false));
            }
        }

        async Task CollectExpiredEntries()
        {
            await Task.Delay(TimeSpan.FromSeconds(30));

            lock (this.cleanupSynObject)
            {
                this.cleanupScheduled = false;
            }

            foreach (TKey key in this.dictionary.Keys)
            {
                if (DateTime.UtcNow > this.dictionary[key])
                {
                    this.dictionary.TryRemove(key, out _);
                }
            }

            this.ScheduleCleanup();
        }
    }
}