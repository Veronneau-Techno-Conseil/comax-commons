using System;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;

namespace CommunAxiom.Commons.ClientUI.Shared.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<PortfolioTreeViewItem> ToTree(this IList<CommunAxiom.Commons.Client.Contracts.Grains.Portfolio.Portfolio> collection,
            Func<CommunAxiom.Commons.Client.Contracts.Grains.Portfolio.Portfolio, Guid> itemIdSelector,
            Func<CommunAxiom.Commons.Client.Contracts.Grains.Portfolio.Portfolio, Guid> parentIdSelector)
        {
            var rootNodes = new List<PortfolioTreeViewItem>();
            var collectionHash = collection.ToLookup(parentIdSelector);

            //find root nodes
            var parentIds = collection.Select(parentIdSelector);
            var itemIds = collection.Select(itemIdSelector);
            var rootIds = parentIds.Except(itemIds);

            foreach (var rootId in rootIds)
            {
                rootNodes.AddRange(
                    GetTreeNodes(
                        itemIdSelector,
                        collectionHash,
                        rootId)
                    );
            }

            return rootNodes;
        }

        private static IEnumerable<PortfolioTreeViewItem> GetTreeNodes(
            Func<CommunAxiom.Commons.Client.Contracts.Grains.Portfolio.Portfolio, Guid> itemIdSelector,
            ILookup<Guid, CommunAxiom.Commons.Client.Contracts.Grains.Portfolio.Portfolio> collectionHash,
            Guid parentId)
        {
            return collectionHash[parentId].Select(collectionItem => {

                Enum.TryParse(collectionItem.Type, out PortfolioType selectedType);

                return new PortfolioTreeViewItem
                {
                    Id = collectionItem.ID,
                    //ParentId = parentId,
                    Text = collectionItem.Name,
                    Type = selectedType,
                    Children = GetTreeNodes(
                        itemIdSelector,
                        collectionHash,
                        itemIdSelector(collectionItem)).ToList()
                };
            });
        }
    }

}

