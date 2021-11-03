using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotVVM.Framework.Compilation.Binding;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Utils;
using Microsoft.EntityFrameworkCore;

namespace GridViewGrouping.Helpers
{
    public static class GridViewDataSetExtensions
    {

        public static async Task LoadFromQueryableAsync<T>(this GridViewDataSet<T> dataSet, IQueryable<T> source)
        {
            source = dataSet.ApplyFilteringToQueryable(source);
            dataSet.Items = await dataSet.ApplyOptionsToQueryable(source).ToListAsync();
            dataSet.PagingOptions.TotalItemsCount = await source.CountAsync();
            dataSet.IsRefreshRequired = false;
        }

        public static async Task LoadFromQueryableWithGroupingAsync<T, TGroupKey>(this GridViewDataSet<T> dataSet, IQueryable<T> source, Func<T, TGroupKey> getGroupKey, Func<T, string> getGroupDisplayName)
            where T : class, IHasGroupInfo<GroupInfo>
        {
            await dataSet.LoadFromQueryableAsync(source);

            var groupInfos = dataSet.Items.Select(i => new GroupInfo()
                {
                    GroupKey = getGroupKey(i)?.ToString(),
                    GroupDisplayName = getGroupDisplayName(i)
                })
                .Distinct()
                .ToList();

            ApplyGroupInfos(dataSet, getGroupKey, groupInfos);
        }

        public static async Task LoadFromQueryableWithGroupingAsync<T, TGroupKey, TGroupInfo>(this GridViewDataSet<T> dataSet, IQueryable<T> source, Func<T, TGroupKey> getGroupKey, Func<List<TGroupKey>, IQueryable<TGroupInfo>> getGroupInfos)
            where T : class, IHasGroupInfo<TGroupInfo>
            where TGroupInfo : GroupInfo
        {
            await dataSet.LoadFromQueryableAsync(source);

            var groupKeys = dataSet.Items.Select(getGroupKey).Distinct().ToList();
            var groupInfos = await getGroupInfos(groupKeys).ToListAsync();

            ApplyGroupInfos(dataSet, getGroupKey, groupInfos);
        }

        private static void ApplyGroupInfos<T, TGroupKey, TGroupInfo>(GridViewDataSet<T> dataSet, Func<T, TGroupKey> getGroupKey, List<TGroupInfo> groupInfos) 
            where T : class, IHasGroupInfo<TGroupInfo> 
            where TGroupInfo : GroupInfo
        {
            var lastExpandedState = false;
            var lastKey = string.Empty;
            for (var i = 0; i < dataSet.Items.Count; i++)
            {
                var key = getGroupKey(dataSet.Items[i]).ToString();
                var groupInfo = groupInfos.Single(x => x.GroupKey == key);
                var isFirstEntryInGroup = i == 0 || lastKey != key;
                var isExpanded = isFirstEntryInGroup ? (dataSet.Items[i].GroupInfo?.IsExpanded ?? true) : lastExpandedState;

                dataSet.Items[i].GroupInfo = groupInfo with
                {
                    IsExpanded = isExpanded,
                    IsFirstEntryInGroup = isFirstEntryInGroup
                };

                lastKey = key;
                lastExpandedState = dataSet.Items[i].GroupInfo.IsExpanded;
            }
        }
    }
}
