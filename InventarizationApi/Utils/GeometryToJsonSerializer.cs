using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using NetTopologySuite.Geometries;

namespace InventarizationApi.Utils;

public class GeometryToJsonSerializer
{
    private readonly JsonOptions _jsonOptions;

    public GeometryToJsonSerializer()
    {
        _jsonOptions = new JsonOptions();
        _jsonOptions.SerializerOptions.Converters.Add(
            new NetTopologySuite.IO.Converters.GeoJsonConverterFactory()
        );
    }

    public string Serialize(Geometry geo)
    {
        var geoJson = JsonSerializer.Serialize(geo, _jsonOptions.SerializerOptions);
        return geoJson;
    }
}