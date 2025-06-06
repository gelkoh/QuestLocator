/* // <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var root = Root.FromJson(jsonString);

using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class Root
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("product")]
    public Product Product { get; set; }

    [JsonProperty("status")]
    public long Status { get; set; }

    [JsonProperty("status_verbose")]
    public string StatusVerbose { get; set; }
}

public partial class Product
{
    [JsonProperty("_id")]
    public string Id { get; set; }

    [JsonProperty("brands_tags")]
    public string[] BrandsTags { get; set; }

    [JsonProperty("ecoscore_grade")]
    public string EcoscoreGrade { get; set; }

    [JsonProperty("ingredients")]
    public Ingredient[] Ingredients { get; set; }

    [JsonProperty("ingredients_analysis_tags")]
    public string[] IngredientsAnalysisTags { get; set; }

    [JsonProperty("nutriscore_grade")]
    public string NutriscoreGrade { get; set; }

    [JsonProperty("product_name")]
    public string ProductName { get; set; }

    [JsonProperty("product_quantity")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long ProductQuantity { get; set; }

    [JsonProperty("product_quantity_unit")]
    public string ProductQuantityUnit { get; set; }

    [JsonProperty("schema_version")]
    public long SchemaVersion { get; set; }
}

public partial class Ingredient
{
    [JsonProperty("ciqual_proxy_food_code", NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(ParseStringConverter))]
    public long? CiqualProxyFoodCode { get; set; }

    [JsonProperty("ecobalyse_code", NullValueHandling = NullValueHandling.Ignore)]
    public string EcobalyseCode { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("is_in_taxonomy")]
    public long IsInTaxonomy { get; set; }

    [JsonProperty("percent_estimate")]
    public double PercentEstimate { get; set; }

    [JsonProperty("percent_max")]
    public double PercentMax { get; set; }

    [JsonProperty("percent_min")]
    public double PercentMin { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("vegan", NullValueHandling = NullValueHandling.Ignore)]
    public string Vegan { get; set; }

    [JsonProperty("vegetarian", NullValueHandling = NullValueHandling.Ignore)]
    public string Vegetarian { get; set; }

    [JsonProperty("ciqual_food_code", NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(ParseStringConverter))]
    public long? CiqualFoodCode { get; set; }

    [JsonProperty("from_palm_oil", NullValueHandling = NullValueHandling.Ignore)]
    public string FromPalmOil { get; set; }
}

/* public partial class Root
{
    public static Root FromJson(string json) => JsonConvert.DeserializeObject<Root>(json, QuickType.Converter.Settings);
}  */

/* public static class Serialize
{
    public static string ToJson(this Root self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
}

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
        {
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        },
    };
} */

/* internal class ParseStringConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        long l;
        if (Int64.TryParse(value, out l))
        {
            return l;
        }
        throw new Exception("Cannot unmarshal type long");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (long)untypedValue;
        serializer.Serialize(writer, value.ToString());
        return;
    }

    public static readonly ParseStringConverter Singleton = new ParseStringConverter();
}
 */
 