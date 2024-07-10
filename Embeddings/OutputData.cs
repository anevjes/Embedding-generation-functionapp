using System.Collections.Generic;

namespace Arian.Sample.Output;
public class OutputSkillSetData
{
    public List<Value> Values { get; set; }
}

public class Value
{
    public string RecordId { get; set; }
    public Data Data { get; set; }
    public List<Error>? Errors { get; set; }
    public List<Warning>? Warnings { get; set; }
}

public class Data
{
    public List<float> VectorData { get; set; }
}

public class Error
{
    public string? Message { get; set; }
}

public class Warning
{
    public string? Message { get; set; }
}