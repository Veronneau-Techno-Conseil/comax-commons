using System;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;

namespace CommunAxiom.Commons.ClientUI.Shared.Extensions
{
    public static class EnumerableExtensions
    {
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
                Enum.TryParse(collectionItem.Type, out PortfolioType selectedType);

                return new PortfolioTreeViewItem
                {
                    Id = collectionItem.ID,
                    ParentId = parentId,
                    Text = collectionItem.Name,
                    Type = selectedType,
                    Children = GetTreeNodes(itemIdSelector, collectionHash, itemIdSelector(collectionItem)).ToList()
                };
            });
        }
    }
}