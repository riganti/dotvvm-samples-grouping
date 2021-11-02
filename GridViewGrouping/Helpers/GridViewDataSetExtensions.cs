using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotVVM.Framework.Controls;
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

        public static async Task ApplyGrouping<T, TGroupKey, TGroupInfo>(this GridViewDataSet<T> dataSet, Func<T, TGroupKey> getGroupKey, Func<List<TGroupKey>, IQueryable<TGroupInfo>> getGroupInfos)
            where T : class, IHasGroupInfo<TGroupInfo>
            where TGroupInfo : GroupInfo
        {
            var groupKeys = dataSet.Items.Select(getGroupKey).Distinct().ToList();
            var groupInfos = await getGroupInfos(groupKeys).ToListAsync();

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

        public static void ResetGrouping<T>(this GridViewDataSet<T> dataSet)
            where T : class
        {
            foreach (var item in dataSet.Items)
            {
                ((dynamic)item).GroupInfo = null;
            }
        }
    }
}
