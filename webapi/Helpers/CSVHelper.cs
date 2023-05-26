using System;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using webapi.Models;

namespace webapi
{
    public class CSVHelper
    {
        List<CsvMappingResult<Details>> csvFileData = null;
        public CSVHelper()
        {
            csvFileData = new List<CsvMappingResult<Details>>();
        }
        void Init(string csvPath)
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvCustomerMapping csvMapper = new CsvCustomerMapping();
            CsvParser<Details> csvParser = new CsvParser<Details>(csvParserOptions, csvMapper);
            csvFileData = csvParser
                         .ReadFromFile(csvPath, Encoding.ASCII)
                         .ToList();
        }
        public ValidationViewModel Validate(string csvPath)
        {
            ValidationViewModel validationViewModel = new ValidationViewModel();
            ValidationModel validationModel = new ValidationModel(csvPath);
            if (File.Exists(csvPath))
            {
                Init(csvPath);

                if (csvFileData != null && csvFileData.Count > 0)
                {
                   validationModel.TotalRecords = csvFileData.Count;
                    validationModel.NoOfInvalidRows = csvFileData.Where(x => !x.IsValid).Count();
                    //check for empty rows
                    int emptyRowsCount = csvFileData.Where(r => r.Result == null).Count();
                    if (emptyRowsCount > 0)
                    {
                        csvFileData.RemoveAll(r => r.Result == null);
                        validationModel.NoOfEmptyRows = emptyRowsCount;
                    }

                    //check for duplicate rows
                    var grouped = csvFileData.GroupBy(line => string.Join(", ", line.Result.FirstName, line.Result.LastName))
                   .ToArray();

                    // "unique entry and first occurrence of duplicate entry" -> first entry in group
                    var unique = grouped.Select(g => g.First());

                    var dupes = grouped.Where(g => g.Count() > 1)
                                       .SelectMany(g => g).ToList();

                    validationModel.NoOfDuplicateRows = dupes.Count();
                    if (dupes.Any())
                    {
                        csvFileData = csvFileData.DistinctBy(line => string.Join(", ", line.Result.FirstName, line.Result.LastName)).ToList();
                    }

                    //Remove Invalid entries
                    List<CsvMappingResult<Details>> invalids = new List<CsvMappingResult<Details>>();
                    foreach (var item in csvFileData)
                    {
                        
                        if (!item.IsValid)
                        {
                            invalids.Add(item);
                        }
                    }
                    if (invalids.Count > 0)
                    {
                        validationModel.NoOfInvalidRows = invalids.Count;
                        csvFileData = csvFileData.Except(invalids).ToList();
                    }


                    validationViewModel.Details = csvFileData.AsEnumerable().Select(r => r.Result).ToList();
                }
                else
                {
                    validationModel.isFileEmpty = true;
                }
            }
            else
            {
                validationModel.isFileEmpty = true;
            }
            
            validationViewModel.ValidationModel = validationModel;

            return validationViewModel;
        }
    }
    public class CsvCustomerMapping : CsvMapping<Details>
    {
        public CsvCustomerMapping()
            : base()
        {
            MapProperty(0, x => x.seqno);
            MapProperty(1, x => x.FirstName);
            MapProperty(2, x => x.LastName);
            MapProperty(3, x => x.Phone);
            MapProperty(4, x => x.Email);
        }
    }
}
