//[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class CatDetailsRequest
{
    public string name;
    public CatDetailsRequest(string _name)
    {
        name = _name;
    }
}

public class CatDetailsResponse
{
    public string name;
    public string description;
    public string color;
    public bool enable;
    public string qr_code;
}


