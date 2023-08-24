using Npgsql;

namespace InventarizationApi.Models.Service.Db;

public class UpdateDbService : IHostedService
{
    private const string CREATE_TABLE_WAREHOUSE_SQL =
        @"
CREATE TABLE if not exists warehouse (
	        id serial PRIMARY key,
	        company_name text,
	        activity_type text
        );

CREATE TABLE if not exists public.warehouse_object (
	id serial4 NOT NULL,
	geom geometry,
	warehouse_id serial4,
	FOREIGN KEY (warehouse_id) REFERENCES warehouse(id)
);
";

    private readonly IConnectorCreator _connectorCreator;

    public UpdateDbService(IConnectorCreator connectorCreator)
    {
        _connectorCreator = connectorCreator;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var connect = _connectorCreator.Create();
        using var createCommand = new NpgsqlCommand(CREATE_TABLE_WAREHOUSE_SQL, connect);
        connect.Open();
        createCommand.ExecuteNonQuery();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}