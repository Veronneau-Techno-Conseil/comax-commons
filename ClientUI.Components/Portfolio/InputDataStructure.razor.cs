using Blazorise;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace ClientUI.Components.Portfolio
{
    public partial class InputDataStructure 
    {
        [Inject] private IPortfolioViewModel PortfolioViewModel { get; set; }
        
        private string _filterFile;
        public DataTable DataTable = new DataTable();

        private void OnChangeDatasource(ChangeEventArgs args)
        {
            _filterFile = args.Value.ToString();
        }
        
        async Task OnChanged(FileChangedEventArgs e)
        {
            var fileStream = e.Files[0];
            using (var stream = new MemoryStream())
            {
                await fileStream.WriteToStreamAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    var fileContent = await reader.ReadToEndAsync();
                    DataTable.Rows.AddRange(JsonConvert.DeserializeObject<List<RowTable>>(fileContent));
                }
            }
        }
    }
}