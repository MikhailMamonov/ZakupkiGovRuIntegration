using AngleSharp;

using System.Xml;

namespace ZakupkiGovRuIntegration.Helpers
{
    public static class HttpClientExtensions
    {
        public static async Task<Dictionary<string,Dictionary<string,string>>> ReadContent44FZAsync(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode == false)
            {
                throw new ApplicationException($"Аукцион с данным реестровым номером не найден. Ответ сервера : {response.ReasonPhrase}");
            }
            var dataAsString = await response.Content.ReadAsStringAsync();

            var rows = new Dictionary<string,Dictionary<string, string>>();

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(dataAsString));

            var wrapper = document.QuerySelector(".wrapper");

            var rowNodes =wrapper.QuerySelectorAll(".blockInfo");
            if (rowNodes != null)
            {
                foreach (var rowNode in rowNodes)
                {
                    var colNode = rowNode.QuerySelector(".col");
                    var title = colNode?.QuerySelector("h2")?.TextContent ?? "Name not available";
                    var sections = colNode?.QuerySelectorAll(".blockInfo__section");
                    var sectionDictionary = new Dictionary<string,string>();

                    if (sections != null&& sections.Count()>0)
                    {
                        foreach (var section in sections)
                        {

                            var sectionTitles = section.QuerySelectorAll("span.section__title").Select(cell => cell?.TextContent ?? "Name not available").ToArray();
                            if (sectionTitles != null && sectionTitles.Length > 0)
                            {
                                if (sectionTitles.Length > 1)
                                {
                                    var tableList = new List<string>();
                                    List<string> rowList = new List<string>();
                                    var tables = section.QuerySelectorAll(".blockInfo__table").ToArray();

                                    for (int i = 0;i < sectionTitles.Length;i++)
                                    {
                                        if (tables != null && tables.Length > 0)
                                        {
                                            var tableRows = tables[i].QuerySelectorAll("tbody tr").ToArray();

                                            for (int j = 0;j < tableRows.Length;j++)
                                            {
                                                if (j == 0)
                                                {
                                                    var colsArr = tableRows[j].QuerySelectorAll("th.table__row-item").Select(row => row?.TextContent ?? "Name not available").ToArray();
                                                    rowList
                                                        .Add(String.Join(", ",colsArr));
                                                }
                                                else
                                                {
                                                    var colsArr = tableRows[j].QuerySelectorAll("td.table__row-item").Select(row => row?.TextContent ?? "Name not available").ToArray();
                                                    rowList
                                                        .Add(String.Join(", ",colsArr));
                                                }
                                            }
                                        }

                                        tableList.Add(sectionTitles[i]);
                                        tableList.AddRange(rowList);
                                    }

                                    sectionDictionary.Add(sectionTitles.First(),String.Join("\n",tableList));
                                }
                                else
                                {
                                    var sectionInfo = section.QuerySelector(".section__info")?.TextContent ?? string.Empty;
                                    sectionDictionary.Add(sectionTitles.First(),sectionInfo);
                                }
                            }
                            else {
                                var sectionInfo = section.QuerySelector(".section__info")?.TextContent ?? string.Empty;
                                sectionDictionary.Add(" ",sectionInfo);
                            }
                        }

                        rows.Add(title.Replace("\t","").Replace("\n"," ").Trim(),sectionDictionary);
                    }
                    else 
                    {
                        var table = colNode?.QuerySelector("table.blockInfo__table.tableBlock");
                        if (table != null) 
                        {
                            List<string> rowList = new List<string>();
                            var tableRows = table.Children.Where(m => m.LocalName == "thead")
                                .SelectMany(ch => ch.Children.Where(m => m.LocalName == "tr")).ToList();
                            tableRows.AddRange(table.QuerySelectorAll("tbody> tr").ToList());
                            tableRows.AddRange(table.QuerySelectorAll("tfoot> tr").ToList());

                            for (int j = 0;j < tableRows.Count;j++)
                            {
                                if (j == 0)
                                {
                                    var colsArr = tableRows[j].QuerySelectorAll("th.tableBlock__col_header ").Select(row => row?.TextContent ?? "Name not available").ToArray();
                                    rowList
                                        .Add(String.Join(", ",colsArr));
                                }
                                else
                                {
                                    var colsArr = tableRows[j].QuerySelectorAll("td.tableBlock__col").Select(row => row?.TextContent ?? "Name not available").ToArray();
                                    rowList
                                        .Add(String.Join(", ",colsArr));
                                }
                            }

                            sectionDictionary.Add("",String.Join(Environment.NewLine,rowList));
                            rows.Add(title.Replace("\t","").Replace("\n"," ").Trim(),sectionDictionary);
                            

                        }

                    }
                }
            }
            return rows;
        }

        public static async Task<Dictionary<string,Dictionary<string,string>>> ReadContent223FZAsync(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode == false)
            {
                throw new ApplicationException($"Аукцион с данным реестровым номером не найден. Ответ сервера : {response.ReasonPhrase}");
            }
            var dataAsString = await response.Content.ReadAsStringAsync();

            var rows = new Dictionary<string,Dictionary<string,string>>();

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(dataAsString));

            var wrapper = document.QuerySelector(".wrapper");

            var rowNodes = wrapper.QuerySelectorAll(".card-common-content");
            if (rowNodes != null)
            {
                foreach (var cardNode in rowNodes)
                {
                    var sectionNode = cardNode.QuerySelector("section.common-text");
                    var containerNode = cardNode.QuerySelector(".container");
                    var rowNode = cardNode.QuerySelector(".row");

                    var columns = sectionNode?.QuerySelectorAll(".col-9");
                    if (columns != null && columns.Count() > 0)
                    {
                        var title = columns.First().QuerySelector(".common-text__caption")?.TextContent ?? "Name not available";
                        var columnDictionary = new Dictionary<string,string>();
                        foreach (var column in columns.Skip(1))
                        { 
                            var columnTitle = column.QuerySelector(".common-text__title")?.TextContent ?? string.Empty;
                            var columnnValue = column.QuerySelector(".common-text__value")?.TextContent ?? string.Empty;
                            columnDictionary.Add(columnTitle,columnnValue);
                                
                        }

                        rows.Add(title.Replace("\t","").Replace("\n"," ").Trim(),columnDictionary);
                    }
                    }
                }
           
            return rows;
        }
    }
}
