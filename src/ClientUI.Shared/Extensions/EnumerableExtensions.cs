using System;
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;

namespace CommunAxiom.Commons.ClientUI.Shared.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TItem> ToTree<TItem>(this IList<TItem> collection) where TItem : ITreeviewItem, new()
        {
            var rootNodes = new List<TItem>();
            var collectionHash = collection.ToLookup(x=>x.ParentId);

            //find root nodes
            var parentIds = collection.Select(x => x.ParentId);
            var itemIds = collection.Select(x=>x.Id);
            var rootIds = parentIds.Except(itemIds);

            foreach (var rootId in rootIds)
            {
                rootNodes.AddRange(GetTreeNodes(collectionHash, rootId));
            }

            return rootNodes;
        }

        private static IEnumerable<TItem> GetTreeNodes<TItem>(
            ILookup<Guid, TItem> collectionHash, Guid parentId) where TItem : ITreeviewItem, new()
        {

            return collectionHash[parentId].Select(collectionItem =>
            {
                collectionItem.Children = GetTreeNodes<TItem>(collectionHash, collectionItem.Id).Cast<ITreeviewItem>().ToList();
                return collectionItem;
            });
        }

        public static IEnumerable<PortfolioTreeViewItem> ToTree(this IList<PortfolioModel> collection,
            Func<PortfolioModel, Guid> itemIdSelector,
            Func<PortfolioModel, Guid> parentIdSelector)
        {
            var rootNodes = new List<PortfolioTreeViewItem>();
            var collectionHash = collection.ToLookup(parentIdSelector);

            //find root nodes
            var parentIds = collection.Select(parentIdSelector);
            var itemIds = collection.Select(itemIdSelector);
            var rootIds = parentIds.Except(itemIds);

            foreach (var rootId in rootIds)
            {
                rootNodes.AddRange(GetTreeNodes(itemIdSelector, collectionHash, rootId));
            }

            return rootNodes;
        }

        private static IEnumerable<PortfolioTreeViewItem> GetTreeNodes(
            Func<PortfolioModel, Guid> itemIdSelector,
            ILookup<Guid, PortfolioModel> collectionHash,
            Guid parentId)
        {
            
            return collectionHash[parentId].Select(collectionItem =>
            {
                
                return new PortfolioTreeViewItem
                {
                    Id = collectionItem.ID,
                    ParentId = parentId,
                    Text = collectionItem.Name,
                    Type = collectionItem.Type,
                    Children = GetTreeNodes(itemIdSelector, collectionHash, itemIdSelector(collectionItem)).ToList()
                };
            });
        }
    }
}