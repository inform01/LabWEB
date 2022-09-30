using Crypto.Models.ExternalData;
using Newtonsoft.Json;

namespace Crypto.Services.ExternalData;

public class DataTransformationService
{
    public CommonTable ToTableData<TItem>(IEnumerable<TItem> items)
    {
        var properties = typeof(TItem).GetProperties();
        
        var table = new CommonTable
        {
            Title = typeof(TItem).Name,
            Columns = properties
                .Select(x => new CommonTable.Column(x.Name))
                .ToList(),
            Rows = new List<CommonTable.Row>()
        };

        foreach (var item in items)
        {
            var row = new CommonTable.Row
            {
                Cells = properties
                    .Select(x => {
                        var value = x.GetValue(item);

                        if (value is string stringValue)
                        {
                            return stringValue;
                        }
                        
                        var serializedValue = JsonConvert.SerializeObject(value);
                        return serializedValue;
                    })
                    .ToList()
            };

            table.Rows.Add(row);
        }

        return table;
    }
    
    public IEnumerable<TItem> FromTableData<TItem>(CommonTable table) where TItem : class, new()
    {
        var properties = typeof(TItem).GetProperties();
        
        if (!table.Validate() || properties.Length != table.Columns.Count)
        {
            throw new ArgumentException("Invalid table", nameof(table));
        }
        
        var items = new List<TItem>();
        
        for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
        {
            TItem item = new TItem();
            for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
            {
                var column = table.Columns[columnIndex];
            
                var property = properties.FirstOrDefault(x => x.Name == column.Title);
                if (property is null)
                {
                    break;
                }
                
                var value = table.Rows[rowIndex].Cells[columnIndex];

                var targetType = property.PropertyType;
                try
                {
                    if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(item, value);
                    }
                    else
                    {
                        var convertedValue = JsonConvert.DeserializeObject(value, targetType);
                    
                        if (convertedValue is null)
                        {
                            item = null;
                            break;
                        }

                        property.SetValue(item, convertedValue);
                    }
                }
                catch (Exception)
                {
                    item = null;
                    break;
                }
                
            }
            if (item is not null)
            {
                items.Add(item);
            }
        }

        return items;
    }
}
