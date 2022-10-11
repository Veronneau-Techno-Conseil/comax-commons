using Blazorise;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.ClientUI.Shared.Extensions;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;
using CommunAxiom.Commons.Ingestion.Ingestor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace ClientUI.Components.Portfolio
{
    public partial class Portfolio
    {
        private bool IsLoading { get; set; }

        [Inject] public IPortfolioViewModel PortfolioViewModel { get; set; }
        [Inject] public IStringLocalizer<Portfolio> Localizer { get; set; }
        [Inject] public IMetadataParser MetadataParser { get; set; }
        public string SelectedDataSource { get; private set; }
        public string SelectedTab = "tab1";
        public IList<PortfolioModel> PortfolioList;
        public IEnumerable<PortfolioTreeViewItem> PortfolioListResult;
        public IList<PortfolioTreeViewItem> ExpandedPortfolio = new List<PortfolioTreeViewItem>();
        public PortfolioModel ModalViewModel = new PortfolioModel();
        private List<string> _types;
        public Modal? ModalRefefence;
        public bool CancelClose;
        private PortfolioTreeViewItem SelectedPortfolio { get; set; } = new PortfolioTreeViewItem();
        private bool HideCard { get; set; } = true;

        private void OnChangeType(string type)
        {
            ModalViewModel.Type = type;
        }

        private void OnSelectedTab(string tabName)
        {
            SelectedTab = tabName;
        }

        private async void ShowCard(PortfolioTreeViewItem treeViewItem)
        {
            
            HideCard = treeViewItem.Type == PortfolioType.Project || treeViewItem.Type == PortfolioType.Folder;
            SelectedPortfolio = treeViewItem;
            FieldMetaDataList.Clear();
            
            IsLoading = true;
            
            var result = await PortfolioViewModel.GetFieldMetaData(treeViewItem.Id.ToString());
            if (result != null)
            {
                FieldMetaDataList = result;
            }
            
            IsLoading = false; 
            StateHasChanged();
        }

        private Task ShowModal(PortfolioTreeViewItem currentPortfolio)
        {
            SelectedPortfolio = currentPortfolio;
            return ModalRefefence.Show();
        }

        private void AddToTree(PortfolioTreeViewItem parent, PortfolioTreeViewItem newItem)
        {
            if (parent.Children == null)
            {
                parent.Children = new List<PortfolioTreeViewItem>();
            }

            parent.Children.Add(newItem);
        }

        private async Task OnModalSave()
        {
            // TODO: check the name of portfolio
            //if (string.IsNullOrEmpty(modalViewModel.Name))
            //{
            //    toastService.ShowError(_localizer["NameIsRequired"], "ERROR");
            //    return;
            //}

            var newPortfolio = new PortfolioModel
            {
                ID = Guid.NewGuid(),
                Name = ModalViewModel.Name,
                ParentId = SelectedPortfolio.Id,
                Type = ModalViewModel.Type
            };

            await PortfolioViewModel.CreatePortfolio(newPortfolio);

            AddToTree(SelectedPortfolio, new PortfolioTreeViewItem
            {
                Id = newPortfolio.ID,
                Text = newPortfolio.Name,
                ParentId = newPortfolio.ParentId,
                Type = Enum.Parse<PortfolioType>(newPortfolio.Type)
            });

            ClearModel();
            CancelClose = false;
            await ModalRefefence.Hide();
        }

        public async Task OnModalCancel()
        {
            ClearModel();
            CancelClose = true;
            await ModalRefefence.Hide();
        }

        private void ClearModel()
        {
            ModalViewModel.ID = Guid.Empty;
            ModalViewModel.ParentId = Guid.Empty;
            ModalViewModel.Name = string.Empty;
            ModalViewModel.Type = _types[0];
        }

        protected override async Task OnInitializedAsync()
        {
            _types = PortfolioViewModel.GetPortfolioTypes();
            ModalViewModel.Type = _types[0];
            PortfolioList = await PortfolioViewModel.GetPortfolios();
            PortfolioListResult = PortfolioList?.ToTree(o => o.ID, o => o.ParentId);
            if (PortfolioListResult != null) ExpandAll(PortfolioListResult);
        }

        private void ExpandAll(IEnumerable<PortfolioTreeViewItem> nodes)
        {
            foreach (var node in nodes)
            {
                node.Children = node.Children;
                ExpandAll(node.Children);
            }

            ((List<PortfolioTreeViewItem>)ExpandedPortfolio).AddRange(nodes);
        }

        public List<FieldMetaData> FieldMetaDataList = new();

        private void OnChangeDatasource(ChangeEventArgs args)
        {
            SelectedDataSource = args.Value.ToString();
        }

        public async Task OnFileChanged(FileChangedEventArgs e)
        {
            FieldMetaDataList.Clear();

            var fileStream = e.Files[0];
            using var stream = new MemoryStream();

            await fileStream.WriteToStreamAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(stream);

            var fileContent = await reader.ReadToEndAsync();
            if (!string.IsNullOrEmpty(fileContent))
            {
                FieldMetaDataList.AddRange(MetadataParser.ReadMetadata(fileContent));
            }
        }


        public void OnFieldDelete(FieldMetaData field)
        {
            FieldMetaDataList.Remove(field);
        }

        public bool GetHiddenUp(FieldMetaData field)
        {
            if (FieldMetaDataList != null)
            {
                var indexes = FieldMetaDataList.Select(o => o.Index).OrderBy(o => o.Value).ToList();

                return field.Index == indexes[0];
            }

            return false;
        }

        public bool GetHiddenDown(FieldMetaData field)
        {
            if (FieldMetaDataList != null)
            {
                var indexes = FieldMetaDataList.Select(o => o.Index).OrderBy(o => o.Value).ToList();

                return field.Index == indexes[indexes.Count() - 1];
            }

            return false;
        }

        public void ArrowDown(FieldMetaData field)
        {
            if (field.Index != null && FieldMetaDataList != null)
            {
                var items = FieldMetaDataList.OrderBy(o => o.Index).ToList();
                for (int i = 0; i < items.Count(); i++)
                {
                    try
                    {
                        if (field.Index == items[i].Index)
                        {
                            (field.Index, items[i + 1].Index) = (items[i + 1].Index, field.Index);
                            break;
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            }
        }

        public void ArrowUp(FieldMetaData field)
        {
            if (field.Index != null && FieldMetaDataList != null)
            {
                var items = FieldMetaDataList.OrderBy(o => o.Index).ToList();
                for (int i = 0; i < items.Count(); i++)
                {
                    try
                    {
                        if (field.Index == items[i].Index && i > 0)
                        {
                            (field.Index, items[i - 1].Index) = (items[i - 1].Index, field.Index);
                            break;
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            }
        }

        public void AddNewRow()
        {
            var last = FieldMetaDataList.OrderBy(x => x.Index.Value).Last();

            FieldMetaDataList.Add(new FieldMetaData { Index = last.Index + 1 });
        }

        public async Task SaveConfig()
        {
            await PortfolioViewModel.SaveConfig(this.SelectedPortfolio.Id.ToString(), null);
        }

        public async Task SaveFieldMetaData()
        {
            await PortfolioViewModel.SaveFieldMetaData(SelectedPortfolio.Id.ToString(), FieldMetaDataList);
            FieldMetaDataList.Clear();
        }
    }
}