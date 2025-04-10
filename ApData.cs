public class ApData
{
    public string Name { get; set; }   // AP Name
    public string Model { get; set; }  // AP Model
    public string MacAddress { get; set; } // MAC Address for username
    public string ApMac { get; set; }  // AP MAC for ap command
    public bool IsChecked { get; set; } // Track whether this AP is selected for deletion
}
