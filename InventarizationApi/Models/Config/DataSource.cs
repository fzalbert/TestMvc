namespace InventarizationApi.Models.Config;

public class DataSource
{
    public string Address { get; set; }
    public int Port { get; set; }
    public string Name { get; set; }
    public string Login { get; set; }

    public string GetConnectionString()
    {
        return $"Server={Address}; Port={Port}; Database={Name}; User Id={Login};";
    }
}