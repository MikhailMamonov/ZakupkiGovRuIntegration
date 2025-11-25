using AngleSharp;
using AngleSharp.Dom;

using System.Collections.Generic;
using System.Text.Json;

using static System.Collections.Specialized.BitVector32;

namespace ZakupkiGovRuIntegration.Helpers
{
    public static class HttpClientExtensions
    {
        private static readonly string[] sectionsWithTables = { "Финансовое обеспечение закупки (всего)","За счет бюджетных средств","Детализация по кодам видов расходов" }; 
        public static async Task<Dictionary<string,Dictionary<string,string>>> ReadContentAsync(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode == false)
                throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");
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
                    var title = colNode?.QuerySelector("h2")?.TextContent ?? "Name not available"; ;
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
    }
}
