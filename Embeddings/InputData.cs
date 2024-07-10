using System.Collections.Generic;

namespace Arian.Sample.Input;
public class InputData
{
    public List<Value> values { get; set; }
}

public class Value
{
    public string recordId { get; set; }
    public data data { get; set; }
}

public class data
{
    public string RawText { get; set; }
}